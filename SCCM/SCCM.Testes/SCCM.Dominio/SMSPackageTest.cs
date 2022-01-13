using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;

namespace SCCM.Testes
{
    [TestClass]
    public class SMSPackageTest
    {
        [TestMethod]
        public void SMSPackage_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            PSEscopo objEscopo = new PSEscopo("PRBBR\\xb194031", "mtp@2134", "SRVSRVCPVWBR05.prbbr.produbanbr.corp");
            objEscopo.DefinirRunspace();

            SMSPackage smsCollection = new SMSPackage();

            var lista = smsCollection.ListarResult();
        }
    }
}
