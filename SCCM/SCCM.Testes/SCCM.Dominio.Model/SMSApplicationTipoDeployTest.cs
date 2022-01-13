using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Testes.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSApplicationTipoDeployTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSApplicationTipoDeploy_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSApplicationTipoDeploy smsAppTpDeploy = new SMSApplicationTipoDeploy();

            IWMIResult result = smsAppTpDeploy.ListarTipoScriptResult();

            Assert.IsNotNull(result);
        }
    }
}
