using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSApplicationCustom
    {
        public IWMIResult ObterNomeResult(string nomeApp)
        {
            SMSApplicationCustom[] smsApplicationCustom = ObterNome(nomeApp);

            return Resultado(smsApplicationCustom);
        }
        public IWMIResult ObterModelNameResult(string nomeApp)
        {
            SMSApplicationCustom smsApplicationCustom = ObterModelName(nomeApp);

            return Resultado(smsApplicationCustom);
        }
        public SMSApplicationCustom[] ObterNome(string nomeApp)
        {
            PSQuery.AddFiltro("LocalizedDisplayName", nomeApp);
            SMSApplicationCustom[] smsApplicationCustom = PSRequisicao.ExecutarColecao(PSQuery.ToString(), this);

            return smsApplicationCustom;
        }
        public SMSApplicationCustom ObterModelName(string modelId)
        {
            PSQuery.AddFiltro("ModelName", modelId);
            SMSApplicationCustom[] smsApplicationCustom = PSRequisicao.ExecutarColecao(PSQuery.ToString(), this);

            if (smsApplicationCustom != null || smsApplicationCustom.Count() == 0)
            {
                return smsApplicationCustom.FirstOrDefault();
            }

            return null;
        }
    }
}
