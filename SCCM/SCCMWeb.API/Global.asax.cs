using HelperComum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace SCCMWeb.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //private Log log = new Log("SCCM-API");

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //log.Novo("SCCM-API api iniciado.", "INFO");
        }
    }
}
