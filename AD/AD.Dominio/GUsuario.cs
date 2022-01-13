using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Dominio
{
    public class GUsuario
    {
        private string _usuario;
        private string[] _grupos;
        public GUsuario(string usuario, string[] grupos)
        {
            _usuario = usuario;
            _grupos = grupos;
        }
        public string Usuario
        {
            get { return _usuario; }
        }
        public string[] Grupos
        {
            get { return _grupos; }
        }
    }
}
