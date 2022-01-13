using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SCCM.Dominio.Comum;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using HelperComum;
using SCCM.Dominio.Model;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [RoutePrefix("api/usuario")]
    public class UsuarioController : BaseController
    {
        public UsuarioController()
        {
            ConectarSCCM();
        }
        public IHttpActionResult Get()
        {
            return NaoTratadoResult("Erro na requisição", "Não implementado");
        }

        [HttpGet]
        [Route("implantacao/{usuario}")]
        public IHttpActionResult Implantacao(string usuario)
        {
            SMSRUserAppDeployment smsUserComputerAppDepl = new SMSRUserAppDeployment(usuario);

            IWMIResult result = smsUserComputerAppDepl.ListarResult();

            return OkResult(result);
        }

        [HttpPost]
        [Route("implantacaoStatus")]
        public IHttpActionResult ImplantacaoStatus([FromBody]JObject data)
        {
            if (!data.HasValues)
            {
                return NaoTratadoResult("Erro na requisição", "Parâmetros não enviados");
            }

            string usuario = string.Empty;
            string idApp = string.Empty;

            try
            {
                usuario = data.GetValue("usuario").ToObject<string>();
                idApp = data.GetValue("ci_UniqueID").ToObject<string>();
            }
            catch (Exception ex)
            {
                return NaoTratadoResult("Os parâmetros enviados não são válidos", ex.InnerException().Message);
            }

            SMSRUserAppStatusDeployment smsUserComputerAppDepl = new SMSRUserAppStatusDeployment(usuario, idApp);

            IWMIResult result = smsUserComputerAppDepl.ObterResult();

            return OkResult(result);
        }
    }
}

