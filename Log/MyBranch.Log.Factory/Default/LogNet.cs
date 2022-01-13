using log4net;
using MyBranch.Log.Dominio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBranch.Log.Factory
{
    public class LogNet : IMyBranchLog
    {
        private const string LOG_DESC = "EComigo_LogNet";
        public static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void NomeArquivoLogDefault()
        {
            BuidLogNet();
        }
        public void NomeArquivoLog(string nome)
        {
            BuidLogNet(nome);
        }
        public bool GravarLog(object objeto)
        {
            _log.Error(objeto);
            return true;
        }
        public bool GravarLog(string msgLog)
        {
            string[] linhas = msgLog.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            linhas.ToList().ForEach(linha =>
            {
                _log.Error(linha);
            });

            return true;
        }
        public bool GravarLog(object objeto, Exception exception)
        {
            _log.Error(objeto, exception);
            return true;
        }
        public bool GravarLog(string msgLog, Exception exception)
        {
            string[] linhas = msgLog.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            linhas.ToList().ForEach(linha =>
            {
                _log.Error(linha);
            });

            _log.Error(null, exception);

            return true;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _log.Logger.Repository.Shutdown();
        }

        #region Metodos Privados
        private void BuidLogNet(string nomeArquivo = null)
        {
            try
            {
                string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string nomeArquivoDef = LOG_DESC;

                if (!string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    nomeArquivoDef = string.Concat(LOG_DESC, "_", nomeArquivo);
                }

                log4net.GlobalContext.Properties["LogFileName"] = System.IO.Path.Combine(currentDir, "Logs", "Exception", nomeArquivoDef);
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na biblioteca de lo Log4Net", ex.InnerException);
            }
        }
        #endregion
    }
}
