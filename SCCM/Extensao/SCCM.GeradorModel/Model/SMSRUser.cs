using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSRUser : WMIObjetoBase
    {
        public SMSRUser(string query)
            : base(query, false)
        {

        }
        public SMSRUser(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
