using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class PSObjetoRemovidoAudit
    {
        public int Total { get; set; }
        public string TotalDesc { get; set; }
        public string DataProc { get; set; }
        public bool Excluiu { get; set; }
    }
}
