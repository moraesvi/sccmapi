using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HelperComum;
using AD.Negocio;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using AD.Dominio;

namespace AD.Testes
{
    [TestClass]
    public class ConsultaTest
    {
        [TestMethod]
        public void NegocioConsulta_VALIDAR_BUSCA_DOS_CONTAINERS_AD_SOFTGRID_PRBBR_BSBR()
        {
            Consulta.DefinirCredenciais(ADDominio.PRBBR);

            IComumResult result = Consulta.ObterGruposADPrbbrBsbr();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_OBTER_USUARIO_GRUPOS_PRBBR()
        {
            //string usuario = "T601264";
            //
            //Consulta.DefinirCredenciais(ADDominio.BSBR);

            //GUsuario usuarioGrupo = Consulta.ObterUsuarioGruposResult(usuario);
            //
            //Helper.ArrayToCSV(usuarioGrupo.Grupos, string.Concat("BSBR_", usuarioGrupo.Usuario.ToUpper()));
            //
            //Assert.IsNotNull(usuarioGrupo);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_ADICIONAR_USUARIO_GRUPO_PRBBR()
        {
            string nomeGrupo = "GAPL_SOFTGRID_ACROBAT_W7";
            string usuario = "xb193749";

            Consulta.DefinirCredenciais(ADDominio.PRBBR);

            IComumResult result = Consulta.AdicionarUsuarioGrupo(nomeGrupo, usuario);

            Assert.IsTrue(result.Executado);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_REMOVER_USUARIO_GRUPO()
        {
            string nomeGrupo = "GAPL_SOFTGRID_ACROBAT_W7";
            string usuario = "xb193749";

            Consulta.DefinirCredenciais(ADDominio.PRBBR);

            IComumResult result = Consulta.RemoverUsuarioGrupo(nomeGrupo, usuario);

            Assert.IsTrue(result.Executado);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_USUARIO_GRUPO()
        {
            string nomeGrupo = "GAPL_SOFTGRID_ACROBAT_W7";
            string usuario = "xb193749";

            Consulta.DefinirCredenciais(ADDominio.PRBBR);

            IComumResult resul = Consulta.VerificarUsuarioGrupo(nomeGrupo, usuario);

            Assert.IsTrue(resul.Executado);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_USUARIO_BLOEQUADO()
        {
            string usuario = "xb202585";

            Consulta.DefinirCredenciais(ADDominio.BSBR);

            IComumResult resul = Consulta.VerificarUsuarioBloqueadoAD(usuario);

            Assert.IsTrue(resul.Executado);
        }
        [TestMethod]
        public void NegocioConsulta_VALIDAR_GERACAO_GRUPOS_CSV()
        {
            //IComumResult result = Consulta.ObterGruposADPrbbrBsbrResult();
            //
            //arrayGrupo.ToList().ForEach(grupo =>
            //{
            //    bool gerado = grupo.CSV(string.Concat("GRUPO_", grupo.Nome.ToUpper()));
            //
            //    Assert.IsTrue(gerado);
            //});
        }
    }
}
