using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI.AcutualConfig
{
    public class CCMSourceUpdateClientConfig
    {
        public string ComponentName { get; set; }
        public bool Enabled { get; set; }
        public UInt32 LocationTimeOut { get; set; }
        public UInt32 MaxRetryCount { get; set; }
        public UInt32 NetworkChangeDelay { get; set; }
        public bool RemoteDPs { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public UInt32 RetryTimeOut { get; set; }
        public UInt32 SiteSettingsKey { get; set; }
    }
}