using NUnit.Framework;
using Scratch.TestDoubles;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class RetrievingAnApplicationTests
    {

        [Test]
        public void CanRetrieveAnApplicationWhenItIsTheStateStore()
        {
            const string applicationDescription = "description";
            const string applicationName = "test";

            var stateStore = new VersionApplicationStateStoreMock();
            stateStore.SetupApplicationStateToReturn(new VersionedApplicationState(applicationName, applicationDescription));

            var classUnderTest = new ApplicationVersionKeeper(stateStore);
            VersionedApplication application = classUnderTest.GetVersionedApplication(applicationName);

            Assert.That(application, Is.Not.Null);
            Assert.That(application.Name, Is.EqualTo(applicationName));
            Assert.That(application.Description, Is.EqualTo(applicationDescription));

            stateStore.AssertThisApplicationNameWasRequested(applicationName);
        }

        [Test]
        public void WhenApplicationDoesNotExistANullApplicationIsReturned()
        {
            var classUnderTest = new ApplicationVersionKeeper(new VersionApplicationStateStoreMock());
            VersionedApplication application = classUnderTest.GetVersionedApplication("applicationName");

            Assert.That(application, Is.TypeOf<NullVersionedApplication>());

            Assert.That(application.Name, Is.Empty);
            Assert.That(application.Description, Is.Empty);
        }
    }
}