using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Parse.Net
{
    public class ParseNet
    {
        private static string TIPO_NAO_DEF = "|TP_NDEF|";

        private static string managementTipo = "System.Management.ManagementObject";

        private static string[] netTipoLowerCase = new string[] { "string", "int", "bool", "byte" };

        private static string netTipoLowerCaseArray = "[]";

        private static string[] netTipo32 = new string[] { "int", "int[]" };

        private static string[] netTipoBool = new string[] { "bool", "bool[]" };

        public static string PropNet(string propridade)
        {
            string propFormat = string.Empty;

            if (!string.IsNullOrWhiteSpace(propridade))
            {
                try
                {
                    string[] prop = propridade.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string tipoFormat = string.Empty;
                    Type netTipo = null;
                    bool tipo32 = false;
                    bool tipoBool = false;

                    string tipo = prop[0];
                    string nome = prop[1];
                    string retorno = prop[2];

                    if (!tipo.ToUpper().Contains("SYSTEM"))
                    {
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

                        netTipo = Type.GetType(string.Concat("System.", tipo), false, true);

                        tipoFormat = netTipo.Name;
                    }

                    tipoFormat = tipo32 ? tipoFormat.Replace("32", "") : tipoFormat;
                    tipoFormat = tipoBool ? tipoFormat.Replace("ean", "") : tipoFormat;

                    bool lowerCase = netTipoLowerCase.ToList()
                                                     .Exists(valor => valor.IndexOf(tipoFormat, StringComparison.OrdinalIgnoreCase) >= 0
                                                                   || string.Concat(valor, netTipoLowerCaseArray).IndexOf(tipoFormat, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (lowerCase)
                    {
                        tipoFormat = tipoFormat.ToLower();
                    }

                    if (tipo.Contains(managementTipo))
                    {
                        tipoFormat = tipo.Replace(managementTipo, "").Replace("#", "").Trim();
                    }

                    tipoFormat = !string.IsNullOrEmpty(tipoFormat) ? tipoFormat : TIPO_NAO_DEF;

                    propFormat = string.Concat("public ", tipoFormat, " ", nome, retorno);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }

            return propFormat;
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
