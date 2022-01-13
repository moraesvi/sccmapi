using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI.AcutualConfig
{
    /// <summary>
    /// Objeto WMI referente busca de politicas de deploy do SCCM
    /// </summary>
    public partial class CCMSoftwareDistributionClientConfig : SMSBase<CCMSoftwareDistributionClientConfig>
    {
        private static string _query = "SELECT * FROM CCM_SoftwareDistributionClientConfig";
        public CCMSoftwareDistributionClientConfig()
            : base(new PSQuery(SCCMHelper.CCMActualConfigNamespace, _query), "")
        {

        }
        public bool ADV_RebootLogoffNotification { get; set; }
        public UInt32 ADV_RebootLogoffNotificationCountdownDuration { get; set; }
        public UInt32 ADV_RebootLogoffNotificationFinalWindow { get; set; }
        public UInt32 ADV_RunNotificationCountdownDuration { get; set; }
        public UInt32 ADV_WhatsNewDuration { get; set; }
        public UInt32 CacheContentTimeout { get; set; }
        public UInt32 CacheSpaceFailureRetryCount { get; set; }
        public UInt32 CacheSpaceFailureRetryInterval { get; set; }
        public UInt32 CacheTombstoneContentMinDuration { get; set; }
        public string ComponentName { get; set; }
        public UInt32 ContentLocationTimeoutInterval { get; set; }
        public UInt32 ContentLocationTimeoutRetryCount { get; set; }
        public UInt32 DefaultMaxDuration { get; set; }
        public bool DisplayNewProgramNotification { get; set; }
        public bool Enabled { get; set; }
        public UInt32 ExecutionFailureRetryCount { get; set; }
        public UInt32[] ExecutionFailureRetryErrorCodes { get; set; }
        public UInt32 ExecutionFailureRetryInterval { get; set; }
        public bool LockSettings { get; set; }
        public UInt32[] LogoffReturnCodes { get; set; }
        public string NetworkAccessPassword { get; set; }
        public string NetworkAccessUsername { get; set; }
        public UInt32 NetworkFailureRetryCount { get; set; }
        public UInt32 NetworkFailureRetryInterval { get; set; }
        public string NewProgramNotificationUI { get; set; }
        public bool PRG_PRF_RunNotification { get; set; }
        public UInt32[] RebootReturnCodes { get; set; }
        public string Reserved { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public UInt32 SiteSettingsKey { get; set; }
        public UInt32[] SuccessReturnCodes { get; set; }
        public UInt32 UIContentLocationTimeoutInterval { get; set; }
        public UInt32 UserPreemptionCountdown { get; set; }
        public UInt32 UserPreemptionTimeout { get; set; }
    }
}