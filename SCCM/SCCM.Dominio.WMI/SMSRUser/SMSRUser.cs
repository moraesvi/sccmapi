using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSRUser : SMSBase<SMSRUser>
    {
        private const string QUERY_BASE = "Select * from SMS_R_User";
        public SMSRUser()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "")
        {

        }
        public string[] AgentName { get; set; }
        public string[] AgentSite { get; set; }
        public string[] AgentTime { get; set; }
        public string CreationDate { get; set; }
        public string DistinguishedName { get; set; }
        public string FullDomainName { get; set; }
        public string FullUserName { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string NetworkOperatingSystem { get; set; }
        public byte[] ObjectGUID { get; set; }
        public UInt32 PrimaryGroupID { get; set; }
        public UInt32 ResourceId { get; set; }
        public UInt32 ResourceType { get; set; }
        public string[] SecurityGroupName { get; set; }
        public string SID { get; set; }
        public string UniqueUserName { get; set; }
        public UInt32 UserAccountControl { get; set; }
        public string[] UserContainerName { get; set; }
        public string[] UserGroupName { get; set; }
        public string UserName { get; set; }
        public string[] UserOUName { get; set; }
        public string UserPrincipalName { get; set; }
        public string WindowsNTDomain { get; set; }
    }
}
