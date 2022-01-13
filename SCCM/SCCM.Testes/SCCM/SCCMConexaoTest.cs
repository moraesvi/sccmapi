using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;

namespace SCCM.Testes.SCCM
{
    [TestClass]
    public class SCCMConexaoTest
    {
        [TestMethod]
        public void VALIDAR_SE_ESTA_SENDO_REALIZADO_UMA_CONEXAO_SCCM()
        {
            PSEscopo objEscopo = new PSEscopo("PRBBR\\xb194031", "mtp@2134", "SRVSRVCPVWBR05.prbbr.produbanbr.corp");
            objEscopo.DefinirRunspace();
        }
    }
}
