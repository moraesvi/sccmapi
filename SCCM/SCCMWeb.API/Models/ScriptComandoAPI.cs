using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.API.Models
{
    public class ScriptExecutar
    {
        private string[] _linhasScript;
        public ScriptExecutar()
        {
            _linhasScript = new string[] { };
        }
        public string ChaveResult { get; set; }
        public string[] LinhasScript
        {
            get { return _linhasScript; }
            set { _linhasScript = value; }
        }
    }
    public class ScriptExecutarLogado
    {
        private string[] _linhasScript;
        public ScriptExecutarLogado()
        {
            _linhasScript = new string[] { };
        }
        public string ChaveResult { get; set; }
        public string Dominio { get; set; }
        public string Usuario { get; set; }

        public string[] LinhasScript
        {
            get { return _linhasScript; }
            set { _linhasScript = value; }
        }
    }
    public class ComandoLogadoAPI
    {
        private ComandoParam[] _comandoParam;
        public ComandoLogadoAPI()
        {
            _comandoParam = new ComandoParam[] { };
        }
        public string ChaveResult { get; set; }
        public string Dominio { get; set; }
        public string Usuario { get; set; }
        public string NomeArquivo { get; set; }
        public string NomeComando { get; set; }
        public ComandoParam[] ComandoParam
        {
            get { return _comandoParam; }
            set { _comandoParam = value; }
        }
    }
    public class ComandoParam
    {
        public int Ordem { get; set;  }
        public string Nome { get; set; }
        public string Valor { get; set; }
    }
}