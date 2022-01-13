using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSClient : WMIObjetoBase
    {
        public const string QUERY = "([WMIClass]\"ROOT\\CCM:SMS_Client\").CreateInstance() | Get-Member -MemberType Property";
        public SMSClient()
            : base(QUERY, false)
        {

        }
        public SMSClient(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
