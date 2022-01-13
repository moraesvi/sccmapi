using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI.AcutualConfig
{
    public class CCMSoftwareMeteringClientConfig
    {
        public string ComponentName { get; set; }
        public bool Enabled { get; set; }
        public UInt32 MaximumUsageInstancesPerReport { get; set; }
        public UInt32 ReportTimeout { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public UInt32 SiteSettingsKey { get; set; }

    }
}