using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model.ActualConfig
{
    public class CCMSoftwareDistributionClientConfig : WMIObjetoBase
    {
        public const string QUERY = "([WMIClass]\"root\\ccm\\Policy\\Machine\\ActualConfig:CCM_SoftwareDistributionClientConfig\").CreateInstance() | Get-Member -MemberType Property";
        public CCMSoftwareDistributionClientConfig(bool newtonsoftJsonSerialization)
            : base(QUERY, newtonsoftJsonSerialization)
        {

        }
    }
}
