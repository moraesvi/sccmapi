using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSBaseClass : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject SMS_BaseClass -NameSpace \"ROOT\\SMS\\site_PR1\" | select -First 1 | Get-Member -MemberType Property";
        public SMSBaseClass()
            : base(QUERY, false)
        {

        }
        public SMSBaseClass(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
