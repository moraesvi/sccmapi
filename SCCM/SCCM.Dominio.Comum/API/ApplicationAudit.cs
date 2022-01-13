using SCCM.Dominio.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class ApplicationAudit 
    {
        private AppInstalacaoStatusAudit _appDetalheAudit;
        public ApplicationAudit()
        {
            _appDetalheAudit = new AppInstalacaoStatusAudit();
        }
        public string CI_UniqueID { get; set; } 
        public AppInstalacaoStatusAudit AppDetalheAudit
        {
            get { return _appDetalheAudit; }
            set { _appDetalheAudit = value; }
        }
    }
}
