using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using System.Collections.Generic;
using SCCM.Dominio.Comum;
using System.IO;

namespace SCCM.Testes
{
    [TestClass]
    public partial class SMSClientTest
    {
        [TestMethod]
        public void SMSClientTest_EXECUTAR_COMANDO_POWERSHELL_ARQUIVO_LOCAL()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();

            /*List<PSComandoParam> comandosParam = new List<PSComandoParam>()
            {
                new PSComandoParam(1, "HOST", "SCCM"),
            };

            SMSClient smsClient = new SMSClient();
            smsClient.ExecutarComandoValidarLogado(comandosParam.ToArray(), "01CHAVE", "SCCM_TESTE", "ADTeste", "sccm_admin", "Remove-Cache-Browser", "Remove-Cache-Browser");*/

            /*List<PSComandoParam> comandosParam = new List<PSComandoParam>()
            {
                new PSComandoParam(1, "HOST", "SCCM"),
                new PSComandoParam(2, "CAMINHOAPP", "windows\\system32\\notepad.exe")
            };

            SMSClient smsClient = new SMSClient();
            smsClient.ExecutarComandoValidarLogado(comandosParam.ToArray(), "01CHAVE", "SCCM_TESTE", "ADTeste", "sccm_admin", "Iniciar-Aplicacao", "Iniciar-Aplicacao");*/

            /*List<PSComandoParam> comandosParam = new List<PSComandoParam>()
            {
                new PSComandoParam(1, "HOST", "SCCM"),
            };

            SMSClient smsClient = new SMSClient();
            smsClient.ExecutarComandoValidarLogado(comandosParam.ToArray(), "01CHAVE", "SCCM_TESTE", "ADTeste", "sccm_admin", "Reiniciar-Computador", "Reiniciar-Computador");*/

            List<PSComandoParam> comandosParam = new List<PSComandoParam>()
            {
                new PSComandoParam(1, "HOST", "WIN10WBSSBR004"),
            };

            string triggerForcar = TriggerSchedule.ApplicationDeploymentEvaluationCycleId;

            SMSClient smsClient = new SMSClient();
            smsClient.ForcarClient(triggerForcar, "WIN10WBSSBR004", true);
        }
        [TestMethod]
        public void SMSClientTest_OBTER_RESULTADO_COMANDO_ARQUIVO_LOCAL()
        {
            SMSClient smsClient = new SMSClient();
            IComumResult resultado = smsClient.ObterArquivoSCCMResultado("01CHAVE");

            Assert.IsNotNull(resultado);
            Assert.IsNotNull(resultado.Result);
        }
    }
}

