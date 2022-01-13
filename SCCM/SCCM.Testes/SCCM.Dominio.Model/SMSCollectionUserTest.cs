using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using DomModel = SCCM.Dominio.Model;
using DomDWMI = SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio.Model
{
    [TestClass]
    public class SMSCollectionUserTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSCollectionUser_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            string nomeCollection = "Teste_App_Catalog";

            DomDWMI.SMSCollection smsCollection = new DomDWMI.SMSCollection();

            DomDWMI.SMSCollection colecao = smsCollection.ObterNome(nomeCollection)[0];

            DomModel.SMSCollectionUser smsCollectionUser = new DomModel.SMSCollectionUser(colecao.CollectionID);

            IWMIResult result = smsCollectionUser.ListarResult();

            Assert.IsNotNull(result);
        }
    }
}
