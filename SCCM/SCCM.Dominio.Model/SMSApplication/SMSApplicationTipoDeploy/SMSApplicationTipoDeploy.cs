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
    public partial class SMSApplicationTipoDeploy : WMIBase<SMSApplicationTipoDeploy>
    {
        private const string QUERY_BASE = @"Select SMS_A.CI_UniqueID, SMS_A.LocalizedDisplayName, SMS_DT.Technology From SMS_Application SMS_A Join SMS_DeploymentType SMS_DT On SMS_A.ModelName = SMS_DT.AppModelName Where SMS_A.IsExpired = 'False' And SMS_A.IsEnabled = 'True'";
        private const string OBJECT_RESULT = "Select-Object @{Name=\"CI_UniqueID\"; Expression={$_.SMS_A.CI_UniqueID}}, @{Name=\"LocalizedDisplayName\"; Expression={$_.SMS_A.LocalizedDisplayName}}, @{Name=\"Technology\"; Expression={$_.SMS_DT.Technology}}";       
        public SMSApplicationTipoDeploy()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE, OBJECT_RESULT), "CI_UniqueID")
        {

        }
        [JsonProperty]
        public string CI_UniqueID { get; set; }
        [JsonProperty]
        public string LocalizedDisplayName { get; set; }
        [JsonProperty]
        public string Technology { get; set; }
    }
}
