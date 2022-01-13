using SCCM.Dominio.Comum;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace SCCMWebNoAuth.API
{
    public class CustomExceptionHandler : ExceptionHandler
    {
        public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(state =>
            {
                CustomErrorsSection cESection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");

                var httpContext = (HttpContext)state;

                bool exibirDetalheExcep = (cESection.Mode == CustomErrorsMode.Off | httpContext.IsDebuggingEnabled);

                string msg = context.Exception.Message;
                string msgDetalhada = context.Exception.InnerException != null ? context.Exception.InnerException.Message : null;
                string stackTrace = string.Concat(context.Exception.StackTrace, "\n", context.Exception.InnerException != null ? context.Exception.InnerException.StackTrace : null);

                IComumResult result = ComumResultFactory.Criar("Erro na requisição", msg, msgDetalhada, stackTrace, exibirDetalheExcep);

                context.Result = new TextPlainErrorResult(result)
                {
                    Request = context.Request,
                };
            }, HttpContext.Current);
        }
        private class TextPlainErrorResult : IHttpActionResult
        {
            private IComumResult _result;
            public TextPlainErrorResult(IComumResult result)
            {
                _result = result;
            }
            public HttpRequestMessage Request { get; set; }
            public string Content { get; set; }
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {                
                HttpResponseMessage response = Request.CreateResponse<IComumResult>(HttpStatusCode.InternalServerError, _result, "application/json");
                response.ReasonPhrase = "SCCM API InternalServerError";

                return Task.FromResult(response);
            }
        }
    }
}