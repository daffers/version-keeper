using NUnit.Framework;
using Scratch.TestDoubles;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class AddingAnApplicationTests
    {
        [Test]
        public void CreatingANewApplicationCallsTheStateStore()
        {
            var stateStoreSpy = new VersionApplicationStateStoreMock();

            var classUnderTest = new ApplicationVersionKeeper(stateStoreSpy);
            classUnderTest.AddVersionedApplication("TestApplication", "Application Description");

            stateStoreSpy.AssertThisApplicationWasCreated("TestApplication", "Application Description");
        }

        [Test]
        public void AVersionedApplicationIsReturedWhenCreated()
        {
            var stateStoreSpy = new VersionApplicationStateStoreMock();

            var classUnderTest = new ApplicationVersionKeeper(stateStoreSpy);
            VersionedApplication versionedApplication = classUnderTest.AddVersionedApplication("TestApplication", "Application Description");

            Assert.That(versionedApplication, Is.Not.Null);
        }

        [Test]
        public void WhenApplicationWithSameNameExistsReturnsDuplicateResult()
        {
            var state = new VersionedApplicationState("test", "description");
            var stateStoreSpy = new VersionApplicationStateStoreMock();
            stateStoreSpy.SetupApplicationStateToReturn(state);

            var classUnderTest = new ApplicationVersionKeeper(stateStoreSpy);

            var existingApplication = classUnderTest.AddVersionedApplication("test", "other description");
            Assert.That(existingApplication, Is.TypeOf<ExistingVersionedApplication>());
        }
    }
}