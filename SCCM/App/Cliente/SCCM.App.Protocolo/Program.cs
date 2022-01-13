using SCCM.App.Cliente;
using SCCM.App.Dominio;
using AppHelper = SCCM.App.Helper;
using System.Windows.Forms;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Diagnostics;
using log4net;

namespace SCCM.App.Protocolo
{
    public class Program
    {
        private const int TOTAL_TENTATIVAS_SCCM_API = 3;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(SCCMClient_UnhandledException);
            string sccmParam = null;

            #region Parametro 

            if (args != null)
            {
                if (args.Length > 0)
                {
                    string[] argsParamSccm = args.FirstOrDefault().Split(':');

                    sccmParam = argsParamSccm.LastOrDefault()
                                             .Trim();
                }
            }

            #endregion

            if (!string.IsNullOrEmpty(sccmParam))
            {
                if (sccmParam.IndexOf("desinstalar", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SCCMClientProtocolo sccmClientProt = HttpClientProtocolo();
                    URLProtocol urlProtocol = new URLProtocol(sccmClientProt.Protocolo, sccmClientProt.Descricao);

                    urlProtocol.Remover();
                }
                else
                {
                    string idApp = sccmParam.Split('|').FirstOrDefault();
                    string chave = sccmParam.Split('|').LastOrDefault();

                    HttpDefinirClientInstalado(chave);

                    ClienteSDK clienteSDK = new ClienteSDK(idApp, chave);
                    string psScriptSDK = clienteSDK.HttpClientSDKScriptApp();

                    ClienteSDKPowerShell clientSDK = new ClienteSDKPowerShell();
                    ClientSDKInstalResult sdkClientResult = clientSDK.ExecutarScriptApp(chave, psScriptSDK);

                    if (!sdkClientResult.OK)
                    {
                        ExibirMessageBox(sdkClientResult.MsgResultado, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                SCCMClientProtocolo sccmClientProt = HttpClientProtocolo();
                string caminhoSCCMClient = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SCCM_API URLProtocolo.exe");

                URLProtocol urlProtocol = new URLProtocol(sccmClientProt.Protocolo, sccmClientProt.Descricao, caminhoSCCMClient);

                urlProtocol.Remover();
                urlProtocol.Registrar();
            }
        }

        #region Metodos Privados
        private static void SCCMClient_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = (Exception)e.ExceptionObject;

                DateTime dataLog = DateTime.Now;
                string dominio = string.Empty;
                string versaoWindows = string.Empty;

                string usuario = WindowsIdentity.GetCurrent().Name;
                string nomePC = System.Environment.MachineName;

                try
                {
                    versaoWindows = new ClientHost().SistemaOperacional();
                }
                catch (Exception ex)
                {
                    versaoWindows = string.Concat("Não foi possível definir a versão do Windows - ", ex.Message);
                }

                bool windows64Bits = System.Environment.Is64BitOperatingSystem;
                string caminhoAplicacao = System.AppDomain.CurrentDomain.BaseDirectory;

                SCCMClientLog clientLog = new SCCMClientLog(usuario, nomePC, dataLog, versaoWindows, windows64Bits, caminhoAplicacao);
                clientLog.DefinirException(exception);

                LogClient.HttpGerarLog(clientLog);

                ExibirMessageBox(string.Concat(exception.Message, "\n\n", exception.InnerException != null ? exception.InnerException.Message : ""), MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Exception exception = (Exception)e.ExceptionObject;

                Exception[] exceptions = new Exception[] { exception, ex };

                foreach (Exception except in exceptions)
                {
                    SCCMClientLog clientLog = new SCCMClientLog();
                    clientLog.DefinirException(ex);

                    LogClient.HttpGerarLog(clientLog);
                }

                ExibirMessageBox(string.Concat(ex.Message, "\n\n", ex.InnerException != null ? ex.InnerException.Message : ""), MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
        private static void TimerConsole(string msg, int tempo = 3000, bool erro = false)
        {
            if (erro)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(msg);
            Console.ReadLine();
            //Timer timerConsole = new Timer(FinalizarTimerConsole, null, tempo, tempo);
        }
        private static void FinalizarTimerConsole(object state)
        {
            Environment.Exit(0);
        }
        private static void ExibirMessageBox(string msg, MessageBoxIcon msgIcon)
        {
            MessageBox.Show(msg, "SCCM Client", MessageBoxButtons.OK, msgIcon);
        }
        private static SCCMClientProtocolo HttpClientProtocolo()
        {
            try
            {
                IWMIResult wmiResult = null;
                SCCMClientProtocolo sccmClientProtocolo = null;

                string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/URLProtocolo");

                string json = AppHelper.WebApiHttp.HttpJsonRequisicao(TOTAL_TENTATIVAS_SCCM_API, sccmApiURL);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    wmiResult = WMIResultFactory.SerializarResult<SCCMClientProtocolo>(json);

                    if (!wmiResult.Executado)
                    {
                        throw new InvalidOperationException("Não foi possível buscar o script SCCMClient Protocolo", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                    }
                }
                else
                {
                    throw new InvalidOperationException("Não foi possível buscar script SCCMClient Protocolo", null);
                }

                sccmClientProtocolo = wmiResult.Result as SCCMClientProtocolo;

                return sccmClientProtocolo;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a requisição SCCMClient Protocolo", ex.InnerException);
            }
        }
        private static bool HttpDefinirClientInstalado(string chave)
        {
            try
            {
                IWMIResult wmiResult = null;
                SCCMClientInstal sccmClientInstal = new SCCMClientInstal();

                bool verificacaoGerado = false;

                sccmClientInstal.Chave = chave;
                sccmClientInstal.DataVerificacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                string sccmApiURL = ConfigurationManager.AppSettings.Get("SCCM_API_URL");

                sccmApiURL = string.Concat(sccmApiURL, "/api/util/verificacaoClient");

                string json = AppHelper.WebApiHttp.HttpJsonRequisicaoPost(TOTAL_TENTATIVAS_SCCM_API, sccmApiURL, sccmClientInstal);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    wmiResult = WMIResultFactory.SerializarResult<bool>(json);

                    if (!wmiResult.Executado)
                    {
                        throw new InvalidOperationException("Não foi possível gerar o arquivo de valicação do scccm client instalado", new Exception(wmiResult.Exception != null ? wmiResult.Exception.MsgDetalhado : null));
                    }
                }
                else
                {
                    throw new InvalidOperationException("Não foi possível buscar realizar a requisição de verificação sccm client", null);
                }

                verificacaoGerado = (bool)wmiResult.Result;

                return verificacaoGerado;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a requisição de verificação de cliente", ex.InnerException);
            }
        }
        #endregion        
    }
}
