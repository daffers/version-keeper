using NUnit.Framework;
using VersionKeeper;

namespace Scratch.TestDoubles
{
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
