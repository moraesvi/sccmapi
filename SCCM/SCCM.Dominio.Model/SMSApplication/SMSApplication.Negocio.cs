using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSApplication
    {
        public IWMIResult ListarNome(string nome)
        {
            SMSApplication[] smsAppResult = PSRequisicao.ExecutarColecao(PSQuery, "LocalizedDisplayName", nome, this);

            return Resultado(smsAppResult);
        }
    }
}
