// Copyright © - 31/08/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Implementations;
using Moq;

namespace GoogleDriveSync.IntegrationTests.Implementations
{
    [TestClass]
    public class GoogleCredentialProviderTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();

        /// <summary>
        /// Checks whether the GetCredentials method sets hasErrored to true when the credentials file is not found.
        /// </summary>
        [TestMethod]
        public async Task TestGetCredentialsFileNotFound()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.OpenRead(It.IsAny<string>())).Throws(new FileNotFoundException());

            GoogleCredentialProvider _provider = new(_MockLogger.Object, _mockFileSystem.Object);

            (Google.Apis.Auth.OAuth2.UserCredential credential, bool hasErrored) = await _provider.GetCredentials();

            Assert.IsNull(credential);
            Assert.IsTrue(hasErrored);
        }

        /// <summary>
        /// Checks whether the GetCredentials method logs an error when a general exception occurs.
        /// </summary>
        [TestMethod]
        public async Task TestGetCredentialsGeneralException()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.OpenRead(It.IsAny<string>())).Throws(new Exception("Test exception"));

            GoogleCredentialProvider _provider = new(_MockLogger.Object, _mockFileSystem.Object);

            (Google.Apis.Auth.OAuth2.UserCredential credential, bool hasErrored) = await _provider.GetCredentials();

            Assert.IsNull(credential);
            Assert.IsTrue(hasErrored);
            _MockLogger.Verify(l => l.LogMessage("Error", It.Is<string>(s => s.Contains("Test exception"))), Times.Once);
        }

        /// <summary>
        /// Checks whether the GetCredentials method calls OpenRead with the configured credentials path.
        /// </summary>
        [TestMethod]
        public async Task TestGetCredentialsOpensConfiguredPath()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.OpenRead(It.IsAny<string>())).Throws(new FileNotFoundException());

            GoogleCredentialProvider _provider = new(_MockLogger.Object, _mockFileSystem.Object);

            await _provider.GetCredentials();

            _mockFileSystem.Verify(fs => fs.OpenRead(It.IsAny<string>()), Times.Once);
        }
    }
}
