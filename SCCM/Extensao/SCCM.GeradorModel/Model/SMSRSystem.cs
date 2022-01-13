using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSRSystem : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\"  -ClassName SMS_R_System | Get-Member -MemberType Property";
        public SMSRSystem()
            : base(QUERY, false)
        {

        }
        public SMSRSystem(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
