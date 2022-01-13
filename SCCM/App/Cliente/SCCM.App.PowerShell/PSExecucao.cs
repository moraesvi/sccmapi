using SCCM.App.Helper;
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
using System.Runtime.Remoting.Messaging;
using AppHelper = SCCM.App.Helper;
using System.Configuration;
using HelperComum;
using SCCM.App.Dominio;

namespace SCCM.App.PowerShell
{
    static class PSExecucao
    {
        private static string _chave;
        internal static void DefinirChave(string chave)
        {
            _chave = chave;
        }
        internal static T ExecutarPSScriptAsync<T>(string queryCompleto, T objeto, Runspace remoteRunspace) where T : class
        {
            try
            {
                using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(queryCompleto);

                    PSDataCollection<PSObject> colecaoResultado = new PSDataCollection<PSObject>();
                    colecaoResultado.DataAdded += ColecaoResultado_DataAdded;

                    IAsyncResult result = powershell.BeginInvoke<PSObject, PSObject>(null, colecaoResultado);

                    while (!result.IsCompleted)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    try
                    {
                        powershell.EndInvoke(result);
                    }
                    catch (ActionPreferenceStopException ex)
                    {
                        ErrorRecord psExeception = ex.ErrorRecord;
                        throw new Exception(psExeception.Exception.Message, psExeception.Exception.InnerException);
                    }

                    PSObject psObject = colecaoResultado.LastOrDefault();

                    ParseObjeto.PSObjectObjeto(psObject, objeto);

                    if (psObject == null)
                    {
                        ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        var result2 = result as AsyncResult;

                        if (errors == null)
                        {
                            //errors = result.
                        }

                        if (errors != null)
                        {
                            if (errors.Count > 0)
                            {
                                throw new Exception("Ocorreu um erro ao realizar a consulta PowerShell", new Exception(errors.FirstOrDefault().ErrorDetails.Message));
                            }

                            throw new Exception("Ocorreu um erro ao realizar a consulta PowerShell");
                        }
                    }

                    return objeto;
                }
            }
            catch (Exception)
            {
                remoteRunspace.Close();
                remoteRunspace.Dispose();

                throw;
            }
        }
        private static void ColecaoResultado_DataAdded(object sender, DataAddedEventArgs e)
        {
            try
            {
                int totalTentativasStatus = 3;
                string[] propInstalArray = new string[] { "Executado", "Instalado", "Status", "StatusDesc", "DetalheStatus" };
                PSObject psObject = (sender as PSDataCollection<PSObject>).LastOrDefault();

                bool propStatusInstal = psObject.Properties.ToList().Exists(prop => propInstalArray.Contains(prop.Name));

                if (string.IsNullOrWhiteSpace(_chave))
                {
                    throw new Exception("Operação inválida", new Exception("Obrigatório definir uma chave de instalação"));
                }

                if (propStatusInstal)
                {
                    ProcInstalacaoAPI objeto = new ProcInstalacaoAPI();

                    objeto.Chave = _chave;
                    ParseObjeto.PSObjectObjeto(psObject, objeto);

                    string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                    sccmApiURL = string.Concat(sccmApiURL, "/api/util/statusInstalacao");

                    Helper.WebApiHttp.HttpJsonRequisicaoPost(totalTentativasStatus, sccmApiURL, objeto);
                }
            }
            catch { }
        }
        internal static string ExecutarPSScript(string queryCompleto, Runspace remoteRunspace)
        {
            using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
            {
                string result = string.Empty;
                powershell.Runspace = remoteRunspace;
                powershell.AddScript(queryCompleto);

                PSObject psObject = powershell.Invoke()
                                              .FirstOrDefault();

                result = psObject.ToString();

                if (psObject == null)
                {
                    ICollection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                    if (errors != null)
                    {
                        if (errors.Count > 0)
                        {
                            throw new Exception("Ocorreu um erro ao realizar a consulta PowerShell", new Exception(errors.FirstOrDefault().ErrorDetails.Message));
                        }

                        throw new Exception("Ocorreu um erro ao realizar a consulta PowerShell");
                    }
                }

                return result;
            }
        }
    }
}
