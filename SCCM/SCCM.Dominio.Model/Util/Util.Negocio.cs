using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCCM.PowerShell;
using SCCM.Dominio.Comum;

namespace SCCM.Dominio.Model
{
    public partial class Util
    {
        public IWMIResult PSComandoResult(string psComando)
        {
            bool executado = PSRequisicao.ExecutarResult(psComando);

            return WMIResult.Resultado<bool>(executado);
        }
        public bool PSComando(string psComando)
        {
            bool executado = PSRequisicao.ExecutarResult(psComando);

            return executado;
        }
    }
}
