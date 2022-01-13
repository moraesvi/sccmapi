using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.Model
{
    public partial class SMSCollection
    {
        public SMSCollection[] ListarNome(string nome)
        {
            SMSCollection[] smsCollectionResult = PSRequisicao.ExecutarColecao(PSQuery, "Name", nome, this);

            return smsCollectionResult;
        }
        public IWMIResult ListarNomeResult(string nome)
        {
            SMSCollection[] smsCollectionResult = PSRequisicao.ExecutarColecao(PSQuery, "Name", nome, this);

            return Resultado<SMSCollection[]>(smsCollectionResult);
        }
    }
}
