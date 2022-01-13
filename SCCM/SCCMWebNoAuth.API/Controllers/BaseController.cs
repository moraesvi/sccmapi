using HelperComum;
using SCCM.Dominio.Comum;
using SCCM.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web.Http;
using SCCMWebNoAuth.API.Models;
using SCCM.API.Avaiable;
using System.Web;

namespace SCCMWebNoAuth.API.Controllers
{
    public abstract partial class BaseController : ApiController
    {
        private const string PREF_COOKIE = "SCCM_API_";

        private static PSEscopo sccmConexao;
        private static PSEscopo sccmConexaoSDK;

        protected string Dominio
        {
            get
            {
                if (sccmConexao != null)
                    return sccmConexao.Dominio;
                else if (sccmConexaoSDK != null)
                    return sccmConexaoSDK.Dominio;

                return string.Empty;
            }
        }
        /// <summary>
        /// Realiza uma conexão válida no servidor SCCM utilizando a chave Settings
        /// </summary>
        protected virtual void ConectarSCCM()
        {
            if (sccmConexaoSDK != null)
            {
                if (sccmConexaoSDK.Conectado)
                {
                    sccmConexaoSDK.Desconectar();
                }
            }
            if (sccmConexao == null || !sccmConexao.Conectado)
            {
                sccmConexao = new PSEscopo();
                sccmConexao.DefinirRunspace();
            }
        }
        /// <summary>
        /// Realiza uma conexão válida no servidor SCCM que contém o SDK de uso utilizando a chave Settings
        /// </summary>
        protected virtual void ConectarSCCMSDK()
        {
            if (sccmConexao != null)
            {
                if (sccmConexao.Conectado)
                {
                    sccmConexao.Desconectar();
                }
            }
            if (sccmConexaoSDK == null || !sccmConexaoSDK.Conectado)
            {
                sccmConexaoSDK = new PSEscopo(true);
                sccmConexaoSDK.DefinirRunspace();
            }
        }
        protected virtual void DesconectarSCCM()
        {
            sccmConexao.Desconectar();
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
            cookie.Path = "/";

            return cookie;
        }
    }
}
