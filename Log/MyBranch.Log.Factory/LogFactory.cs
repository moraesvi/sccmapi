using HelperComum;
using MyBranch.Log.Dominio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace MyBranch.Log.Factory
{
    public static class LogFactory
    {
        private static List<Assembly> lstAssemblyLog = null;

        public static IMyBranchLog[] ReflexInstanciar(string caminhoAssemblyLog = null)
        {
            string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrWhiteSpace(caminhoAssemblyLog))
            {
                currentDir = caminhoAssemblyLog;
            }

            CriacaoDiretorios(currentDir);

            string diretorioAssemblyLog = Path.Combine(currentDir, "Ext");//Caminho Dlls Log

            if (!Directory.Exists(diretorioAssemblyLog))
            {
                return new IMyBranchLog[] { };
            }

            FileInfo[] arrayAssemblyParse = ObterArrayAssembly(diretorioAssemblyLog);
            IMyBranchLog assemblyLog = null;

            List<IMyBranchLog> lstAssemblyLog = new List<IMyBranchLog>();

            foreach (FileInfo dll in arrayAssemblyParse)
            {
                assemblyLog = ObterInstanciaAssembly(dll, assemblyLog, true);

                if (assemblyLog != null)
                {
                    lstAssemblyLog.Add(assemblyLog);
                }
            }

            return lstAssemblyLog.ToArray();
        }
        public static IMyBranchLogEmail[] ReflexMailInstanciar(string caminhoAssemblyLog = null)
        {
            string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrWhiteSpace(caminhoAssemblyLog))
            {
                currentDir = caminhoAssemblyLog;
            }

            CriacaoDiretorios(currentDir);

            string diretorioAssemblyLog = Path.Combine(currentDir, "Ext");//Caminho Dlls Log 

            if (!Directory.Exists(diretorioAssemblyLog))
            {
                return new IMyBranchLogEmail[] { };
            }

            FileInfo[] arrayAssemblyParse = ObterArrayAssembly(diretorioAssemblyLog);
            IMyBranchLogEmail assemblyLog = null;

            List<IMyBranchLogEmail> lstAssemblyLog = new List<IMyBranchLogEmail>();

            foreach (FileInfo dll in arrayAssemblyParse)
            {
                assemblyLog = ObterInstanciaAssembly(dll, assemblyLog, true);

                if (assemblyLog != null)
                {
                    lstAssemblyLog.Add(assemblyLog);
                }
            }

            return lstAssemblyLog.ToArray();
        }
        public static void CriacaoDiretorios(string caminhoAssemblyLog = null)
        {
            string currentDirLogs = Path.Combine(caminhoAssemblyLog, "Logs");

            if (!Directory.Exists(currentDirLogs))
            {
                try
                {
                    Directory.CreateDirectory(currentDirLogs);

                    currentDirLogs = Path.Combine(currentDirLogs, "Exception");

                    if (!Directory.Exists(currentDirLogs))
                        Directory.CreateDirectory(currentDirLogs);
                }
                catch {}
            }
        }
        public static void GravarLog(IMyBranchLog log, string msgLog)
        {
            try
            {
                log.NomeArquivoLogDefault();
                log.GravarLog(msgLog);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, string msgLog, string nomeArquivoLog)
        {
            try
            {
                log.NomeArquivoLog(nomeArquivoLog);
                log.GravarLog(msgLog);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, object objetoLog)
        {
            try
            {
                log.NomeArquivoLogDefault();
                log.GravarLog(objetoLog);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, object objetoLog, string nomeArquivoLog)
        {
            try
            {
                log.NomeArquivoLog(nomeArquivoLog);
                log.GravarLog(objetoLog);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, string msgLog, Exception exception)
        {
            try
            {
                log.NomeArquivoLogDefault();
                log.GravarLog(msgLog, exception);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, string msgLog, string nomeArquivo, Exception exception)
        {
            try
            {
                log.NomeArquivoLog(nomeArquivo);
                log.GravarLog(msgLog, exception);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, object objLog, Exception exception)
        {
            try
            {
                log.NomeArquivoLogDefault();
                log.GravarLog(objLog, exception);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog log, object objLog, string nomeArquivo, Exception exception)
        {
            try
            {
                log.NomeArquivoLog(nomeArquivo);
                log.GravarLog(objLog, exception);
            }
            catch { }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, string msgLog)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLogDefault();
                assemblyLog.GravarLog(msgLog);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, string nomeArquivo, string msgLog)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLog(nomeArquivo);
                assemblyLog.GravarLog(msgLog);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, object objetoLog)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLogDefault();
                assemblyLog.GravarLog(objetoLog);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, string nomeArquivo, object objetoLog)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLog(nomeArquivo);
                assemblyLog.GravarLog(objetoLog);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, string msgLog, Exception exception)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLogDefault();
                assemblyLog.GravarLog(msgLog, exception);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, string msgLog, string nomeArquivo, Exception exception)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLog(nomeArquivo);
                assemblyLog.GravarLog(msgLog, exception);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, object objLog, Exception exception)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLogDefault();
                assemblyLog.GravarLog(objLog, exception);
            }
        }
        public static void GravarLog(IMyBranchLog[] arrayLog, object objLog, string nomeArquivo, Exception exception)
        {
            foreach (IMyBranchLog assemblyLog in arrayLog)
            {
                assemblyLog.NomeArquivoLog(nomeArquivo);
                assemblyLog.GravarLog(objLog, exception);
            }
        }
        public static void EnviarEmail(IMyBranchLogEmail[] arrayEmail, string usuario, SecureString password, string hostSMTP, string emailFrom, string[] emailsPara, string assunto, string conteudoEmail, int portaSMTP = 25, int timeoutConnection = 10)
        {
            foreach (IMyBranchLogEmail assemblyEmail in arrayEmail)
            {
                assemblyEmail.Configuracao(usuario, password, hostSMTP);
                assemblyEmail.Enviar(emailFrom, emailsPara, assunto, conteudoEmail);
            }
        }
        public static void Dispose()
        {
            if (lstAssemblyLog == null)
                return;

            foreach (Assembly assembly in lstAssemblyLog)
            {
                GC.SuppressFinalize(assembly);
            }
        }

        #region Metodos Privados
        private static FileInfo[] ObterArrayAssembly(string diretorio, bool gerarException = false, string exception = null, string innerException = null)
        {
            DirectoryInfo diretorioDllParse = new DirectoryInfo(diretorio);
            FileInfo[] arrayAssembly = diretorioDllParse.GetFiles("*.dll");

            if (arrayAssembly == null || arrayAssembly.Length == 0)
            {
                if (gerarException)
                {
                    throw new InvalidOperationException(exception, new Exception(innerException));
                }
            }

            return arrayAssembly;
        }
        private static T ObterInstanciaAssembly<T>(FileInfo dll, T instance, bool gerarException = false) where T : class
        {
            try
            {
                lstAssemblyLog = new List<Assembly>();

                Assembly assembly = Assembly.Load(File.ReadAllBytes(dll.FullName));

                bool implementaInterf = typeof(T).IsAssignableFrom(assembly.ExportedTypes.FirstOrDefault());

                if (implementaInterf)
                {
                    if (instance == null)
                    {
                        instance = (T)ObterInstancia(instance, assembly.ExportedTypes.FirstOrDefault());
                    }
                    //Assembly diferente - Nova Instância.
                    else if (instance.GetType().FullName != assembly.ExportedTypes.FirstOrDefault().FullName)
                    {
                        instance = (T)ObterInstancia(instance, assembly.ExportedTypes.FirstOrDefault());
                    }

                    return instance;
                }

                return instance;
            }
            catch (Exception)
            {
                if (gerarException)
                {
                    throw;
                }

                return null;
            }
        }
        private static object ObterInstancia(object instance, Type iImplementacoes)
        {
            if (instance == null)
            {
                instance = Helper.CriarInstancia(iImplementacoes);
            }
            else if (instance.GetType().Name != iImplementacoes.Name)
            {
                instance = Helper.CriarInstancia(iImplementacoes);
            }

            return instance;
        }
        #endregion
    }
}
