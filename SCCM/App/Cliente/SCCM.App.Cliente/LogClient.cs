using HelperComum;
using SCCM.App.Dominio;
using AppHelper = SCCM.App.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MyBranch.Log.Dominio;
using MyBranch.Log.Factory;

namespace SCCM.App.Cliente
{
    public class LogClient
    {
        private const int TOTAL_TENTATIVAS_SCCM_API = 3;
        public static void GerarLogLocal(SCCMClientLog clientLog)
        {
            if (clientLog == null)
                return;

            string logFormat = clientLog.LogFormat();

            Task.WaitAll(GravarLog(logFormat, string.Concat("SCCM_Client-", clientLog.PC), clientLog.LogException));
        }
        public static void HttpGerarLog(SCCMClientLog clientLog)
        {
            HttpInserirLog(clientLog);
        }
        private static bool HttpInserirLog(SCCMClientLog clientLog)
        {
            try
            {
                string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/sccmClientLog");

                AppHelper.WebApiHttp.HttpJsonRequisicaoPost(TOTAL_TENTATIVAS_SCCM_API, sccmApiURL, clientLog);

                return true;
            }
            catch (InvalidOperationException ex)
            {
                Exception[] exceptions = new Exception[] { clientLog.LogException, ex };

                foreach (Exception expt in exceptions)
                {
                    clientLog.DefinirException(expt);
                    GerarLogLocal(clientLog);
                }

                return false;
            }
            catch (Exception ex)
            {
                ex = new Exception("Não foi possível realizar a requisição de inserção de Log", ex.InnerException);

                Exception[] exceptions = new Exception[] { clientLog.LogException, ex };

                foreach (Exception expt in exceptions)
                {
                    clientLog.DefinirException(expt);
                    GerarLogLocal(clientLog);
                }

                return false;
            }
        }
        public static async Task GravarLog(string log, Exception exception = null)
        {
            await LogGravar(log, null, exception);
        }
        public static async Task GravarLog(string log, string nomeArquivo, Exception exception = null)
        {
            await LogGravar(log, nomeArquivo, exception);
        }

        #region Metodos Privados
        private static async Task LogGravar(string log, string nomeArquivo, Exception exception)
        {
            bool gerarNomeArquivo = !string.IsNullOrWhiteSpace(nomeArquivo);

            try
            {
                await Task.Factory.StartNew(except =>
                {
                    IMyBranchLog[] logs = LogFactory.ReflexInstanciar();

                    if (logs == null || logs.Count() == 0)
                    {
                        LogFactory.GravarLog(new LogNet(), log, nomeArquivo, (Exception)except);
                    }
                    else if (except == null)
                    {
                        LogFactory.GravarLog(logs, log);
                    }
                    else
                    {
                        LogFactory.GravarLog(logs, log, (Exception)except);
                    }

                    LogFactory.Dispose();
                }, exception);
            }
            catch (Exception ex)
            {
                Exception except = ex.GetBaseException();

                Exception[] exceptions = new Exception[] { exception, except };

                await Task.Factory.StartNew(excepts =>
                {
                    foreach (Exception expt in (Exception[])excepts)
                    {
                        if (gerarNomeArquivo)
                            LogFactory.GravarLog(new LogNet(), ex.Message, nomeArquivo, (Exception)expt);
                        else
                            LogFactory.GravarLog(new LogNet(), ex.Message, (Exception)expt);
                    }
                }, exceptions);
            }
        }
        #endregion
    }
}
