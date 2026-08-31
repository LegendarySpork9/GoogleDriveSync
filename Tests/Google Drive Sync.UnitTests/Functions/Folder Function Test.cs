// Copyright © - 31/08/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Functions;
using Moq;

namespace GoogleDriveSync.UnitTests.Functions
{
    [TestClass]
    public class FolderFunctionTest
    {
        private readonly Mock<IFileSystem> _MockFileSystem = new();

        /// <summary>
        /// Checks whether the CheckPath method creates the directory when it does not exist.
        /// </summary>
        [TestMethod]
        public void TestCheckPathCreatesDirectory()
        {
            _MockFileSystem.Setup(fs => fs.DirectoryExists(It.IsAny<string>())).Returns(false);

            FolderFunction _folderFunction = new(_MockFileSystem.Object);

            _folderFunction.CheckPath(@"C:\Test\SubFolder\Test.txt");

            _MockFileSystem.Verify(fs => fs.CreateDirectory(@"C:\Test\SubFolder"), Times.Once);
        }

        /// <summary>
        /// Checks whether the CheckPath method does not create the directory when it already exists.
        /// </summary>
        [TestMethod]
        public void TestCheckPathDirectoryExists()
        {
            _MockFileSystem.Setup(fs => fs.DirectoryExists(It.IsAny<string>())).Returns(true);

            FolderFunction _folderFunction = new(_MockFileSystem.Object);

            _folderFunction.CheckPath(@"C:\Test\SubFolder\Test.txt");

            _MockFileSystem.Verify(fs => fs.CreateDirectory(It.IsAny<string>()), Times.Never);
        }
    }
}
