using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel
{
    public interface IObjetoWMI
    {
        WMIModelResult Obter(Runspace remoteRunspace, System.Diagnostics.TraceSource PSCode);
    }
}
