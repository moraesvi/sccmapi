using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSComando
    {
        private string _comando;
        private string _script;
        private List<Chave> _param;
        private bool _arquivo;
        public PSComando(string comando, string script, List<Chave> param, bool arquivo = false)
        {
            _comando = comando;
            _script = script;
            _param = param;
            _arquivo = arquivo;
        }
        public bool Arquivo
        {
            get { return _arquivo; }
        }
        public string Comando
        {
            get { return _comando; }
        }
        public string Script
        {
            get
            {
                if (_arquivo)
                {
                    string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

                    _script = Path.Combine(currentDir, "PSScripts", _script);
                }

                return _script;
            }
        }
        public List<Chave> Param
        {
            get { return _param; }
        }
    }
}
