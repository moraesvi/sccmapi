using HelperComum;
using SCCM.Dominio.Comum;
using SCCM.Infraestrutura;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.PowerShell
{
    public class PSEscopo : IDisposable
    {
        private const string QUERY_DOMINIO = "Select Domain, Name From Win32_ComputerSystem";

        private PSConexao _psConexao;
        private string _dominio;
        private string _server;
        /// <summary>
        /// Inicia o acesso ao SCCM - Busca credenciais do App settings da aplicação
        /// </summary
        public PSEscopo(bool serverSDK = false)
        {
            string servidor = string.Empty;

            if (!serverSDK)
            {
                ValidarConfigCredenciais();
                servidor = ConfigurationManager.AppSettings.Get("SCCM_SERVER");
            }
            else
            {
                ValidarConfigCredenciaisSDK();
                servidor = ConfigurationManager.AppSettings.Get("SCCM_SERVER_SDK");
            }

            string credUsuario = ConfigurationManager.AppSettings.Get("SCCM_CRED_USUARIO");
            string credSenha = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA");
            string credSenhaSecure = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA_SECURE");

            if (!string.IsNullOrWhiteSpace(credSenha))
            {
                _psConexao = new PSConexao(credUsuario, credSenha, servidor);
            }
            else if (!string.IsNullOrWhiteSpace(credSenhaSecure))
            {
                _psConexao = new PSConexao(credUsuario, credSenha.ToSecureString(), servidor);
            }

            _server = servidor;
            _dominio = ObterDominioSCCM(_psConexao.RemoteRunspace);
        }
        /// <summary>
        /// Inicia o acesso ao SCCM
        /// </summary>
        /// <param name="credencial">Credenciais de acesso</param>
        /// <param name="hostname">Nome do computador SCCM</param>
        /// <param name="ssl">Servidor SCCM configurado com SSL</param>
        /// <param name="wsManPort">Porta WSMan do SCCM (Padrão instalação = 5985)</param>
        public PSEscopo(string usuario, string senha, string hostname, bool ssl = false, int portaWsMan = 5985)
        {
            _psConexao = new PSConexao(usuario, senha, hostname, ssl, portaWsMan);
            _dominio = ObterDominioSCCM(_psConexao.RemoteRunspace);
        }
        /// <summary>
        /// Inicia o acesso ao SCCM
        /// </summary>
        /// <param name="hostname">Nome do computador SCCM</param>
        /// <param name="ssl">Servidor SCCM configurado com SSL</param>
        /// <param name="wsManPort">Porta WSMan do SCCM (Padrão instalação = 5985)</param>
        public PSEscopo(string hostname, bool ssl = false, int portaWsMan = 5985)
        {
            _psConexao = new PSConexao(hostname, ssl, portaWsMan);
            _dominio = ObterDominioSCCM(_psConexao.RemoteRunspace);
        }
        public bool Conectado
        {
            get { return _psConexao.Conectado; }
        }
        public string Server
        {
            get { return _server; }
        }
        public string Dominio
        {
            get { return _dominio; }
        }
        public void DefinirRunspace()
        {
            _psConexao.DefinirRunspacePS();
        }
        public void Desconectar()
        {
            _psConexao.Desconectar();
        }
        public void Dispose()
        {
            _psConexao.Desconectar();
            GC.SuppressFinalize(this);
        }

        #region Metodos Privados
        private string ObterDominioSCCM(Runspace remoteRunspace)
        {
            PSQuery psQueryDominio = new PSQuery(QUERY_DOMINIO);
            Win32ComputerSystemDomain dominioComputador = null;

            string psqQueryDom = string.Concat(psQueryDominio.ToString(), " | Select -First 1");

            PSRequisicao.DefinirRunspace(remoteRunspace, null);

            dominioComputador = PSRequisicao.Executar(psqQueryDom, new Win32ComputerSystemDomain());

            return dominioComputador.Domain;

        }
        private bool ValidarConfigCredenciais()
        {
            string credUsuario = ConfigurationManager.AppSettings.Get("SCCM_CRED_USUARIO");
            string credSenha = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA");
            string credSenhaSecure = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA_SECURE");
            string servidor = ConfigurationManager.AppSettings.Get("SCCM_SERVER");

            if (string.IsNullOrWhiteSpace(credUsuario) && string.IsNullOrWhiteSpace(servidor) &&
                   (string.IsNullOrWhiteSpace(credSenha) || string.IsNullOrWhiteSpace(credSenhaSecure)))
            {
                throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir as credenciais de acesso ao servidor SCCM no .config"));
            }

            return true;
        }
        private bool ValidarConfigCredenciaisSDK()
        {
            string credUsuario = ConfigurationManager.AppSettings.Get("SCCM_CRED_USUARIO");
            string credSenha = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA");
            string credSenhaSecure = ConfigurationManager.AppSettings.Get("SCCM_CRED_SENHA_SECURE");
            string servidorSDK = ConfigurationManager.AppSettings.Get("SCCM_SERVER_SDK");

            if (string.IsNullOrWhiteSpace(credUsuario) && string.IsNullOrWhiteSpace(servidorSDK) &&
                   (string.IsNullOrWhiteSpace(credSenha) || string.IsNullOrWhiteSpace(credSenhaSecure)))
            {
                throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir as credenciais de acesso ao servidor SCCM SDK no .config"));
            }

            return true;
        }

        #endregion
    }
    internal class Win32ComputerSystemDomain
    {
        public Win32ComputerSystemDomain()
        {

        }
        public string Domain { get; set; }
        public string Name { get; set; }
    }
}
