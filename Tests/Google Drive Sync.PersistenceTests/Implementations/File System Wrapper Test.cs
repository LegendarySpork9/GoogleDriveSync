// Copyright © - 31/08/2026 - Toby Hunter
using GoogleDriveSync.Implementations;

namespace GoogleDriveSync.PersistenceTests.Implementations
{
    [TestClass]
    public class FileSystemWrapperTest
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
        /// Checks whether the FileExists method returns true for an existing file.
        /// </summary>
        [TestMethod]
        public void TestFileExistsReturnsTrue()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "test.txt");
            File.WriteAllText(filePath, "test content");

            bool result = _wrapper.FileExists(filePath);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks whether the FileExists method returns false for a non-existing file.
        /// </summary>
        [TestMethod]
        public void TestFileExistsReturnsFalse()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "nonexistent.txt");

            bool result = _wrapper.FileExists(filePath);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Checks whether the DirectoryExists method returns true for an existing directory.
        /// </summary>
        [TestMethod]
        public void TestDirectoryExistsReturnsTrue()
        {
            FileSystemWrapper _wrapper = new();

            bool result = _wrapper.DirectoryExists(_TempDirectory);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks whether the DirectoryExists method returns false for a non-existing directory.
        /// </summary>
        [TestMethod]
        public void TestDirectoryExistsReturnsFalse()
        {
            FileSystemWrapper _wrapper = new();

            bool result = _wrapper.DirectoryExists(Path.Combine(_TempDirectory, "nonexistent"));

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Checks whether the CreateDirectory method creates the directory.
        /// </summary>
        [TestMethod]
        public void TestCreateDirectory()
        {
            FileSystemWrapper _wrapper = new();

            string newDir = Path.Combine(_TempDirectory, "newdir");
            _wrapper.CreateDirectory(newDir);

            Assert.IsTrue(Directory.Exists(newDir));
        }

        /// <summary>
        /// Checks whether the WriteDataToFile method writes the expected bytes.
        /// </summary>
        [TestMethod]
        public void TestWriteDataToFile()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "data.bin");
            byte[] data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

            _wrapper.WriteDataToFile(filePath, data);

            byte[] result = File.ReadAllBytes(filePath);
            CollectionAssert.AreEqual(data, result);
        }

        /// <summary>
        /// Checks whether the DeleteFile method removes the file.
        /// </summary>
        [TestMethod]
        public void TestDeleteFile()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "todelete.txt");
            File.WriteAllText(filePath, "delete me");

            _wrapper.DeleteFile(filePath);

            Assert.IsFalse(File.Exists(filePath));
        }

        /// <summary>
        /// Checks whether the TryOpenRead method returns true when the file is not locked.
        /// </summary>
        [TestMethod]
        public void TestTryOpenReadNotLocked()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "unlocked.txt");
            File.WriteAllText(filePath, "content");

            bool result = _wrapper.TryOpenRead(filePath);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks whether the TryOpenRead method returns false when the file is locked.
        /// </summary>
        [TestMethod]
        public void TestTryOpenReadLocked()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "locked.txt");
            File.WriteAllText(filePath, "content");

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                bool result = _wrapper.TryOpenRead(filePath);

                Assert.IsFalse(result);
            }
        }

        /// <summary>
        /// Checks whether the SetCreatedTime method updates the file creation time.
        /// </summary>
        [TestMethod]
        public void TestSetCreatedTime()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "created.txt");
            File.WriteAllText(filePath, "content");

            DateTime expected = new DateTime(2020, 06, 15, 10, 30, 0);
            _wrapper.SetCreatedTime(filePath, expected);

            Assert.AreEqual(expected, File.GetCreationTime(filePath));
        }

        /// <summary>
        /// Checks whether the SetModifiedTime method updates the file modification time.
        /// </summary>
        [TestMethod]
        public void TestSetModifiedTime()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "modified.txt");
            File.WriteAllText(filePath, "content");

            DateTime expected = new DateTime(2021, 03, 20, 14, 45, 0);
            _wrapper.SetModifiedTime(filePath, expected);

            Assert.AreEqual(expected, File.GetLastWriteTime(filePath));
        }

        /// <summary>
        /// Checks whether the GetFiles method returns the expected files.
        /// </summary>
        [TestMethod]
        public void TestGetFiles()
        {
            FileSystemWrapper _wrapper = new();

            File.WriteAllText(Path.Combine(_TempDirectory, "file1.txt"), "a");
            File.WriteAllText(Path.Combine(_TempDirectory, "file2.txt"), "b");

            string[] result = _wrapper.GetFiles(_TempDirectory);

            Assert.AreEqual(2, result.Length);
        }

        /// <summary>
        /// Checks whether the GetDirectories method returns the expected directories.
        /// </summary>
        [TestMethod]
        public void TestGetDirectories()
        {
            FileSystemWrapper _wrapper = new();

            Directory.CreateDirectory(Path.Combine(_TempDirectory, "sub1"));
            Directory.CreateDirectory(Path.Combine(_TempDirectory, "sub2"));

            string[] result = _wrapper.GetDirectories(_TempDirectory);

            Assert.AreEqual(2, result.Length);
        }

        /// <summary>
        /// Checks whether the SetAttributes method updates the file attributes.
        /// </summary>
        [TestMethod]
        public void TestSetAttributes()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "hidden.txt");
            File.WriteAllText(filePath, "content");

            _wrapper.SetAttributes(filePath, FileAttributes.Hidden | FileAttributes.Archive);

            FileAttributes result = File.GetAttributes(filePath);
            Assert.IsTrue(result.HasFlag(FileAttributes.Hidden));
        }

        /// <summary>
        /// Checks whether the Open method returns a stream.
        /// </summary>
        [TestMethod]
        public void TestOpen()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "open.txt");
            File.WriteAllText(filePath, "test");

            using (Stream stream = _wrapper.Open(filePath))
            {
                Assert.IsTrue(stream.CanRead);
                Assert.IsTrue(stream.CanWrite);
            }
        }

        /// <summary>
        /// Checks whether the OpenRead method returns a readable stream.
        /// </summary>
        [TestMethod]
        public void TestOpenRead()
        {
            FileSystemWrapper _wrapper = new();

            string filePath = Path.Combine(_TempDirectory, "read.txt");
            File.WriteAllText(filePath, "test");

            using (Stream stream = _wrapper.OpenRead(filePath))
            {
                Assert.IsTrue(stream.CanRead);
                Assert.IsTrue(stream.Length > 0);
            }
        }
    }
}
