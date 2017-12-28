using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace _4GProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "WeatherRoute",
            //    routeTemplate: "4gapi/{controller}/weather/{ln}/{lt}",
            //    defaults: new { ln = RouteParameter.Optional, lt = RouteParameter.Optional }
            //);
        }
    }
}
