using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.Dominio.WMI.RequestedConfig;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio.RequestedConfig
{
    [TestClass]
    public class CCMSoftwareDistributionClientConfigTest
    {
        [TestMethod]
        public void CCMSoftwareDistributionClientConfig_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            CCMSoftwareDistributionClientConfig politicasClient = new CCMSoftwareDistributionClientConfig();

            IWMIResult result = politicasClient.ListarResult();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void CCMSoftwareDistributionClientConfig_DESABILITAR_POLITICAS_TESTE()
        {
            CCMSoftwareDistributionClientConfig politicasClient = new CCMSoftwareDistributionClientConfig();

            politicasClient.Desabilitar();
        }
    }
}
