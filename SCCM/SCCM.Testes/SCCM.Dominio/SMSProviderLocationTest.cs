using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;
using System.Collections.Generic;
using System.Threading;

namespace SCCM.Testes
{
    [TestClass]
    public class SMSProviderLocationTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSProviderLocation_POPULAR_MODEL_CACHE_VIA_SCCM_TESTE()
        {
            SMSProviderLocation smsCollection = new SMSProviderLocation();

            IWMIResult result = smsCollection.SiteCodeCacheResult();

            Assert.IsNotNull(result);
        }       
    }
}
