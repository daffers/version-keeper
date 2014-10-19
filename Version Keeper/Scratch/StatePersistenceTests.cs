using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VersionKeeper;

namespace Scratch
{
    [TestFixture]
    public class StatePersistenceTests
    {
        [Test]
        public void CanCreateAFileSystemStateStore()
        {
            var stateStore = new FileSystemStateStore();
        }

        [Test]
        public void FileSystemStateStoreImplemetsCorrectInteface()
        {
            var stateStore = new FileSystemStateStore();
            Assert.That(stateStore, Is.AssignableTo<IVersionApplicationStateStore>());
        }

        [Test]
        public void CanSpecifyADirectoryWhereStateCanBePersistedToo()
        {

            var stateStore = new FileSystemStateStore();
        }
    }

    public class FileSystemStateStore : IVersionApplicationStateStore
    {
        public void Write(VersionedApplicationState state)
        {
            throw new NotImplementedException();
        }

        public VersionedApplicationState Read(string applicationName)
        {
            throw new NotImplementedException();
        }
    }
}
