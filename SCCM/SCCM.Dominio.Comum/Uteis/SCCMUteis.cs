using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class SCCMUteis
    {
        private const string WMI_CLASS_TEMPLATE = "${0} = ([WMIClass]\"\\{1}:{2}\").CreateInstance()";
        public static string PSInstance<T>(PSClasse PSClasse, T objeto) where T : class
        {
            string psClasseInstance = string.Format(WMI_CLASS_TEMPLATE, objeto.GetType().Name, PSClasse.Namespace, PSClasse.Classe);

            return psClasseInstance;
        }
        public static string PSInstanceToPowershell<T>(string PSNamespace, string classe, T objeto) where T : class
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(PSInstance(PSNamespace, classe, objeto));
            sb.AppendLine(SCCMHelper.ToPowershell(objeto, objeto.GetType().Name, false));

            return sb.ToString();
        }
        public static string PSInstance<T>(string PSNamespace, string classe, T objeto) where T : class
        {
            string psClasseInstance = string.Format(WMI_CLASS_TEMPLATE, objeto.GetType().Name, PSNamespace, classe);

            return psClasseInstance;
        }
        public static string ToPowershell<T>(string PSNamespace, string classe, bool PSInstancia, T objeto) where T : class
        {
            StringBuilder sb = new StringBuilder();

            if (PSInstancia)
                sb.AppendLine(PSInstance(PSNamespace, classe, objeto));

             sb.AppendLine(SCCMHelper.ToPowershell(objeto, objeto.GetType().Name, false));

            return sb.ToString();
        }
        public static string ToPowerShellModel<T>(string PSNamespace, string classe, T objeto) where T : class
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(SCCMHelper.ToPowershell(objeto, objeto.GetType().Name, true));

            return sb.ToString();
        }
    }
}
