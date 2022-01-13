using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class SMSBase : WMIBase
    {
        public UInt32 DomainID { get; set; }
        public string DomainMode { get; set; }
        public string DomainName { get; set; }
        public UInt32 Flags { get; set; }
        public UInt32 ForestID { get; set; }
        public string LastDiscoveryTime { get; set; }
    }
}
