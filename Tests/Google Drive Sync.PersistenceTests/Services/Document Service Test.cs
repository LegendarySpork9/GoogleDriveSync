// Copyright © - 31/08/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Implementations;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;

namespace GoogleDriveSync.PersistenceTests.Services
{
    [TestClass]
    [DoNotParallelize]
    public class DocumentServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IUserNotifier> _MockUserNotifier = new();
        private string _TempDirectory = null!;

        /// <summary>
        /// Creates a temporary directory and resets static state before each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _TempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "Root");
            Directory.CreateDirectory(_TempDirectory);

            AppSettingsModel.IgnoreFolders = [];
            AppSettingsModel.IgnoreFiles = [];
        }

        /// <summary>
        /// Removes the temporary directory after each test.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            string parent = Directory.GetParent(_TempDirectory)!.FullName;

            if (Directory.Exists(parent))
            {
                foreach (string file in Directory.GetFiles(parent, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                Directory.Delete(parent, true);
            }
        }

        /// <summary>
        /// Checks whether the GetHasErrored method returns false on initialisation.
        /// </summary>
        [TestMethod]
        public void TestGetHasErrored()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            bool hasErrored = _documentService.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }

        /// <summary>
        /// Checks whether the GetData method returns a file from the root directory.
        /// </summary>
        [TestMethod]
        public void TestGetData()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "Test.txt");
            File.WriteAllText(filePath, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(filePath, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
        }

        /// <summary>
        /// Checks whether the GetData method returns files from subdirectories.
        /// </summary>
        [TestMethod]
        public void TestGetDataSubFolder()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string subDir = Path.Combine(_TempDirectory, "SubFolder");
            Directory.CreateDirectory(subDir);

            string file1 = Path.Combine(_TempDirectory, "Test.txt");
            string file2 = Path.Combine(subDir, "Test 2.txt");
            File.WriteAllText(file1, "content");
            File.WriteAllText(file2, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(2, files.Count);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("Test 2", files[1].Name);
        }

        /// <summary>
        /// Checks whether the GetData method returns no files from an empty subdirectory.
        /// </summary>
        [TestMethod]
        public void TestGetDataSubFolderEmpty()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            Directory.CreateDirectory(Path.Combine(_TempDirectory, "EmptySub"));

            string file1 = Path.Combine(_TempDirectory, "Test.txt");
            File.WriteAllText(file1, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("Test", files[0].Name);
        }

        /// <summary>
        /// Checks whether the GetData method excludes files in the ignore list.
        /// </summary>
        [TestMethod]
        public void TestGetDataExcludedFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            File.WriteAllText(Path.Combine(_TempDirectory, "Test.txt"), "content");
            File.WriteAllText(Path.Combine(_TempDirectory, "Excluded.txt"), "content");

            AppSettingsModel.IgnoreFiles = ["Excluded.txt"];

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("Test", files[0].Name);
        }

        /// <summary>
        /// Checks whether the GetData method excludes folders in the ignore list.
        /// </summary>
        [TestMethod]
        public void TestGetDataExcludedFolder()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string excludedDir = Path.Combine(_TempDirectory, "Excluded");
            Directory.CreateDirectory(excludedDir);
            File.WriteAllText(Path.Combine(excludedDir, "Hidden.txt"), "content");
            File.WriteAllText(Path.Combine(_TempDirectory, "Test.txt"), "content");

            AppSettingsModel.IgnoreFolders = ["Excluded"];

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("Test", files[0].Name);
        }

        /// <summary>
        /// Checks whether the GetData method returns the correct file metadata.
        /// </summary>
        [TestMethod]
        public void TestGetDataReturnsFileMetadata()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "Meta.txt");
            File.WriteAllText(filePath, "content");

            DateTime expectedCreated = new DateTime(2020, 06, 15, 10, 30, 0);
            DateTime expectedModified = new DateTime(2021, 03, 20, 14, 45, 0);
            File.SetCreationTime(filePath, expectedCreated);
            File.SetLastWriteTime(filePath, expectedModified);

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(expectedCreated, files[0].Created);
            Assert.AreEqual(expectedModified, files[0].LastModified);
            Assert.IsFalse(files[0].Hidden);
        }

        /// <summary>
        /// Checks whether the GetData method detects hidden files.
        /// </summary>
        [TestMethod]
        public void TestGetDataDetectsHiddenFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "Hidden.txt");
            File.WriteAllText(filePath, "content");
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.IsTrue(files[0].Hidden);
        }

        /// <summary>
        /// Checks whether the DeleteFile method removes the file from disk.
        /// </summary>
        [TestMethod]
        public void TestDeleteFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "ToDelete.txt");
            File.WriteAllText(filePath, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            _documentService.DeleteFile(filePath);

            Assert.IsFalse(File.Exists(filePath));
        }

        /// <summary>
        /// Checks whether the HideFile method sets the hidden attribute.
        /// </summary>
        [TestMethod]
        public void TestHideFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "ToHide.txt");
            File.WriteAllText(filePath, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            _documentService.HideFile(filePath, true);

            Assert.IsTrue(File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));
        }

        /// <summary>
        /// Checks whether the UnblockFile method removes the zone identifier.
        /// </summary>
        [TestMethod]
        public void TestUnblockFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "Downloaded.txt");
            File.WriteAllText(filePath, "content");

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            _documentService.UnblockFile(filePath);

            Assert.IsTrue(File.Exists(filePath));
        }

        /// <summary>
        /// Checks whether the HideFile method removes the hidden attribute.
        /// </summary>
        [TestMethod]
        public void TestUnhideFile()
        {
            FileSystemWrapper _fileSystem = new();
            FileMetadataProvider _fileMetadata = new();

            string filePath = Path.Combine(_TempDirectory, "ToUnhide.txt");
            File.WriteAllText(filePath, "content");
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

            DocumentService _documentService = new(_MockLogger.Object, _fileSystem, _fileMetadata, _MockUserNotifier.Object, _TempDirectory);

            _documentService.HideFile(filePath, false);

            Assert.IsFalse(File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));
        }
    }
}
