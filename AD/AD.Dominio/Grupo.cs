using HelperComum;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AD.Dominio
{
    public class Grupo
    {
        private string _nome;
        private string[] _grupos;
        public Grupo()
        {

        }
        public Grupo(string nome, string[] grupos)
        {
            _nome = nome;
            _grupos = grupos;
        }
        public string Nome
        {
            get { return _nome; }
        }
        public string[] Grupos
        {
            get { return _grupos; }
        }
        public bool Existe(string dominio, string nomeGrupo, string container, string usuario, SecureString password)
        {
            try
            {
                bool grupoeExiste = false;

                using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, container, ContextOptions.Negotiate, usuario, password.ParseString())) 
                {
                    GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);
                    if (groupPrincipal != null)
                    {
                        grupoeExiste = true;
                    }
                }

                return grupoeExiste;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação do grupo", ex.InnerException);
            }
        }
        public bool CSV(string nomeArquivo)
        {
            try
            {
                bool gerado = Helper.ArrayToCSV(_grupos, nomeArquivo);

                return gerado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
