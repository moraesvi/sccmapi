using SCCM.Servico.Contratos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLResult
{
    public class XML : IPowerShellResult
    {
        private const string XML_CORPO = "CORPO";
        private const string XML_CHAVE_RESULT = "CHAVE-RESULT";
        private const string XML_DISPOSITIVO = "DISPOSITIVO";
        private const string XML_RESULT_LINHA = "RESULT-LINHA-";
        private const string XML_RESULT = "RESULT";

        public const string EXTENSAO = "XML";

        private string _chave;

        public string Chave
        {
            get
            {
                return _chave;
            }
        }
        public string ExtensaoArquivo
        {
            get
            {
                return EXTENSAO;
            }
        }
        public string ObterResult(string dispositivo, string chave)
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
                    GerarCabecalhoXML(xml, dispositivo, chave);
                    GerarCorpoXMLVazio(xml);

                    xml.WriteEndElement();
                    xml.Flush();
                }

                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;
        }
        public string ObterResult(string dispositivo, string msgException, string chave)
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
                    GerarCabecalhoXML(xml, dispositivo, chave);
                    GerarCorpoXMLException(xml, msgException);

                    xml.WriteEndElement();
                    xml.Flush();
                }

                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;
        }
        public string ObterResult(ICollection<PSObject> psObjectResults, string dispositivo, string chave)
        {
            StringBuilder sbXML = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            string result = string.Empty;

            Encoding utf8noBOM = new UTF8Encoding(false);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = utf8noBOM;

            PSObject psObject = psObjectResults.Where(ps => ps != null)
                                               .FirstOrDefault();

            using (MemoryStream output = new MemoryStream())
            {
                using (XmlWriter xml = XmlWriter.Create(output, settings))
                {
                    GerarCabecalhoXML(xml, dispositivo, chave);
                    GerarCorpoXML(xml, psObject);

                    xml.WriteEndElement();
                    xml.Flush();
                }

                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;
        }
        public string ObterResult(ICollection<ErrorRecord> erros, string dispositivo, string chave)
        {
            StringBuilder sbXML = new StringBuilder();
            MemoryStream stream = new MemoryStream();
            string result = string.Empty;

            using (XmlTextWriter xml = new XmlTextWriter(stream, Encoding.UTF8))
            {
                GerarCabecalhoXML(xml, dispositivo, chave);
                GerarCorpoXML(xml, erros);

                xml.WriteEndElement();
                xml.Flush();

                StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
                stream.Seek(0, SeekOrigin.Begin);
                result = reader.ReadToEnd();

                return result;
            }
        }
        private void GerarCabecalhoXML(XmlWriter xml, string dispositivo, string chave)
        {
            _chave = chave;

            xml.WriteStartDocument();
            xml.WriteStartElement(XML_CORPO);
            xml.WriteAttributeString(XML_CHAVE_RESULT, "chave-busca");
            xml.WriteElementString(XML_CHAVE_RESULT, chave);
            xml.WriteElementString(XML_DISPOSITIVO, dispositivo);
        }
        private void GerarCorpoXML(XmlWriter xml, PSObject psObjectResult)
        {
            PSMemberInfoCollection<PSPropertyInfo> propriedades = psObjectResult.Properties;

            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 1), "Script executado com sucesso");

            for (int indice = 0; indice < propriedades.Count(); indice++)
            {
                string linhaXML = string.Concat(XML_RESULT_LINHA, indice);
                PSPropertyInfo psProp = propriedades.ElementAt(indice);

                string valor = (psProp.Value == null) ? string.Empty : psProp.Value.ToString();

                xml.WriteElementString(string.Concat(XML_RESULT_LINHA, (indice + 2)), valor);
            }
        }
        private void GerarCorpoXML(XmlTextWriter xml, ICollection<ErrorRecord> erros)
        {
            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 1), "Script gerou erros na execução");

            for (int indice = 0; indice < erros.Count(); indice++)
            {
                string linhaXML = string.Concat(XML_RESULT_LINHA, indice);
                ErrorRecord errorRecord = erros.ElementAt(indice);

                xml.WriteElementString(string.Concat(XML_RESULT_LINHA, (indice + 2)), errorRecord.ToString());
            }
        }
        private static void GerarCorpoXMLVazio(XmlWriter xml)
        {
            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 1), "Script executado com sucesso");
            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 2), "Não houve resultado PowerShell");
        }
        private static void GerarCorpoXMLException(XmlWriter xml, string msgException)
        {
            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 1), "Ocorreu uma exception na execução do script");
            xml.WriteElementString(string.Concat(XML_RESULT_LINHA, 2), msgException);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
