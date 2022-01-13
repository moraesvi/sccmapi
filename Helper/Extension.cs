using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HelperComum
{
    public static class Extension
    {
        public static string ToJson(this object[] array)
        {
            StringBuilder sbJson = new StringBuilder();

            string json = ObterJson(array);
            sbJson.AppendLine(json);

            return sbJson.ToString();
        }
        public static string ToJson(this List<object> lstObjeto)
        {
            StringBuilder sbJson = new StringBuilder();

            foreach (object valor in lstObjeto)
            {
                string json = ObterJson(valor);
                sbJson.AppendLine(json);
            }

            return sbJson.ToString();
        }
        public static string ToJson(this object objeto)
        {
            string json = ObterJson(objeto);

            return json;
        }
        public static string ParseString(this SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);
            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
        public static SecureString ToSecureString(this string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;
            else
            {
                SecureString result = new SecureString();

                foreach (char c in valor.ToCharArray())
                    result.AppendChar(c);

                return result;
            }
        }
        public static string Get(this NameValueCollection valor, string chaveConfig, bool exception = true)
        {
            string valorSettings = ConfigurationManager.AppSettings.Get(chaveConfig);

            if (string.IsNullOrWhiteSpace(valorSettings))
            {
                if (exception)
                {
                    throw new Exception("Operação inválida", new Exception(string.Format("Não foi possível buscar ou definir o endereço da chave '{0}'", chaveConfig)));
                }
            }

            return valorSettings;
        }
        public static Exception InnerException(this Exception exception)
        {
            string innerExcept = exception.InnerException != null ? exception.InnerException.Message : string.Empty;

            return new Exception(innerExcept);
        }
        private static string ObterJson(object objeto)
        {
            if (objeto == null)
                return string.Empty;

            string json = JsonConvert.SerializeObject(objeto, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            });

            return json;
        }
    }
}
