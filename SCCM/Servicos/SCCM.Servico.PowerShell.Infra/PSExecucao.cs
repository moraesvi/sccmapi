using HelperComum;
using SCCM.Servico.Contratos;
using SCCM.Servico.Dominio;
using SCCM.Servico.ParseObjetoHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCCM.Servico.PowerShell.Infra
{
    static class PSExecucao
    {
        private const string XML_CORPO = "CORPO";
        private const string XML_CHAVE_RESULT = "CHAVE";
        private const string XML_DISPOSITIVO = "DISPOSITIVO";
        private const string XML_COMANDO_LINHA = "SCRIPT-LINHA-";

        private const string PS_LEITURA_ARQUIVO = @"
                             $propResult = @{
                                 Nome = """"
                                 Conteudo = """"
                             };
                             $items = Get-ChildItem -Path [caminho] -Force
                             ForEach ($item in $items) {	
                             
                                 $textoArq = Get-Content $item.fullname | Out-String -ErrorAction SilentlyContinue;
                                 
                                 $propResult.Nome = $item.Name;
                                 $propResult.Conteudo = $textoArq.ToString();
                                 
                                 New-Object -TypeName PSObject -prop $propResult;
                             
                             }";

        internal static void ConexaoRunspace(WSManConnectionInfo connectionInfo, ref Runspace remoteRunspace)
        {
            remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo);
            remoteRunspace.Open();
        }
        internal static bool ExecutarResultPSScript(string script, Runspace remoteRunspace)
        {

            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    PSObject psObject = powershell.Invoke()
                                                  .FirstOrDefault();

                    if (psObject == null)
                    {
                        ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        if (errors != null && errors.Count > 0)
                            throw new Exception("Ocorreu um erro ao realizar a execução do script PowerShell");
                    }

                    return true;
                }
            }
            catch
            { }

            return false;
        }
        internal static ICollection<T> ExecutarColecaoPSScript<T>(string script, T objeto, Runspace remoteRunspace) where T : class
        {

            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> PSResults = powershell.Invoke();
                    ICollection<T> lstResult = new List<T>();

                    foreach (PSObject psObject in PSResults)
                    {
                        if (psObject != null)
                        {
                            objeto = Helper.CriarInstancia(objeto.GetType()) as T;
                            ParseObjeto.PSObjectObjeto(psObject, objeto);

                            lstResult.Add(objeto);
                        }
                    }

                    return lstResult;
                }
            }
            catch
            { }

            return new List<T>();
        }
        internal static ICollection<string> ExecutarColecaoStringPSScript(string script, Runspace remoteRunspace)
        {

            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> PSResults = powershell.Invoke();
                    ICollection<string> lstResult = new List<string>();

                    foreach (PSObject psObject in PSResults)
                    {
                        if (psObject != null)
                        {
                            lstResult.Add(psObject.ToString());
                        }
                    }

                    return lstResult;
                }
            }
            catch
            { }

            return new List<string>();
        }
        internal static ICollection<PSArquivoResult> ExecutarBuscaArquivoPSScript(string caminho, Runspace remoteRunspace)
        {

            try
            {
                string script = PS_LEITURA_ARQUIVO.Replace("[caminho]", caminho);

                ICollection<PSArquivoResult> lstResult = ExecutarColecaoPSScript(script, new PSArquivoResult(), remoteRunspace);

                return lstResult;
            }
            catch
            { }

            return new List<PSArquivoResult>();
        }
        internal static KeyValuePair<bool, string> ExecutarResultXMLPSScript(string script, string dispositivo, string chave, Runspace remoteRunspace)
        {
            string xml = string.Empty;
            bool executado = false;

            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> psObject = powershell.Invoke();

                    if (psObject == null || psObject.Count == 0)
                    {
                        ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        if (errors != null && errors.Count > 0)
                        {
                            xml = ObterXMLResult(dispositivo, null, errors, chave);
                        }
                        else
                        {
                            xml = ObterXMLResult(dispositivo, null, null, chave);
                            executado = true;
                        }
                    }
                    else
                    {
                        PSObject[] psObjectResult = psObject.Where(t => t != null)
                                                            .ToArray();

                        xml = ObterXMLResult(dispositivo, psObjectResult, null, chave);
                        executado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                xml = ObterXMLResult(dispositivo, null, null, chave, true, ex.Message);
            }

            return new KeyValuePair<bool, string>(executado, xml);
        }
        internal static KeyValuePair<bool, string> ExecutarResultPSScript(string script, string dispositivo, string chave, IPowerShellResult psResultado, Runspace remoteRunspace)
        {
            string xml = string.Empty;
            bool executado = false;

            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(script);

                    ICollection<PSObject> psObject = powershell.Invoke();

                    if (psObject == null || psObject.Count == 0)
                    {
                        ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        if (errors != null && errors.Count > 0)
                        {
                            xml = psResultado.ObterResult(errors, dispositivo, chave);
                        }
                        else
                        {
                            xml = psResultado.ObterResult(dispositivo, chave);
                            executado = true;
                        }
                    }
                    else
                    {
                        psResultado.ObterResult(psObject, dispositivo, chave);
                        executado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                xml = psResultado.ObterResult(dispositivo, ex.Message, chave);
            }

            return new KeyValuePair<bool, string>(executado, xml);
        }

        #region Metodos Privados

        private static string ObterXMLResult(string dispositivo, PSObject[] psObjectArray, ICollection<ErrorRecord> erros, string chave, bool ocorreuException = false, string msgException = null)
        {
            StringBuilder sbXML = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            string result = string.Empty;

            Encoding utf8noBOM = new UTF8Encoding(false);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = utf8noBOM;

            using (MemoryStream output = new MemoryStream())
            {
                using (XmlWriter xml = XmlWriter.Create(output, settings))
                {
                    xml.WriteStartDocument();
                    xml.WriteStartElement(XML_CORPO);
                    xml.WriteAttributeString(XML_CHAVE_RESULT, "chave-busca");
                    xml.WriteElementString(XML_CHAVE_RESULT, chave);
                    xml.WriteElementString(XML_DISPOSITIVO, dispositivo);

                    if (!ocorreuException)
                    {
                        if (psObjectArray != null)
                        {
                            if (psObjectArray.Count() > 1)
                            {
                                GerarCorpoXML(xml, psObjectArray);
                            }
                            else
                            {
                                PSObject PSObject = psObjectArray.SingleOrDefault();
                                GerarCorpoXML(xml, PSObject);
                            }
                        }
                        else if (erros != null)
                        {
                            GerarCorpoXML(xml, erros);
                        }
                        else
                        {
                            GerarCorpoXMLVazio(xml);
                        }
                    }
                    else
                    {
                        GerarCorpoXMLException(xml, msgException);
                    }

                    xml.WriteEndElement();
                    xml.Flush();
                }

                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;
        }
        private static void GerarCorpoXML(XmlWriter xml, PSObject psObjectResult)
        {
            PSMemberInfoCollection<PSPropertyInfo> propriedades = psObjectResult.Properties;

            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Script executado com sucesso");

            for (int indice = 0; indice < propriedades.Count(); indice++)
            {
                string linhaXML = string.Concat(XML_COMANDO_LINHA, indice);
                PSPropertyInfo psProp = propriedades.ElementAt(indice);

                string value = (psProp.Value == null) ? string.Empty : psProp.Value.ToString();

                xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, (indice + 2)), value);
            }
        }
        private static void GerarCorpoXML(XmlWriter xml, PSObject[] psObjectResult)
        {
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Script executado com sucesso");

            for (int indiceResult = 0; indiceResult < psObjectResult.Count(); indiceResult++)
            {
                PSObject psObject = psObjectResult.ElementAtOrDefault(indiceResult);

                PSMemberInfoCollection<PSPropertyInfo> propriedades = psObject.Properties;

                for (int indice = 0; indice < propriedades.Count(); indice++)
                {
                    string linhaXML = string.Concat(XML_COMANDO_LINHA, indice);
                    PSPropertyInfo psProp = propriedades.ElementAt(indice);

                    string value = (psProp.Value == null) ? string.Empty : psProp.Value.ToString();

                    xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, (indiceResult + 2)), value);
                }
            }
        }
        private static void GerarCorpoXML(XmlWriter xml, ICollection<ErrorRecord> erros)
        {
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Script gerou erros na execução");

            for (int indice = 0; indice < erros.Count(); indice++)
            {
                string linhaXML = string.Concat(XML_COMANDO_LINHA, indice);
                ErrorRecord errorRecord = erros.ElementAt(indice);

                xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, (indice + 2)), errorRecord.ToString());
            }
        }
        private static void GerarCorpoXMLVazio(XmlWriter xml)
        {
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Script executado com sucesso");
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 2), "Não houve resultado PowerShell");
        }
        private static void GerarCorpoXMLException(XmlWriter xml, string msgException)
        {
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 1), "Ocorreu uma exception na execução do script");
            xml.WriteElementString(string.Concat(XML_COMANDO_LINHA, 2), msgException);
        }

        #endregion
    }
}
