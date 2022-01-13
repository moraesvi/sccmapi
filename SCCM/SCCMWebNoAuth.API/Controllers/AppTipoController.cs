using SCCM.Dominio.Comum;
using SCCM.Dominio.Model;
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
    [XMLCorsPolicy]
    [Route("api/appTipo")]
    public class AppTipoController : BaseController
    {
        public AppTipoController()
        {
            ConectarSCCM();
        }

        [HttpGet]
        [Route("api/appTipo/script")]
        public IHttpActionResult Script()
        {
            SMSApplicationTipoDeploy smsAppTpDeploy = new SMSApplicationTipoDeploy();

            IWMIResult result = smsAppTpDeploy.ListarTipoScriptResult();

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, result));
        }
    }
}
