using System;
using System.Linq;
using NUnit.Framework;
using Scratch.TestDoubles;
using Semver;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class IncrementingMinorVersionTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void MultipleCallsToIncrementMinorVersionCorrectlyAffectsVersionNumber(int numberOfIncrements)
        {
            var application = ConfigureVersionKeeperWithApplicationState();

            for (int i = 0; i < numberOfIncrements; i++)
                application.IncrementMinorVersion();

            Assert.That((object) application.Version.Minor, Is.EqualTo(numberOfIncrements));
        }

        [Test]
        public void PatchVersionsAreResetWhenMinorVersionIncremented()
        {
            var state = new VersionedApplicationState("test", "test");
            state.BuildHistory.Add(new VersionControlIdEntry(Guid.NewGuid().ToString(), new SemVersion(0,0,1)));
            var application = ConfigureVersionKeeperWithApplicationState(state);

            application.IncrementMinorVersion();

            Assert.That((object) application.Version.Patch, Is.EqualTo(0));
        }

        [Test]
        public void BuildVersionResetToZeroWhenMinorVersionIncremenmted()
        {
            var state = new VersionedApplicationState("test", "test");
            state.BuildHistory.Add(new VersionControlIdEntry(Guid.NewGuid().ToString(), new SemVersion(0, 0,0,build:"1")));
            var application = ConfigureVersionKeeperWithApplicationState(state);

            application.IncrementMinorVersion();

            Assert.That((object) application.Version.Build, Is.EqualTo("0"));
        }

        [Test]
        public void LatestsVersionControlIdIsRecordedAgainstNewMinorVersionNumber()
        {
            string versionControlId = Guid.NewGuid().ToString();
            var state = new VersionedApplicationState("test", "test");
            state.BuildHistory.Add(new VersionControlIdEntry(versionControlId, new SemVersion(0, 0, 0, build: "1")));
            var application = ConfigureVersionKeeperWithApplicationState(state);

            application.IncrementMinorVersion();

            var latestBuild = state.BuildHistory.Last();
            Assert.That(latestBuild.HighestVersion.Minor, Is.EqualTo(1));
            Assert.That(latestBuild.VersionControlId, Is.EqualTo(versionControlId));
        }

        private static VersionedApplication ConfigureVersionKeeperWithApplicationState(VersionedApplicationState applicationState = null)
        {
            if (applicationState == null)
                applicationState = new VersionedApplicationState("name", "desc");

            var versionApplicationStateStoreMock = new VersionApplicationStateStoreMock();
            versionApplicationStateStoreMock.SetupApplicationStateToReturn(applicationState);
            var classUnderTest = new ApplicationVersionKeeper(versionApplicationStateStoreMock);
            VersionedApplication application = classUnderTest.AddVersionedApplication("name", "desc");
            return application;
        }
    }
}