// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using Moq;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class FileFunctionTest
    {
        private readonly Mock<ILoggerService> _MockLogger = new();

        /// <summary>
        /// Checks whether the CompareForChanges method returns the expected changes.
        /// </summary>
        [TestMethod]
        public void TestCompareForChangesFullUp()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(mc => mc.DefaultDate).Returns(new DateTime(1900, 01, 01));

            FileFunction _fileFunction = new(_MockLogger.Object, _mockFileSystem.Object, _mockClock.Object);

            List<FileModel> googleDrive =
            [
                new()
                {
                    Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                    Name = "Test",
                    Type = "txt",
                    PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0,c7fpJ6Y6eUMpNuqrGmLVhztZOL4l5pxT7",
                    Path = "Test Folder,Test Folder Two",
                    Created = DateTime.Parse("01/06/1985 11:05:12"),
                    LastModified = DateTime.Parse("05/09/1987 13:45:00")
                }
            ];

            List<FileModel> localDrive =
            [
                new()
                {
                    Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                    Name = "Test",
                    Type = "txt",
                    Path = "Test Folder",
                    Hidden = false,
                    Created = DateTime.Parse("01/06/1985 10:55:56"),
                    LastModified = DateTime.Parse("03/08/1987 14:23:12")
                }
            ];

            List<FileModel> result = _fileFunction.CompareForChanges(googleDrive, localDrive);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Changes.Count);

            Assert.AreEqual("Path", result[0].Changes[0].Field);
            Assert.AreEqual("Up", result[0].Changes[0].Stream);
            Assert.AreEqual(localDrive[0].Path, result[0].Changes[0].OldValue);
            Assert.AreEqual(googleDrive[0].Path, result[0].Changes[0].NewValue);

            Assert.AreEqual("Last Modified", result[0].Changes[1].Field);
            Assert.AreEqual("Up", result[0].Changes[1].Stream);
            Assert.AreEqual(localDrive[0].LastModified.ToString(), result[0].Changes[1].OldValue);
            Assert.AreEqual(googleDrive[0].LastModified.ToString(), result[0].Changes[1].NewValue);
        }

        // /hecks whether the CompareForChanges method returns the expected changes.
        [TestMethod]
        public void TestCompareForChangesPathUp()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(mc => mc.DefaultDate).Returns(new DateTime(1900, 01, 01));

            FileFunction _fileFunction = new(_MockLogger.Object, _mockFileSystem.Object, _mockClock.Object);

            List<FileModel> googleDrive =
            [
                new()
                {
                    Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                    Name = "Test",
                    Type = "txt",
                    PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0,c7fpJ6Y6eUMpNuqrGmLVhztZOL4l5pxT7",
                    Path = "Test Folder,Test Folder Two",
                    Created = DateTime.Parse("01/06/1985 11:05:12"),
                    LastModified = DateTime.Parse("05/09/1987 13:45:00")
                }
            ];

            List<FileModel> localDrive =
            [
                new()
                {
                    Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                    Name = "Test",
                    Type = "txt",
                    Path = "Test Folder",
                    Hidden = false,
                    Created = DateTime.Parse("01/06/1985 10:55:56"),
                    LastModified = DateTime.Parse("05/09/1987 13:45:00")
                }
            ];

            List<FileModel> result = _fileFunction.CompareForChanges(googleDrive, localDrive);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Changes.Count);

            Assert.AreEqual("Path", result[0].Changes[0].Field);
            Assert.AreEqual("Up", result[0].Changes[0].Stream);
            Assert.AreEqual(localDrive[0].Path, result[0].Changes[0].OldValue);
            Assert.AreEqual(googleDrive[0].Path, result[0].Changes[0].NewValue);
        }

        /// <summary>
        /// Checks whether the CompareForChanges method returns the expected changes.
        /// </summary>
        [TestMethod]
        public void TestCompareForChangesModifiedUp()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(mc => mc.DefaultDate).Returns(new DateTime(1900, 01, 01));

            FileFunction _fileFunction = new(_MockLogger.Object, _mockFileSystem.Object, _mockClock.Object);

            List<FileModel> googleDrive =
            [
                new()
                {
                    Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                    Name = "Test",
                    Type = "txt",
                    PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0",
                    Path = "Test Folder",
                    Created = DateTime.Parse("01/06/1985 11:05:12"),
                    LastModified = DateTime.Parse("05/09/1987 13:45:00")
                }
            ];

            List<FileModel> localDrive =
            [
                new()
                {
                    Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                    Name = "Test",
                    Type = "txt",
                    Path = "Test Folder",
                    Hidden = false,
                    Created = DateTime.Parse("01/06/1985 10:55:56"),
                    LastModified = DateTime.Parse("03/08/1987 14:23:12")
                }
            ];

            List<FileModel> result = _fileFunction.CompareForChanges(googleDrive, localDrive);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Changes.Count);

            Assert.AreEqual("Last Modified", result[0].Changes[0].Field);
            Assert.AreEqual("Up", result[0].Changes[0].Stream);
            Assert.AreEqual(localDrive[0].LastModified.ToString(), result[0].Changes[0].OldValue);
            Assert.AreEqual(googleDrive[0].LastModified.ToString(), result[0].Changes[0].NewValue);
        }

        /// <summary>
        /// Checks whether the CompareForChanges method returns the expected changes.
        /// </summary>
        [TestMethod]
        public void TestCompareForChangesModifiedDown()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            Mock<IClock> _mockClock = new();
            _mockClock.Setup(mc => mc.DefaultDate).Returns(new DateTime(1900, 01, 01));

            FileFunction _fileFunction = new(_MockLogger.Object, _mockFileSystem.Object, _mockClock.Object);

            List<FileModel> googleDrive =
            [
                new()
                {
                    Id = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE",
                    Name = "Test",
                    Type = "txt",
                    PathIds = "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0",
                    Path = "Test Folder",
                    Created = DateTime.Parse("01/06/1985 11:05:12"),
                    LastModified = DateTime.Parse("03/08/1987 13:45:00")
                }
            ];

            List<FileModel> localDrive =
            [
                new()
                {
                    Id = "C:\\GDSTests\\Book Tests\\Test.txt",
                    Name = "Test",
                    Type = "txt",
                    Path = "Test Folder",
                    Hidden = false,
                    Created = DateTime.Parse("01/06/1985 10:55:56"),
                    LastModified = DateTime.Parse("05/09/1987 14:23:12")
                }
            ];

            List<FileModel> result = _fileFunction.CompareForChanges(googleDrive, localDrive);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Changes.Count);

            Assert.AreEqual("Last Modified", result[0].Changes[0].Field);
            Assert.AreEqual("Down", result[0].Changes[0].Stream);
            Assert.AreEqual(googleDrive[0].LastModified.ToString(), result[0].Changes[0].OldValue);
            Assert.AreEqual(localDrive[0].LastModified.ToString(), result[0].Changes[0].NewValue);
        }

        /// <summary>
        /// Checks whether the IsFileLocked method returns the expected value.
        /// </summary>
        [TestMethod]
        public void TestIsFileLocked()
        {
            Mock<IFileSystem> _mockFileSystem = new();
            _mockFileSystem.Setup(fs => fs.TryOpenRead(It.IsAny<string>())).Returns(true);
            Mock<IClock> _mockClock = new();

            FileFunction _fileFunction = new(_MockLogger.Object, _mockFileSystem.Object, _mockClock.Object);

            bool result = _fileFunction.IsFileLocked(new("C:\\GDSTests\\Book Tests\\Test.txt"));

            Assert.IsFalse(result);
        }
    }
}
