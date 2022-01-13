using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class CCMSoftwareCatalogUtilities
    {
        private const string INSTANCE = "[wmiclass]'root/ccm/clientSDK:CCM_SoftwareCatalogUtilities'";
        public string ClientId { get; set; }
        public string SignedClientId { get; set; }
    }
}
