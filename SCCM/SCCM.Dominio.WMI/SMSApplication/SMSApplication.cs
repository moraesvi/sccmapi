using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSApplication : SMSBase<SMSApplication>
    {
        private const string QUERY_BASE = "Select * from SMS_Application";
        public SMSApplication()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "CI_UniqueID")
        {

        }
        public string ApplicabilityCondition { get; set; }
        public string[] CategoryInstance_UniqueIDs { get; set; }
        public UInt32 CIType_ID { get; set; }
        public UInt32 CIVersion { get; set; }
        public UInt32 CI_ID { get; set; }
        public string CI_UniqueID { get; set; }
        public string CreatedBy { get; set; }
        public string DateCreated { get; set; }
        public string DateLastModified { get; set; }
        public string EffectiveDate { get; set; }
        public UInt32 EULAAccepted { get; set; }
        public bool EULAExists { get; set; }
        public string EULASignoffDate { get; set; }
        public string EULASignoffUser { get; set; }
        public UInt32 ExecutionContext { get; set; }
        public UInt32 Featured { get; set; }
        public bool HasContent { get; set; }
        public bool IsBundle { get; set; }
        public bool IsDeployable { get; set; }
        public bool IsDeployed { get; set; }
        public bool IsDigest { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsExpired { get; set; }
        public bool IsHidden { get; set; }
        public bool IsLatest { get; set; }
        public bool IsQuarantined { get; set; }
        public bool IsSuperseded { get; set; }
        public bool IsSuperseding { get; set; }
        public bool IsUserDefined { get; set; }
        public bool IsVersionCompatible { get; set; }
        public string LastModifiedBy { get; set; }
        public string[] LocalizedCategoryInstanceNames { get; set; }
        public string LocalizedDescription { get; set; }
        public string LocalizedDisplayName { get; set; }
        public string LocalizedInformativeURL { get; set; }
        public UInt32 LocalizedPropertyLocaleID { get; set; }
        public UInt32 LogonRequirement { get; set; }
        public string Manufacturer { get; set; }
        public UInt32 ModelID { get; set; }
        public string ModelName { get; set; }
        public UInt32 NumberOfDependentDTs { get; set; }
        public UInt32 NumberOfDependentTS { get; set; }
        public UInt32 NumberOfDeployments { get; set; }
        public UInt32 NumberOfDeploymentTypes { get; set; }
        public UInt32 NumberOfDevicesWithApp { get; set; }
        public UInt32 NumberOfDevicesWithFailure { get; set; }
        public UInt32 NumberOfSettings { get; set; }
        public UInt32 NumberOfUsersWithApp { get; set; }
        public UInt32 NumberOfUsersWithFailure { get; set; }
        public UInt32 NumberOfUsersWithRequest { get; set; }
        public UInt32 NumberOfVirtualEnvironments { get; set; }
        public string PackageID { get; set; }
        public UInt32 PermittedUses { get; set; }
        public string[] PlatformCategoryInstance_UniqueIDs { get; set; }
        public UInt32 PlatformType { get; set; }
        //public SMS_SDMPackageLocalizedData[] SDMPackageLocalizedData { get; set; }
        public UInt32 SDMPackageVersion { get; set; }
        public string SDMPackageXML { get; set; }
        public string[] SecuredScopeNames { get; set; }
        public string SedoObjectVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public UInt32 SourceCIVersion { get; set; }
        public string SourceModelName { get; set; }
        public string SourceSite { get; set; }
        public string SummarizationTime { get; set; }
    }
}
