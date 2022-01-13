using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class TriggerSchedule
    {

        public static readonly string ApplicationDeploymentEvaluationCycleId = "{00000000-0000-0000-0000-000000000121}"; 
        public static readonly string DiscoveryDataCollectionCycleId = "{00000000-0000-0000-0000-000000000003}";
        public static readonly string FileCollectionCycleId = "{00000000-0000-0000-0000-000000000010}";
        public static readonly string HardwareInventoryCycleId = "{00000000-0000-0000-0000-000000000021}"; 
        public static readonly string MachinePolicyRetrievalCycleId = "{00000000-0000-0000-0000-000000000021}"; 
        public static readonly string MachinePolicyEvaluationCycleId = "{00000000-0000-0000-0000-000000000022}"; 
        public static readonly string SoftwareInventoryCycleId = "{00000000-0000-0000-0000-000000000002}"; 
        public static readonly string SoftwareMeteringUsageReportCycleId =  "{00000000-0000-0000-0000-000000000031}"; 
        public static readonly string SoftwareUpdatesAssignmentsEvaluationCycleId = "{00000000-0000-0000-0000-000000000108}"; 
        public static readonly string SoftwareUpdateScanCycleId = "{00000000-0000-0000-0000-000000000113}";
        public static readonly string StateMessageRefreshId = "{00000000-0000-0000-0000-000000000111}"; 
        public static readonly string UserPolicyRetrievalCycleId = "{00000000-0000-0000-0000-000000000026}"; 
        public static readonly string UserPolicyEvaluationCycleId = "{00000000-0000-0000-0000-000000000027}"; 
        public static readonly string WindowsInstallersSourceListUpdateCycleId = "{00000000-0000-0000-0000-000000000032}"; 
    }
}
