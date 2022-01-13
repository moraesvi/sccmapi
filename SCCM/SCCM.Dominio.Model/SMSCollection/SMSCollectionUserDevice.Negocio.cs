using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;

namespace SCCM.Dominio.Model
{
    public partial class SMSCollectionUserDevice
    {
        public IWMIResult Obter()
        {
            PSQuery.AddFiltro(_idColecao);
            SMSCollectionUserDevice[] smsCollectionUserDeviceResult = PSRequisicao.ExecutarColecao(PSQuery.ToString(), this);

            return Resultado(smsCollectionUserDeviceResult);
        }
    }
}
