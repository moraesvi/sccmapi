using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class SMSApplicationCustom : WMIBase<SMSApplicationCustom>
    {
        private const string QUERY_BASE = @"Select CI_UniqueID, LocalizedDisplayName From SMS_Application Where IsExpired = 'False' And IsEnabled = 'True'";
        public SMSApplicationCustom()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CI_UniqueID")
        {

        }
        [JsonProperty]
        public string CI_UniqueID { get; set; }
        [JsonProperty]
        public string LocalizedDisplayName { get; set; }
    }
}
