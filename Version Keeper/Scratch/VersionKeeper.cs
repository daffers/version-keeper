using System;
using System;
using System.Collections.Generic;
using Semver;

namespace Scratch
{
    public class VersionedApplication
    {
        private readonly VersionedApplicationState _state;
        private int _patchNumber;

        private readonly Dictionary<string, int> _buildHistory;

        public VersionedApplication(VersionedApplicationState state)
        {
            _patchNumber = 0;
            _state = state;
            _buildHistory = new Dictionary<string, int>();
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
            if(!_buildHistory.ContainsKey(versionControlChangeIdentifier))
            {
                _buildHistory.Add(versionControlChangeIdentifier, 1);
                _patchNumber ++;
            }
            else
                _buildHistory[versionControlChangeIdentifier]++;
            
            _state.CurrentVersion = _state.CurrentVersion.Change(build: _buildHistory[versionControlChangeIdentifier].ToString());
            _state.CurrentVersion = _state.CurrentVersion.Change(patch: _patchNumber);
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

        public VersionedApplicationState(string applicationName, string applicationDescription)
        {
            ApplicationName = applicationName;
            ApplicationDescription = applicationDescription;
        }
    }
}
