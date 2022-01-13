using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.App.Protocolo;
using SCCM.App.Dominio;
using AppHelper = SCCM.App.Helper;
using System.Text;
using HelperComum;

namespace SCCM.App.Testes
{
    [TestClass]
    public class ProtocoloTest
    {
        [TestMethod]
        public void VALIDAR_INSTALACAO_CLIENT_TESTE()
        {
            Program.Main(new string[] { });
        }
        [TestMethod]
        public void VALIDAR_DESINSTALACAO_CLIENT_TESTE()
        {
            Program.Main(new string[] { "desinstalar" });
        }
        [TestMethod]
        public void VALIDAR_EXECUCAO_SCRIPT_SDK_POWERSHELL_TESTE()
        {
            string protocoloParam = BuscaScriptSimulacao();
                
            Program.Main(new string[] { protocoloParam });
        }
        private string BuscaScriptSimulacao()
        {
            try
            {
                //string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_e43168e0-3ef1-46fa-a55e-865ba1706b5b";
                //string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_10931bdb-80a7-498b-9b51-ca3205650c73";
                //string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_5b6b73cc-0522-45ba-b90c-4dbd68d853eb";
                string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_28844877-7881-412e-a051-2821c6651e64";
                //string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_9c4a7b86-9347-4472-bef8-821cb2791567";//OK
                //string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_8b86fdc3-5c3a-4041-aaa8-940235ef5f4e";
                string json = string.Empty;

                IWMIResult wmiResult = null;
                SCCMURLProtocoloApp scriptApp = null;

                string sccmApiURL = System.Configuration.ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/URLProtocoloApp");

                object data = new { ci_UniqueID = idApp };

                json = AppHelper.WebApiHttp.HttpJsonRequisicaoPost(3, sccmApiURL, data);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    wmiResult = WMIResultFactory.SerializarResult<SCCMURLProtocoloApp>(json);

                    if (!wmiResult.Executado)
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                scriptApp = wmiResult.Result as SCCMURLProtocoloApp;

                return scriptApp.ProtocoloPortalURLFormat;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    internal class SCCMURLProtocoloApp
    {
        public string ChaveTransacao { get; set; }
        public string ProtocoloPortalURLFormat { get; set; }
    }
}
