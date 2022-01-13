using HelperComum;
using MyBranch.Log.Dominio;
using MyBranch.Log.Factory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum.Uteis
{
    public class SCCMLog
    {
        public static string GerarLogTituloTemplateSCCMWebApi(string nomeDeploy, string usuario, bool anonimo, string tipoAuth, string metodoHttp, string url, string controller, string caminhoApp)
        {
            return DefinirLogTituloTemplateSCCMWebApi(nomeDeploy, usuario, anonimo, tipoAuth, metodoHttp, DateTime.Now, url, controller, caminhoApp);
        }
        public static string GerarLogTituloTemplateSCCMClient(string usuario, string pc, DateTime dataLog, string versaoWindows, bool? win64Bits, string caminhoApp)
        {
            return DefinirLogTituloTemplateSCCMClient(usuario, pc, dataLog, versaoWindows, win64Bits, caminhoApp);
        }
        public static async Task GravarLog(string log, Exception exception = null)
        {
            await LogGravar(log, null, exception);
            await LogEnviarEmail(null, log, exception);
        }
        public static async Task GravarLog(string log, string nomeArquivo, Exception exception = null)
        {
            await LogGravar(log, nomeArquivo, exception);
            await LogEnviarEmail(nomeArquivo, log, exception);
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
                        if (gerarNomeArquivo)
                            LogFactory.GravarLog(new LogNet(), log, nomeArquivo, (Exception)except);
                        else
                            LogFactory.GravarLog(new LogNet(), log, (Exception)except);
                    }
                    else if (except == null)
                    {
                        if (gerarNomeArquivo)
                            LogFactory.GravarLog(logs, nomeArquivo, log);
                        else
                            LogFactory.GravarLog(logs, log);
                    }
                    else
                    {
                        if (gerarNomeArquivo)
                            LogFactory.GravarLog(logs, log, nomeArquivo, (Exception)except);
                        else
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

                        LogFactory.Dispose();
                    }
                }, exceptions);
            }
        }
        private static async Task LogEnviarEmail(string nomeArquivo, string conteudoEmail, Exception exception)
        {
            bool gerarNomeArquivo = !string.IsNullOrWhiteSpace(nomeArquivo);

            try
            {
                await Task.Factory.StartNew(except =>
                {
                    IMyBranchLogEmail[] logEmail = LogFactory.ReflexMailInstanciar();

                    if (logEmail != null)
                    {
                        if (logEmail.Count() > 0)
                        {
                            string assuntoEmail = "SCCM EComigoSantander_EmailLog";
                            StringBuilder sbEmail = new StringBuilder();

                            string defaultDns = Helper.ObterValorSettings("EMAIL_DEFAULDNS", true);
                            string smtp = Helper.ObterValorSettings("EMAIL_SMTP", true);
                            string usuario = Helper.ObterValorSettings("EMAIL_USUARIO", true);
                            string password = Helper.ObterValorSettings("EMAIL_PASSWORD", true);

                            string de = Helper.ObterValorSettings("EMAIL_DE", true);
                            string[] para = Helper.ObterValorSettings("EMAIL_PARA", true).Split(',');

                            sbEmail.AppendLine(conteudoEmail);

                            if (except != null)
                            {
                                sbEmail.AppendLine(exception.Message);
                                sbEmail.AppendLine(exception.InnerException().Message);
                                sbEmail.AppendLine(exception.StackTrace);
                                sbEmail.AppendLine(exception.InnerException().StackTrace);
                            }

                            LogFactory.EnviarEmail(logEmail, usuario, password.ToSecureString(), smtp, de, para, assuntoEmail, sbEmail.ToString());
                            LogFactory.Dispose();
                        }
                    }
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

                        LogFactory.Dispose();
                    }
                }, exceptions);
            }
        }
        private static string DefinirLogTituloTemplateSCCMClient(string usuario, string pc, DateTime dataLog, string versaoWindows, bool? win64Bits, string caminhoApp)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Concat("Hostname - ", pc));
            sb.AppendLine(dataLog.ToLongDateString());
            sb.AppendLine(dataLog.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("-------------------------------------------------------------------------------------");
            sb.AppendLine(usuario);
            sb.AppendLine(versaoWindows);
            sb.AppendLine(string.Concat(!win64Bits.HasValue ? "Sistema operacional de(Não definido)" : win64Bits.Value ? "Sistema Operacional de 64 bits" : "Sistema Operacional de 32 bits"));
            sb.AppendLine("-------------------------------------------------------------------------------------");
            sb.AppendLine(caminhoApp);
            sb.AppendLine("-------------------------------------------------------------------------------------");

            return sb.ToString();
        }
        private static string DefinirLogTituloTemplateSCCMWebApi(string nomeDeploy, string usuario, bool anonimo, string tipoAuth, string metodoHttp, DateTime dataLog, string url, string controller, string caminhoApp)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Concat("SCCM WebApi - ", nomeDeploy));
            sb.AppendLine(dataLog.ToLongDateString());
            sb.AppendLine(dataLog.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("-------------------------------------------------------------------------------------");
            sb.AppendLine(usuario);
            sb.AppendLine(string.Concat(anonimo ? "Acesso anônimo" : string.Concat("Acesso autenticado - ", tipoAuth)));
            sb.AppendLine(string.Concat("Método HTTP - ", metodoHttp.ToUpper()));
            sb.AppendLine("-------------------------------------------------------------------------------------");
            sb.AppendLine(url);

            if (!string.IsNullOrWhiteSpace(controller))
                sb.AppendLine(controller);

            sb.AppendLine("-------------------------------------------------------------------------------------");
            sb.AppendLine(caminhoApp);
            sb.AppendLine("-------------------------------------------------------------------------------------");

            return sb.ToString();
        }
        #endregion
    }
}
