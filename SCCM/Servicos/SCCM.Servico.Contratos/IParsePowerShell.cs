using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Servico.Contratos
{
    public interface IParsePowerShell : IDisposable
    {
        string Chave { get; }
        string Dispositivo { get; }
        string ExtensaoArquivo { get; }
        string ObterComandoPS(string arquivoCaminhoCompleto);
        bool Validar(string arquivoCaminhoCompleto);
        bool ExtensaoValido(string arquivoCaminhoCompleto);
    }
}
