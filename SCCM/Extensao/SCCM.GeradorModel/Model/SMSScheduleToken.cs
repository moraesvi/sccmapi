using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model
{
    public class SMSScheduleToken : WMIObjetoBase
    {
        public SMSScheduleToken()
            : base("SMS_ScheduleToken", false)
        {

        }
        public SMSScheduleToken(bool newtonsoftJsonSerialization)
            : base("SMS_ScheduleToken", newtonsoftJsonSerialization)
        {

        }
        public SMSScheduleToken(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
