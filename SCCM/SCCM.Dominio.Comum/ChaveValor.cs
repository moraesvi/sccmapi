using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class ChaveValor
    {
        public object Chave { get; set; }
        public object Valor { get; set; }
    }

    public struct Chave
    {
        public int IdChave { get; set; }
        public object Nome { get; set; }
    }

    public struct ValorChave
    {
        public int IdChave { get; set; }
        public object Valor { get; set; }
    }
}
