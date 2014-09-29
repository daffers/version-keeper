using Semver;

namespace VersionKeeper
{
    public class ApplicationVersionKeeper
    {
        private readonly IVersionApplicationStateStore _stateStore;

        public ApplicationVersionKeeper(IVersionApplicationStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        public VersionedApplication AddVersionedApplication(string applicationIdentifier, string applicationDescription)
        {
            var existingApplication = _stateStore.Read(applicationIdentifier);

            if (existingApplication != null)
                return new ExistingVersionedApplication(existingApplication);

            var versionedApplicationState = new VersionedApplicationState(applicationIdentifier, applicationDescription);
            _stateStore.Write(versionedApplicationState);
            return new VersionedApplication(versionedApplicationState);
        }

        public VersionedApplication GetVersionedApplication(string applicationName)
        {
            var state = _stateStore.Read(applicationName);
            if (state == null)
                return new NullVersionedApplication();

            return new VersionedApplication(state);
        }

        public VersionedApplication AddVersionedApplication(string applicationIdentifier, string applicationDescription,
            SemVersion startingVersion)
        {
            var versionedApplicationState = new VersionedApplicationState("", "");
            versionedApplicationState.BuildHistory.Add(new VersionControlIdEntry("", startingVersion));
            return new VersionedApplication(versionedApplicationState);
        }
    }
}
