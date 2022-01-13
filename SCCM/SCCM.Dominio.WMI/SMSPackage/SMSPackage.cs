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
    public partial class SMSPackage : SMSBase<SMSPackage>
    {
        private const string QUERY_BASE = "Select * from SMS_Package";

        public SMSPackage()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE), "")
        {

        }
        public UInt32 ActionInProgress { get; set; }
        public string AlternateContentProviders { get; set; }
        public int DefaultImageFlags { get; set; }
        public string Description { get; set; }
        public byte[] ExtendedData { get; set; }
        public UInt32 ExtendedDataSize { get; set; }
        public UInt32 ForcedDisconnectDelay { get; set; }
        public UInt32 ForcedDisconnectNumRetries { get; set; }
        public byte[] Icon { get; set; }
        public UInt32 IconSize { get; set; }
        public byte[] ISVData { get; set; }
        public UInt32 ISVDataSize { get; set; }
        public string ISVString { get; set; }
        public string Language { get; set; }
        public string LastRefreshTime { get; set; }
        public string[] LocalizedCategoryInstanceNames { get; set; }
        public string Manufacturer { get; set; }
        public string MIFFilename { get; set; }
        public string MIFName { get; set; }
        public string MIFPublisher { get; set; }
        public string MIFVersion { get; set; }
        public string Name { get; set; }
        public UInt32 NumOfPrograms { get; set; }
        public string PackageID { get; set; }
        public UInt32 PackageSize { get; set; }
        public UInt32 PackageType { get; set; }
        public UInt32 PkgFlags { get; set; }
        public UInt32 PkgSourceFlag { get; set; }
        public string PkgSourcePath { get; set; }
        public string PreferredAddressType { get; set; }
        public UInt32 Priority { get; set; }
        //public SMS_ScheduleToken[] RefreshSchedule { get; set; }
        public string[] SecuredScopeNames { get; set; }
        public string SedoObjectVersion { get; set; }
        public string ShareName { get; set; }
        public UInt32 ShareType { get; set; }
        public string SourceDate { get; set; }
        public string SourceSite { get; set; }
        public UInt32 SourceVersion { get; set; }
        public string StoredPkgPath { get; set; }
        public UInt32 StoredPkgVersion { get; set; }
        public string TransformAnalysisDate { get; set; }
        public UInt32 TransformReadiness { get; set; }
        public string Version { get; set; }
    }
}
