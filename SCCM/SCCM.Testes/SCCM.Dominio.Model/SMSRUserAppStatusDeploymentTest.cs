using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;

namespace SCCM.Test.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSRUserAppStatusDeploymentTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSRUserAppStatusDeployment_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            string usuario = "N879315";
            string idApp = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_28844877-7881-412e-a051-2821c6651e64";

            SMSRUserAppStatusDeployment smsUserComputerAppDepl = new SMSRUserAppStatusDeployment(usuario, idApp);

            IWMIResult result = smsUserComputerAppDepl.ObterResult();           

            Assert.IsNotNull(result);
        }
    }
}
