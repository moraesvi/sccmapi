using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSDeploymentInfo : SMSBase<SMSDeploymentInfo>
    {
        private const string QUERY_BASE = "Select * from SMS_DeploymentInfo";
        public SMSDeploymentInfo()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "DeploymentID")
        {

        }
        public string CollectionID { get; set; }
        public string CollectionName { get; set; }
        public string DeploymentID { get; set; }
        public UInt32 DeploymentIntent { get; set; }
        public string DeploymentName { get; set; }
        public UInt32 DeploymentType { get; set; }
        public UInt32 DeploymentTypeID { get; set; }
        public string TargetID { get; set; }
        public string TargetName { get; set; }
        public UInt32 TargetSecurityTypeID { get; set; }
        public string TargetSubName { get; set; }
    }
}
