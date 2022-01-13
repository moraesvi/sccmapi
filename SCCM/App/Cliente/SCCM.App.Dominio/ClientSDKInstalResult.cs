using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.App.Dominio
{
    public class ClientSDKInstalResult
    {
        public bool OK { get; set; }
        public string MsgTipo { get; set; }
        public string MsgResultado { get; set; }
    }
}
