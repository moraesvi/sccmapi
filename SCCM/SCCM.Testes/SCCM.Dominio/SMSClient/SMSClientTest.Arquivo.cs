using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using System.Collections.Generic;
using SCCM.Dominio.Comum;
using System.IO;

namespace SCCM.Testes
{
    public partial class SMSClientTest
    {
        private List<string> lstLinhasArquivo = new List<string>()
        {
            "function Trigger-Force { Param([ValidateNotNullOrEmpty()] [string]$Host, [string]$Nome)",
            "   #SCCM_COMANDO",
            "   $WMICaminho = \"\\$Host\\ROOT\\ccm:SMS_Client",
            "   $SMSWMI = [WmiClass]$WMICaminho",
            "   $SMSWMI.TriggerSchedule('{00000000-0000-0000-0000-000000000121}')",
            "}",
        };
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SMSClientArquivoTest_GERAR_ARQUIVO_POWERSHELL_LOCAL()
        {
            SMSClient smsClient = new SMSClient();

            Dictionary<string, List<string>> dctArquivos = ObteArquivosTeste();

            foreach (var keyValueArquivo in dctArquivos)
            {
                bool gerado = smsClient.GravarArquivo(keyValueArquivo.Value.ToArray(), keyValueArquivo.Key);

                Assert.IsTrue(gerado);
            }
        }
        [TestMethod]
        public void SMSClientArquivoTest_REMOVER_ARQUIVO_POWERSHELL_LOCAL()
        {
            SMSClient smsCollection = new SMSClient();

            smsCollection.GravarArquivo(lstLinhasArquivo.ToArray(), "Trigger-Force");
            bool removido = smsCollection.RemoverArquivo("Trigger-Force");

            Assert.IsTrue(removido);
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SMSClientArquivoTest_GERAR_ARQUIVOS_MESMO_NOME_POWERSHELL_LOCAL()
        {
            SMSClient smsCollection = new SMSClient();

            smsCollection.RemoverArquivo("Trigger-Force");

            smsCollection.GravarArquivo(lstLinhasArquivo.ToArray(), "Trigger-Force");
            smsCollection.GravarArquivo(lstLinhasArquivo.ToArray(), "Trigger-Force");
        }
        [TestCleanup]
        public void SMSClientArquivoTest_LimparTeste()
        {
            SMSClient smsCollection = new SMSClient();

            //smsCollection.RemoverArquivo("Trigger-Force");
        }
        private Dictionary<string, List<string>> ObteArquivosTeste()
        {
            List<string> lstLinhaArquivo = null;
            Dictionary<string, List<string>> dctArquivos = new Dictionary<string, List<string>>();

            string currentDir = System.AppDomain.CurrentDomain.BaseDirectory;

            string caminhoPasta = Path.Combine(currentDir, "PSScripts");

            DirectoryInfo diretorioDllParse = new DirectoryInfo(caminhoPasta);
            FileInfo[] arrayArquivo = diretorioDllParse.GetFiles();

            foreach (FileInfo arquivo in arrayArquivo)
            {
                lstLinhaArquivo = new List<string>();

                using (FileStream stream = File.Open(arquivo.FullName, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string linha = string.Empty;
                        while ((linha = reader.ReadLine()) != null)
                        {
                            lstLinhaArquivo.Add(linha);
                        }
                    }
                }

                dctArquivos.Add(arquivo.Name, lstLinhaArquivo);
            }

            return dctArquivos;
        }
    }
}

