using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Servico.Contratos
{
    public interface IPowerShellResult : IDisposable
    {
        string Chave { get; }
        string ExtensaoArquivo { get; }
        string ObterResult(string dispositivo, string chave);
        string ObterResult(string dispositivo, string msgException, string chave);
        string ObterResult(ICollection<PSObject> psObjectResults, string dispositivo, string chave);
        string ObterResult(ICollection<ErrorRecord> erros, string dispositivo, string chave);
    }
}
