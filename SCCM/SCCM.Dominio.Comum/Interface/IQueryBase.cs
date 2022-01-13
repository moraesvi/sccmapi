using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public interface IModelBaseResult<T> where T : class                                        
    {
        IWMIResult Listar();
        IWMIResult Obter(object id);
        IWMIResult ObterColecao(object id);
    }
    public interface IModelBase<T> where T : class
    {
        T[] Listar();
        T Obter(object id);
        T[] ObterColecao(object id);
    }
}
