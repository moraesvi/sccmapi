using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servico = SCCM.WindowsService.PowerShellResult;

namespace SCCM.WindowsService.Testes
{
    [TestClass]
    public class PowerShellResultTest
    {
        Servico.PowerShellResult powerShellResult = null;
        [TestInitialize]
        public void Inicializacao()
        {
            powerShellResult = new Servico.PowerShellResult();
        }
        [TestMethod]
        public void PowerShellResult_Conversores_Busca_Arquivos()
        {
            powerShellResult.Processar();
        }
    }
}
