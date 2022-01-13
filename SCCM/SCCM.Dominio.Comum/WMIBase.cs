using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Comum
{
    public class WMIBase
    {
        public string __CLASS { get; set; }
        public string[] __DERIVATION { get; set; }
        public string __DYNASTY { get; set; }
        public int __GENUS { get; set; }
        public string __NAMESPACE { get; set; }
        public string __PATH { get; set; }
        public int __PROPERTY_COUNT { get; set; }
        public string __RELPATH { get; set; }
        public string __SERVER { get; set; }
        public string __SUPERCLASS { get; set; }
    }
}
