using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model.RequestedConfig
{
    public class CCMSoftwareMeteringClientConfig : WMIObjetoBase
    {
        private const string QUERY = "([WMIClass]\"root\\ccm\\Policy\\Machine\\RequestedConfig:CCM_SoftwareMeteringClientConfig\").CreateInstance() | Get-Member -MemberType Property";
        public CCMSoftwareMeteringClientConfig(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
