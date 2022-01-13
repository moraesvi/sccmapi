using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSProviderLocation : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -Query \"Select* From SMS_ProviderLocation\" -Namespace \"root\\SMS\"";
        public SMSProviderLocation()
            : base(QUERY, false)
        {

        }
        public SMSProviderLocation(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
