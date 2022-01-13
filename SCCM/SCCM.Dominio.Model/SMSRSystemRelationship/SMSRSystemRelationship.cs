using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class SMSRSystemRelationship : WMIBase<SMSRSystemRelationship>
    {
        private const string WHERE_USUARIO = "Where SMS_R_User.UniqueUserName = '$usuario'";
        private const string WHERE_COLECAO = "Where SMS_R_User.UniqueUserName = '$idColecao'";
        private const string QUERY_BASE = "Select SMS_R_System.Name, SMS_R_System.NetbiosName, SMS_R_System.IsVirtualMachine, SMS_R_System.LastLogonTimestamp, SMS_R_System.FullDomainName From SMS_R_System Join SMS_UserMachineRelationship On SMS_UserMachineRelationship.ResourceName = SMS_R_System.Name Join SMS_R_User On LOWER(SMS_R_User.UniqueUserName) = LOWER(SMS_UserMachineRelationship.UniqueUserName)";
        private const string OBJECT_RESULT = "Select-Object @{Name=\"Name\"; Expression={$_.Name}}, @{Name=\"NetbiosName\"; Expression={$_.NetbiosName}}, @{Name=\"IsVirtualMachine\"; Expression={$_.IsVirtualMachine}}, @{Name=\"LastLogonTimestamp\"; Expression={$_.ConvertToDateTime($_.LastLogonTimestamp)}}, @{Name=\"FullDomainName\"; Expression={$_.FullDomainName}}";
        private const string PREF_CACHE = "CACHE-";

        private ICache _cache;
        /// <summary>
        /// Cache Default - Arquivo JSON
        /// </summary>
        public SMSRSystemRelationship()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE, OBJECT_RESULT), "Name")
        {
            _cache = new CacheArquivoJSON();
        }
        public SMSRSystemRelationship(ICache cache)
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE, OBJECT_RESULT), "Name")
        {
            _cache = cache;
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
        public DateTime LastLogonTimestamp { get; set; }
    }
}
