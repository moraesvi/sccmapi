using HelperComum;
using Newtonsoft.Json.Linq;
using SCCM.Dominio.Comum;
using SCCM.Dominio.WMI;
using SCCM.Dominio.WMI.SCCM_SDK;
using SCCMWebNoAuth.API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MyBranch.Log.Factory;
using MyBranch.Log.Dominio;
using SCCM.Dominio.Comum.Uteis;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/util")]
    public class UtilController : BaseController
    {
        private const string ECOMIGO_SANTANDER_PROTOCOLO = "ecsClient:";
        private const string ECOMIGO_SANTANDER_PROTOCOLO_DESC = "Santander Tecnologia - EComigo Santander - Cliente de execução de comandos utilizando o SDK do SCCM(System Center)";
        [HttpGet]
        [Route("api/util/URLProtocolo")]
        public IHttpActionResult URLProtocolo()
        {
            object objetoProtocolo = new { Protocolo = ECOMIGO_SANTANDER_PROTOCOLO, Descricao = ECOMIGO_SANTANDER_PROTOCOLO_DESC };

            IComumResult result = ComumResultFactory.Criar("Busca realizada", objetoProtocolo);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/util/URLProtocoloApp")]
        public IHttpActionResult URLProtocoloApp([FromBody]JObject data)
        {
            if (data == null || !data.HasValues)
            {
                return ParamInvalidosResult();
            }

            string appId = string.Empty;
            string chave = string.Empty;

            try
            {
                chave = SCCMApi.ObterChaveTransacao();
                appId = data.GetValue("ci_UniqueID").ToObject<string>();
            }
            catch (Exception ex)
            {
                return ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.InnerException().Message);
            }

            object objetoProtocoloPortalURL = new { ProtocoloPortalURLFormat = string.Concat(ECOMIGO_SANTANDER_PROTOCOLO, appId, "|", chave), ChaveTransacao = chave };

            IComumResult result = ComumResultFactory.Criar("Busca realizada", objetoProtocoloPortalURL);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/util/ClientSDKScriptApp")]
        public IHttpActionResult ClientSDKScriptApp([FromBody]JObject data)
        {
            if (data == null || !data.HasValues)
            {
                ParamInvalidosResult();
            }

            string appId = string.Empty;

            try
            {
                appId = data.GetValue("ci_UniqueID").ToObject<string>(); ;
            }
            catch (Exception ex)
            {
                ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.InnerException().Message);
            }

            ClientScriptApp clientSApp = new ClientScriptApp(appId);

            IWMIResult result = clientSApp.DefinirScriptResult();

            return OkResult(result);
        }
        [HttpGet]
        [Route("api/util/verificacaoClient/{chaveInstalacao}")]
        public IHttpActionResult ObterVerificacaoClient(string chaveInstalacao)
        {
            SCCMClientInstalAPI procInstal = null;

            try
            {
                procInstal = ObterVerifClient(chaveInstalacao);
            }
            catch (Exception ex)
            {
                return ErroResult("Erro na requisição", ex.InnerException().Message);
            }

            IComumResult result = ComumResultFactory.Criar("Detalhes da verificação do sccm client", procInstal);

            return OkResult(result);
        }
        [HttpGet]
        [Route("api/util/statusInstalacao/{chaveInstalacao}")]
        public IHttpActionResult ObterStatusIntalacao(string chaveInstalacao)
        {
            ProcInstalacaoAPI procInstal = procInstal = ObterStatus(chaveInstalacao);

            IComumResult result = ComumResultFactory.Criar("Detalhes do status", procInstal);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/util/statusInstalacao")]
        public IHttpActionResult SalvarStatusIntalacao(ProcInstalacaoAPI procInstal)
        {
            string procInstalJson = procInstal.ToJson();

            NovoStatus(procInstal.Chave, procInstalJson);

            IComumResult result = ComumResultFactory.Criar("Gravação de Status Realizado", procInstal);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/util/verificacaoClient")]
        public IHttpActionResult SalvarVerificacaoClient(SCCMClientInstalAPI sccmClientInstal)
        {
            string procInstalJson = sccmClientInstal.ToJson();

            GravarVerificacaoClient(sccmClientInstal.Chave, procInstalJson);

            IComumResult result = ComumResultFactory.Criar("Gravação de verificação do client sccm realizado", true);

            return OkResult(result);
        }
        [HttpGet]
        [Route("api/util/caminho")]
        public IHttpActionResult ObterCaminhoInstalacao(ProcInstalacaoAPI procInstal)
        {
            string diretorio = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            IComumResult result = ComumResultFactory.Criar("Diretorio resultado", diretorio);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/util/sccmClientLog")]
        public async Task<IHttpActionResult> LogErroSCCMCLient([FromBody]SCCMClientLogAPI sccmLog)
        {
            await GravarLog(sccmLog);

            IComumResult result = ComumResultFactory.Criar("Log gerado", true);

            return Ok(result);
        }

        #region Metodos Privados
        private void GravarVerificacaoClient(string chave, string dados)
        {
            string nomeArquivo = string.Concat("client_", chave, ".json");
            string caminhoArquivo = Path.Combine(ConfigurationManager.AppSettings.Get("SCCM_FILES"));

            string caminhoArquivoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);

            if (string.IsNullOrEmpty(caminhoArquivo))
            {
                caminhoArquivo = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }

            if (Directory.Exists(caminhoArquivo) && !File.Exists(caminhoArquivoCompleto))
            {
                try
                {
                    using (FileStream stream = File.Create(caminhoArquivoCompleto))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(dados);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat("Ocorreu um erro na geração da verificação do client sccm instalado", chave), ex.InnerException);
                }
            }
        }
        private void NovoStatus(string chave, string dados)
        {
            string nomeArquivo = string.Concat("status_", chave, ".json");
            string caminhoArquivo = Path.Combine(ConfigurationManager.AppSettings.Get("SCCM_FILES"), "Status");

            string caminhoArquivoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);

            if (string.IsNullOrEmpty(caminhoArquivo))
            {
                caminhoArquivo = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Status"));
            }

            if (!Directory.Exists(caminhoArquivo))
            {
                Directory.CreateDirectory(caminhoArquivo);
            }

            if (File.Exists(caminhoArquivoCompleto)) //Caso já exista um status com o nome, é gerado status indexados.
            {
                string[] totalArquivo = Directory.GetFiles(caminhoArquivo, string.Concat("status_", chave, "*.*"));

                nomeArquivo = string.Concat("status_", chave, "_", totalArquivo.Count(), ".json");

                caminhoArquivoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);
            }

            try
            {
                using (FileStream stream = File.Create(caminhoArquivoCompleto))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(dados);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro na geração do status de instalação", chave), ex.InnerException);
            }
        }
        private ProcInstalacaoAPI ObterStatus(string chave)
        {
            string caminhoArquivo = string.Empty;
            string nomeArquivo = string.Concat("status_", chave);

            string caminhoArquivoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("SCCM_FILES")))
            {
                caminhoArquivo = Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Status"));
            }
            else
            {
                caminhoArquivo = Path.Combine(ConfigurationManager.AppSettings.Get("SCCM_FILES"), "Status");

                if (!Directory.Exists(ConfigurationManager.AppSettings.Get("SCCM_FILES")))
                {
                    throw new InvalidOperationException("Operação Inválida", new Exception(string.Format("O caminho - {0} - não é válido", caminhoArquivo)));
                }
            }

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(caminhoArquivo);
                FileInfo[] fileInfo = dInfo.GetFiles(string.Concat(nomeArquivo, "*.*"));

                if (fileInfo != null)
                {
                    if (fileInfo.Count() > 0)
                    {
                        FileInfo file = fileInfo.LastOrDefault();
                        string json = File.ReadAllText(file.FullName);

                        ProcInstalacaoAPI procInstall = JsonConvert.DeserializeObject<ProcInstalacaoAPI>(json);

                        File.Delete(file.FullName);

                        return procInstall;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro ao obter o status de instalação - Chave: ", chave), ex.InnerException);
            }
        }
        private SCCMClientInstalAPI ObterVerifClient(string chave)
        {
            string caminhoArquivo = string.Empty;
            string nomeArquivo = string.Concat("client_", chave, ".json");

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("SCCM_FILES")))
            {
                caminhoArquivo = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }
            else
            {
                caminhoArquivo = Path.Combine(ConfigurationManager.AppSettings.Get("SCCM_FILES"));

                if (!Directory.Exists(ConfigurationManager.AppSettings.Get("SCCM_FILES")))
                {
                    throw new InvalidOperationException("Operação Inválida", new Exception(string.Format("O caminho {0} não é válido", caminhoArquivo)));
                }
            }

            try
            {
                string caminhoArquivoCompleto = Path.Combine(caminhoArquivo, nomeArquivo);

                if (!File.Exists(caminhoArquivoCompleto))
                    return null;

                string json = File.ReadAllText(caminhoArquivoCompleto);

                SCCMClientInstalAPI valSCCMClient = JsonConvert.DeserializeObject<SCCMClientInstalAPI>(json);

                File.Delete(caminhoArquivoCompleto);
           
                return valSCCMClient;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Ocorreu um erro na verificação do client instalado - Chave: ", chave), ex.InnerException);
            }
        }
        private async Task GravarLog(SCCMClientLogAPI sccmLog)
        {
            string tituloLog = SCCMLog.GerarLogTituloTemplateSCCMClient(sccmLog.Usuario, sccmLog.PC, Convert.ToDateTime(sccmLog.DataLog), sccmLog.VersaoWindows, sccmLog.Windows64Bits, sccmLog.CaminhoApp);

            await SCCMLog.GravarLog(tituloLog, string.Concat("SCCM_Client-", sccmLog.PC), sccmLog.LogException);
        }
        #endregion
    }
}