using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.API.Models
{
    public class CadastroScript
    {
        string[] _linhasScript;
        public CadastroScript()
        {
            _linhasScript = new string[] { };
        }
        public string[] LinhasScript
        {
            get { return _linhasScript; }
            set { _linhasScript = value; }
        }
        public string NomeArquivo { get; set; }
    }
    public class RemoverScript
    { 
        public string NomeArquivo { get; set; }
    }
}