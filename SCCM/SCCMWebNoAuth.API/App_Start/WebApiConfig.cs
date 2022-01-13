using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

namespace SCCMWebNoAuth.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Formatters
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new BsonMediaTypeFormatter());

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            ICorsPolicyProvider cors = new EnableCorsAttribute("*", "*", "*");

            config.EnableCors(cors);

            // Web API - Log
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
