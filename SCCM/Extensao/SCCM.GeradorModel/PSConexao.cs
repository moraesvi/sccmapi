using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class PSConexao : IDisposable
    {
        private WSManConnectionInfo _connectionInfo;
        private Runspace _remoteRunspace;
        private bool _ipConectado;
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
            set { _PSCode = value;  }
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
        /// <param name="credencial">Credenciais de acesso</param>
        /// <param name="hostname">Nome do computador SCCM</param>
        /// <param name="ssl">Servidor SCCM configurado com SSL</param>
        /// <param name="wsManPort">Porta WSMan do SCCM (Padrão instalação = 5985)</param>
        public PSConexao(string usuario, string senha, string hostname, bool ssl = false, int portaWsMan = 5985)
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
        public PSConexao(string hostname, bool ssl = false, int portaWsMan = 5985)
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
            _ipConectado = false;

            PSCode = new TraceSource("PSCode");
            PSCode.Switch.Level = SourceLevels.All;

            if (string.IsNullOrEmpty(_usuario))
            {
                if (!_ssl)
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", _hostname, _portaWSMan)));
                else
                    _connectionInfo = new WSManConnectionInfo(new Uri(string.Format("https://{0}:{1}/wsman", _hostname, _portaWSMan)));
                _ipConectado = true;
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
            _connectionInfo.OpenTimeout = 120000;
            _connectionInfo.OperationTimeout = 120000;
            _connectionInfo.OpenTimeout = 120000;
            _connectionInfo.CancelTimeout = 10000;
            _connectionInfo.IdleTimeout = 120000;

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
                WSMan.ConexaoRunspace(_connectionInfo, ref remoteRunspace);
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
        /// Retorna um string Model referente a classe.
        /// </summary>
        /// <param name="objeto">Objeto WMI buscado</param>
        /// <returns></returns>
        public WMIModelResult ObterModel(IObjetoWMI objeto)
        {
            WMIModelResult modelResult = objeto.Obter(_remoteRunspace, _PSCode);

            return modelResult;
        }

        public List<WMIModelResult> ObterModel(List<IObjetoWMI> lstObjeto, bool newtonsoftJsonSerialization = false)
        {
            List<WMIModelResult> lstModelResult = new List<WMIModelResult>();

            lstObjeto.ForEach(objeto =>
            {
                WMIModelResult modelResult = objeto.Obter(_remoteRunspace, _PSCode);

                lstModelResult.Add(modelResult);
            });

            return lstModelResult;
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
    }
}
