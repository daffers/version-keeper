using System;
using NUnit.Framework;
using Semver;
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


    [TestFixture]
    public class VersioningApplicationTests
    {
        [Test]
        public void TheDefaultVersionForAnApplicaitonIs_0_0_0_0()
        {
            var classUnderTest = new ApplicationVersionKeeper(new VersionApplicationStateStoreMock());
            VersionedApplication application = classUnderTest.AddVersionedApplication("name", "desc");

            Assert.That(application.Version, Is.TypeOf<SemVersion>());
            Assert.That(application.Version.ToString(), Is.EqualTo("0.0.0"));
        }

        [Test]
        public void CanCreateAnApplicationAtASpecificVersion()
        {
            const string setVersion = "1.2.3-4";
            var classUnderTest = new ApplicationVersionKeeper(new VersionApplicationStateStoreMock());
            VersionedApplication application = classUnderTest.AddVersionedApplication("name", "desc",
                SemVersion.Parse(setVersion));

            Assert.That(application.Version.ToString(), Is.EqualTo(setVersion));
        }
    }

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


    public class VersionApplicationStateStoreMock : IVersionApplicationStateStore
    {
        private string _applicationName;
        private string _applicationDescription;
        
        public void Write(VersionedApplicationState state)
        {
            _applicationName = state.ApplicationName;
            _applicationDescription = state.ApplicationDescription;
        }

        private string _applicationStateRequested;
        private VersionedApplicationState _applicationStateToReturn;
        public VersionedApplicationState Read(string applicationName)
        {
            _applicationStateRequested = applicationName;
            return _applicationStateToReturn;
        }

        public void AssertThisApplicationWasCreated(string applicationName, string applicationDescription)
        {
            Assert.That(_applicationName, Is.EqualTo(applicationName));
            Assert.That(_applicationDescription, Is.EqualTo(applicationDescription));
        }

        public void AssertThisApplicationNameWasRequested(string applicationName)
        {
            Assert.That(_applicationStateRequested, Is.EqualTo(applicationName));
        }

        public void SetupApplicationStateToReturn(VersionedApplicationState applicationState)
        {
            _applicationStateToReturn = applicationState;
        }
    }
}
