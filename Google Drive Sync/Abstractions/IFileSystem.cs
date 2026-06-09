// Copyright © - 16/03/2026 - Toby Hunter
using System;
using System.IO; 

namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the file system operations.
    /// </summary>
    public interface IFileSystem
    {
        #region Files

        bool FileExists(string file);
        bool TryOpenRead(string file);
        FileAttributes GetAttributes(string file);
        void SetAttributes(string file, FileAttributes attributes);
        void DeleteFile(string file);
        void WriteDataToFile(string file, byte[] data);
        void SetCreatedTime(string file, DateTime created);
        void SetModifiedTime(string file, DateTime modified);

        #endregion

        #region Directories

        bool DirectoryExists(string fileDirectory);
        void CreateDirectory(string fileDirectory);
        string[] GetDirectories(string folder);
        string[] GetFiles(string folder);

        #endregion

        #region Stream

        Stream Open(string file);
        Stream OpenRead(string file);

        #endregion
    }
}
