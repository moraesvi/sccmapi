using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Dominio
{
    public class ProcInstalacaoAPI
    {
        public string Chave { get; set; }
        public bool Executado { get; set; }
        public bool Instalado { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string DetalheStatus { get; set; }
    }
}
