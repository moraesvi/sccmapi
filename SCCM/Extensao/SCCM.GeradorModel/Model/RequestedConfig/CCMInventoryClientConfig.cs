using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model.RequestedConfig
{
    public class CCMInventoryClientConfig : WMIObjetoBase
    {
        public CCMInventoryClientConfig(string query, bool newtonsoftJsonSerialization)
            : base(query, newtonsoftJsonSerialization)
        {
            throw new NotImplementedException("Não encontrado no SCCM");
        }
    }
}
