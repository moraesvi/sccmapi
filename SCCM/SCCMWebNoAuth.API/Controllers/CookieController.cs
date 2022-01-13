using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/cookie")]
    public class CookieController : ApiController
    {
        public HttpResponseMessage Get()
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            CookieHeaderValue cookie = new CookieHeaderValue("SCCM_API", "SCCM_API");

            cookie.Expires = DateTimeOffset.Now.AddYears(1);
            //cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";
            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

            return resp;
        }
    }
}
