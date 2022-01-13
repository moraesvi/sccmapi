using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSApplicationAssignment : SMSBase<SMSApplicationAssignment>
    {
        private const string QUERY_BASE = "Select * from SMS_ApplicationAssignment";
        public SMSApplicationAssignment()
           : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CI_UniqueID")
        {

        }
        public string ApplicationName { get; set; }
        public bool? ApplyToSubTargets { get; set; }
        public UInt32 AppModelID { get; set; }
        public int[] AssignedCIs { get; set; }
        public string AssignedCI_UniqueID { get; set; }
        public int? AssignmentAction { get; set; }
        public string AssignmentDescription { get; set; }
        public int? AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public int? AssignmentType { get; set; }
        public string AssignmentUniqueID { get; set; }
        public string CollectionName { get; set; }
        public bool? ContainsExpiredUpdates { get; set; }
        public string CreationTime { get; set; }
        public int? DesiredConfigType { get; set; }
        public bool? DisableMomAlerts { get; set; }
        public UInt32 DPLocality { get; set; }
        public bool? Enabled { get; set; }
        public string EnforcementDeadline { get; set; }
        public string EvaluationSchedule { get; set; }
        public string ExpirationTime { get; set; }
        public string LastModificationTime { get; set; }
        public string LastModifiedBy { get; set; }
        public UInt32 LocaleID { get; set; }
        public bool? LogComplianceToWinEvent { get; set; }
        public int NonComplianceCriticality { get; set; }
        public bool? NotifyUser { get; set; }
        public UInt32 OfferFlags { get; set; }
        public int? OfferTypeID { get; set; }
        public bool? OverrideServiceWindows { get; set; }
        public bool? PersistOnWriteFilterDevices { get; set; }
        //public SMS_ApplicationPolicyTemplateBinding[] PolicyBinding { get; set; }
        public int? Priority { get; set; }
        public bool? RaiseMomAlertsOnFailure { get; set; }
        public bool? RebootOutsideOfServiceWindows { get; set; }
        public bool? RequireApproval { get; set; }
        public bool? SendDetailedNonComplianceStatus { get; set; }
        public string SourceSite { get; set; }
        public string StartTime { get; set; }
        public UInt32 StateMessagePriority { get; set; }
        public UInt32 SuppressReboot { get; set; }
        public string TargetCollectionID { get; set; }
        public string UpdateDeadline { get; set; }
        public bool? UpdateSupersedence { get; set; }
        public bool? UseGMTTimes { get; set; }
        public bool? UserUIExperience { get; set; }
        public bool? WoLEnabled { get; set; }
    }
}
