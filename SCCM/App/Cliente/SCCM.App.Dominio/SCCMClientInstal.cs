using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Dominio
{
    public class SCCMClientInstal
    {
        private bool _instalado;
        public SCCMClientInstal()
        {
            _instalado = true;
        }
        public string Chave { get; set; }
        public bool Instalado
        {
            get { return _instalado; }
        }
        public string DataVerificacao { get; set; }
    }
}
