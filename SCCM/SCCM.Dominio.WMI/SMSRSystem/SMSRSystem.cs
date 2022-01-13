using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSRSystem : SMSBase<SMSRSystem>
    {
        private const string QUERY_BASE = "Select * from SMS_R_System";
        private const string PREF_CACHE = "CACHE-";

        /// <summary>
        /// Cache Default - Arquivo JSON
        /// </summary>
        public SMSRSystem()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "")
        {
            
        }
        public UInt32 Active { get; set; }
        public string ADSiteName { get; set; }
        public string[] AgentName { get; set; }
        public string[] AgentSite { get; set; }
        public string[] AgentTime { get; set; }
        public UInt32 AlwaysInternet { get; set; }
        public string AMTFullVersion { get; set; }
        public UInt32 AMTStatus { get; set; }
        public UInt32 Client { get; set; }
        public UInt32 ClientEdition { get; set; }
        public UInt32 ClientType { get; set; }
        public string ClientVersion { get; set; }
        public string CPUType { get; set; }
        public string CreationDate { get; set; }
        public UInt32 Decommissioned { get; set; }
        public UInt32 DeviceOwner { get; set; }
        public string DistinguishedName { get; set; }
        public string EASDeviceID { get; set; }
        public string FullDomainName { get; set; }
        public string HardwareID { get; set; }
        public UInt32 InternetEnabled { get; set; }
        public string[] IPAddresses { get; set; }
        public string[] IPSubnets { get; set; }
        public string[] IPv6Addresses { get; set; }
        public string[] IPv6Prefixes { get; set; }
        public UInt32 IsClientAMT30Compatible { get; set; }
        public string LastLogonTimestamp { get; set; }
        public string LastLogonUserDomain { get; set; }
        public string LastLogonUserName { get; set; }
        public string[] MACAddresses { get; set; }
        public string MDMComplianceStatus { get; set; }
        public string Name { get; set; }
        public string NetbiosName { get; set; }
        public byte[] ObjectGUID { get; set; }
        public UInt32 Obsolete { get; set; }
        public string OperatingSystemNameandVersion { get; set; }
        public string PreviousSMSUUID { get; set; }
        public UInt32 PrimaryGroupID { get; set; }
        public string PublisherDeviceID { get; set; }
        public string ResourceDomainORWorkgroup { get; set; }
        public UInt32 ResourceId { get; set; }
        public string[] ResourceNames { get; set; }
        public UInt32 ResourceType { get; set; }
        public string[] SecurityGroupName { get; set; }
        public string SID { get; set; }
        public string SMBIOSGUID { get; set; }
        public string[] SMSAssignedSites { get; set; }
        public string[] SMSInstalledSites { get; set; }
        public string[] SMSResidentSites { get; set; }
        public string SMSUniqueIdentifier { get; set; }
        public string SMSUUIDChangeDate { get; set; }
        public string SNMPCommunityName { get; set; }
        public UInt32 SuppressAutoProvision { get; set; }
        public string[] SystemContainerName { get; set; }
        public string[] SystemGroupName { get; set; }
        public string[] SystemOUName { get; set; }
        public string[] SystemRoles { get; set; }
        public UInt32 Unknown { get; set; }
        public UInt32 UserAccountControl { get; set; }
        public string VirtualMachineHostName { get; set; }
        public UInt32 VirtualMachineType { get; set; }
        public UInt32 WipeStatus { get; set; }
        public string WTGUniqueKey { get; set; }
    }
}
