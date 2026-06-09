// Copyright © - 16/03/2026 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Upload;
using GoogleDriveSync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the Google Drive operations.
    /// </summary>
    public interface IGoogleDriveClient
    {
        void CreateGoogleDriveService(UserCredential credentials);
        Task<(IList<File>, bool)> GetFolders(string folderId = null);
        Task<(IList<File>, bool)> GetFiles(string folderId);
        Task<(File, bool)> CreateFolder(string folderName, string parent);
        Task<(IUploadProgress, bool)> CreateFile(FileModel file, string parent);
        Task<(IUploadProgress, bool)> UpdateFile(FileModel file);
        Task<(IUploadProgress, bool)> MoveFile(FileModel file, string oldParent, string newParent);
        Task<bool> DownloadFile(FileModel file, string filePath);
        Task<bool> DeleteFile(FileModel file);
    }
}
