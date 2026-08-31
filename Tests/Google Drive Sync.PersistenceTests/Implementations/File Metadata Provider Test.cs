// Copyright © - 31/08/2026 - Toby Hunter
using GoogleDriveSync.Implementations;

namespace GoogleDriveSync.PersistenceTests.Implementations
{
    [TestClass]
    public class FileMetadataProviderTest
    {
        private string _TempDirectory = null!;

        /// <summary>
        /// Creates a temporary directory for test isolation.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _TempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_TempDirectory);
        }

        /// <summary>
        /// Removes the temporary directory after each test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_TempDirectory))
            {
                foreach (string file in Directory.GetFiles(_TempDirectory))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                Directory.Delete(_TempDirectory, true);
            }
        }

        /// <summary>
        /// Checks whether the GetFileInformation method returns the expected timestamps.
        /// </summary>
        [TestMethod]
        public void TestGetFileInformationTimestamps()
        {
            FileMetadataProvider _provider = new();

            string filePath = Path.Combine(_TempDirectory, "test.txt");
            File.WriteAllText(filePath, "content");

            DateTime expectedCreated = new DateTime(2020, 06, 15, 10, 30, 0);
            DateTime expectedModified = new DateTime(2021, 03, 20, 14, 45, 0);
            File.SetCreationTime(filePath, expectedCreated);
            File.SetLastWriteTime(filePath, expectedModified);

            (DateTime created, DateTime modified, bool hidden) = _provider.GetFileInformation(filePath);

            Assert.AreEqual(expectedCreated, created);
            Assert.AreEqual(expectedModified, modified);
        }

        /// <summary>
        /// Checks whether the GetFileInformation method returns false for a non-hidden file.
        /// </summary>
        [TestMethod]
        public void TestGetFileInformationNotHidden()
        {
            FileMetadataProvider _provider = new();

            string filePath = Path.Combine(_TempDirectory, "visible.txt");
            File.WriteAllText(filePath, "content");

            (DateTime created, DateTime modified, bool hidden) = _provider.GetFileInformation(filePath);

            Assert.IsFalse(hidden);
        }

        /// <summary>
        /// Checks whether the GetFileInformation method returns true for a hidden file.
        /// </summary>
        [TestMethod]
        public void TestGetFileInformationHidden()
        {
            FileMetadataProvider _provider = new();

            string filePath = Path.Combine(_TempDirectory, "hidden.txt");
            File.WriteAllText(filePath, "content");
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

            (DateTime created, DateTime modified, bool hidden) = _provider.GetFileInformation(filePath);

            Assert.IsTrue(hidden);
        }
    }
}
