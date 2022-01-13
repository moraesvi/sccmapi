using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Model;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSRUserDeviceTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSRUserDevice_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSRUserDevice smsUserDevice = new SMSRUserDevice("ADTeste", "sccm_admin", "SMS00002");

            SMSRUserDevice[] result = smsUserDevice.Listar();

            Assert.IsNotNull(result);
        }
    }
}
