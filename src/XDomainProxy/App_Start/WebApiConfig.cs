using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using XDomainProxy.Handlers;

namespace XDomainProxy
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            
            config.Routes.Clear();
            //Web Api Proxy 
            config.Routes.MapHttpRoute(name: "Proxy", routeTemplate: "{*path}", handler:
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
