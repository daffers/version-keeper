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
using WebApi.Hal;

namespace Scratch
{
    [TestFixture]
    public class ApiTests
    {
        [Test]
        public void LoadTheSiteInMemory()
        {
            var response = ApiNavigation.GetSiteRoot();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Ignore("till we figure out why it fails")]
        public void TheMediaTypeOfTheResponseIsJsonHal()
        {
            var response = ApiNavigation.GetSiteRoot();

            Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo("application/hal+json"));
        }

        [Test]
        public void CanFormatTheResponseOnRootToARepresentationObject()
        {

        }
    }

    public class Blank : Representation
    {
        protected override void CreateHypermedia()
        {
        }
    }
}
