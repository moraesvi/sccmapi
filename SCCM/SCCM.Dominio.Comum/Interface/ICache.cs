using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public interface ICache
    {
        bool Definir(string dados, string chave);
        string Obter(string chave);
        T Obter<T>(string chave) where T : class;
        T[] ObterArray<T>(string chave) where T : class;
    }
}
