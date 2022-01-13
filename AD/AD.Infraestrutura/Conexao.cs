using AD.Dominio;
using HelperComum;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AD.Infraestrutura
{
    public class Conexao
    {
        private const string DOMINIO_AGBANESPA = "AGBANESPA";
        private const string IP_AGBANESPA = "180.128.170.84";

        public static ICollection<string> ObterServicosDirectory(string dominio, string container, string usuario, SecureString senha)
        {
            try
            {
                PrincipalContext onde = new PrincipalContext(ContextType.Domain, dominio, container, ContextOptions.Negotiate, usuario, senha.ParseString());

                string[] grupos = BuscarGruposDirectory(onde);

                return grupos;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na busca dos grupos do Active Directory", ex.InnerException);
            }
        }
        public static ICollection<string> ObterUsuarioGrupos(string dominio, string usuario, string usuarioCred, SecureString senhaCred)
        {
            try
            {
                List<string> lstGrupos = new List<string>();

                PrincipalContext pcDomain;
                if (dominio.IndexOf(DOMINIO_AGBANESPA, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, IP_AGBANESPA, usuarioCred, senhaCred.ParseString());
                }
                else
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, dominio);
                }

                try
                {
                    using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, usuarioCred, senhaCred.ParseString()))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(contexto, usuario);

                        if (user == null)
                        {
                            return null;
                        }

                        PrincipalSearchResult<Principal> lstGroups = user.GetGroups();

                        lstGroups.ToList().ForEach(obj =>
                        {
                            string grupoDesc = obj.Name;
                            lstGrupos.Add(grupoDesc);
                        });
                    }

                    return lstGrupos;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new Exception("Ocorreu um erro na listagem de grupos do usuário no Active Directory", ex.InnerException);

                }
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na listagem de grupos do usuário no Active Directory", ex.InnerException);
            }
        }
        public static bool AdicionarUsuarioGrupo(string dominio, string nomeGrupo, string usuario, string usuarioCred, SecureString senhaCred)
        {
            try
            {
                PrincipalContext pcDomain;
                if (dominio.IndexOf(DOMINIO_AGBANESPA, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, IP_AGBANESPA, usuarioCred, senhaCred.ParseString());
                }
                else
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, dominio);
                }

                try
                {
                    using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, usuarioCred, senhaCred.ParseString()))
                    {
                        GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);

                        if (groupPrincipal == null)
                        {
                            return false;
                        }

                        UserPrincipal user = UserPrincipal.FindByIdentity(contexto, usuario);

                        if (user != null)
                        {
                            var sam = user.SamAccountName;

                            try
                            {
                                GroupPrincipal group = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);
                                group.Members.Add(pcDomain, IdentityType.SamAccountName, sam);
                                group.Save();
                            }
                            catch 
                            {
                                throw new InvalidOperationException("Não foi possível inserir o usuário no grupo.");
                            }

                            return true;
                        }
                    }

                    return false;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new Exception("Ocorreu um erro na inclusão do usuário no grupo no Active Directory", ex.InnerException);

                }
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na inclusão do usuário no grupo no Active Directory", ex.InnerException);
            }
        }
        public static bool RemoverUsuarioGrupo(string dominio, string nomeGrupo, string usuario, string usuarioCred, SecureString senhaCred)
        {
            try
            {
                PrincipalContext pcDomain;
                if (dominio.IndexOf(DOMINIO_AGBANESPA, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, IP_AGBANESPA, usuarioCred, senhaCred.ParseString());
                }
                else
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, dominio);
                }

                try
                {
                    using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, usuarioCred, senhaCred.ParseString()))
                    {
                        GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);

                        if (groupPrincipal == null)
                        {
                            return false;
                        }

                        UserPrincipal user = UserPrincipal.FindByIdentity(contexto, usuario);

                        if (user != null)
                        {
                            var sam = user.SamAccountName;

                            GroupPrincipal group = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);
                            group.Members.Remove(pcDomain, IdentityType.SamAccountName, sam);
                            group.Save();

                            return true;
                        }
                    }

                    return false;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new Exception("Ocorreu um erro na inclusão do grupo no Active Directory", ex.InnerException);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na inclusão do grupo no Active Directory", ex.InnerException);
            }
        }
        public static bool UsuarioGrupo(string dominio, string nomeGrupo, string usuario, string usuarioCred, SecureString senhaCred)
        {
            try
            {
                PrincipalContext pcDomain;
                if (dominio.IndexOf(DOMINIO_AGBANESPA, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, IP_AGBANESPA, usuarioCred, senhaCred.ParseString());
                }
                else
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, dominio);
                }

                try
                {
                    using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, usuarioCred, senhaCred.ParseString()))
                    {
                        GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);

                        if (groupPrincipal == null)
                        {
                            return false;
                        }

                        UserPrincipal user = UserPrincipal.FindByIdentity(contexto, usuario);

                        if (user != null)
                        {
                            GroupPrincipal group = GroupPrincipal.FindByIdentity(contexto, nomeGrupo);

                            if (user.IsMemberOf(group))
                            {
                                return true;
                            }
 
                            return false;
                        }
                    }

                    return false;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new Exception("Ocorreu um erro na verificação do grupo no Active Directory", ex.InnerException);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação do grupo no Active Directory", ex.InnerException);
            }
        }
        public static bool UsuarioBloqueadoAD(string dominio, string usuario, string usuarioCred, SecureString senhaCred)
        {
            try
            {
                PrincipalContext pcDomain;
                if (dominio.IndexOf(DOMINIO_AGBANESPA, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, IP_AGBANESPA, usuarioCred, senhaCred.ParseString());
                }
                else
                {
                    pcDomain = new PrincipalContext(ContextType.Domain, dominio);
                }

                try
                {
                    using (PrincipalContext contexto = new PrincipalContext(ContextType.Domain, dominio, usuarioCred, senhaCred.ParseString()))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(contexto, usuario);

                        if (user != null)
                        {
                            bool bloqueado = user.IsAccountLockedOut();

                            if (bloqueado)
                                return true;

                            DirectoryEntry usuarioDirEntry = user.GetUnderlyingObject() as DirectoryEntry;

                            if (usuarioDirEntry.NativeGuid == null)
                                return false;

                            string uac = "UserAccountControl";

                            if (usuarioDirEntry.Properties[uac] != null && usuarioDirEntry.Properties[uac].Value != null)
                            {
                                UserFlags userFlags = (UserFlags)usuarioDirEntry.Properties[uac].Value;

                                return (userFlags == UserFlags.AccountDisabled);
                            }
                        }
                    }

                    return false;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw new Exception("Ocorreu um erro na verificação do grupo no Active Directory", ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na verificação do grupo no Active Directory", ex.InnerException);
            }
        }

        #region Metodos Privados
        private static string[] BuscarGruposDirectory(PrincipalContext clausulaOnde)
        {
            try
            {
                List<string> grupos = new List<string>();

                GroupPrincipal findAllGroups = new GroupPrincipal(clausulaOnde, "*");
                PrincipalSearcher ps = new PrincipalSearcher(findAllGroups);

                ps.FindAll().ToList().ForEach(grupo =>
                {
                    grupos.Add(grupo.Name);
                });

                return grupos.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro na busca dos grupos do Active Directory", ex.InnerException);
            }
        }
        private enum Grupo
        {
            PRBBR,
            BSBR
        }
        #endregion
    }
}
