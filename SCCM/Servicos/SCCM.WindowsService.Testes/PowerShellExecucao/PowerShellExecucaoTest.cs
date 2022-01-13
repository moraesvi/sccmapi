using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servico = SCCM.WindowsService.PowerShellExecucao;

namespace SCCM.WindowsService.Testes
{
    [TestClass]
    public class PowerShellExecucaoTest
    {
        Servico.PowerShellExecucao powerShellExecucao = null;
        [TestInitialize]
        public void Inicializacao()
        {
            powerShellExecucao = new Servico.PowerShellExecucao();
        }
        [TestMethod]
        [ExpectedException(typeof(Exception), "Conversor inválido")]
        public void PowerShellExecucao_Conversores_Nao_Encontrados_Teste()
        {
            powerShellExecucao.Processar();
        }
        [TestMethod]
        [ExpectedException(typeof(Exception), "Conversor inválido")]
        public void PowerShellExecucao_Realizar_Conversão_Arquivo()
        {
            powerShellExecucao.Processar();
        }
    }
}
