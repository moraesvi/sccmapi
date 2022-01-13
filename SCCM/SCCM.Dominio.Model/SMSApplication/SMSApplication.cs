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
    public partial class SMSApplication : WMIBase<SMSApplication>
    {
        private const string QUERY_BASE = "Select CI_UniqueID, LocalizedDisplayName, IsEnabled, IsDeployable, NumberOfDevicesWithApp from SMS_Application";
        public SMSApplication()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CI_UniqueID")
        {
            
        }
        [JsonProperty]
        public string CI_UniqueID { get; set; }
        [JsonProperty]
        public string LocalizedDisplayName { get; set; }
        [JsonProperty]
        public bool IsEnabled { get; set; }
        [JsonProperty]
        public bool IsDeployable { get; set; }
        [JsonProperty]
        public UInt32 NumberOfDevicesWithApp { get; set; }
    }
}
