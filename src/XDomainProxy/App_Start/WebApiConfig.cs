using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using XDomainProxy.Handlers;
using  Config = System.Web.Configuration.WebConfigurationManager;

namespace XDomainProxy
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //if ((Config.AppSettings["Proxy:EnableCors"] ?? "false").ToLower() == "true")
            //{
            //    config.EnableCors();
            //}
            

            config.Routes.Clear();
            
            //Web Api Proxy 
            config.Routes.MapHttpRoute(name: "Proxy", routeTemplate: "{*path*}", handler:
            HttpClientFactory.CreatePipeline
            (
                innerHandler: new HttpClientHandler(), // will never get here if proxy is doing its job
                handlers: new DelegatingHandler[] { new ProxyHandler() }
            ),
                defaults: new { path = RouteParameter.Optional },
                constraints: null
            );



        }
    }
}
