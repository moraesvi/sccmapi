using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSProviderLocation : SMSBase<SMSProviderLocation>
    {
        private const string QUERY_BASE = "Select* From SMS_ProviderLocation";
        public SMSProviderLocation()
            : base(new PSQuery(SCCMHelper.SMSSiteNamespace, QUERY_BASE))
        {

        }
        public string Machine { get; set; }
        public string NamespacePath { get; set; }
        public bool ProviderForLocalSite { get; set; }
        public string SiteCode { get; set; }
    }
}
