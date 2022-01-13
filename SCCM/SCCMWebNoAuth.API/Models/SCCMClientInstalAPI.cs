using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCCMWebNoAuth.API.Models
{
    public class SCCMClientInstalAPI
    {
        public string Chave { get; set; }
        public bool Instalado { get; set; }
        public string DataVerificacao { get; set; }
    }
}