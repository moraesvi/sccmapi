using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSSTRecurInterval : SMSScheduleToken
    {
        private PSClasse _PSClasse;
        public SMSSTRecurInterval()
        {
            _PSClasse = new PSClasse(SCCMHelper.SMSSiteNamespace, "SMS_ST_RecurInterval");
        }
        public UInt32 HourSpan { get; set; }
        public UInt32 DaySpan { get; set; }
    }
}