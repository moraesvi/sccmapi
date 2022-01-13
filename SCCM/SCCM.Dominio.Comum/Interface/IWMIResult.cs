using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public interface IWMIResult
    {
        bool Executado { get; }
        string Domain { get; }
        object Result { get; }
        string MsgResul { get; }
        string Data { get; }
        ExceptionModel Exception { get; }
    }
}
