using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Model;
using SCCM.Dominio.Comum;

namespace SCCM.Test.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSApplicationCustomModelTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSApplicationCustomModel_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSApplicationCustom smsCollection = new SMSApplicationCustom();

            IWMIResult result = smsCollection.ListarResult();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void SMSApplicationCustomModel_POPULAR_MODEL_PELO_NOME_VIA_SCCM_TESTE()
        {
            SMSApplicationCustom smsCollection = new SMSApplicationCustom();

            IWMIResult result = smsCollection.ObterNomeResult("VLC Player 2.2.2");

            Assert.IsNotNull(result);
        }
    }
}
