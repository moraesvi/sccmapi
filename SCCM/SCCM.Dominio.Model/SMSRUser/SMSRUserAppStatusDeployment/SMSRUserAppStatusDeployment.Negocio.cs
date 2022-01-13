using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSRUserAppStatusDeployment
    {
        public IWMIResult ObterResult(bool verificarStatusAndamento = false)
        {
            try
            {
                SMSRUserAppStatusDeployment smsDeployment = PSRequisicao.Executar(PSComando, ParamValor, this);

                return Resultado(smsDeployment);
            }
            catch (Exception ex)
            {
                IWMIResult WMIErroResult = WMIResultFactory.Criar(null, "Erro na requisição", ex.Message, (ex.InnerException != null ? ex.InnerException.Message : null));

                return WMIErroResult;
            }
        }
        public SMSRUserAppStatusDeployment Obter()
        {
            try
            {
                SMSRUserAppStatusDeployment smsDeployment = PSRequisicao.Executar(PSComando, ParamValor, this);

                return smsDeployment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
