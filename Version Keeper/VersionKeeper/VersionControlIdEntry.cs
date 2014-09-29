using Semver;

namespace VersionKeeper
{
    public class VersionControlIdEntry
    {
        public VersionControlIdEntry(string versionControlId, SemVersion highestVersion)
        {
            VersionControlId = versionControlId;
            HighestVersion = highestVersion;
        }

        public string VersionControlId { get; set; }
        public SemVersion HighestVersion { get; set; }
    }
}