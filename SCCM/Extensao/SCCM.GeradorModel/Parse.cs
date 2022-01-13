using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class Parse
    {
        private static string TIPO_NAO_DEF = "|TP_NDEF|";

        private static string managementTipo = "System.Management.ManagementObject";

        private static string[] metodosExclusao = new string[] { "ToString()", "Equals()", "GetHashCode()", "GetType()" };

        private static string[] netTipoLowerCase = new string[] { "string", "int", "bool", "byte" };

        private static string netTipoLowerCaseArray = "[]";

        private static string[] netTipo32 = new string[] { "int", "int[]" };

        private static string[] netTipoBool = new string[] { "bool", "bool[]" };

        public static string PropNet(string propriedade)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(propriedade))
                {
                    return null;
                }

                string propFormat = string.Empty;

                string[] prop = propriedade.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string tipoFormat = string.Empty;
                Type netTipo = null;
                bool tipo32 = false;
                bool tipoBool = false;

                string tipo = prop[0];
                string nome = AjustarNomePropriedade(prop[1]);
                string retorno = ContemGetSet(propriedade) ? prop[2] : "{get;set;}";

                if (metodosExclusao.Contains(nome.ToUpper(), new CompararOrdinal()))
                {
                    return null;
                }

                tipo32 = netTipo32.Contains(tipo, new CompararOrdinal());
                tipoBool = netTipoBool.Contains(tipo, new CompararOrdinal());

                tipo = tipo32 ? tipo.Contains("[]")
                              ? string.Concat(tipo.Replace("[]", ""), "32[]")
                                : string.Concat(tipo, "32")
                              : tipo;

                tipo = tipoBool ? tipo.Contains("[]")
                                ? string.Concat(tipo.Replace("[]", ""), "ean[]")
                                  : string.Concat(tipo, "ean")
                                : tipo;

                if (!tipo.Contains(managementTipo))
                {

                    tipo = !tipo.ToUpper().Contains("SYSTEM") ? string.Concat("System.", tipo) : tipo;

                    netTipo = Type.GetType(tipo, false, true);

                    tipoFormat = netTipo.Name;

                    tipoFormat = tipo32 ? tipoFormat.Replace("32", "") : tipoFormat;
                    tipoFormat = tipoBool ? tipoFormat.Replace("ean", "") : tipoFormat;

                    bool lowerCase = netTipoLowerCase.ToList()
                                                     .Exists(valor => valor.IndexOf(tipoFormat, StringComparison.OrdinalIgnoreCase) >= 0
                                                                   || string.Concat(valor, netTipoLowerCaseArray).IndexOf(tipoFormat, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (lowerCase)
                    {
                        tipoFormat = tipoFormat.ToLower();
                    }
                }
                else if (tipo.Contains(managementTipo))
                {
                    tipoFormat = tipo.Replace(managementTipo, "").Replace("#", "").Trim();
                }

                tipoFormat = !string.IsNullOrEmpty(tipoFormat) ? tipoFormat : TIPO_NAO_DEF;

                propFormat = string.Concat("public ", tipoFormat, " ", nome, retorno);


                return propFormat;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public static bool ContemGetSet(string propriedade)
        {
            if (string.IsNullOrWhiteSpace(propriedade))
                return false;

            bool possui = false;
            string[] getSetArray = new string[] { "GET", "SET" };

            if (!string.IsNullOrWhiteSpace(propriedade))
            {
                string[] prop = propriedade.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string valor in prop)
                {
                    possui = getSetArray.Contains(valor);

                    if (possui || metodosExclusao.Contains(valor, new CompararOrdinal()))
                        break;
                }
            }

            return possui;
        }
        public static bool ContemMembros(PSObject objeto)
        {
            if (objeto == null)
                return false;

            PSMemberInfo[] membros = objeto.Members.ToArray();
            if (membros != null)
            {
                if (membros.Count() > 0)
                    return true;
            }

            return false;
        }
        public static string FormatarPropriedade(string tipo, string nome)
        {
            if ((!string.IsNullOrWhiteSpace(tipo) && !string.IsNullOrWhiteSpace(nome)) && !metodosExclusao.Contains(nome.ToUpper(), new CompararOrdinal()))
            {
                return string.Concat(tipo, " ", nome, " {get;set;}");
            }

            return string.Empty;
        }
        public static string AjustarNomePropriedade(string nomeProp)
        {
            if (!string.IsNullOrWhiteSpace(nomeProp))
            {
                if (nomeProp.Contains("="))
                    nomeProp = nomeProp.Substring(0, (nomeProp.IndexOf('=')));

                if (nomeProp.Contains("("))
                    nomeProp = nomeProp.Substring(0, (nomeProp.IndexOf('(')));

                return nomeProp;
            }

            return string.Empty;
        }
        private static PSMemberInfo[] RemoverMetodosNaoValidos(PSMemberInfo[] membros)
        {
            if (membros != null)
            {
                membros = membros.Where(membro => !metodosExclusao.Contains(membro.Name))
                                 .ToArray();
            }

            return membros;
        }
    }
    internal class CompararOrdinal : IEqualityComparer<string>
    {
        public bool Equals(string valor1, string valor2)
        {
            if (valor1.IndexOf(valor2, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(string obj)
        {
            throw new NotImplementedException();
        }
    }
}
