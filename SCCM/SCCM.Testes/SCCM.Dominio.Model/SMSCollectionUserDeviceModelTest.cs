using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Model;
using SCCM.Dominio.Comum;

namespace SCCM.Test.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSCollectionUserDeviceModelTest
    {
        [TestMethod]
        public void SMSCollectionUserDeviceModel_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();

            SMSCollectionUserDevice smsCollection = new SMSCollectionUserDevice("PR10012A");

            IWMIResult result = smsCollection.Obter();

            Assert.IsNotNull(result);
        }
    }
}
