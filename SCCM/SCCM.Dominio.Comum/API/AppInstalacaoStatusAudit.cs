using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class AppInstalacaoStatusAudit
    {
        public int ErrorCode { get; set; }
        public string StatusInstalacao { get; set; }
    }

    public class AppInstalacaoStatusDetalheAudit
    {
        public string User { get; set; }
        public string LocalizedDisplayName { get; set; }
        public string MachineName { get; set; }
        public string ComplianceState { get; set; }
        public string StatusInstalacao { get; set; }
        public string StateDetail { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StartTime { get; set; }
    }
}
