// Copyright © - 16/03/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using System;
using System.IO;

namespace GoogleDriveSync.Implementations
{
    public class FileSystemWrapper : IFileSystem
    {
        #region Files

        /// <summary>
        /// Returns whether the given file exists.
        /// </summary>
        public bool FileExists(string file) => File.Exists(file);

        /// <summary>
        /// Returns the attributes for the given file.
        /// </summary>
        public FileAttributes GetAttributes(string file) => File.GetAttributes(file);

        /// <summary>
        /// Changes the value of an attribute for the given file.
        /// </summary>
        public void SetAttributes(string file, FileAttributes attribute) => File.SetAttributes(file, attribute);

        /// <summary>
        /// Deletes the given file.
        /// </summary>
        public void DeleteFile(string file) => File.Delete(file);

        /// <summary>
        /// Returns whether the file is in use.
        /// </summary>
        public bool TryOpenRead(string file)
        {
            try
            {
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }

            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes bytes to the given file.
        /// </summary>
        public void WriteDataToFile(string file, byte[] data) => File.WriteAllBytes(file, data);

        /// <summary>
        /// Sets the created time of the given file.
        /// </summary>
        public void SetCreatedTime(string file, DateTime created) => File.SetCreationTime(file, created);

        /// <summary>
        /// Sets the modified time of the given file.
        /// </summary>
        public void SetModifiedTime(string file, DateTime modified) => File.SetLastWriteTime(file, modified);

        #endregion

        #region Directories

        /// <summary>
        /// Returns whether the given directory exists.
        /// </summary>
        public bool DirectoryExists(string fileDirectory) => Directory.Exists(fileDirectory);

        /// <summary>
        /// Creates the given directory.
        /// </summary>
        public void CreateDirectory(string fileDirectory) => Directory.CreateDirectory(fileDirectory);

        /// <summary>
        /// Returns an array of the directories in a given folder.
        /// </summary>
        public string[] GetDirectories(string folder) => Directory.GetDirectories(folder);

        /// <summary>
        /// Returns an array of the files in a given folder.
        /// </summary>
        public string[] GetFiles(string folder) => Directory.GetFiles(folder);

        #endregion

        #region Stream

        /// <summary>
        /// Returns a new stream for the given file.
        /// </summary>
        public Stream Open(string file) => new FileStream(file, FileMode.Open);

        /// <summary>
        /// Returns a new read stream for the given file.
        /// </summary>
        public Stream OpenRead(string file) => new FileStream(file, FileMode.Open, FileAccess.Read);

        #endregion
    }
}
