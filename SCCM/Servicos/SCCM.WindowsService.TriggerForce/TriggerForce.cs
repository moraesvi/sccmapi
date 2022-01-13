using HelperComum;
using SCCM.PowerShell;
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
using System.Xml.Linq;

namespace SCCM.WindowsService.TriggerForce
{
    public partial class TriggerForce : ServiceBase
    {

        private const int TENTATIVAS = 3;
        private static Runspace _runspace;
        private Log log;

        Thread threadProcessar;
        AutoResetEvent threadEvents = new AutoResetEvent(false);

        public TriggerForce()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("Servico-Trigger"))
            {
                EventLog.CreateEventSource("Servico-Trigger", "Servico-Trigger-Log");
            }

            log = new Log("Servico-Trigger");
        }
        protected override void OnStart(string[] args)
        {
            ThreadStart threadDelegate = new ThreadStart(Processar);
            threadProcessar = new Thread(threadDelegate);
            threadProcessar.Start();
        }
        protected override void OnStop()
        {
            log.Novo("Servico SCCM TriggerForce parado");
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
                TraceSource PSCode = new TraceSource("servico-trigger");

                log.Novo("Servico SCCM TriggerForce iniciando");

                string logMsg = string.Empty;

                for (;;)
                {

                    DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings.Get("TriggerForce"));
                    FileInfo[] arrayArquivo = diretorio.GetFiles("*.xml");

                    foreach (FileInfo arquivo in arrayArquivo)
                    {
                        if (!Conectado())
                        {
                            ConectarPowerShell();
                        }

                        string contents = File.ReadAllText(arquivo.FullName);

                        StringBuilder sbXML = new StringBuilder();
                        XDocument xml = XDocument.Load(arquivo.FullName);

                        bool valido = XMLTriggerValido(xml);
                        bool forcado = false;
                        string dispositivo = string.Empty;

                        if (valido)
                        {
                            dispositivo = xml.Descendants("corpo").SingleOrDefault().Element("dispositivo").Value;

                            string wmi = xml.Descendants("corpo").SingleOrDefault().Element("script-1").Value;
                            string wmiClasse = xml.Descendants("corpo").SingleOrDefault().Element("script-2").Value;
                            string wmiTrigger = xml.Descendants("corpo").SingleOrDefault().Element("script-3").Value;

                            sbXML.AppendLine(wmi);
                            sbXML.AppendLine(wmiClasse);
                            sbXML.AppendLine(wmiTrigger);

                            for (int indice = 1; indice <= TENTATIVAS; indice++)
                            {

                                PSRequisicao.DefinirRunspace(_runspace, PSCode);
                                forcado = PSRequisicao.ExecutarResult(sbXML.ToString());

                                if (forcado)
                                {
                                    logMsg = GerarLog(LogStatus.Sucesso, string.Concat("Dispositivo: ", dispositivo.ToUpper()));
                                    break;
                                }
                            }

                            if (!forcado)
                            {
                                logMsg = GerarLog(LogStatus.NaoSucesso, string.Concat("Dispositivo: ", dispositivo.ToUpper()), TENTATIVAS);
                            }

                            arquivo.Delete();

                            log.Novo(logMsg);
                            EventLog.WriteEntry(logMsg, EventLogEntryType.Information);
                        }
                    }

                    if (Conectado())
                        _runspace.Close();

                    System.Threading.Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                string erroLogMsg = GerarLog(LogStatus.ErroServico, string.Concat("ERRO SERVIÇO - SCCM TriggerForce\n", ex.StackTrace));
                log.Novo(erroLogMsg);

                EventLog.WriteEntry(string.Concat("ERRO SERVIÇO - SCCM TriggerForce\n", ex.StackTrace), EventLogEntryType.Error);
            }
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
        }
        private bool XMLTriggerValido(XDocument xml)
        {
            bool valido = xml.Descendants("smsclient").SingleOrDefault().HasElements ? true : false;

            if (!valido)
                return false;

            valido = (xml.Descendants("smsclient").SingleOrDefault().Element("dispositivo").Value != null)
                     && (xml.Descendants("smsclient").SingleOrDefault().Element("script-1").Value != null)
                     && (xml.Descendants("smsclient").SingleOrDefault().Element("script-2").Value != null)
                     && (xml.Descendants("smsclient").SingleOrDefault().Element("script-3").Value != null)
                     ? true : false;

            return valido;
        }
        private string GerarLog(LogStatus status, string mensagem, int totalExecucoes = 0)
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

            msgFinal = string.Concat(msgFinal, mensagem);

            return msgFinal;
        }
        private enum LogStatus
        {
            Sucesso = 1,
            NaoSucesso = 2,
            ErroServico = 3
        }
    }
}
