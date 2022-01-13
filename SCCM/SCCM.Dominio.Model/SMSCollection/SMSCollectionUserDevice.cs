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
    public partial class SMSCollectionUserDevice : WMIBase<SMSCollectionUserDevice>
    {
        private string _idColecao;
        private const string QUERY_BASE = "Select SMS_R_System.Name, SMS_R_System.LastLogonTimestamp, SMS_R_User.Name From SMS_R_User Join SMS_UserMachineRelationship On SMS_R_User.UniqueUserName = SMS_UserMachineRelationship.UniqueUserName Join SMS_R_System On SMS_UserMachineRelationship.ResourceName = SMS_R_System.Name Join SMS_FullCollectionMembership On LOWER(SMS_UserMachineRelationship.UniqueUserName) = LOWER(SMS_FullCollectionMembership.SMSID) Where SMS_FullCollectionMembership.CollectionId = '{0}'";
        private const string OBJECT_RESULT = "Select-Object @{Name=\"UserName\"; Expression={$_.SMS_R_User.Name}}, @{Name=\"DeviceName\"; Expression={$_.SMS_R_System.Name}}, @{Name=\"LastLogonTimestamp\"; Expression={$_.ConvertToDateTime($_.SMS_R_System.LastLogonTimestamp)}}";
        public SMSCollectionUserDevice(string idColecao)
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE, OBJECT_RESULT))
        {
            _idColecao = idColecao;
        }
        [JsonProperty]
        public string DeviceName { get; set; }
        [JsonProperty]
        public DateTime LastLogonTimestamp { get; set; }
        [JsonProperty]
        public string UserName { get; set; }
    }
}
