using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model.SMSRUser.SMSRUserDevice
{
    public class UserDevice
    {
        public string Name { get; set; }
        public string NetbiosName { get; set; }
        public string FullDomainName { get; set; }
        public bool IsVirtualMachine { get; set; }
        public DateTime LastLogonTimestamp { get; set; }
        public bool Active { get; set; }
    }
}
