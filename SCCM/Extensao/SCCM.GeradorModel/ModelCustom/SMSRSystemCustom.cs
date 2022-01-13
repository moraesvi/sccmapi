using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.ModelCustom
{
    public class SMSRSystemCustom : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -Query \"Select Name, NetbiosName, IsVirtualMachine, LastLogonTimestamp, FullDomainName From SMS_R_System\" -Namespace \"root\\SMS\\Site_PR1\" | Get-Member";
        public SMSRSystemCustom()
            :base(QUERY, true)
        {

        }
    }
}
