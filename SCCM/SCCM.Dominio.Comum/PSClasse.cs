using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSClasse
    {
        private string _namespace;
        private string _classe;
        public PSClasse(string wmiNamespace, string classe)
        {
            _namespace = wmiNamespace;
            _classe = classe;
        }
        public string Namespace
        {
            get { return _namespace; }
        }
        public string Classe
        {
            get { return _classe; }
        }
        public override string ToString()
        {
            string PSClasse = string.Format("{0}:{1}", _namespace, _classe);

            return PSClasse;
        }
    }
}
