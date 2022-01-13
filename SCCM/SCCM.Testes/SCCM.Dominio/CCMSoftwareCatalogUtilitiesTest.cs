using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.Dominio.Comum;
using SCCM.Dominio.WMI;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM.Testes.SCCM.Dominio
{
    [TestClass]
    public class CCMSoftwareCatalogUtilitiesTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSApplicationAssignment_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            CCMSoftwareCatalogUtilities sms = new CCMSoftwareCatalogUtilities();

            string result = sms.DeviceID();

            Assert.IsNotNull(result);
        }
    }
}
