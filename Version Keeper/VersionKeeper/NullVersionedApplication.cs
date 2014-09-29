namespace VersionKeeper
{
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
}