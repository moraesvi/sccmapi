using AD.Dominio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace ADWeb.API.Extensao
{
    public class CustomApiController : ApiController
    {
        public bool ExibirDetalheException
        {
            get
            {
                CustomErrorsSection cESection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");

                return (cESection.Mode == CustomErrorsMode.Off || HttpContext.Current.IsDebuggingEnabled);
            }
        }
        protected IHttpActionResult OkResult(IComumResult result = null)
        {
            if (result == null)
                return Ok();

            return Content(HttpStatusCode.OK, result);
        }
        protected IHttpActionResult ParamInvalidosResult(string msgException = null)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, "Erro na requisição", new Exception("Os parâmetros enviados não são válidos"));
        }
        protected IHttpActionResult ParamInvalidosResult(string msgException, string msgExceptionDetalhado)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, msgException, new Exception(msgExceptionDetalhado));
        }
        protected IHttpActionResult NaoTratadoResult(string msgException, string msgExceptionDetalhado = null)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, msgException, new Exception(msgExceptionDetalhado));
        }
    }
}