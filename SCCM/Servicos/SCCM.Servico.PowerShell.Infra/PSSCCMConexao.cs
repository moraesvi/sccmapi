using SCCM.Servico.Dominio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Servico.PowerShell.Infra
{
    public class PSSCCMConexao
    {
        private WSManConnectionInfo _connectionInfo;
        private Runspace _remoteRunspace;
        private TraceSource _PSCode;

        private string _usuario;
        private string _senha;
        private string _hostname;
        private bool _ssl;
        private int _portaWSMan;

        /// <summary>
        /// TraceSource realiza comandos PowerShell
        /// </summary>
        public TraceSource PSCode
        {
            get { return _PSCode; }
            set { _PSCode = value; }
        }
        /// <summary>
        /// true = Conectado       
        /// </summary>
        public bool Conectado
        {
            get
            {
                if (_remoteRunspace != null)
                {
                    if (_remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Inicia o acesso ao SCCM
        /// </summary>
        public PSSCCMConexao(SCCMDominio dominio, bool ssl = false, int portaWsMan = 5985)
        {
            _ssl = ssl;
            _portaWSMan = portaWsMan;

            if (dominio != SCCMDominio.Local)
                DefinirCredenciais(dominio);

            Inicializar();
        }
        /// <summary>
        /// Inicia o acesso ao SCCM
        /// </summary>
        /// <param name="credencial">Credenciais de acesso</param>
        /// <param name="hostname">Nome do computador SCCM</param>
        /// <param name="ssl">Servidor SCCM configurado com SSL</param>
        /// <param name="wsManPort">Porta WSMan do SCCM (Padrão instalação = 5985)</param>
        public PSSCCMConexao(string usuario, string senha, string hostname, bool ssl = false, int portaWsMan = 5985)
        {
            _usuario = usuario;
            _senha = senha;
            _hostname = hostname;
            _ssl = ssl;
            _portaWSMan = portaWsMan;

            Inicializar();
        }
        /// <summary>
        /// Inicia o acesso ao SCCM
        /// </summary>
        /// <param name="hostname">Nome do computador SCCM</param>
        /// <param name="ssl">Servidor SCCM configurado com SSL</param>
        /// <param name="wsManPort">Porta WSMan do SCCM (Padrão instalação = 5985)</param>
        public PSSCCMConexao(string hostname, bool ssl = false, int portaWsMan = 5985)
        {
            _hostname = hostname;
            _ssl = ssl;
            _portaWSMan = portaWsMan;

            Inicializar();
        }
        /// <summary>
        /// Prepara conexão a um servidor SCCM usando WSMan
        /// </summary>
        private void Inicializar()
        {
            PSCode = new TraceSource("PSCode");
            PSCode.Switch.Level = SourceLevels.All;

            if (string.IsNullOrEmpty(_usuario))
            {
                if (!_ssl)
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", _hostname, _portaWSMan)));
                else
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("https://{0}:{1}/wsman", _hostname, _portaWSMan)));
            }
            else
            {
                System.Security.SecureString secPassword = new System.Security.SecureString();

                foreach (char c in _senha.ToCharArray())
                {
                    secPassword.AppendChar(c);
                }

                PSCredential psc = new PSCredential(_usuario, secPassword);
                if (!_ssl)
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", _hostname, _portaWSMan)), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psc);
                else
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("https://{0}:{1}/wsman", _hostname, _portaWSMan)), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psc);

            }

            //Default
            _connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            _connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;

            //Timeout 2min
            _connectionInfo.OpenTimeout = (60000 * 2);
            _connectionInfo.OperationTimeout = (60000 * 2);
            _connectionInfo.OpenTimeout = (60000 * 2);
            _connectionInfo.CancelTimeout = 10000;
            _connectionInfo.IdleTimeout = (60000 * 2);

            Conectar();
        }
        /// <summary>
        /// Realiza conexao ao SCCM
        /// </summary>
        /// <exception cref="System.Exception">Não foi possível conectar.</exception>
        private void Conectar()
        {
            Runspace remoteRunspace = null;

            try
            {
                PSExecucao.ConexaoRunspace(_connectionInfo, ref remoteRunspace);
            }
            catch (Exception ex)
            {
                PSCode.TraceInformation(ex.Message);
            }

            if (remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                _remoteRunspace = remoteRunspace;
            }
            else
            {
                throw new Exception("Não foi possível realizar conexão ao SCCM");
            }
        }
        public void Desconectar()
        {
            _remoteRunspace.Close();
        }
        /// <summary>
        /// Define a conexão WSMAN com PowerShell.
        /// </summary>
        /// <returns></returns>
        public void DefinirRunspacePS()
        {
            PSRunspace.DefinirRunspace(_remoteRunspace, _PSCode);
        }
        public void Dispose()
        {
            try
            {
                if (Conectado)
                {
                    Desconectar();
                }
            }
            catch { }

            _connectionInfo = null;

            if (_remoteRunspace != null)
                _remoteRunspace.Dispose();

            GC.SuppressFinalize(this);
        }
        private void DefinirCredenciais(SCCMDominio dominio)
        {
            switch (dominio)
            {
                case SCCMDominio.PRBBR:
                    _usuario = "PRBBR\\xb194031";
                    _senha = "mtp@3421";
                    _hostname = "SRVSRVCPVWBR05.prbbr.produbanbr.corp";
                    break;
                case SCCMDominio.BSBR://Em definição.
                    _usuario = "PRBBR\\xb194031";
                    _senha = "mtp@3421";
                    _hostname = "SRVSRVCPVWBR05.prbbr.produbanbr.corp";
                    break;
                case SCCMDominio.ADTeste:
                    _usuario = "ADTeste\\sccm_admin";
                    _senha = "mtp@1234";
                    _hostname = "SCCM";
                    break;
                default:
                    throw new InvalidOperationException("Ocorreu um erro na validação do domínio.", new Exception("Domínio não válido, ou não foi inserido!"));
            }
        }
    }
}
