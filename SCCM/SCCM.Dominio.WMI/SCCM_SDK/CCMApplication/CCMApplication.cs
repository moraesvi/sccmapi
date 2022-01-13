using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI.SCCM_SDK
{
    public partial class CCMApplication : WMIBase<CCMApplication>
    {
        private const string QUERY_BASE = "Select * from CCM_Application";
        public CCMApplication()
            : base(new PSQuery(SCCMHelper.SCCMSDKNamespace, QUERY_BASE), "Id")
        {

        }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Revision { get; set; }
        public bool IsMachineTarget { get; set; }
        public string ApplicabilityState { get; set; }
    }
}
