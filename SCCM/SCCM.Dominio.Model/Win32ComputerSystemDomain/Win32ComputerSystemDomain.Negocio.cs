using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class Win32ComputerSystemDomain
    {
        public Win32ComputerSystemDomain Obter()
        {
            Win32ComputerSystemDomain dominio = this.Listar().ToList()
                                                             .SingleOrDefault();

            return dominio;
        }
    }
}
