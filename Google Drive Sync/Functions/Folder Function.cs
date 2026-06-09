// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using System.IO;

namespace GoogleDriveSync.Functions
{
    public class FolderFunction
    {
        private readonly IFileSystem _FileSystem;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public FolderFunction(
            IFileSystem _fileSystem)
        {
            _FileSystem = _fileSystem;
        }

        /// <summary>
        /// Creates the local directory if it is not present.
        /// </summary>
        public void CheckPath(string filePath)
        {
            string fileDirectory = Path.GetDirectoryName(filePath);

            if (!_FileSystem.DirectoryExists(fileDirectory))
            {
                _FileSystem.CreateDirectory(fileDirectory);
            }
        }
    }
}
