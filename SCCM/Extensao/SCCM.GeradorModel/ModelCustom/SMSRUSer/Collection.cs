using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.ModelCustom.SMSRUSer
{
    public class SMSCollection : WMIObjetoBase
    {
        private const string QUERY = "Get-WmiObject -NameSpace \"ROOT\\SMS\\site_PR1\" -Query \"Select CollectionID, CollectionType, Comment, CurrentStatus, MemberCount From SMS_Collection\" | Get-Member -MemberType Property";
        public SMSCollection(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
        public SMSCollection(string query, bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
