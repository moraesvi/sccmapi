using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSCollection : WMIObjetoBase
    {       
        private const string QUERY = "Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\"  -ClassName SMS_Collection | Get-Member -MemberType Property";
        public SMSCollection()
            : base(QUERY, false)
        {

        }
        public SMSCollection(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
