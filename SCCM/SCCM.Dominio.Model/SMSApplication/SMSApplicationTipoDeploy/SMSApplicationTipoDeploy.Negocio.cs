using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSApplicationTipoDeploy
    {
        public IWMIResult ListarTipoScriptResult()
        {
            PSQuery.AddFiltro("Technology", "Script");
            SMSApplicationTipoDeploy[] smsApplicationTpDeploy = PSRequisicao.ExecutarColecao(PSQuery.ToString(), this);

            return Resultado(smsApplicationTpDeploy);
        }
        public SMSApplicationTipoDeploy[] ListarTipoScript()
        {
            PSQuery.AddFiltro("Technology", "Script");
            SMSApplicationTipoDeploy[] smsApplicationTpDeploy = PSRequisicao.ExecutarColecao(PSQuery.ToString(), this);

            return smsApplicationTpDeploy;
        }
    }
}
