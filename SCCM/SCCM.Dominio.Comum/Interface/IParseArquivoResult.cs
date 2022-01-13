using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum.Interface
{
    public interface IParseArquivoResult
    {
        bool Valido(string textoXml);
        string ParseResult(string textoXml);
    }
}
