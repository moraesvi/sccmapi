using HelperComum;
using SCCM.Servico.Contratos;
using SCCM.Servico.PowerShell.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCCM.WindowsService.PowerShellExecucao
{
    public partial class PowerShellExecucao : ServiceBase
    {
        private static Runspace _runspace;

        private const int TENTATIVAS = 2;
        private const string PREF_RESULT_NOME = "SCCM_ECOMIGO_Result-";
        private const string PREF_ARQUIVO = "SCCM_ECOMIGO-";
        private const string RESULT_EXTENSAO_DEFAULT = "XML";
        private const string NOME_SERVICO = "PowerShellExecucao";

        private IParsePowerShell parsePowerShell = null;
        private IPowerShellResult powerShellResult = null;

        private Log log;
        private Thread threadProcessar;
        private AutoResetEvent threadEvents = new AutoResetEvent(false);

        public PowerShellExecucao()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("ServicoPSAgent"))
            {
                EventLog.CreateEventSource("ServicoPSAgent", "ServicoPSAgent-Log");
            }

            log = new Log("Servico-PowerShellExecucao");
        }
        protected override void OnStart(string[] args)
        {
            ThreadStart threadDelegate = new ThreadStart(Processar);
            threadProcessar = new Thread(threadDelegate);
            threadProcessar.Start();
        }
        protected override void OnStop()
        {
            log.Novo("Servico SCCM PowerShellExecucao parado");
            EventLog.WriteEntry("Servico SCCM TriggerForce parado", EventLogEntryType.Information);

            if ((threadProcessar != null) && (threadProcessar.IsAlive))
            {
                Thread.Sleep(5000);
                threadProcessar.Abort();
            }
        }
        public void Processar()
        {
            try
            {
                TraceSource PSCode = new TraceSource("Servico-PowerShellExecucao");

                log.Novo("Servico SCCM PowerShellExecucao iniciado");

                string logMsg = string.Empty;

                string diretorioArquivos = ConfigurationManager.AppSettings.Get("PSDiretorioArquivos");

                for (;;)
                {
                    string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

                    string diretorioDllParse = Path.Combine(currentDir, "DLLParse");//Caminho Dlls Parse;
                    string diretorioDllResult = Path.Combine(currentDir, "DLLResultado");//Caminho Dll Resultado;

                    if (!Directory.Exists(diretorioDllParse))
                    {
                        Directory.CreateDirectory(diretorioDllParse);
                        throw new InvalidOperationException("Caminho Inválido", new Exception("Não foi encontrado a pasta de conversores PowerShell!"));
                    }

                    FileInfo[] arrayDllParse = ObterArrayDll(diretorioDllParse, true, "Conversor inválido", "Não foi encontrado um conversor de PowerShell!");

                    foreach (FileInfo dll in arrayDllParse)
                    {
                        parsePowerShell = ObterInstanciaDll(dll, parsePowerShell, true, "Conversor inválido", string.Concat("Não foi possível criar uma instância do conversor: ", dll.Name));

                        DirectoryInfo diretorio = new DirectoryInfo(diretorioArquivos);
                        FileInfo[] arrayArquivo = diretorio.GetFiles(string.Concat("*.", parsePowerShell.ExtensaoArquivo));

                        if (arrayArquivo.Count() == 0)
                        {
                            parsePowerShell.Dispose();
                            parsePowerShell = null;
                            if (powerShellResult != null)
                            {
                                powerShellResult.Dispose();
                                powerShellResult = null;
                            }

                            continue;
                        }

                        foreach (FileInfo arquivo in arrayArquivo)
                        {
                            if (!arquivo.Name.ToUpper().Contains(PREF_ARQUIVO.ToUpper()))//Somente arquivos gerados pela API
                                continue;

                            if (!Conectado())
                            {
                                ConectarPowerShell();
                                PSRunspace.DefinirRunspace(_runspace, PSCode);
                            }

                            try
                            {
                                bool arquivoValido = ValidarArquivo(parsePowerShell, arquivo);

                                if (!arquivoValido)
                                {
                                    arquivo.Delete();
                                    continue;
                                }

                                RealizarComandoPoweShell(parsePowerShell, arquivo, diretorioArquivos, diretorioDllResult);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException("Ocorreu um erro na conversão para PowerShell.", ex.InnerException);
                            }
                        }
                    }

                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", ex.Message, "\n", ex.InnerException), ex.StackTrace);
                log.Novo(erroLogMsg);

                EventLog.WriteEntry(string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", ex.StackTrace), EventLogEntryType.Error);
            }
        }
        private void RealizarComandoPoweShell(IParsePowerShell parsePowerShell, FileInfo arquivo, string diretorioArquivos, string diretorioDllResult)
        {
            try
            {
                string nomeLog = string.Empty;
                string dataProcessamento = string.Empty;
                string extensaoArquivoResult = string.Empty;

                bool PSExecutado = false;

                KeyValuePair<bool, string> result = new KeyValuePair<bool, string>(false, "");
                string logMsg = string.Empty;

                EventLogEntryType eventLogTipo = EventLogEntryType.SuccessAudit;

                string diretorioResultado = Path.Combine(diretorioArquivos, "PSResultLog");

                #region Dll Resultado

                if (!Directory.Exists(diretorioResultado))
                {
                    Directory.CreateDirectory(diretorioResultado);
                }

                if (Directory.Exists(diretorioDllResult))
                {
                    FileInfo[] arrayDllResult = ObterArrayDll(diretorioDllResult, false);

                    if (arrayDllResult.Count() > 0)
                    {
                        ValidarImplementacoesInterface<IPowerShellResult>(arrayDllResult, 1, true, "Dll Resultado Inválido", "Não é permitido incluir mais de uma dll de conversão");

                        FileInfo dllResult = arrayDllResult.FirstOrDefault();
                        powerShellResult = ObterInstanciaDll<IPowerShellResult>(dllResult, powerShellResult, true, "Dll Resultado Inválido", string.Concat("Não foi possível criar uma instância da dll de resultado: ", dllResult.Name));
                    }
                }

                #endregion

                string comandoPS = parsePowerShell.ObterComandoPS(arquivo.FullName);
                string dispositivo = parsePowerShell.Dispositivo.ToUpper();

                for (int indice = 1; indice <= TENTATIVAS; indice++)
                {
                    if (powerShellResult == null)
                    {
                        result = PSRunspace.ExecutarResultXML(comandoPS, parsePowerShell.Chave, dispositivo);
                        extensaoArquivoResult = RESULT_EXTENSAO_DEFAULT.ToLower();
                    }
                    else
                    {
                        result = PSRunspace.ExecutarResult(comandoPS, dispositivo, parsePowerShell.Chave, powerShellResult);
                        extensaoArquivoResult = powerShellResult.ExtensaoArquivo;
                    }

                    if (result.Key)
                    {
                        logMsg = GerarLog(LogStatus.Sucesso, string.Concat("Dispositivo: ", dispositivo));
                        eventLogTipo = EventLogEntryType.Information;
                        PSExecutado = true;
                        nomeLog = "OK";

                        break;
                    }
                }

                if (!PSExecutado)
                {
                    logMsg = GerarLog(LogStatus.NaoSucesso, string.Concat("Dispositivo: ", dispositivo), TENTATIVAS);
                    eventLogTipo = EventLogEntryType.Warning;

                    nomeLog = "NAO_EXEC";
                }

                arquivo.Delete();

                #region Resultado Processamento Arquivo

                string arquivoResult = string.Concat(PREF_RESULT_NOME, parsePowerShell.Chave, "_", parsePowerShell.Dispositivo, "_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".", extensaoArquivoResult);
                diretorioResultado = Path.Combine(diretorioResultado, arquivoResult);

                using (FileStream fs = File.Create(diretorioResultado)) ;
                File.WriteAllText(diretorioResultado, result.Value, Encoding.UTF8);

                log.Novo(logMsg, nomeLog);
                EventLog.WriteEntry(logMsg, eventLogTipo);

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private bool ValidarArquivo(IParsePowerShell parsePowerShell, FileInfo arquivo)
        {
            bool extensaoValido = false;
            bool valido = false;

            try
            {
                extensaoValido = parsePowerShell.ExtensaoValido(arquivo.FullName);

                if (!extensaoValido)
                {
                    string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", "Extensao arquivo inválido."));
                    log.Novo(erroLogMsg, "ARQ_INVALIDO");
                }

                valido = parsePowerShell.Validar(arquivo.FullName);

                if (!valido)
                {
                    string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", "Arquivo inválido."));
                    log.Novo(erroLogMsg, "ARQ_INVALIDO");
                }
            }
            catch (Exception ex)
            {
                string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", ex.Message, "\n", ex.InnerException), ex.StackTrace);
                log.Novo(erroLogMsg, "ARQ_INVALIDO");
            }

            return (valido && extensaoValido);
        }
        private bool ValidarImplementacoesInterface<T>(FileInfo[] dlls, int maximoImplemetacoes, bool gerarException = false, string msgResult = null, string innerExcetion = null) where T : class
        {
            int total = 0;

            foreach (FileInfo dll in dlls)
            {
                Assembly assembly = Assembly.LoadFile(dll.FullName);

                bool result = typeof(T).IsAssignableFrom(assembly.ExportedTypes.FirstOrDefault());
                if (result)
                {
                    total++;

                    if (total > maximoImplemetacoes)
                    {
                        break;
                    }
                }
            };

            if (total > maximoImplemetacoes)
            {
                if (gerarException)
                    throw new InvalidOperationException(msgResult, new Exception(innerExcetion));

                return false;
            }

            return true;
        }
        private T ObterInstanciaDll<T>(FileInfo dll, T instance, bool gerarException = false, string exception = null, string innerException = null) where T : class
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(dll.FullName);

                bool implementaInterf = typeof(T).IsAssignableFrom(assembly.ExportedTypes.FirstOrDefault());

                if (implementaInterf)
                {
                    try
                    {
                        if (instance == null)
                        {
                            instance = (T)ObterInstancia(instance, assembly.ExportedTypes.FirstOrDefault());
                        }
                        //Dll diferente - Nova Instância.
                        else if (instance.GetType().FullName != assembly.ExportedTypes.FirstOrDefault().FullName)
                        {
                            instance = (T)ObterInstancia(instance, assembly.ExportedTypes.FirstOrDefault());
                        }
                    }
                    catch
                    {
                        if (gerarException)
                            throw new InvalidOperationException(exception, new Exception(innerException));

                        return null;
                    }
                }

                return instance;
            }
            catch
            {
                if (gerarException)
                {
                    throw new InvalidOperationException(exception, new Exception(innerException));
                }

                return null;
            }
        }
        private FileInfo[] ObterArrayDll(string diretorio, bool gerarException = false, string exception = null, string innerException = null)
        {
            DirectoryInfo diretorioDllParse = new DirectoryInfo(diretorio);
            FileInfo[] arrayDllParse = diretorioDllParse.GetFiles("*.dll");

            if (arrayDllParse == null || arrayDllParse.Length == 0)
            {
                if (gerarException)
                {
                    throw new InvalidOperationException(exception, new Exception(innerException));
                }
            }

            return arrayDllParse;
        }
        private bool Conectado()
        {
            if (_runspace != null)
            {
                if (_runspace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    return true;
                }
            }

            return false;
        }
        private void ConectarPowerShell()
        {
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo();

            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;

            //Timeout 1min
            connectionInfo.OpenTimeout = 60000;
            connectionInfo.OperationTimeout = 60000;
            connectionInfo.OpenTimeout = 60000;
            connectionInfo.CancelTimeout = 10000;
            connectionInfo.IdleTimeout = 60000;

            _runspace = RunspaceFactory.CreateRunspace();
            _runspace.Open();

            RunspaceInvoke rsInvoker = new RunspaceInvoke(_runspace);
            rsInvoker.Invoke("Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted");
        }        
        private static object ObterInstancia(object instance, Type iImplementacoes)
        {
            if (instance == null)
            {
                instance = CriarInstancia(iImplementacoes);
            }
            else if (instance.GetType().Name != iImplementacoes.Name)
            {
                instance = CriarInstancia(iImplementacoes);
            }

            return instance;
        }
        public static object CriarInstancia(Type type)
        {
            var ctor = type.GetConstructors()
                           .FirstOrDefault(c => c.GetParameters().Length > 0);

            if (ctor == null)
            {
                return Activator.CreateInstance(type);
            }

            var result =
                ctor.Invoke
                    (ctor.GetParameters()
                        .Select(p =>
                            p.HasDefaultValue ? p.DefaultValue :
                            p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null
                                ? Activator.CreateInstance(p.ParameterType)
                                : null
                        ).ToArray()
                    );

            return result;
        }

        #region Log
        private string GerarLog(LogStatus status, string mensagem)
        {
            return Log(status, mensagem, null, null);
        }
        private string GerarLog(LogStatus status, string mensagem, string stackTrace)
        {
            return Log(status, mensagem, null, stackTrace);
        }
        private string GerarLog(LogStatus status, string mensagem, int totalExecucoes)
        {
            return Log(status, mensagem, totalExecucoes, null);
        }
        private string Log(LogStatus status, string mensagem, int? totalExecucoes, string stackTrace)
        {
            string msgFinal = string.Empty;
            switch (status)
            {
                case LogStatus.Sucesso:
                    msgFinal = "1 - Executado com sucesso.\n\n";
                    break;
                case LogStatus.NaoSucesso:
                    msgFinal = string.Concat("2 - Execução não realizada, foram realizadas ", totalExecucoes, " tentativas.\n\n");
                    break;
                case LogStatus.ErroServico:
                    msgFinal = string.Concat("3 - Ocorreu um erro no serviço.\n\n");
                    break;
            }

            if (!string.IsNullOrEmpty(stackTrace))
            {
                msgFinal = string.Concat(msgFinal, mensagem, "\n\n", stackTrace);
            }
            else
            {
                msgFinal = string.Concat(msgFinal, mensagem);
            }

            return msgFinal;
        }
        private enum LogStatus
        {
            Sucesso = 1,
            NaoSucesso = 2,
            ErroServico = 3
        }
        #endregion

    }
}
