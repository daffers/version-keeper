using System;
using System;
using System.Collections.Generic;
using Semver;

namespace Scratch
{
    public class VersionedApplication
    {
        private readonly VersionedApplicationState _state;
        
        public VersionedApplication(VersionedApplicationState state)
        {
            _state = state;
        }

        public string Name
        {
            get { return _state.ApplicationName; }
        }

        public string Description
        {
            get { return _state.ApplicationDescription; }
        }

        public SemVersion Version
        {
            get { return _state.CurrentVersion; }
        }

        public void RecordBuild(string versionControlChangeIdentifier)
        {
            if(!_state.BuildHistory.ContainsKey(versionControlChangeIdentifier))
            {
                _state.BuildHistory.Add(versionControlChangeIdentifier, );
                _state.PatchNumber ++;
            }
            else
                _state.BuildHistory[versionControlChangeIdentifier]++;

            _state.CurrentVersion = _state.CurrentVersion.Change(build: _state.BuildHistory[versionControlChangeIdentifier].ToString());
            _state.CurrentVersion = _state.CurrentVersion.Change(patch: _state.PatchNumber);
        }

        public SemVersion GetHighestVersionForVersionControlId(string versionControlId)
        {
            return Version;
        }
    }

    public class ExistingVersionedApplication : VersionedApplication
    {
        public ExistingVersionedApplication(VersionedApplicationState state) : base(state)
        {
        }
    }

    public class NullVersionedApplication : VersionedApplication
    {
        public NullVersionedApplication()
            : base(new VersionedApplicationState(string.Empty, string.Empty))
        {
        }

        public new void RecordBuild(string versionControlChangeIdentifier)
        {
        }
    }

    public class VersionKeeper
    {
        private readonly IVersionApplicationStateStore _stateStore;

        public VersionKeeper(IVersionApplicationStateStore stateStore)
        {
            _stateStore = stateStore;
        }

        public VersionedApplication AddVersionedApplication(string applicationIdentifier, string applicationDescription)
        {
            var existingApplication = _stateStore.Read(applicationIdentifier);

            if (existingApplication != null)
                return new ExistingVersionedApplication(existingApplication);

            var versionedApplicationState = new VersionedApplicationState(applicationIdentifier, applicationDescription);
            versionedApplicationState.CurrentVersion = new SemVersion(new Version());
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
            versionedApplicationState.CurrentVersion = startingVersion;
            return new VersionedApplication(versionedApplicationState);
        }
    }

    public interface IVersionApplicationStateStore
    {
        void Write(VersionedApplicationState state);
        VersionedApplicationState Read(string applicationName);
    }

    //[Serializable]
    public class VersionedApplicationState
    {
        public string ApplicationName { get; private set; }
        public string ApplicationDescription { get; private set; }
        public SemVersion CurrentVersion { get; set; }
        public readonly Dictionary<string, SemVersion> BuildHistory;
        public int PatchNumber;
        

        public VersionedApplicationState(string applicationName, string applicationDescription)
        {
            ApplicationName = applicationName;
            ApplicationDescription = applicationDescription;
            BuildHistory = new Dictionary<string, SemVersion>();
            PatchNumber = 0;
        }
    }

    public static class SemVersionExtensions
    {
        public static int GetBuildNumber(this SemVersion version)
        {
            int buildNumber = 0;
            if (int.TryParse(version.Build, out buildNumber))
                return buildNumber;

            return -1;
        }

        public static void SetBuildNumber(this SemVersion version, int buildNumber)
        {
            version.Change(build: buildNumber.ToString());
        }
    }
}
