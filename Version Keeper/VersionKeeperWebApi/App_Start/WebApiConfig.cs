using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.Hal;

namespace VersionKeeperWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            GlobalConfiguration.Configuration.Formatters.Add(new JsonHalMediaTypeFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Root",
                routeTemplate: "api",
                defaults: new { controller = "Root"});

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
