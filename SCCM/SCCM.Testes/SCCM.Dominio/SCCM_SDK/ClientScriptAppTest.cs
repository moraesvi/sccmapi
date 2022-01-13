using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.Dominio.WMI;
using SCCM.Dominio.Comum;

namespace SCCM.Testes.SCCM.Dominio.SCCM_SDK
{
    [TestClass]
    public class ClientScriptAppTest
    {
        [TestMethod]
        public void ClientScriptApp_OBTER_SCRIPT_INSTALACAO_CLIENT()
        {
            string appId = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_28844877-7881-412e-a051-2821c6651e64";

            string ECSExtensao = "runapp:";

            ClientScriptApp clientSApp = new ClientScriptApp(appId);

            clientSApp.DefinirScript();
        }

        [TestMethod]
        public void ClientScriptApp_OBTER_SCRIPT_JSON_INSTALACAO_CLIENT()
        {
            string appId = "ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_28844877-7881-412e-a051-2821c6651e64";

            ClientScriptApp clientSApp = new ClientScriptApp(appId);

            IWMIResult result = clientSApp.DefinirScriptResult();

            Assert.IsNotNull(result);
        }
    }
}
