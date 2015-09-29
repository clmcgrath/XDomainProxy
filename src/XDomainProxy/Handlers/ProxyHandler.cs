using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Config = System.Web.Configuration.WebConfigurationManager;

namespace XDomainProxy.Handlers
{
    public class ProxyHandler : DelegatingHandler
    {
        public string UrlHeaderKey { get; } = Config.AppSettings["Proxy:UrlHeaderKey"] ?? "X-Forward-Url";
        public string PortHeaderKey { get; } = Config.AppSettings["Proxy:PortHeaderKey"] ?? "X-Forward-Port";


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var forwardUri = new UriBuilder(GetProxyUri(request) + request.RequestUri.PathAndQuery) {Port = GetProxyPort(request)};

            //strip off the proxy port and replace with an Http port
            //send it on to the requested URL
            request.RequestUri = forwardUri.Uri;

            //have to explicitly null it to avoid protocol violation
            if (request.Method == HttpMethod.Get)
                request.Content = null;

            var client = new HttpClient();

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return response;
        }

        protected bool ContainsForwardUrlHeader(HttpRequestMessage request)
        {
            return request.Headers.Contains(UrlHeaderKey);
        }

        protected Uri GetProxyUri(HttpRequestMessage request)
        {
            if (ContainsForwardUrlHeader(request))
            {
                var first = request.Headers.GetValues(UrlHeaderKey).FirstOrDefault();
                if (first != null) return new Uri(first);
            }
            var address = WebConfigurationManager.AppSettings["Proxy:ForwardAddress"];

            return address != null ? new Uri(address) : request.RequestUri;
        }

        protected int GetProxyPort(HttpRequestMessage request)
        {
            if (ContainsForwardPortHeader(request))
            {
                var portString = request.Headers.GetValues(PortHeaderKey).FirstOrDefault();
                if (portString != null) return Convert.ToInt32(portString);
            }
            var port = Convert.ToInt32(WebConfigurationManager.AppSettings["Proxy:ForwardPort"]);

            return port > 0 ? port : request.RequestUri.Port;
        }

        private bool ContainsForwardPortHeader(HttpRequestMessage request)
        {
            return request.Headers.Contains(PortHeaderKey);
        }
    }
}