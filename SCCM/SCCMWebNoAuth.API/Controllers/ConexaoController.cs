using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/conexao")]
    [XMLCorsPolicy]
    public class ConexaoController : BaseController
    {
        [HttpPost]
        [Route("conectar")]
        public IHttpActionResult PSConectar()
        {
            ConectarSCCM();

            return Ok("Conectado ao SCCM - " + DateTime.Now);
        }
        [HttpPost]
        [Route("desconectar")]
        public IHttpActionResult PSDesconectar()
        {
            DesconectarSCCM();

            return Ok("Desconetado do SCCM - " + DateTime.Now);
        }
    }
}
