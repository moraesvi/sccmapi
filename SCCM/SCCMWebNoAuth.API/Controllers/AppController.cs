using DomModel = SCCM.Dominio.Model;
using DomDWMI = SCCM.Dominio.WMI;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using SCCM.Dominio.Comum;
using System.Net.Http.Headers;
using System.Web.Http.Cors;
using System.Security.Principal;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/aplicativo")]
    public class AppController : BaseController
    {
        public AppController()
        {
            ConectarSCCM();
        }
        public IHttpActionResult Get()
        {
            DomModel.SMSApplicationCustom smsApplication = new DomModel.SMSApplicationCustom();

            IWMIResult result = smsApplication.ListarResult();

            return OkResult(result);
        }
        [Route("api/aplicativo/{nomeApp}/")]
        public IHttpActionResult Get(string nomeApp)
        {
            DomModel.SMSApplicationCustom smsApplication = new DomModel.SMSApplicationCustom();

            IWMIResult result = smsApplication.ObterNomeResult(nomeApp);

            return OkResult(result);
        }
        [HttpPost]
        [Route("api/aplicativo/implantar/")]
        public IHttpActionResult Implantar([FromBody]JObject data)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            if (!data.HasValues)
            {
                return ParamInvalidosResult("Parâmetros não enviados.");
            }

            object result = null;

            string dataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string idApp = data.GetValue("idApp").ToObject<string>();
            string idColecao = data.GetValue("idColecao").ToObject<string>();

            DomDWMI.SMSApplication smsApp = new DomDWMI.SMSApplication();
            smsApp = smsApp.Obter(idApp);

            bool realizado = smsApp.ImplantarAplicativo(idApp, idColecao, DeployOfferTypeID.Required);

            if (realizado)
            {
                result = new { msgResult = string.Format("Implantação do aplicativo {0} realizado.", smsApp.LocalizedDisplayName), implantado = realizado, data = dataTransacao };

                return Ok(result);
            }

            result = new { msgResult = string.Format("Não foi possível realizar a implatação do aplicativo - {0}", smsApp.LocalizedDisplayName), implantado = realizado, data = dataTransacao };

            return Ok(result);
        }
        [HttpPost]
        [Route("api/aplicativo/implantarForcar/")]
        public HttpResponseMessage ImplantarForcar([FromBody]JObject data)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            if (!data.HasValues)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmetros não enviados.");
            }

            object result = null;

            string dataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string idApp = data.GetValue("idApp").ToObject<string>();
            string dominio = data.GetValue("dominio").ToObject<string>();
            string usuario = data.GetValue("usuario").ToObject<string>();

            DomDWMI.SMSApplication smsApp = new DomDWMI.SMSApplication();
            smsApp = smsApp.Obter(idApp);

            string chaveCookie = ObterChaveTransacaoCookie();

            bool implantado = smsApp.ImplantarAplicativoForcar(idApp, dominio, usuario, chaveCookie, DeployOfferTypeID.Required);

            if (implantado)
            {
                result = new { msgResult = string.Format("Implantação do aplicativo {0} realizado.", smsApp.LocalizedDisplayName), implantado = implantado, chaveResult = chaveCookie, data = dataTransacao };

                resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveCookie) });
                resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

                return resp;
            }

            result = new { msgResult = string.Format("Não foi possível realizar a implatação do aplicativo - {0}", smsApp.LocalizedDisplayName), implantado = implantado, chaveResult = "", data = dataTransacao };
            resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

            return resp;
        }
        [HttpPost]
        [Route("api/aplicativo/remover/")]
        public HttpResponseMessage Remover([FromBody]JObject data)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            if (!data.HasValues)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmetros não enviados.");
            }

            object result = null;

            string dataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string idApp = data.GetValue("idApp").ToObject<string>();
            string idColecao = data.GetValue("idColecao").ToObject<string>();
            string tipoImplatacao = data.GetValue("tipoImplatacao").ToObject<string>();

            DomDWMI.SMSApplication smsApp = new DomDWMI.SMSApplication();

            string chaveCookie = ObterChaveTransacaoCookie();

            bool realizado = smsApp.ImplantarAplicativo(idApp, idColecao, DeployOfferTypeID.Required);

            if (realizado)
            {
                result = new { msgResult = string.Format("Implantação do aplicativo {0} realizado.", smsApp.LocalizedDisplayName), implantado = realizado, data = dataTransacao };

                resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveCookie) });
                resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

                return resp;
            }

            result = new { msgResult = string.Format("Não foi possível realizar a implatação do aplicativo - {0}", smsApp.LocalizedDisplayName), implantado = realizado, data = dataTransacao };
            resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

            return resp;
        }
        [HttpPost]
        [Route("api/aplicativo/removerForcar/")]
        public HttpResponseMessage RemoverForcar([FromBody]JObject data)
        {
            HttpResponseMessage resp = new HttpResponseMessage();

            if (!data.HasValues)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Parâmetros não enviados.");
            }

            object result = null;

            string dataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string idApp = data.GetValue("idApp").ToObject<string>();
            string dominio = data.GetValue("dominio").ToObject<string>();
            string usuario = data.GetValue("usuario").ToObject<string>();

            DomDWMI.SMSApplication smsApp = new DomDWMI.SMSApplication();

            string chaveCookie = ObterChaveTransacaoCookie();

            bool implantadoRemocao = smsApp.RemoverAplicativoForcar(idApp, dominio, usuario, "SCCM_TESTE", DeployOfferTypeID.Required);

            if (implantadoRemocao)
            {
                string chaveResult = smsApp.GerarChaveForceArquivo(idApp, usuario);
                result = new { msgResult = string.Format("Implantação do aplicativo {0} realizado.", smsApp.LocalizedDisplayName), implantado = implantadoRemocao, chaveResult = chaveResult, data = dataTransacao };

                resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveCookie) });
                resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

                return resp;
            }

            result = new { msgResult = string.Format("Não foi possível realizar a remoção do aplicativo - {0}", smsApp.LocalizedDisplayName), implantado = implantadoRemocao, chaveResult = "", data = dataTransacao };

            return Request.CreateResponse(Request.CreateResponse(HttpStatusCode.OK, result));
        }
    }
}
