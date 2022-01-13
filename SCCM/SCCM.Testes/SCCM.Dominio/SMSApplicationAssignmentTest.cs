using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio
{
    [TestClass]
    public class SMSApplicationAssignmentTest
    {
        [TestMethod]
        public void SMSApplicationAssignment_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            PSEscopo objEscopo = new PSEscopo("PRBBR\\xb194031", "mtp@2134", "SRVSRVCPVWBR05.prbbr.produbanbr.corp");
            objEscopo.DefinirRunspace();

            SMSApplicationAssignment smsCollection = new SMSApplicationAssignment();

            IWMIResult result = smsCollection.ListarResult();

            Assert.IsNotNull(result);
        }
    }
}
