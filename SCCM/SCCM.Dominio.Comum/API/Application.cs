using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class ApplicationModel
    {
        private Application[] _applications;
        public ApplicationModel()
        {

        }
        public int TotalLinhas { get; set; }
        public int MaximoReg { get; set; }
        public Application[] Applications
        {
            get { return _applications; }
            set { _applications = value; }
        }
    }
    public class Application
    {
        public string CI_UniqueID { get; set; }
        public string LocalizedDisplayName { get; set; }
        public string AppVersion { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Publisher { get; set; }
    }
}
