using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.GeradorModel;

namespace SCCM.Testes.GeradorModel
{
    [TestClass]
    public class GerarModelTest
    {
        [TestMethod]
        public void VALIDAR_SE_O_MODEL_ESTA_SENDO_GERADO_TESTE()
        {
            WMICSModel obj = new WMICSModel();

            string result = obj.Gerar();
        }

        [TestMethod]
        public void VALIDAR_SE_O_MODEL_CUSTOMIZADO_ESTA_SENDO_GERADO_TESTE()
        {
            WMICSModelCustom obj = new WMICSModelCustom();

            string result = obj.Gerar();
        }
    }
}
