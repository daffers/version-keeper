using System;
using System.Net.Http;
using System.Web.Http;
using VersionKeeperWebApi;

namespace Scratch
{
    public class ApiNavigation
    {
        public static HttpResponseMessage GetSiteRoot()
        {
            HttpResponseMessage response = null;
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            using (var server = new HttpServer(httpConfiguration))
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/api"));
                using (var client = new HttpClient(server))
                {
                    response = client.SendAsync(request).Result;
                }
            }
            return response;
        }
    }
}