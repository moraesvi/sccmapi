using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCCM.API.Avaiable;
using SCCM.Dominio.Comum;
using System.Collections.Generic;
using Dominio;
using System.Net;
using HelperComum;
using SCCM.PowerShell;
using SCCM.API.Infraestrutura.ApplicationViewService;

namespace SCCM.Testes.API
{
    [TestClass]
    public class UsuarioAppTest
    {
        [TestInitialize]
        public void Inicializar()
        {
            //NetworkCredential credenciais = new NetworkCredential("sccm_admin", "mtp@1234".ToSecureString(), "ADTeste");
            NetworkCredential credenciais = new NetworkCredential("xb194031", "mtp@1243".ToSecureString(), "PRBBR");
            //NetworkCredential credenciais = new NetworkCredential("xb193747", "7ygv@WSX".ToSecureString(), "PRBBR");
            //NetworkCredential credenciais = new NetworkCredential("xb185325", "mtp#$j0s1".ToSecureString(), "PRBBR");
            //NetworkCredential credenciais = new NetworkCredential("xb198516", "mtp@1010".ToSecureString(), "PRBBR");
            PSEscopo psEscopo = new PSEscopo();

            WSAutenticacao.DefinirEscopo(credenciais);
            psEscopo.DefinirRunspace();
        }
        [TestMethod]
        public void OBTER_APLICATIVOS_DISPONIVEIS_USUARIO_TESTE()
        {
            WSUsuario usuario = new WSUsuario();
            int pagina = 1;

            IComumResult result = usuario.ListarAppResult(pagina);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Exception == null);
        }
        [TestMethod]
        public void OBTER_APLICATIVO_USUARIO_DETALHE_TESTE()
        {
            WSUsuario usuario = new WSUsuario();

            IComumResult result = usuario.DetalhesAppResult("ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_10931bdb-80a7-498b-9b51-ca3205650c73");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Exception == null);
        }
        [TestMethod]
        public void OBTER_APLICATIVO_USUARIO_STATUS_TESTE()
        {
            WSUsuario usuario = new WSUsuario();

            IComumResult result = usuario.StatusAppResult("ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_10931bdb-80a7-498b-9b51-ca3205650c73");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Exception == null);
        }
        [TestMethod]
        public void INSTALAR_APLICATIVO_USUARIO_TESTE()
        {
            WSUsuario usuario = new WSUsuario();

            //IComumResult result = usuario.InstalarAppResult("ScopeId_13A41E2A-DA2F-4B94-AD6C-7E7F16C11968/Application_9c4a7b86-9347-4472-bef8-821cb2791567");
            //ADTeste
            //IComumResult result = usuario.InstalacaoAppScriptResult("ScopeId_BF6E6AAF-7013-41E2-9E5F-BC07244396B8/Application_56404ad9-cc3d-4a40-b4c0-86a7375b71af");

            //Assert.IsNotNull(result);
        }
    }
}
