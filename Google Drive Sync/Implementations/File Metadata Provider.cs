// Copyright © - 16/03/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using System;
using System.IO;

namespace GoogleDriveSync.Implementations
{
    public class FileMetadataProvider : IFileMetadata
    {
        /// <summary>
        /// Returns a files created, modified and hidden attributes.
        /// </summary>
        public (DateTime, DateTime, bool) GetFileInformation(string file)
        {
            FileInfo info = new FileInfo(file);
            FileAttributes attributes = info.Attributes;

            bool hidden = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

            return (info.CreationTime, info.LastWriteTime, hidden);
        }
    }
}
