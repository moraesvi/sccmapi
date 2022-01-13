using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public interface IParseArquivo
    {
        string Extensao { get; }
        void Definir(string linha);
        string Obter(string chave, string[] linhas);
        string Obter(string chave);
    }
}
