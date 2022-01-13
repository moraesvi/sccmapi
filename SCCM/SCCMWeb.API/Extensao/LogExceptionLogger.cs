using HelperComum;
using MyBranch.Log.Dominio;
using MyBranch.Log.Factory;
using SCCM.Dominio.Comum.Uteis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace SCCMWeb.API
{
    public class LogExceptionLogger : ExceptionLogger
    {
        public override async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async appDomain =>
            {
                WindowsIdentity windowsIdentity = (WindowsIdentity)context.RequestContext.Principal.Identity;

                string baseDiretory = (string)appDomain;

                string metodoHttp = context.Request.Method.Method;
                string url = context.Request.RequestUri.AbsoluteUri;
                string controller = context.ExceptionContext.ControllerContext != null ? context.ExceptionContext.ControllerContext.Controller.ToString() : "n/a";
                string caminhoAplicacao = baseDiretory;
                string tipoAuth = windowsIdentity.AuthenticationType;
                string usuario = windowsIdentity.Name;
                bool anonimo = windowsIdentity.IsAnonymous;

                string nomeApp = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

                string tituloLog = SCCMLog.GerarLogTituloTemplateSCCMWebApi(nomeApp, usuario, anonimo, tipoAuth, metodoHttp, url, controller, caminhoAplicacao);
                await SCCMLog.GravarLog(tituloLog, context.Exception);
            }, System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}