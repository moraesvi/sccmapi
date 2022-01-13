using SCCM.Dominio.Comum;
using SCCM.Dominio.WMI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SCCMWebNoAuth.API.Models;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/script")]
    public class ScriptController : ApiController
    {
        [HttpGet]
        [Route("api/script/arquivoResultado/{chaveArquivo}/")]
        public IHttpActionResult ArquivoResultado(string chaveArquivo)
        {
            if (string.IsNullOrWhiteSpace(chaveArquivo))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmentros não enviados"));
            }

            SMSClient client = new SMSClient();

            IComumResult result = client.ObterArquivoSCCMResultado(chaveArquivo);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
        }
        [HttpPost]
        [Route("api/script/cadastrar/")]
        public IHttpActionResult Cadastrar([FromBody]CadastroScript param)
        {
            if (param == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmentros não enviados"));
            }

            SMSClient client = new SMSClient();

            IComumResult result = client.GravarArquivoResult(param.LinhasScript, param.NomeArquivo);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
        }
        [HttpPost]
        [Route("api/script/remover")]
        public IHttpActionResult Remover([FromBody]RemoverScript param)
        {
            if (param == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmentros não enviados"));
            }

            SMSClient client = new SMSClient();

            IComumResult result = client.RemoverArquivoResult(param.NomeArquivo);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
        }
    }
}
