namespace VersionKeeper
{
    public interface IVersionApplicationStateStore
    {
        void Write(VersionedApplicationState state);
        VersionedApplicationState Read(string applicationName);
    }
}