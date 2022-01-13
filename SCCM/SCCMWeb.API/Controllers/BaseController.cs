using HelperComum;
using SCCM.API.Avaiable;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using Web.API.Models;

namespace SCCMWeb.API.Controllers
{
    public abstract partial class BaseController : ApiController
    {
        private const string PREF_COOKIE = "SCCM_API_";

        private static PSEscopo sccmConexao;
        public PSEscopo SCCMConexao
        {
            get
            {
                ConectarSCCM();
                return sccmConexao;
            }
        }
        protected IComumResult WinAutenticacaoResult()
        {
            WindowsIdentity identidade = (WindowsIdentity)System.Web.HttpContext.Current.User.Identity;

            IComumResult result = WSAutenticacao.DefinirEscopoResult(identidade.Name);

            return result;
        }
        protected IComumResult WinAutenticacaoSimulacaoResult()
        {
            WindowsIdentity identidade = (WindowsIdentity)System.Web.HttpContext.Current.User.Identity;

            NetworkCredential ntcSimu = new NetworkCredential("xb194031", "MTP@4433", "PRBBR");

            IComumResult result = WSAutenticacao.DefinirEscopoResult(ntcSimu);

            return result;
        }
        protected IHttpActionResult OkResult(IComumResult result = null)
        {
            return Content(HttpStatusCode.OK, result);
        }
        protected IHttpActionResult OkResult(IWMIResult result = null)
        {
            return Content(HttpStatusCode.OK, result);
        }
        protected IHttpActionResult ParamInvalidosResult(string msgException = null)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, "Erro na requisição", new Exception(!string.IsNullOrEmpty(msgException) ? msgException : "Os parâmetros enviados não são válidos"));
        }
        protected IHttpActionResult ParamInvalidosResult(string msgException, string msgExceptionDetalhado)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, msgException, new Exception(msgExceptionDetalhado));
        }
        protected IHttpActionResult NaoTratadoResult(string msgException, string msgExceptionDetalhado = null)
        {
            throw new HttpException((int)HttpStatusCode.InternalServerError, msgException, new Exception(msgExceptionDetalhado));
        }
        protected IHttpActionResult ErroResult(string msgException, string msgExceptionDetalhado = null)
        {
            throw new HttpException((int)HttpStatusCode.BadRequest, msgException, new Exception(msgExceptionDetalhado));
        }
        protected virtual void ConectarSCCM()
        {
            if (sccmConexao == null || !sccmConexao.Conectado)
            {
                sccmConexao = new PSEscopo();
                sccmConexao.DefinirRunspace();
            }
        }
        protected virtual void Desconectar()
        {
            sccmConexao.Desconectar();
        }
        protected static string ObterChaveTransacao()
        {
            string chaveTransacao = SCCMApi.ObterChaveTransacao();

            return chaveTransacao;
        }
        protected static string ObterChaveTransacaoCookie()
        {
            string chave = SCCMApi.ObterChaveTransacao();
            string chaveTransacao = string.Concat(PREF_COOKIE, chave);

            return chaveTransacao;
        }
        protected static CookieHeaderValue GerarCookieTransacao(string chaveTransacao)
        {
            CookieHeaderValue cookie = new CookieHeaderValue(chaveTransacao, chaveTransacao);
            cookie.Expires = DateTimeOffset.Now.AddMonths(1);
            //cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";

            return cookie;
        }
    }
}
