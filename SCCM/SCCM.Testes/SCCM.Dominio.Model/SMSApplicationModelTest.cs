using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Model;
using SCCM.Dominio.Comum;

namespace SCCM.Test.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSApplicationModelTest
    {
        [TestMethod]
        public void SMSApplicationModel_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            PSEscopo escopo = new PSEscopo("PRBBR\\xb194031", "mtp@2134", "SRVSRVCPVWBR05.prbbr.produbanbr.corp");
            escopo.DefinirRunspace();

            SMSApplication smsCollection = new SMSApplication();

            IWMIResult result = smsCollection.ListarResult();

            Assert.IsNotNull(result);
        }
    }
}
