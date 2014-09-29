using Semver;

namespace VersionKeeper
{
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

        public static SemVersion IncrementBuildNumber(this SemVersion version)
        {
            int buildNumber;
            if (int.TryParse(version.Build, out buildNumber))
            {
                buildNumber++;
                return version.Change(build: (buildNumber).ToString());
            }

            return version;
        }

        public static SemVersion IncrementPatchNumber(this SemVersion version)
        {
            return version.Change(patch: version.Patch + 1);
        }
    }
}