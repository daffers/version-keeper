using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WebApi.Hal;

namespace Scratch
{
    [Binding]
    public class ApiBrowsingStepDefinitions
    {
        public HttpResponseMessage LastResponse
        {
            get { return ScenarioContext.Current.Get<HttpResponseMessage>(); }
            set { ScenarioContext.Current.Set(value);}
        }

        [When(@"When i navigate to the root of the api")]
        public void GetApiRoot()
        {
            LastResponse = ApiNavigation.GetSiteRoot();
        }

        [Then(@"i should see the link allowing me to create an application")]
        public void ThenIShouldSeeTheLinkAllowingMeToCreateAnApplication()
        {
            Representation lastResponse = null;

            LastResponse.TryGetContentValue(out lastResponse);

            var createApplicationLink = lastResponse.Links.SingleOrDefault(link => link.Rel == "VersionedApplications");
            Assert.That(createApplicationLink, Is.Not.Null);
        }


        [Then(@"i should see the link allowing me to retrieve an application")]
        public void ThenIShouldSeeTheLinkAllowingMeToRetrieveAnApplication()
        {
            Representation lastResponse = null;

            LastResponse.TryGetContentValue(out lastResponse);

            var createApplicationLink = lastResponse.Links.SingleOrDefault(link => link.Rel == "VersionedApplications");
            Assert.That(createApplicationLink, Is.Not.Null);
        }


    }


}
