using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class AppDetalheAudit
    {
        public string LocalizedDisplayName { get; set; }
        public string AppVersion { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int ErrorCode { get; set; }
        public string Publisher { get; set; }
        public string StatusInstalacao { get; set; }
    }
}
