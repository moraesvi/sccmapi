using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;

namespace SCCM.Testes
{
    [TestClass]
    public class SMSCollectionTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo objEscopo = new PSEscopo();
            objEscopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSCollection_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSCollection smsCollection = new SMSCollection();

            IWMIResult result = smsCollection.ListarResult();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void SMSCollection_POPULAR_MODEL_PELO_ID_VIA_SCCM_TESTE()
        {
            string collectionId = "PR100107";
            SMSCollection smsCollection = new SMSCollection();

            IWMIResult colecao = smsCollection.ObterResult(collectionId);

            Assert.IsNotNull(colecao);
        }
        [TestMethod]
        public void SMSCollection_POPULAR_MODEL_PELO_NOME_VIA_SCCM_TESTE()
        {
            string nomeCollection = "[SCCM_API]-TEMP_XB194031";
            SMSCollection smsCollection = new SMSCollection();

            SMSCollection[] result = smsCollection.ObterNome(nomeCollection);

            Assert.IsTrue(result.Length > 0);
        }
        [TestMethod]
        public void SMSCollection_REALIZAR_INCLUSAO_DE_COLECAO_USUARIO()
        {
            string dominio = "PRBBR";
            string usuario = "XB194037";

            SMSCollection smsCollection = new SMSCollection();

            PSColecaoAudit collectionResult = smsCollection.AdicionarUsuario(dominio, usuario);

            Assert.IsNotNull(collectionResult);

            string collectionId = collectionResult.CollectionId();

            Assert.AreNotEqual(collectionId, string.Empty);
            Assert.AreNotEqual(collectionId, null);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Uma coleção com mesmo nome já existe no SCCM")]
        public void SMSCollection_REALIZAR_INCLUSAO_DE_COLECAO()
        {
            SMSCollection smsCollection = new SMSCollection("Colecao_TESTE_FORCE", "Coleção refente o desenvolvimento do projeto SCCM_EComigo", TipoSMSColecao.Usuario);
            string limiteColecaoId = "SMS00002";

            bool collectionResult = smsCollection.Adicionar(limiteColecaoId, smsCollection);

            Assert.IsTrue(collectionResult);
        }
        [TestMethod]
        public void SMSCollection_REALIZAR_REMOCAO_DE_COLECAO()
        {
            SMSCollection smsCollection = new SMSCollection();
            string collectionId = "PR100018";

            PSObjetoRemovidoAudit removidoAudit = smsCollection.Remover(collectionId);
        }
        [TestMethod]
        public void SMSCollection_REALIZAR_INCLUSAO_DE_MEMBRO_EM_COLECAO()
        {
            SMSCollection smsCollection = new SMSCollection();

            //Teste - V Moraes - Andre Piva
            string query = "Select * From SMS_R_User Where (SMS_R_User.UniqueUserName = 'PRBBR\\\\XB194031' Or SMS_R_User.UniqueUserName = 'PRBBR\\\\XB183391' Or SMS_R_User.UniqueUserName = 'PRBBR\\\\N879315' Or SMS_R_User.UniqueUserName = 'PRBBR\\\\N793404' Or SMS_R_User.UniqueUserName = 'PRBBR\\\\N816477')";

            smsCollection = smsCollection.ObterNome("[SCCM_API]-Colecao_TESTE_FORCE")[0];

            bool collectionResult = smsCollection.AdicionarRegra(smsCollection.CollectionID, "[SCCM_API]-Regra", query);

            Assert.IsTrue(collectionResult);
        }
        [TestMethod]
        public void SMSCollection_VALIDAR_NAO_INCLUSAO_DE_COLECAO_REPETIDO()
        {
            string nomeCollection = "[SCCM_API]-Colecao_FINAL";

            SMSCollection smsCollection = new SMSCollection();

            SMSCollection collection = smsCollection.ObterNome(nomeCollection)[0];

            bool result = smsCollection.Existe(collection.CollectionID);

            Assert.IsTrue(result);
        }
    }
}
