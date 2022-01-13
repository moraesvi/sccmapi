using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI.RequestedConfig
{
    public class CCMSoftwareMeteringClientConfig
    {
        public string ComponentName { get; set; }
        public bool Enabled { get; set; }
        public UInt32 MaximumUsageInstancesPerReport { get; set; }
        public string PolicyID { get; set; }
        public string PolicyInstanceID { get; set; }
        public UInt32 PolicyPrecedence { get; set; }
        public string PolicyRuleID { get; set; }
        public string PolicySource { get; set; }
        public string PolicyVersion { get; set; }
        public UInt32 ReportTimeout { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public UInt32 SiteSettingsKey { get; set; }
    }
}