using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCCM.PowerShell;
using SCCM.Dominio.Comum;

namespace SCCM.Dominio.WMI.SCCM_SDK
{
    public partial class CPApplet
    {
        private const string PS_CPAPLET_TEMPLATE =
@"$conapp =  new-object -comobject ""CPApplet.CPAppletMgr""
  $actions = $conapp.GetClientActions()
  Foreach( $act in $actions)
  {
      If( $act.ActionId -eq ""{00000000-0000-0000-0000-000000000123}"") { $act.PerformAction() }
  }";
        public IWMIResult ExecutarAcoesResult()
        {
            bool acoesProcessadas = ExecutarAcoes();

            return WMIResult.Resultado<bool>(acoesProcessadas);
        }
        public bool ExecutarAcoes()
        {
            bool acoesProcessadas = PSRequisicao.ExecutarResult(PS_CPAPLET_TEMPLATE);

            return acoesProcessadas;
        }
    }
}
