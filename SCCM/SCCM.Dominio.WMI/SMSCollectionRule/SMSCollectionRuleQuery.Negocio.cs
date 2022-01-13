using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Dominio.WMI
{
    public partial class SMSCollectionRuleQuery
    {
        public string PsInstance()
        {
            string psInstance = SCCMUteis.PSInstance(this.PSClasse, this);

            return psInstance;
        }
        public string PSInstance()
        {
            string psInstance = SCCMUteis.PSInstance(this.PSClasse, this);

            return psInstance;
        }
        internal bool ValidarQuery()
        {
            string scriptPS = string.Concat("Invoke-WmiMethod -Namespace ", this.PSClasse.Namespace, " -Class ", this.PSClasse.Classe, " ValidateQuery ", " -ArgumentList \"", this.QueryExpression, "\"");

            string result = PSRequisicao.Executar(scriptPS);

            if (string.IsNullOrEmpty(result))
            {
                throw new InvalidOperationException("Ocorreu um erro na validação de MembershipRule", new Exception("A query inserida é inválida."));
            }

            return true;
        }
    }
}
