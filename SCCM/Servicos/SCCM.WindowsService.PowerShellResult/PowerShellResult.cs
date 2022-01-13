using HelperComum;
using SCCM.Servico.Dominio;
using SCCM.Servico.PowerShell.Infra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCCM.WindowsService.PowerShellResult
{
    public partial class PowerShellResult : ServiceBase
    {
        private const string PREF_RESULT_NOME = "SCCM_ECOMIGO_Result-";
        private const string NOME_SERVICO = "PowerShellResult";

        private PSSCCMConexao _runspace;

        private Log log;
        private Thread threadProcessar;
        private AutoResetEvent threadEvents = new AutoResetEvent(false);

        public PowerShellResult()
        {
            InitializeComponent();

            //if (!EventLog.SourceExists("ServicoPSAgentResult"))
            //{
            //    EventLog.CreateEventSource("ServicoPSAgentResult", "ServicoPSAgentResult-Log");
            //}

            log = new Log("Servico-PowerShellResult");
        }
        protected override void OnStart(string[] args)
        {
            ThreadStart threadDelegate = new ThreadStart(Processar);
            threadProcessar = new Thread(threadDelegate);
            threadProcessar.Start();
        }
        protected override void OnStop()
        {
            log.Novo("Servico SCCM PowerShellResult parado");
            EventLog.WriteEntry("Servico SCCM PowerShellResult parado", EventLogEntryType.Information);

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
                for (;;)
                {
                    TraceSource PSCode = new TraceSource("Servico-PowerShellResult");

                    log.Novo("Servico SCCM PowerShellResult iniciado");

                    string logMsg = string.Empty;

                    string diretorioArquivos = ConfigurationManager.AppSettings.Get("PSDiretorioArquivos");
                    string diretorioArquivosLocal = ConfigurationManager.AppSettings.Get("PSDiretorioArquivosLocal");

                    if (_runspace == null)
                    {
                        ConectarPowerShell();
                    }
                    else if (!_runspace.Conectado)
                    {
                        ConectarPowerShell();
                    }

                    string diretorioArquivosResult = Path.Combine(diretorioArquivos, "PSResultLog");
                    string diretorioResultado = Path.Combine(diretorioArquivosLocal, "PSResultados");

                    if (!Directory.Exists(diretorioResultado))
                    {
                        Directory.CreateDirectory(diretorioResultado);
                    }

                    PSArquivoResult[] arrayArquivo = PSRunspace.ExecutarBuscaArquivo(diretorioArquivosResult);

                    foreach (PSArquivoResult arquivo in arrayArquivo)
                    {
                        try
                        {
                            if (!arquivo.Nome.ToUpper().Contains(PREF_RESULT_NOME.ToUpper()))//Somente arquivos gerados pela API
                                continue;

                            string textoArquivo = arquivo.Conteudo;
                            string diretorioResultadoLocal = Path.Combine(diretorioResultado, arquivo.Nome);

                            using (FileStream fs = File.Create(diretorioResultadoLocal)) ;
                            File.WriteAllText(diretorioResultadoLocal, textoArquivo, Encoding.UTF8);
                        }
                        catch (Exception ex)
                        {
                            new Exception(string.Concat("Ocorreu um erro no processamento do arquivo/resultdo: ", arquivo.Nome), ex.InnerException);
                        }
                    }

                    Thread.Sleep(6000);
                }
            }
            catch (Exception ex)
            {
                string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", ex.Message, "\n", ex.InnerException), ex.StackTrace);
                log.Novo(erroLogMsg);

                EventLog.WriteEntry(string.Concat("ERRO SERVIÇO - ", NOME_SERVICO, "\n", ex.StackTrace), EventLogEntryType.Error);
            }
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

        private void ConectarPowerShell()
        {
            try
            {
                //TODO: Buscar de uma lista de servidores SCCM.            
                string servidor = ConfigurationManager.AppSettings.Get("PSDominio");
                SCCMDominio dominio = SCCMDominio.Local;
                Enum.TryParse<SCCMDominio>(servidor, true, out dominio);

                _runspace = new PSSCCMConexao(dominio);
                _runspace.DefinirRunspacePS();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

    }
}
