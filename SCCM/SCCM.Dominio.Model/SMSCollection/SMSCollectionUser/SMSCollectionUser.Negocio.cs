using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSCollectionUser
    {
        public SMSCollectionUser[] ObterDados()
        {
            SMSCollectionUser[] smsCollectionResult = PSRequisicao.ExecutarColecao(this.PSComando, this.ParamValor, this);

            return smsCollectionResult;
        }
        public IWMIResult ObterDadosResult()
        {
            SMSCollectionUser[] smsCollectionResult = PSRequisicao.ExecutarColecao(this.PSComando, this.ParamValor, this);

            return Resultado<SMSCollectionUser[]>(smsCollectionResult);
        }
    }
}
