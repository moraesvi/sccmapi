using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.GeradorModel.Model.ActualConfig
{
    public class CCMInventoryClientConfig : WMIObjetoBase
    {
        public CCMInventoryClientConfig(bool newtonsoftJsonSerialization)
            : base("", newtonsoftJsonSerialization)
        {
            throw new NotImplementedException("Não encontrado no SCCM");
        }
    }
}
