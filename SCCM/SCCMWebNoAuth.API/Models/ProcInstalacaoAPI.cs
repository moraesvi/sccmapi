using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCCMWebNoAuth.API.Models
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