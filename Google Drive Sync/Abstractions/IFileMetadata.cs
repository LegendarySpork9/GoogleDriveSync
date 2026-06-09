// Copyright © - 16/03/2026 - Toby Hunter
using System;

namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for file metadata operations.
    /// </summary>
    public interface IFileMetadata
    {
        (DateTime, DateTime, bool) GetFileInformation(string file);
    }
}
