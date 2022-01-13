using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;

namespace SCCM.Test.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSRUserAppDeploymentTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSRUserAppDeploymentTest_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            string usuario = "N879315";

            SMSRUserAppDeployment smsUserComputerAppDepl = new SMSRUserAppDeployment(usuario);

            IWMIResult result = smsUserComputerAppDepl.ListarResult();           

            Assert.IsNotNull(result);
        }
    }
}
