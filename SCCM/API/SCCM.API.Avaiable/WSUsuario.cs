using Dominio;
using HelperComum;
using Newtonsoft.Json.Linq;
using SCCM.API.Infraestrutura.ApplicationViewService;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using SCCM.Dominio.Model;
using SCCM.Dominio.WMI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCCM.API.Avaiable
{
    public class WSUsuario
    {
        private const int MAX_REG = 50;
        private const string PREF_CACHE = "cache-";

        private const int TOTAL_TENTATIVAS_SCCM_API = 3;

        private ApplicationViewServiceSoapClient _sccmWS;
        private ICache _cache;
        private bool _cacheDefinido;
        //private SCCMPoliticasModelResult _politicasProc = null;

        public WSUsuario()
        {
            _sccmWS = WSAutenticacao.ObterEscopo();
            _cache = DefinirInstanciaCache();
        }
        public IComumResult ListarAppResult(int indicePagina = 1)
        {
            ApplicationModel appModelResult = ListarApp(indicePagina, _cacheDefinido);

            if (appModelResult == null)
            {
                return ComumResultFactory.Criar("Não houve resultado na busca dos aplicativos", appModelResult);
            }

            return ComumResultFactory.Criar(string.Format("Listagem de aplicativos realizado, página {0}", indicePagina), appModelResult);
        }
        public IComumResult DetalhesAppResult(string ci_UniqueID)
        {
            Application app = DetalhesAplicativo(ci_UniqueID, false);

            if (app == null)
            {
                return ComumResultFactory.Criar(true, "Não houve resultado na busca do detalhe do aplicativo", app);
            }

            return ComumResultFactory.Criar(true, string.Format("Detalhes do aplicativo {0}", app.LocalizedDisplayName), app);
        }
        public IComumResult StatusAppResult(string ci_UniqueID)
        {
            AppInstalacaoStatusDetalheAudit appStatus = AplicativoStatusDetalhes(ci_UniqueID, WSAutenticacao.Usuario);

            if (appStatus == null)
            {
                return ComumResultFactory.Criar(true, "Não houve resultado na busca do status do aplicativo", appStatus);
            }

            return ComumResultFactory.Criar(true, string.Format("Status do aplicativo {0}", appStatus.LocalizedDisplayName), appStatus);
        }
        public ApplicationModel ListarApp(int indicePagina = 1, bool defCacheResult = false)
        {
            List<Application> lstApp = new List<Application>();
            int totalLinhas = 0;

            AppDetailView[] applications = null;
            ApplicationModel appModel = null;

            if (defCacheResult)
            {
                ApplicationModel appModelResult = ObterCache<ApplicationModel>(WSAutenticacao.Dominio, WSAutenticacao.Usuario, string.Concat("usuario-app-", indicePagina, "-"));

                if (appModelResult != null)
                {
                    return appModelResult;
                }
            }

            try
            {
                applications = _sccmWS.GetApplications(ApplicationProperty.Id, null, ApplicationProperty.Id, "", MAX_REG, 0, true, ApplicationClassicDisplayName.ProgramPackageName, false, null, out totalLinhas);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na busca dos aplicativos", new Exception(ex.Message));
            }

            if (applications != null)
            {
                foreach (AppDetailView appV in applications)
                {
                    Application app = new Application();

                    app.CI_UniqueID = appV.ApplicationId;
                    app.LocalizedDisplayName = appV.Name;
                    appV.Description = appV.Description;
                    appV.AppVersion = appV.AppVersion;
                    appV.Category = appV.Category;
                    appV.Publisher = appV.Publisher;

                    lstApp.Add(app);
                }

                appModel = new ApplicationModel();
                appModel.TotalLinhas = totalLinhas;
                appModel.MaximoReg = MAX_REG;
                appModel.Applications = lstApp.ToArray();

                if (defCacheResult)
                {
                    GerarCache(WSAutenticacao.Dominio, WSAutenticacao.Usuario, string.Concat("usuario-app-", indicePagina, "-"), appModel);
                }
            }

            return appModel;
        }
        public Application DetalhesAplicativo(string ci_UniqueID, bool defCacheResult = false)
        {
            AppDetailView appDetalhe = null;
            AppDetalheAudit appDetAudit = null;
            Application app = new Application();

            if (defCacheResult)
            {
                Application appModelResult = ObterCache<Application>(WSAutenticacao.Dominio, WSAutenticacao.Usuario, "usuario-app-det-");

                if (appModelResult != null)
                {
                    return appModelResult;
                }
            }

            try
            {
                appDetalhe = _sccmWS.GetApplicationDetails(ci_UniqueID, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na busca do detalhe do aplicativo", new Exception(ex.Message));
            }

            if (appDetalhe != null)
            {
                app.CI_UniqueID = ci_UniqueID;
                app.LocalizedDisplayName = appDetalhe.Name;
                app.AppVersion = appDetalhe.AppVersion;
                app.Description = appDetalhe.Description;
                app.Category = appDetalhe.Category;
                app.Publisher = appDetalhe.Publisher;

                if (defCacheResult)
                {
                    GerarCache(WSAutenticacao.Dominio, WSAutenticacao.Usuario, "usuario-app-det-", appDetAudit);
                }
            }

            return app;
        }
        public AppInstalacaoStatusAudit AplicativoStatus(string ci_UniqueID, string usuario)
        {
            if (string.IsNullOrEmpty(ci_UniqueID))
            {
                throw new InvalidOperationException("Operação Inválida", new InvalidOperationException("ID do Aplicativo inválido ou vazio."));
            }

            string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");
            string json = string.Empty;

            IWMIResult wmiResult = null;

            #region SCCM API Requisicao

            if (string.IsNullOrWhiteSpace(sccmApiURL))
            {
                throw new InvalidOperationException("Operação Inválida", new Exception("Não foi possível buscar ou definir o endereço da chave 'SCCM_API_URL'"));
            }

            for (int indice = 0; indice < TOTAL_TENTATIVAS_SCCM_API; indice++)
            {
                sccmApiURL = string.Concat(sccmApiURL, "/api/usuario/implantacaoStatus");

                object data = new { usuario = usuario, ci_UniqueID = ci_UniqueID };

                json = WebApiHttp.RequisicaoPost(sccmApiURL, data);

                if (!string.IsNullOrWhiteSpace(json))
                    break;
            }

            #endregion

            if (!string.IsNullOrWhiteSpace(json))
            {
                wmiResult = WMIResultFactory.SerializarResult<SCCMUsuarioAppStatusInstalacao>(json);

                if (!wmiResult.Executado)
                {
                    throw new InvalidOperationException("Não foi possível buscar o status de instalação do App", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                }
            }
            else
            {
                throw new InvalidOperationException("Não foi possível buscar o status de instalação do App", null);
            }

            AppInstalacaoStatusAudit appStatusAudit = new AppInstalacaoStatusAudit();

            SCCMUsuarioAppStatusInstalacao sccmAppStatus = wmiResult.Result as SCCMUsuarioAppStatusInstalacao;

            appStatusAudit = new AppInstalacaoStatusAudit();
            appStatusAudit.ErrorCode = (int)sccmAppStatus.ErrorCode;
            appStatusAudit.StatusInstalacao = sccmAppStatus.State;
            return appStatusAudit;
        }
        public AppInstalacaoStatusDetalheAudit AplicativoStatusDetalhes(string ci_UniqueID, string usuario)
        {
            if (string.IsNullOrEmpty(ci_UniqueID))
            {
                throw new InvalidOperationException("Operação Inválida", new InvalidOperationException("ID do Aplicativo inválido ou vazio."));
            }

            IWMIResult wmiResult = null;
            SCCMUsuarioAppStatusInstalacao usuarioStatusInstApp = null;

            string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");
            string json = string.Empty;

            #region SCCM API Requisicao

            if (string.IsNullOrWhiteSpace(sccmApiURL))
            {
                throw new InvalidOperationException("Operação Inválida", new Exception("Não foi possível buscar ou definir o endereço da chave 'SCCM_API_URL'"));
            }

            for (int indice = 0; indice < TOTAL_TENTATIVAS_SCCM_API; indice++)
            {
                sccmApiURL = string.Concat(sccmApiURL, "/api/usuario/implantacaoStatus");

                object data = new { usuario = usuario, idApp = ci_UniqueID };

                try
                {
                    json = WebApiHttp.RequisicaoPost(sccmApiURL, data);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("API - Ocorreu um erro na requisição de busca do status de implantação.", new Exception(ex.InnerException != null ? ex.InnerException.Message : null));
                }

                if (!string.IsNullOrWhiteSpace(json))
                    break;
            }

            #endregion

            if (!string.IsNullOrWhiteSpace(json))
            {
                wmiResult = WMIResultFactory.SerializarResult<SCCMUsuarioAppStatusInstalacao>(json);

                if (!wmiResult.Executado)
                {
                    throw new InvalidOperationException("Não foi possível buscar o status de instalação do App", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                }
            }
            else
            {
                throw new InvalidOperationException("Não foi possível buscar o status de instalação do App", null);
            }

            usuarioStatusInstApp = wmiResult.Result as SCCMUsuarioAppStatusInstalacao;
            AppInstalacaoStatusDetalheAudit appStatusDetalheAudit = new AppInstalacaoStatusDetalheAudit();

            appStatusDetalheAudit.User = usuarioStatusInstApp.User;
            appStatusDetalheAudit.LocalizedDisplayName = usuarioStatusInstApp.SoftwareName;
            appStatusDetalheAudit.MachineName = usuarioStatusInstApp.MachineName;
            appStatusDetalheAudit.ComplianceState = usuarioStatusInstApp.ComplianceState;
            appStatusDetalheAudit.ErrorCode = (int)usuarioStatusInstApp.ErrorCode;
            appStatusDetalheAudit.ErrorMessage = usuarioStatusInstApp.ErrorMessage;
            appStatusDetalheAudit.StatusInstalacao = usuarioStatusInstApp.State;
            appStatusDetalheAudit.StateDetail = usuarioStatusInstApp.StateDetail;
            appStatusDetalheAudit.StartTime = usuarioStatusInstApp.StartTime.ToString("dd/MM/yyyy HH:mm:ss");

            return appStatusDetalheAudit;
        }
        private bool VerificarStatusInstalandoApp(string ci_uniqueId, string deviceId)
        {
            AppInstalacaoStatusAudit appStatus = AplicativoStatus(ci_uniqueId, deviceId);

            AppInstallationState enumStatus = (AppInstallationState)Enum.Parse(typeof(AppInstallationState), appStatus.StatusInstalacao);

            if (enumStatus == AppInstallationState.Started)
            {
                return true;
            }

            return false;
        }
        private ICache DefinirInstanciaCache()
        {
            try
            {
                ICache cache = null;

                string cacheAppSettings = ConfigurationManager.AppSettings.Get("CACHE_API_WS");

                if (string.IsNullOrEmpty(cacheAppSettings))
                {
                    cache = new CacheArquivoJSON();
                    _cacheDefinido = false;
                }
                else
                {
                    int tempoCache = Convert.ToInt32(cacheAppSettings);
                    cache = new CacheArquivoJSON(tempoCache);
                    _cacheDefinido = true;
                }

                return cache;
            }
            catch
            {
                throw new Exception("Erro de sistema", new Exception("Ocorreu um erro na definição do cache de busca de aplicativos do usuário"));
            }
        }
        private bool GerarCache<T>(string dominio, string usuario, string prefCache, T dados)
        {
            try
            {
                if (!_cacheDefinido)
                    return false;

                string chaveCache = string.Concat(dominio, "_", usuario);

                string json = dados.ToJson();

                bool definido = _cache.Definir(json, string.Concat(PREF_CACHE, prefCache, chaveCache));

                return definido;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private T ObterCache<T>(string dominio, string usuario, string prefCache) where T : class
        {
            try
            {
                if (!_cacheDefinido)
                    return null;

                string chaveCache = string.Concat(dominio, "_", usuario);

                T objeto = _cache.Obter<T>(string.Concat(PREF_CACHE, prefCache, chaveCache));

                return objeto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
    public class SCCMUsuarioAppStatusInstalacao
    {
        public string User { get; set; }
        public string MachineName { get; set; }
        public string SoftwareName { get; set; }
        public string ComplianceState { get; set; }
        public string State { get; set; }
        public string StateDetail { get; set; }
        public UInt32 ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
    }
}

