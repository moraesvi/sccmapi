using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCCM.Dominio.WMI
{
    public partial class SMSScheduleToken
    {
        private PSClasse _PSClasse;
        public SMSScheduleToken()
        {
            _PSClasse = new PSClasse(SCCMHelper.SMSSiteNamespace, "SMS_ScheduleToken");
        }
        internal PSClasse PSClasse
        {
            get { return _PSClasse; }
        }
        public UInt32 DayDuration { get; set; }
        public UInt32 HourDuration { get; set; }
        public Boolean IsGMT { get; set; }
        public UInt32 MinuteDuration { get; set; }
        public string StartTime { get; set; }
    }
}