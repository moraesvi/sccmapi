using AD.Dominio;
using AD.Negocio;
using ADWeb.API.Extensao;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ADWeb.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/grupo")]
    public class GrupoController : CustomApiController
    {
        public GrupoController()
        {
            Consulta.DefinirCredenciais(ADDominio.BSBR);
        }
        public IHttpActionResult Get()
        {
            IComumResult result = Consulta.ObterGruposADPrbbrBsbr();

            return OkResult(result);
        }
        [HttpPost]
        public IHttpActionResult Post([FromBody]JObject data)
        {
            if (data == null || !data.HasValues)
            {
                return ParamInvalidosResult();
            }

            string dominio = string.Empty;
            string nomeGrupo = string.Empty;
            string usuario = string.Empty;

            try
            {
                dominio = data.GetValue("dominio").ToObject<string>();
                nomeGrupo = data.GetValue("nomeGrupo").ToObject<string>();
                usuario = data.GetValue("usuario").ToObject<string>();
            }
            catch (Exception ex)
            {
                return ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.Message);
            }

            IComumResult result = Consulta.AdicionarUsuarioGrupo(nomeGrupo, usuario);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/grupo/remover/")]
        public IHttpActionResult Remover([FromBody]JObject data)
        {
            if (data == null || !data.HasValues)
            {
                return ParamInvalidosResult();
            }

            string dominio = string.Empty;
            string nomeGrupo = string.Empty;
            string usuario = string.Empty;

            try
            {
                dominio = data.GetValue("dominio").ToObject<string>();
                nomeGrupo = data.GetValue("nomeGrupo").ToObject<string>();
                usuario = data.GetValue("usuario").ToObject<string>();
            }
            catch (Exception ex)
            {
                return ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.Message);
            }

            IComumResult result = Consulta.RemoverUsuarioGrupo(nomeGrupo, usuario);

            return OkResult(result);
        }
        [HttpGet]
        [Route("api/grupo/usuarioGrupo/{usuario}/{nomeGrupo}")]
        public IHttpActionResult UsuarioGrupo(string usuario, string nomeGrupo)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(nomeGrupo))
            {
                return ParamInvalidosResult();
            }

            IComumResult result = Consulta.VerificarUsuarioGrupo(nomeGrupo, usuario);

            return OkResult(result);
        }
    }
}
