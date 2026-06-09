// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;
using Moq;

namespace GoogleDriveSync.Tests.Services
{
    [TestClass]
    [DoNotParallelize]
    public class GoogleAPIServiceTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();
        private readonly Mock<ICredentialProvider> _MockCredentialProvider = new();
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
            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();

            GoogleAPIService _googleAPIService = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, string.Empty);

            bool hasErrored = _googleAPIService.GetHasErrored();

            Assert.IsFalse(hasErrored);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public async Task TestGetData()
        {
            string root = "bbe016edbc66f26f71ba7d8c445af164c2f305421f62ac4261f6122ab8b08ead";

            Google.Apis.Drive.v3.Data.File mockFolder = new()
            {
                Id = root,
                Name = "Test",
                Parents = []
            };
            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "a62cb3e9cca31abd408ccdf19517def36aab2e1cae70778e2d1e65653a612578",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([mockFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(mockFile.Id, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataSubFolder()
        {
            string root = "bbe016edbc66f26f71ba7d8c445af164c2f305421f62ac4261f6122ab8b08ead";

            Google.Apis.Drive.v3.Data.File mockFolder = new()
            {
                Id = root,
                Name = "Test",
                Parents = []
            };
            Google.Apis.Drive.v3.Data.File mockFolderSub = new()
            {
                Id = "9e741f507fb570fadebda58f93bca1851f95d90850d2cec767b5c57eb51ec33f",
                Name = "Test 2",
                Parents = [root]
            };

            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "a62cb3e9cca31abd408ccdf19517def36aab2e1cae70778e2d1e65653a612578",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };
            Google.Apis.Drive.v3.Data.File mockFile2 = new()
            {
                Id = "35e5494510bc2b9c9dabc6c1b0772fa62d7bef1b3ba7455a637f7d591c23ecd8",
                Name = "Test 2.txt",
                CreatedTime = new DateTime(1900, 01, 05),
                ModifiedTime = new DateTime(1900, 01, 06)
            };

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([mockFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([mockFolderSub], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(mockFolderSub.Id)).ReturnsAsync(([mockFile2], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(2, files.Count);

            Assert.AreEqual(mockFile.Id, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);

            Assert.AreEqual(mockFile2.Id, files[1].Id);
            Assert.AreEqual("Test 2", files[1].Name);
            Assert.AreEqual("txt", files[1].Type);
            Assert.AreEqual($@"{root}\{mockFolderSub.Id}", files[1].PathIds);
            Assert.AreEqual(@"Test\Test 2", files[1].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataSubFolderEmpty()
        {
            string root = "bbe016edbc66f26f71ba7d8c445af164c2f305421f62ac4261f6122ab8b08ead";

            Google.Apis.Drive.v3.Data.File mockFolder = new()
            {
                Id = root,
                Name = "Test",
                Parents = []
            };
            Google.Apis.Drive.v3.Data.File mockFolderSub = new()
            {
                Id = "9e741f507fb570fadebda58f93bca1851f95d90850d2cec767b5c57eb51ec33f",
                Name = "Test 2",
                Parents = [root]
            };

            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "a62cb3e9cca31abd408ccdf19517def36aab2e1cae70778e2d1e65653a612578",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([mockFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([mockFolderSub], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(mockFolderSub.Id)).ReturnsAsync(([], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(mockFile.Id, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataExcludedFile()
        {
            string root = "bbe016edbc66f26f71ba7d8c445af164c2f305421f62ac4261f6122ab8b08ead";

            Google.Apis.Drive.v3.Data.File mockFolder = new()
            {
                Id = root,
                Name = "Test",
                Parents = []
            };

            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "a62cb3e9cca31abd408ccdf19517def36aab2e1cae70778e2d1e65653a612578",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };
            Google.Apis.Drive.v3.Data.File excludedFile = new()
            {
                Id = "35e5494510bc2b9c9dabc6c1b0772fa62d7bef1b3ba7455a637f7d591c23ecd8",
                Name = "Excluded.txt",
                CreatedTime = new DateTime(1900, 01, 05),
                ModifiedTime = new DateTime(1900, 01, 06)
            };

            AppSettingsModel.IgnoreFiles = [excludedFile.Name];

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([mockFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile, excludedFile], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(mockFile.Id, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);
        }

        /// <summary>
        /// Checks whether the GetData method returns the expected list.
        /// </summary>
        [TestMethod]
        public async Task TestGetDataExcludedFolder()
        {
            string root = "bbe016edbc66f26f71ba7d8c445af164c2f305421f62ac4261f6122ab8b08ead";

            Google.Apis.Drive.v3.Data.File mockFolder = new()
            {
                Id = root,
                Name = "Test",
                Parents = []
            };
            Google.Apis.Drive.v3.Data.File excludedSub = new()
            {
                Id = "9e741f507fb570fadebda58f93bca1851f95d90850d2cec767b5c57eb51ec33f",
                Name = "Excluded",
                Parents = [root]
            };

            Google.Apis.Drive.v3.Data.File mockFile = new()
            {
                Id = "a62cb3e9cca31abd408ccdf19517def36aab2e1cae70778e2d1e65653a612578",
                Name = "Test.txt",
                CreatedTime = new DateTime(1900, 01, 01),
                ModifiedTime = new DateTime(1900, 01, 02)
            };

            AppSettingsModel.IgnoreFolders = [excludedSub.Name];

            Mock<IGoogleDriveClient> _mockGoogleDriveClient = new();
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(null)).ReturnsAsync(([mockFolder], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFolders(root)).ReturnsAsync(([excludedSub], false));
            _mockGoogleDriveClient.Setup(gdc => gdc.GetFiles(root)).ReturnsAsync(([mockFile], false));

            GoogleAPIService _googleAPI = new(_MockLogger.Object, _MockCredentialProvider.Object, _mockGoogleDriveClient.Object, _MockUserNotifier.Object, root);

            List<FileModel> files = await _googleAPI.GetData();

            Assert.AreEqual(1, files.Count);

            Assert.AreEqual(mockFile.Id, files[0].Id);
            Assert.AreEqual("Test", files[0].Name);
            Assert.AreEqual("txt", files[0].Type);
            Assert.AreEqual(root, files[0].PathIds);
            Assert.AreEqual("Test", files[0].Path);
        }
    }
}
