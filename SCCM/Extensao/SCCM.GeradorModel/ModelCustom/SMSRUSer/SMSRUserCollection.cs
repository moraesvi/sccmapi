using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.ModelCustom.SMSRUSer
{
    public class SMSRUserCollection : WMIObjetoBase
    {
        private static string _query = "Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\" -Query \"Select Name, UniqueUserName From SMS_R_User\" | Get-Member -MemberType Property";
        public SMSRUserCollection(bool newtonsoftJsonSerialization)
            : base(_query, newtonsoftJsonSerialization)
        {

        }
        public SMSRUserCollection(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {

        }
    }
}
