using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.PowerShell;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;
using System.Collections.Generic;
using System.Threading;

namespace SCCM.Testes
{
    [TestClass]
    public class SMSApplicationTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            PSEscopo escopo = new PSEscopo();
            escopo.DefinirRunspace();
        }
        [TestMethod]
        public void SMSApplication_POPULAR_MODEL_VIA_SCCM_TESTE()
        {
            SMSApplication smsCollection = new SMSApplication();

            IWMIResult result = smsCollection.ListarResult();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void SMSApplication_REALIZAR_DEPLOY_INSTALACAO_DE_APLICACAO_REQUIRED()
        {
            string appId = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_e43168e0-3ef1-46fa-a55e-865ba1706b5b/1";
            string collectionId = "PR100143";

            SMSApplication smsApplication = new SMSApplication();

            bool result = smsApplication.ImplantarAplicativo(appId, collectionId, DeployOfferTypeID.Required);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void SMSApplication_REALIZAR_DEPLOY_INSTALACAO_DE_APLICACAO_REQUIRED_FORCANDO()
        {
            string appId = "ScopeId_060F752B-BCF0-41BD-B351-E24DC3C859FA/Application_4e1b3f2f-38a0-4d26-96ce-42d87e4e8150/2";

            string dominio = "ADTeste";
            string usuario = "Administrador";

            string chaveCookie = "SCCM_TESTE";

            SMSApplication smsApplication = new SMSApplication();

            bool implantado = smsApplication.ImplantarAplicativoForcar(appId, dominio, usuario, chaveCookie, DeployOfferTypeID.Required);

            Assert.IsTrue(implantado);
        }
        [TestMethod]
        public void SMSApplication_REALIZAR_DEPLOY_REMOCAO_DE_APLICACAO_REQUIRED_FORCANDO()
        {
            string appId = "ScopeId_060F752B-BCF0-41BD-B351-E24DC3C859FA/Application_e07b964c-2d1e-49fc-978e-f1fe759c0b99/1";

            string dominio = "ADTeste";
            string usuario = "sccm_admin";

            string chaveCookie = "SCCM_TESTE";

            SMSApplication smsApplication = new SMSApplication();

            bool implantado = smsApplication.RemoverAplicativoForcar(appId, dominio, usuario, chaveCookie, DeployOfferTypeID.Required);

            Thread.Sleep(2 * 10000);
        }
        [TestMethod]
        public void SMSApplication_REALIZAR_REMOCAO_DO_DEPLOY_DA_APLICACAO()
        {
            string deploymentId = "{70546DD4-0DC3-4561-885C-DED31E41B93A}";

            SMSApplication smsApplication = new SMSApplication();

            PSObjetoRemovidoAudit result = smsApplication.RemoverImplatacao(deploymentId);

            Assert.IsNotNull(result);
        }
    }
}
