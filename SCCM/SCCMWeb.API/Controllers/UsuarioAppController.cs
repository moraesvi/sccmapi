using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCCM.API.Avaiable;
using SCCM.Dominio.Comum;
using HelperComum;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Web.API.Models;

namespace SCCMWeb.API.Controllers
{
    [XMLCorsPolicy]
    [Route("api/usuarioApp")]
    public class UsuarioAppController : BaseController
    {
        public UsuarioAppController()
        {
            if (System.Web.HttpContext.Current.IsDebuggingEnabled)
            {
                WinAutenticacaoSimulacaoResult();
            }
            else
            {
                WinAutenticacaoResult();
            }
        }
        public IHttpActionResult Get(int pagina = 1)
        {
            WSUsuario usuario = new WSUsuario();
            IComumResult result = usuario.ListarAppResult(pagina);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/usuarioApp/detalhe/", Order = 2)]
        public IHttpActionResult Detalhe(FormDataCollection data)
        {
            string ci_uniqueId = string.Empty;

            if (data == null || data.Count() == 0)
            {
                return ParamInvalidosResult();
            }

            try
            {
                ci_uniqueId = data.Get("ci_uniqueId");
            }
            catch (Exception ex)
            {
                return ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.InnerException().Message);
            }

            WSUsuario usuario = new WSUsuario();

            IComumResult result = usuario.DetalhesAppResult(ci_uniqueId);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/usuarioApp/status/", Order = 4)]
        public IHttpActionResult Status(FormDataCollection data)
        {
            string ci_uniqueId = string.Empty;

            if (data == null || data.Count() == 0)
            {
                return ParamInvalidosResult();
            }

            try
            {
                ci_uniqueId = data.Get("ci_uniqueId");
            }
            catch (Exception ex)
            {
                return ParamInvalidosResult("Os parâmetros enviados não são válidos", ex.InnerException().Message);
            }

            WSUsuario usuario = new WSUsuario();

            IComumResult result = usuario.StatusAppResult(ci_uniqueId);

            return OkResult(result);
        }
    }
}
