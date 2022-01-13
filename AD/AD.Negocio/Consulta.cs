using HelperComum;
using AD.Dominio;
using AD.Infraestrutura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AD.Negocio
{
    public class Consulta
    {
        private static readonly string[] _arrayDominioValido = { "PRBBR", "BSBR" };

        private const string CONTAINER_PRBBR_PRODUBAN = "OU=Softgrid,OU=Grupos de Acesso,OU=Controle de Acesso,DC=prbbr,DC=produbanbr,DC=corp";
        private const string CONTAINER_BSBR_PRODUBAN = "OU=Softgrid,OU=Grupos de Acesso,OU=Controle de Acesso,DC=bs,DC=br,DC=bsch";

        private static string _usuarioCred = string.Empty;
        private static SecureString _passwordCred = null;
        private static string _dominio = string.Empty;
        public static void DefinirCredenciais(ADDominio dominio)
        {
            switch (dominio)
            {
                case ADDominio.PRBBR:
                    _usuarioCred = "PRBBR\\SPMBR01";
                    _passwordCred = "4~ZBDR7!2k4$)lce".ToSecureString();
                    _dominio = "PRBBR";
                    break;
                case ADDominio.BSBR:
                    _usuarioCred = "BSBR\\SPMBR01";
                    _passwordCred = "Alm90!QsV$*9t*O0".ToSecureString();
                    _dominio = "BSBR";
                    break;
                default:
                    throw new InvalidOperationException("Ocorreu um erro na validação do domínio.", new Exception("Domínio não válido, ou não foi inserido!"));
            }
        }
        public static IComumResult ObterGruposADPrbbrBsbr()
        {
            List<Grupo> lstGrupo = new List<Grupo>();

            Dictionary<string, string> dctDominio = new Dictionary<string, string>();

            dctDominio.Add(ADDominio.PRBBR.ToString(), CONTAINER_PRBBR_PRODUBAN);
            dctDominio.Add(ADDominio.BSBR.ToString(), CONTAINER_BSBR_PRODUBAN);

            dctDominio.ToList().ForEach(dct =>
            {
                string[] arrayGrupos = Conexao.ObterServicosDirectory(dct.Key, dct.Value, _usuarioCred, _passwordCred).ToArray();

                Grupo grupo = new Grupo(dct.Key, arrayGrupos);

                lstGrupo.Add(grupo);
            });

            IComumResult result = ComumResultFactory.Criar("Busca dos grupos realizado", lstGrupo.ToArray());

            return result;
        }
        public static IComumResult ObterUsuarioGrupos(string usuario)
        {
            string[] grupos = Conexao.ObterUsuarioGrupos(_dominio, usuario, _usuarioCred, _passwordCred).ToArray();

            GUsuario GUsuario = new GUsuario(usuario, grupos);

            IComumResult result = ComumResultFactory.Criar("Busca dos grupos do usuário realizado", GUsuario);

            return result;
        }
        public static IComumResult AdicionarUsuarioGrupo(string nomeGrupo, string usuario)
        {
            IComumResult result = null;
            bool inserido = false;
            bool existe = DominioGrupoExiste(nomeGrupo);

            if (!existe)
            {
                throw new InvalidOperationException("Ocorreu um erro na inserção", new Exception(string.Format("O grupo {0} não existe no AD", nomeGrupo)));
            }

            bool usuarioGrupo = UsuarioGrupo(nomeGrupo, usuario);

            if (!usuarioGrupo)
            {
                inserido = Conexao.AdicionarUsuarioGrupo(_dominio, nomeGrupo, usuario, _usuarioCred, _passwordCred);

                if (inserido)
                    result = ComumResultFactory.Criar(string.Format("Usuário: {0} inserido no grupo: {1}", usuario, nomeGrupo), true);
                else
                    result = ComumResultFactory.Criar(false, string.Format("Não foi possível inserir o usuário: {0} no grupo: {1}", usuario, nomeGrupo), false);
            }
            else
            {
                result = ComumResultFactory.Criar(string.Format("Usuário: {0} já faz parte do grupo: {1}", usuario, nomeGrupo), false);
            }

            return result;
        }
        public static IComumResult RemoverUsuarioGrupo(string nomeGrupo, string usuario)
        {
            IComumResult result = null;

            bool usuarioGrupo = UsuarioGrupo(nomeGrupo, usuario);

            if (usuarioGrupo)
            {
                bool removido = Conexao.RemoverUsuarioGrupo(_dominio, nomeGrupo, usuario, _usuarioCred, _passwordCred);

                if (removido)
                    result = ComumResultFactory.Criar(string.Format("Usuário: {0} removido do grupo: {1}", usuario, nomeGrupo), true);
                else
                    result = ComumResultFactory.Criar(string.Format("Não foi possível remover o usuário: {0} do grupo: {1}", usuario, nomeGrupo), false);

                return result;
            }
            else
            {
                result = ComumResultFactory.Criar(string.Format("Usuário: {0} não faz parte do grupo: {1}", usuario, nomeGrupo), true);
            }

            return result;
        }
        public static IComumResult VerificarUsuarioGrupo(string nomeGrupo, string usuario)
        {
            IComumResult result = null;
            bool valido = UsuarioGrupo(nomeGrupo, usuario);

            if (valido)
                result = ComumResultFactory.Criar(string.Format("Usuário: {0} está no grupo: {1}", usuario, nomeGrupo), true);
            else
                result = ComumResultFactory.Criar(string.Format("Usuário: {0} não está no grupo: {1}", usuario, nomeGrupo), false);

            return result;
        }
        public static IComumResult VerificarUsuarioBloqueadoAD(string usuario)
        {
            IComumResult result = null;
            bool bloqueado = UsuarioBloqueadoAD(usuario);

            if (bloqueado)
                result = ComumResultFactory.Criar(string.Format("Dominio\\Usuário: {0}\\{1} está bloqueado no AD", _dominio, usuario), true);
            else
                result = ComumResultFactory.Criar(string.Format("Dominio\\Usuário: {0}\\{1} ativo no AD", _dominio, usuario), false);

            return result;
        }

        #region Metodos Privados 
        private static bool UsuarioGrupo(string nomeGrupo, string usuario)
        {
            bool valido = Conexao.UsuarioGrupo(_dominio, nomeGrupo, usuario, _usuarioCred, _passwordCred);

            return valido;
        }
        private static bool UsuarioBloqueadoAD(string usuario)
        {
            bool bloqueado = Conexao.UsuarioBloqueadoAD(_dominio, usuario, _usuarioCred, _passwordCred);

            return bloqueado;
        }
        private static bool DominioGrupoExiste(string nomeGrupo)
        {
            Grupo grupo = new Grupo();

            string container = ObterContainer(_dominio);

            bool existe = grupo.Existe(_dominio, nomeGrupo, container, _usuarioCred, _passwordCred);

            return existe;
        }
        private static string ObterContainer(string dominio)
        {
            string container = string.Empty;

            if (!_arrayDominioValido.Contains(dominio.ToUpper()))
            {
                throw new InvalidOperationException("Ocorreu um erro na validação do domínio.", new Exception("Domínio não válido!"));
            }

            ADDominio enumDominio = ADDominio.PRBBR;

            Enum.TryParse<ADDominio>(dominio, out enumDominio);

            switch (enumDominio)
            {
                case ADDominio.PRBBR:
                    container = CONTAINER_PRBBR_PRODUBAN;
                    break;
                case ADDominio.BSBR:
                    container = CONTAINER_BSBR_PRODUBAN;
                    break;
                default:
                    throw new InvalidOperationException("Ocorreu um erro na validação do domínio.", new Exception("Domínio não válido!"));
            }

            return container;
        }
        #endregion
    }
}
