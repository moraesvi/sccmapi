using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model.RequestedConfig
{
    public class CCMSourceUpdateClientConfig : WMIObjetoBase
    {
        public const string QUERY = "([WMIClass]\"root\\ccm\\Policy\\Machine\\RequestedConfig:CCM_SourceUpdateClientConfig\").CreateInstance() | Get-Member -MemberType Property";
        public CCMSourceUpdateClientConfig(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
