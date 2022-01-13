using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public interface IComandoBaseResult<T> where T : class
    {
        IWMIResult ExecutarResult();
        IWMIResult ExecutarColecaoResult();
    }
    public interface IComandoBase<T> where T : class
    {
        T Executar();
        T[] ExecutarColecao();
        PSFuncaoExisteAudit ComandoExiste(string nomeFuncao);
    }
}
