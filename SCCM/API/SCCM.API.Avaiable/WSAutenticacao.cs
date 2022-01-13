using SCCM.API.Infraestrutura.ApplicationViewService;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.API.Avaiable
{
    public class WSAutenticacao
    {
        private static string _dominio;
        private static SCCMDominio _enumDominio;
        private static string _usuario;

        public static string Dominio
        {
            get { return _dominio; }
        }
        public static SCCMDominio EnumDominio
        {
            get { return _enumDominio; }
        }
        public static string Usuario
        {
            get { return _usuario; }
        }
        public static IComumResult DefinirEscopoResult(NetworkCredential crendenciais)
        {
            try
            {
                _dominio = crendenciais.Domain;
                _usuario = crendenciais.UserName;
                _enumDominio = (SCCMDominio)Enum.Parse(typeof(SCCMDominio), crendenciais.Domain, true);

                WSEscopo.Definir(crendenciais);

                return ComumResultFactory.Criar(true, string.Format("Usuário {0} autenticado", crendenciais.UserName));
            }
            catch (Exception ex)
            {
                return ComumResultFactory.Criar(false, string.Format("Ocorreu um erro na autenticação do usuário {0}", crendenciais.UserName), ex.Message, ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty));
            }
        }
        public static IComumResult DefinirEscopoResult(string usuario)
        {
            try
            {
                string [] usuarioDominio = usuario.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                _dominio = usuarioDominio.ElementAt(0);
                _usuario = usuarioDominio.ElementAt(1);
                _enumDominio = (SCCMDominio)Enum.Parse(typeof(SCCMDominio), _dominio, true);

                WSEscopo.Definir(_enumDominio);

                return ComumResultFactory.Criar(true, string.Format("Usuário {0} autenticado", usuario));
            }
            catch (Exception ex)
            {
                return ComumResultFactory.Criar(false, string.Format("Ocorreu um erro na autenticação do usuário {0}", usuario), ex.Message, ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty));
            }
        }
        public static void DefinirEscopo(NetworkCredential crendenciais)
        {
            _dominio = crendenciais.Domain;
            _usuario = crendenciais.UserName;

            WSEscopo.Definir(crendenciais);
        }
        public static ApplicationViewServiceSoapClient ObterEscopo()
        {
            return WSEscopo.Obter();
        }
    }
}

