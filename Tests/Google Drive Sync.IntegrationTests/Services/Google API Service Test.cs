// Copyright © - 31/08/2026 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;

namespace GoogleDriveSync.IntegrationTests.Services
{
    [TestClass]
    [DoNotParallelize]
    public class GoogleAPIServiceTest
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
        /// Checks whether the GetData method retrieves files from a single folder.
        /// </summary>
        [TestMethod]
        public async Task TestGetData()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };
            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "fileId1",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("fileId1", files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);
            Assert.IsFalse(_googleAPI.GetHasErrored());
        }

        /// <summary>
        /// Checks whether the GetData method traverses subfolders correctly.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataSubFolder()
        {
            string root = "rootFolderId";
            string subId = "subFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };
            Google.Apis.Drive.v3.Data.File subFolder = new() { Id = subId, Name = "Sub", Parents = [root] };
            Google.Apis.Drive.v3.Data.File file1 = new()
            {
                Id = "fileId1", Name = "Root.txt",
                CreatedTime = new DateTime(1900, 01, 01), ModifiedTime = new DateTime(1900, 01, 02)
            };
            Google.Apis.Drive.v3.Data.File file2 = new()
            {
                Id = "fileId2", Name = "Child.txt",
                CreatedTime = new DateTime(1900, 01, 03), ModifiedTime = new DateTime(1900, 01, 04)
            };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([subFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(subId)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([file1], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(subId)).ReturnsAsync(([file2], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(2, files.Count);
            Assert.AreEqual("Root", files[0].Name);
            Assert.AreEqual("Test", files[0].Path);
            Assert.AreEqual("Child", files[1].Name);
            Assert.AreEqual(@"Test\Sub", files[1].Path);
        }

        /// <summary>
        /// Checks whether the GetData method excludes files in the ignore list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataExcludedFile()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };
            Google.Apis.Drive.v3.Data.File keepFile = new()
            {
                Id = "fileId1", Name = "Keep.txt",
                CreatedTime = new DateTime(1900, 01, 01), ModifiedTime = new DateTime(1900, 01, 02)
            };
            Google.Apis.Drive.v3.Data.File excludeFile = new()
            {
                Id = "fileId2", Name = "Excluded.txt",
                CreatedTime = new DateTime(1900, 01, 03), ModifiedTime = new DateTime(1900, 01, 04)
            };

            AppSettingsModel.IgnoreFiles = ["Excluded.txt"];

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([keepFile, excludeFile], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("Keep", files[0].Name);
        }

        /// <summary>
        /// Checks whether the GetData method excludes folders in the ignore list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataExcludedFolder()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };
            Google.Apis.Drive.v3.Data.File excludedFolder = new() { Id = "excludedId", Name = "Excluded", Parents = [root] };
            Google.Apis.Drive.v3.Data.File file1 = new()
            {
                Id = "fileId1", Name = "Root.txt",
                CreatedTime = new DateTime(1900, 01, 01), ModifiedTime = new DateTime(1900, 01, 02)
            };

            AppSettingsModel.IgnoreFolders = ["Excluded"];

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([excludedFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([file1], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual("Root", files[0].Name);
            _mockGoogleDriveClient.Verify(gdc => gdc.GetFiles("excludedId"), Times.Never);
        }

        /// <summary>
        /// Checks whether the GetData method propagates error state from the Google Drive client.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataPropagatesError()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], true));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            Assert.IsTrue(_googleAPI.GetHasErrored());
            _MockUserNotifier.Verify(un => un.ShowMessage(It.IsAny<string>(), "Warning"), Times.Once);
        }

        /// <summary>
        /// Checks whether the GetData method notifies the user when credentials fail.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataCredentialError()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, true));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            _MockUserNotifier.Verify(un => un.ShowMessage(It.Is<string>(s => s.Contains("credentials")), "Warning"), Times.Once);
        }

        /// <summary>
        /// Checks whether the ResetHasErrored method clears the error state.
        /// </summary>
        [TestMethod]
        public async Task TestResetHasErrored()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], true));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            Assert.IsTrue(_googleAPI.GetHasErrored());

            _googleAPI.ResetHasErrored();

            Assert.IsFalse(_googleAPI.GetHasErrored());
        }
        /// <summary>
        /// Checks whether the DeleteFile method calls the Google Drive client.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteFile()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.DeleteFile(It.IsAny<FileModel>())).ReturnsAsync(false);

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            FileModel testFile = new() { Name = "Test", Type = "txt", Id = "fileId1" };

            await _googleAPI.DeleteFile(testFile);

            Assert.IsFalse(_googleAPI.GetHasErrored());
            _mockGoogleDriveClient.Verify(gdc => gdc.DeleteFile(testFile), Times.Once);
        }

        /// <summary>
        /// Checks whether the DeleteFile method propagates errors from the Google Drive client.
        /// </summary>
        [TestMethod]
        public async Task TestDeleteFileError()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.DeleteFile(It.IsAny<FileModel>())).ReturnsAsync(true);

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            FileModel testFile = new() { Name = "Test", Type = "txt", Id = "fileId1" };

            await _googleAPI.DeleteFile(testFile);

            Assert.IsTrue(_googleAPI.GetHasErrored());
            _MockUserNotifier.Verify(un => un.ShowMessage(It.Is<string>(s => s.Contains("delete")), "Warning"), Times.Once);
        }

        /// <summary>
        /// Checks whether the DownloadFile method calls the Google Drive client.
        /// </summary>
        [TestMethod]
        public async Task TestDownloadFile()
        {
            string root = "rootFolderId";

            AppSettingsModel.LocalFolder = @"C:\GDSTests\Root";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.DownloadFile(It.IsAny<FileModel>(), It.IsAny<string>())).ReturnsAsync(false);

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            FileModel testFile = new() { Name = "Test", Type = "txt", Id = "fileId1", Path = "Test Folder,Local Folder" };

            await _googleAPI.DownloadFile(testFile);

            Assert.IsFalse(_googleAPI.GetHasErrored());
            _mockGoogleDriveClient.Verify(gdc => gdc.DownloadFile(testFile, It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Checks whether the UpdateFile method calls the Google Drive client with the correct file.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateFile()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            Mock<Google.Apis.Upload.IUploadProgress> _mockProgress = new();
            _mockProgress.Setup(p => p.Status).Returns(Google.Apis.Upload.UploadStatus.Completed);

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.UpdateFile(It.IsAny<FileModel>())).ReturnsAsync((_mockProgress.Object, false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            FileModel testFile = new() { Name = "Test", Type = "txt", Id = "fileId1,C:\\local\\Test.txt" };

            await _googleAPI.UpdateFile(testFile);

            Assert.IsFalse(_googleAPI.GetHasErrored());
            _mockGoogleDriveClient.Verify(gdc => gdc.UpdateFile(testFile), Times.Once);
        }

        /// <summary>
        /// Checks whether the UpdateFile method propagates errors from the Google Drive client.
        /// </summary>
        [TestMethod]
        public async Task TestUpdateFileError()
        {
            string root = "rootFolderId";

            Mock<ICredentialProvider> _mockCredentialProvider = new();
            _mockCredentialProvider.Setup(cp => cp.GetCredentials()).ReturnsAsync(((UserCredential)null!, false));

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            Mock<Google.Apis.Upload.IUploadProgress> _mockProgress = new();
            _mockProgress.Setup(p => p.Status).Returns(Google.Apis.Upload.UploadStatus.Failed);

            Google.Apis.Drive.v3.Data.File rootFolder = new() { Id = root, Name = "Test", Parents = [] };

            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([rootFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.UpdateFile(It.IsAny<FileModel>())).ReturnsAsync((_mockProgress.Object, true));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _mockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            await _googleAPI.GetData();

            FileModel testFile = new() { Name = "Test", Type = "txt", Id = "fileId1,C:\\local\\Test.txt" };

            await _googleAPI.UpdateFile(testFile);

            Assert.IsTrue(_googleAPI.GetHasErrored());
            _MockUserNotifier.Verify(un => un.ShowMessage(It.Is<string>(s => s.Contains("update")), "Warning"), Times.Once);
        }
    }
}
