using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public class WMIModelResult
    {
        private List<WMIPropriedade> _prop;

        public WMIModelResult()
        {
            _prop = new List<WMIPropriedade>();
        }

        public string NomeClasse { get; set; }

        public List<WMIPropriedade> Prop
        {
            get { return _prop; }
            set { _prop = value; }
        }
    }

    public class WMIPropriedade
    {
        private string _nome;
        public WMIPropriedade(string nome)
        {
            _nome = nome;
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }
    }
}
