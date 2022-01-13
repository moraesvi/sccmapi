using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSRUserDevice
    {
        public SMSRUserDevice Executar()
        {
            try
            {
                SMSRUserDevice smsUserDevice = PSRequisicao.Executar(PSComando, ParamValor, this);

                return smsUserDevice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
