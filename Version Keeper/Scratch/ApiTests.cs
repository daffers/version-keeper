using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using VersionKeeperWebApi;

namespace Scratch
{
    [TestFixture]
    public class ApiTests
    {
        [Test]
        public void LoadTheSiteInMemory()
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

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

    }
}
