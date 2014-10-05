using System;
using NUnit.Framework;
using Scratch.TestDoubles;
using Semver;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class RecordingBuildsForANewApplication
    {
        [TestCase(1, "1")]
        [TestCase(2, "2")]
        [TestCase(3, "3")]
        [TestCase(23, "23")]
        public void BuildNumberReflectsTheNumberOfRecordedBuildsForAVersionControlIdentifer(int numberOfBuildsRecorded, string expectedBuildNumber)
        {
            const string applicationName = "test";

            var classUnderTest = BuildUpClassWithNewApplication(applicationName);
            var application = classUnderTest.GetVersionedApplication("test");

            string versionControlChangeIdentifier = Guid.NewGuid().ToString();

            for (int i = 0; i < numberOfBuildsRecorded; i++)
                application.RecordBuild(versionControlChangeIdentifier);    

            Assert.That(application.Version.Build, Is.EqualTo(expectedBuildNumber));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(8)]
        [TestCase(23)]
        public void RecordingMultipleBuildsWithDifferentVersionControlIdentifiersDoesNotIncrementBuild(int numberOfBuildsRecorded)
        {
            const string applicationName = "test";

            var classUnderTest = BuildUpClassWithNewApplication(applicationName);

            var application = classUnderTest.GetVersionedApplication(applicationName);
            
            for (int i = 0; i < numberOfBuildsRecorded; i++)
            {
                application.RecordBuild(GenerateRandomVersionControlIdentifier());
                Assert.That(application.Version.Build, Is.EqualTo("1"));
            }
        }

        [Test]
        public void RecordingDifferentBuildIdentifiersIncrementsThePatchNumber()
        {
            const string applicationName = "test";

            var classUnderTest = BuildUpClassWithNewApplication(applicationName);

            var application = classUnderTest.GetVersionedApplication(applicationName);

            application.RecordBuild(GenerateRandomVersionControlIdentifier());
            Assert.That(application.Version.Patch, Is.EqualTo(0));

            application.RecordBuild(GenerateRandomVersionControlIdentifier());
            Assert.That(application.Version.Patch, Is.EqualTo(1));
        }

        [Test]
        public void TestCombinations()
        {
            const string applicationName = "test";

            var classUnderTest = BuildUpClassWithNewApplication(applicationName);

            var application = classUnderTest.GetVersionedApplication(applicationName);

            string firstVersionControlNumber = GenerateRandomVersionControlIdentifier();
            application.RecordBuild(firstVersionControlNumber);
            application.RecordBuild(firstVersionControlNumber);

            string secondVersionControlNumber = GenerateRandomVersionControlIdentifier();

            application.RecordBuild(secondVersionControlNumber);
            application.RecordBuild(secondVersionControlNumber);

            Assert.That(application.Version.ToString(), Is.EqualTo("0.0.1+2"));
        }

        [Test]
        public void CannotRecordBuildsForAVersionControlIdentifierIfItIsNotTheMostRecentAndHasAlreadyBeenRecorded()
        {
            const string applicationName = "test";

            var classUnderTest = BuildUpClassWithNewApplication(applicationName);

            var application = classUnderTest.GetVersionedApplication(applicationName);

            string firstVersionControlNumber = GenerateRandomVersionControlIdentifier();
            string secondVersionControlNumber = GenerateRandomVersionControlIdentifier();

            application.RecordBuild(firstVersionControlNumber);
            application.RecordBuild(secondVersionControlNumber);
            application.RecordBuild(firstVersionControlNumber);
        }

        [Test]
        public void CanGetTheHighestVersionNumberForVersionControlId()
        {
            const string applicationName = "test";

            ApplicationVersionKeeper classUnderTest = BuildUpClassWithNewApplication(applicationName);
            VersionedApplication application = classUnderTest.GetVersionedApplication(applicationName);

            string firstVersionControlNumber = GenerateRandomVersionControlIdentifier();
            application.RecordBuild(firstVersionControlNumber);
            application.RecordBuild(firstVersionControlNumber);

            string secondVersionControlNumber = GenerateRandomVersionControlIdentifier();

            application.RecordBuild(secondVersionControlNumber);
            application.RecordBuild(secondVersionControlNumber);

            SemVersion version = application.GetHighestVersionForVersionControlId(firstVersionControlNumber);

            Assert.That(version.ToString(), Is.EqualTo("0.0.1+2"));
        }

        [Test]
        public void VersionNumberIncrementedCorrectlyAccrossInstancesOfVersionKeeper()
        {
            const string applicationName = "test";

            var stateStore = new VersionApplicationStateStoreMock();

            ApplicationVersionKeeper firstInstanceOfApplicationVersionKeeper = BuildUpClassWithNewApplication(applicationName, stateStore);
            VersionedApplication application = firstInstanceOfApplicationVersionKeeper.GetVersionedApplication(applicationName);

            string firstVersionControlNumber = GenerateRandomVersionControlIdentifier();
            application.RecordBuild(firstVersionControlNumber);
            application.RecordBuild(firstVersionControlNumber);

            string secondVersionControlNumber = GenerateRandomVersionControlIdentifier();

            application.RecordBuild(secondVersionControlNumber);
            application.RecordBuild(secondVersionControlNumber);
            Assert.That(application.Version.ToString(), Is.EqualTo("0.0.1+2"));

            var secondInstanceOfVersionKeeper = new ApplicationVersionKeeper(stateStore);
            application = secondInstanceOfVersionKeeper.GetVersionedApplication(applicationName);
            
            Assert.That(application.Version.ToString(), Is.EqualTo("0.0.1+2"));
        }
        
        private static ApplicationVersionKeeper BuildUpClassWithNewApplication(string applicationName, VersionApplicationStateStoreMock stateStore = null)
        {
            if (stateStore == null)
                stateStore = new VersionApplicationStateStoreMock();

            var versionedApplicationState = new VersionedApplicationState(applicationName, "");
            stateStore.SetupApplicationStateToReturn(versionedApplicationState);
            var classUnderTest = new ApplicationVersionKeeper(stateStore);
            return classUnderTest;
        }

        private static string GenerateRandomVersionControlIdentifier()
        {
            return Guid.NewGuid().ToString();
        }
    }
}