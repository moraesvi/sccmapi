using HelperComum;
using SCCM.App.Dominio;
using AppHelper = SCCM.App.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Cliente
{
    public class ClienteSDK
    {
        private const int TOTAL_TENTATIVAS_SCCM_API = 3;

        private string _idApp;
        private string _chaveTransacao;
        public ClienteSDK(string idApp, string chaveTransacao)
        {
            _idApp = idApp;
            _chaveTransacao = chaveTransacao;
        }
        public string HttpClientSDKScriptApp()
        {
            try
            {
                string json = string.Empty;

                IWMIResult wmiResult = null;
                SCCMClientScriptApp scriptApp = null;

                string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/ClientSDKScriptApp");

                object data = new { ci_UniqueID = _idApp };

                json = AppHelper.WebApiHttp.HttpJsonRequisicaoPost(TOTAL_TENTATIVAS_SCCM_API, sccmApiURL, data);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    wmiResult = WMIResultFactory.SerializarResult<SCCMClientScriptApp>(json);

                    if (!wmiResult.Executado)
                    {
                        throw new InvalidOperationException("API - Ocorreu um erro na requisição de busca do script de instalação do app", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                    }
                }
                else
                {
                    throw new InvalidOperationException("Não foi possível buscar o script SCCMClient ScriptApp", null);
                }

                scriptApp = wmiResult.Result as SCCMClientScriptApp;

                string psPolitica = System.Text.Encoding.UTF8.GetString(scriptApp.Script);

                return psPolitica;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a requisição de busca do SCCMClient", ex.InnerException);
            }
        }
    }
    internal class SCCMClientScriptApp
    {
        public string AppId { get; set; }
        public byte[] Script { get; set; }
    }
}
