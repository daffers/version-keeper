using NUnit.Framework;
using Scratch.TestDoubles;
using Semver;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class VersioningApplicationTests
    {
        [Test]
        public void TheDefaultVersionForAnApplicaitonIs_0_0_0_0()
        {
            var classUnderTest = new ApplicationVersionKeeper(new VersionApplicationStateStoreMock());
            VersionedApplication application = classUnderTest.AddVersionedApplication("name", "desc");

            Assert.That(application.Version, Is.TypeOf<SemVersion>());
            Assert.That(application.Version.ToString(), Is.EqualTo("0.0.0"));
        }

        [Test]
        public void CanCreateAnApplicationAtASpecificVersion()
        {
            const string setVersion = "1.2.3-4";
            var classUnderTest = new ApplicationVersionKeeper(new VersionApplicationStateStoreMock());
            VersionedApplication application = classUnderTest.AddVersionedApplication("name", "desc",
                SemVersion.Parse(setVersion));

            Assert.That(application.Version.ToString(), Is.EqualTo(setVersion));
        }
    }
}