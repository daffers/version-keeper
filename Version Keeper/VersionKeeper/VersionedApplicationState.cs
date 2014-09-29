using System.Collections.Generic;

namespace VersionKeeper
{
    public class VersionedApplicationState
    {
        public string ApplicationName { get; private set; }
        public string ApplicationDescription { get; private set; }
        public readonly List<VersionControlIdEntry> BuildHistory;
        

        public VersionedApplicationState(string applicationName, string applicationDescription)
        {
            ApplicationName = applicationName;
            ApplicationDescription = applicationDescription;
            BuildHistory = new List<VersionControlIdEntry>();
        }
    }
}