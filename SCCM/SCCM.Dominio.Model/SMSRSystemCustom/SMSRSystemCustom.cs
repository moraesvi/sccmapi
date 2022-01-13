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
    public partial class SMSRSystemCustom : WMIBase<SMSRSystemCustom>
    {
        private const string QUERY_BASE = "Select Name, NetbiosName, FullDomainName, IsVirtualMachine, LastLogonTimestamp From SMS_R_System Where LastLogonTimestamp <> ''";
        public SMSRSystemCustom()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "Name")
        {

        }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string NetbiosName { get; set; }
        [JsonProperty]
        public string FullDomainName { get; set; }
        [JsonProperty]
        public bool IsVirtualMachine { get; set; }
        [JsonProperty]
        public string LastLogonTimestamp { get; set; }
    }
}
