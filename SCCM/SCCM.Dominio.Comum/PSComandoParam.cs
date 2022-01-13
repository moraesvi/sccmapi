using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSComandoParam
    {
        private int _ordem;
        private string _nome;
        private string _valor;
        public PSComandoParam(int ordem, string nome, string valor)
        {
            _ordem = ordem;
            _nome = nome;
            _valor = valor;
        }
        public PSComandoParam(int indice, string nome)
        {
            _ordem = indice;
            _nome = nome;
        }
        public int Ordem
        {
            get { return _ordem; }
        }
        public string Nome
        {
            get { return _nome; }
        }
        public string Valor
        {
            get { return _valor; }
        }
    }
}
