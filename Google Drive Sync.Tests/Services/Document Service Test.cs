// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;

namespace GoogleDriveSync.Tests.Services
{
    [TestClass]
    [DoNotParallelize]
    public class DocumentServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<IUserNotifier> _MockUserNotifier = new();

        /// <summary>
        /// Resets the static state before each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            AppSettingsModel.IgnoreFolders = [];
            AppSettingsModel.IgnoreFiles = [];
        }

        /// <summary>
        /// Checks whether the GetHasErrored method returns the expected value.
        /// </summary>
        [TestMethod]
        public void TestGetHasErrored()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            Mock<IFileMetadata> _mockFileMetadata = new();

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, string.Empty);

            bool hasErrored = _documentService.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public void TestGetData()
        {
            string root = @"C:\Test";
            string file = @"C:\Test\Test.txt";

            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.GetDirectories(root)).Returns([]);
            _mockFileSystem.Setup(fs => fs.GetFiles(root)).Returns([file]);
            Mock<IFileMetadata> _mockFileMetadata = new();
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file)).Returns((new DateTime(1900, 01, 01), new DateTime(1900, 01, 02), false));

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(file, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public void TestGetDataSubFolder()
        {
            string root = @"C:\Test";
            string sub = @"C:\Test\Test 2";

            string file = @"C:\Test\Test.txt";
            string file2 = @"C:\Test\Test 2.txt";

            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.GetDirectories(root)).Returns([sub]);
            _mockFileSystem.Setup(fs => fs.GetFiles(root)).Returns([file]);
            _mockFileSystem.Setup(fs => fs.GetFiles(sub)).Returns([file2]);
            Mock<IFileMetadata> _mockFileMetadata = new();
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file)).Returns((new DateTime(1900, 01, 01), new DateTime(1900, 01, 02), false));
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file2)).Returns((new DateTime(1900, 01, 05), new DateTime(1900, 01, 06), true));

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(2, files.Count);

            Assert.AreEqual(file, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual("Test", files[0].Path);

            Assert.AreEqual(file2, files[1].Id);
            Assert.AreEqual("Test 2", files[1].Name);
            Assert.AreEqual("txt", files[1].Type);
            Assert.AreEqual(@"Test\Test 2", files[1].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public void TestGetDataSubFolderEmpty()
        {
            string root = @"C:\Test";
            string sub = @"C:\Test\Test 2";

            string file = @"C:\Test\Test.txt";

            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.GetDirectories(root)).Returns([sub]);
            _mockFileSystem.Setup(fs => fs.GetFiles(root)).Returns([file]);
            _mockFileSystem.Setup(fs => fs.GetFiles(sub)).Returns([]);
            Mock<IFileMetadata> _mockFileMetadata = new();
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file)).Returns((new DateTime(1900, 01, 01), new DateTime(1900, 01, 02), false));

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(file, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public void TestGetDataExcludedFile()
        {
            string root = @"C:\Test";
            string file = @"C:\Test\Test.txt";
            string excludedFile = @"C:\Test\Excluded.txt";

            AppSettingsModel.IgnoreFiles = [excludedFile.Replace(@"C:\Test\", "")];

            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.GetDirectories(root)).Returns([]);
            _mockFileSystem.Setup(fs => fs.GetFiles(root)).Returns([file, excludedFile]);
            Mock<IFileMetadata> _mockFileMetadata = new();
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file)).Returns((new DateTime(1900, 01, 01), new DateTime(1900, 01, 02), false));

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(file, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public void TestGetDataExcludedFolder()
        {
            string root = @"C:\Test";
            string excludedSub = @"C:\Test\Excluded";

            string file = @"C:\Test\Test.txt";

            AppSettingsModel.IgnoreFolders = [excludedSub.Replace( @"C:\Test\", "")];

            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.GetDirectories(root)).Returns([excludedSub]);
            _mockFileSystem.Setup(fs => fs.GetFiles(root)).Returns([file]);
            Mock<IFileMetadata> _mockFileMetadata = new();
            _mockFileMetadata.Setup(fmd => fmd.GetFileInformation(file)).Returns((new DateTime(1900, 01, 01), new DateTime(1900, 01, 02), false));

            DocumentService _documentService = new(_MockLogger.Object, _mockFileSystem.Object, _mockFileMetadata.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = _documentService.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(file, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual("Test", files[0].Path);
        }
    }
}
