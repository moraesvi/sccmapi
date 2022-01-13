using Newtonsoft.Json;
using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Win32ComputerSystemDomain : WMIBase<Win32ComputerSystemDomain>
    {
        private const string QUERY_BASE = "Select Domain, Name From Win32_ComputerSystem";
        public Win32ComputerSystemDomain()
            : base(new PSQuery(QUERY_BASE))
        {

        }
        [JsonProperty]
        public string Domain { get; set; }
        [JsonProperty]
        public string Name { get; set; }
    }
}
