using System;
using System.Linq;
using Semver;

namespace VersionKeeper
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
            get
            {
                if (NoBuildHistoryForApplication()) 
                    return new SemVersion(new Version());

                return _state.BuildHistory.Last().HighestVersion;
            }
        }

        public void RecordBuild(string versionControlChangeIdentifier)
        {
            if (NoBuildHistoryForApplication())
                RecordFirstBuildVersion(versionControlChangeIdentifier);
            else if (VersionControlIdentifierAlreadyRecorded(versionControlChangeIdentifier))
                IncrementBuildNumber(versionControlChangeIdentifier);
            else
                IncrementPatchNumber(versionControlChangeIdentifier);
        }

        private void IncrementPatchNumber(string versionControlChangeIdentifier)
        {
            SemVersion highestVersion = _state.BuildHistory.Last().HighestVersion.IncrementPatchNumber();
            highestVersion = highestVersion.Change(build: "1");
            _state.BuildHistory.Add(new VersionControlIdEntry(versionControlChangeIdentifier, highestVersion));
        }

        private void IncrementBuildNumber(string versionControlChangeIdentifier)
        {
            var entry = _state.BuildHistory.Single(idEntry => idEntry.VersionControlId == versionControlChangeIdentifier);
            entry.HighestVersion = entry.HighestVersion.IncrementBuildNumber();
        }

        private void RecordFirstBuildVersion(string versionControlChangeIdentifier)
        {
            SemVersion startingVersion = new SemVersion(0, 0, 0, build: "1");
            _state.BuildHistory.Add(new VersionControlIdEntry(versionControlChangeIdentifier, startingVersion));
        }

        private bool NoBuildHistoryForApplication()
        {
            return !_state.BuildHistory.Any();
        }

        private bool VersionControlIdentifierAlreadyRecorded(string versionControlChangeIdentifier)
        {
            return _state.BuildHistory.Any(entry => entry.VersionControlId == versionControlChangeIdentifier);
        }

        public SemVersion GetHighestVersionForVersionControlId(string versionControlId)
        {
            return Version;
        }

        public void IncrementMinorVersion()
        {
            if (NoBuildHistoryForApplication())
                _state.BuildHistory.Add(new VersionControlIdEntry("",  new SemVersion(0,1)));
            else
            {
                VersionControlIdEntry latestBuild = _state.BuildHistory.Last();
                var highestVersion = latestBuild.HighestVersion;
                var nextMinor = highestVersion.Minor + 1;
                var nextVersion = highestVersion.Change(minor: nextMinor, patch:0, build:"0");
                _state.BuildHistory.Add(new VersionControlIdEntry(latestBuild.VersionControlId, nextVersion));
            }
        }
    }
}