using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.Model;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSRSystemCustomTest
    {
        PSEscopo escopo = null;
        [TestInitialize]
        public void Inicializar()
        {
            escopo = new PSEscopo();
            //escopo = new PSEscopo(SCCMDominio.ADTeste);
            escopo.DefinirRunspace();
        }
        /*[TestMethod]
        public void SMSRSystemCustom_OBTER_DISPOSITIVOS_CACHE()
        {
            SMSRSystemCustom smsSystem = new SMSRSystemCustom();

            SMSRSystemCustom[] result = smsSystem.ListarCache();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void SMSRSystemCustom_OBTER_DISPOSITIVOS_USUARIO_CACHE()
        {
            SMSRSystemCustom smsSystem = new SMSRSystemCustom();

            SMSRSystemCustom[] result = smsSystem.ObterDispositivo("PRBBR", "N879315");

            Assert.IsNotNull(result);
        }*/
    }
}
