using SCCM.Dominio.Comum;
using SCCM.Dominio.Comum.Concreto;
using SCCM.Dominio.Comum.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSClient : SMSBase<SMSClient>
    {
        private const string QUERY_BASE = "Select * From SMS_Client";
        private IParseArquivoResult _parseResultado;

        public SMSClient()
           : base(new PSQuery(SCCMHelper.CCMNamespace, QUERY_BASE), "")
        {
            _parseResultado = new ParseXMLArquivoJSONResult();
        }
        public bool AllowLocalAdminOverride { get; set; }
        public UInt32 ClientType { get; set; }
        public string ClientVersion { get; set; }
        public Boolean EnableAutoAssignment { get; set; }
    }
}
