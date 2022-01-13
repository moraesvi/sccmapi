using SCCM.Dominio.Comum;
using SCCM.Dominio.WMI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using SCCMWebNoAuth.API.Models;

namespace SCCMWebNoAuth.API.Controllers
{
    [AllowAnonymous]
    [XMLCorsPolicy]
    [Route("api/scriptComando")]
    public class ScriptComandoController : BaseController
    {
        public ScriptComandoController()
        {
            ConectarSCCM();
        }
        [HttpPost]
        [Route("api/scriptComando/executarComando/")]
        public HttpResponseMessage ExecutarComando([FromBody]ComandoLogadoAPI param)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            SMSClient client = new SMSClient();

            if (param == null)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros não enviados - Chave: {0}", param.ChaveResult));
            }
            if (param.ComandoParam == null && param.ComandoParam.Count() == 0)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros do comando não enviados - Chave: {0}", param.ChaveResult));
            }

            PSComandoParam[] comandosParam = DefinirComando(param.ComandoParam);

            string chaveTransacao = ObterChaveTransacao();

            IComumResult result = client.ExecutarComandoValidarLogadoResult(comandosParam, param.ChaveResult, chaveTransacao, param.Dominio, param.Usuario, param.NomeArquivo, param.NomeComando, true);

            resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveTransacao) });
            resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

            return resp;
        }
        [HttpPost]
        [Route("api/scriptComando/executarScript/")]
        public HttpResponseMessage ExecutarScript([FromBody]ScriptExecutar param)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            SMSClient client = new SMSClient();

            if (param == null)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros não enviados - Chave: {0}", param.ChaveResult));
            }
            if (param.LinhasScript == null && param.LinhasScript.Count() == 0)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros do comando não enviados - Chave: {0}", param.ChaveResult));
            }

            string chaveTransacao = ObterChaveTransacao();

            IComumResult result = client.ExecutarScriptResult(param.LinhasScript, param.ChaveResult);

            resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveTransacao) });
            resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

            return resp;
        }
        [HttpPost]
        [Route("api/scriptComando/executarScriptLogado/")]
        public HttpResponseMessage ExecutarScriptLogado([FromBody]ScriptExecutarLogado param)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            SMSClient client = new SMSClient();

            if (param == null)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros não enviados - Chave: {0}", param.ChaveResult));
            }
            if (param.LinhasScript == null && param.LinhasScript.Count() == 0)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                resp.Content = new StringContent(string.Format("Parâmentros do comando não enviados - Chave: {0}", param.ChaveResult));
            }

            string chaveTransacao = ObterChaveTransacao();

            IComumResult result = client.ExecutarScriptValidarLogadoResult(param.LinhasScript, param.Dominio, param.Usuario, param.ChaveResult, chaveTransacao, true);

            resp.Headers.AddCookies(new CookieHeaderValue[] { GerarCookieTransacao(chaveTransacao) });
            resp.Content = new ObjectContent<object>(result, Configuration.Formatters.JsonFormatter);

            return resp;
        }

        #region Metodos Privados

        private PSComandoParam[] DefinirComando(ComandoParam[] comandosParam)
        {
            if (comandosParam == null && comandosParam.Count() == 0)
            {
                return null;
                //throw new Exception("Operação inválida", new Exception("Parâmentros do comando não enviados."));
            }

            List<PSComandoParam> lstComandoParam = new List<PSComandoParam>();

            comandosParam.ToList().ForEach(param =>
            {
                PSComandoParam comandoParam = new PSComandoParam(param.Ordem, param.Nome, param.Valor);
                lstComandoParam.Add(comandoParam);
            });

            return lstComandoParam.ToArray();
        }

        #endregion
    }
}
