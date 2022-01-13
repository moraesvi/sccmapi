using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SCCM.Dominio.Comum;
using SCCM.API.Infraestrutura.ApplicationViewService;
using System.Security.Principal;

namespace Infraestrutura
{
    public class AcessoWS
    {
        private static ApplicationViewServiceSoapClient _sccmWS;

        private static SCCMDominio _dominio = SCCMDominio.Indefinido;

        public static ApplicationViewServiceSoapClient WSAppViewService
        {
            get
            {
                if (_sccmWS == null)
                {
                    throw new InvalidOperationException("Operação inválida", new Exception("Não foi definido uma conexão válida ao WebService do SCCM."));
                }

                return _sccmWS;
            }
        }

        public static void Definir(NetworkCredential crendenciais)
        {
            try
            {
                if (string.IsNullOrEmpty(crendenciais.Domain))
                {
                    throw new InvalidOperationException("Operação inválida", new Exception("Domínio do usuário inválido ou não inserido."));
                }

                _dominio = (SCCMDominio)Enum.Parse(typeof(SCCMDominio), crendenciais.Domain, true);

                string wsUrl = URLWSDominio(_dominio);

                _sccmWS = new ApplicationViewServiceSoapClient();
                _sccmWS.Endpoint.Address = new System.ServiceModel.EndpointAddress(wsUrl);
                _sccmWS.ClientCredentials.Windows.ClientCredential = crendenciais;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception)
            {
                throw new Exception("Operação inválida", new Exception("Não foi possível definir/inválido o domínio do usuário."));
            }
        }

        public static void Definir(SCCMDominio dominio)
        {
            try
            {
                string wsUrl = string.Empty;

                wsUrl = URLWSDominio(dominio);

                _sccmWS = new ApplicationViewServiceSoapClient();
                _sccmWS.Endpoint.Address = new System.ServiceModel.EndpointAddress(wsUrl);
                _sccmWS.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception)
            {
                throw new Exception("Operação inválida", new Exception("Não foi possível definir/inválido o domínio do usuário."));
            }
        }

        private static string URLWSDominio(SCCMDominio dominio)
        {
            string urlWS = string.Empty;

            switch (dominio)
            {
                case SCCMDominio.PRBBR:
                    urlWS = ConfigurationManager.AppSettings.Get("WS_PRRBR");
                    break;
                case SCCMDominio.BSBR:
                    urlWS = ConfigurationManager.AppSettings.Get("WS_BSBR");
                    break;
                case SCCMDominio.ADTeste:
                    urlWS = ConfigurationManager.AppSettings.Get("WS_LAB");
                    break;
                default:
                    throw new InvalidOperationException("Operação inválida", new Exception("Não foi possível definir/inválido o domínio do usuário."));
            }

            return urlWS;
        }
    }
}
