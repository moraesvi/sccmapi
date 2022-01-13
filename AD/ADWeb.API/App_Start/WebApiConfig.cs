using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

namespace ADWeb.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Formatters
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Web API cors
            ICorsPolicyProvider cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API exception - log
            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new LogExceptionLogger());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
