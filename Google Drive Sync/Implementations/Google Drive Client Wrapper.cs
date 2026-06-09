// Copyright © - 16/03/2026 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveSync.Implementations
{
    public class GoogleDriveClientWrapper : IGoogleDriveClient
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;

        private DriveService _Service;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public GoogleDriveClientWrapper(
            ILoggerService _logger,
            IFileSystem _fileSystem)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
        }

        /// <summary>
        /// Creates the Google Drive service.
        /// </summary>
        public void CreateGoogleDriveService(UserCredential credential)
        {
            _Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Drive Sync"
            });

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Service");
        }

        /// <summary>
        /// Gets the folders or sub folders in Google Drive or the specified folder.
        /// </summary>
        public async Task<(IList<Google.Apis.Drive.v3.Data.File>, bool)> GetFolders(string folderId = null)
        {
            bool hasErrored = false;

            Google.Apis.Drive.v3.Data.FileList response = null;

            string query = string.Empty;

            if (!string.IsNullOrWhiteSpace(folderId))
            {
                query = $"'{folderId}' in parents and ";
            }

            query += "mimeType = 'application/vnd.google-apps.folder'";

            try
            {
                FilesResource.ListRequest request = _Service.Files.List();
                request.Q = query;
                request.Fields = "files(id, name, parents)";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Request");

                response = await request.ExecuteAsync();
            }

            catch (Exception ex)
            {
                hasErrored = true;

                if (!string.IsNullOrWhiteSpace(folderId))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the sub folders from Google Drive for folder id {folderId}");
                }

                else
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, "An error occured when trying to get the folders from Google Drive");
                }

                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (response.Files ?? new List<Google.Apis.Drive.v3.Data.File>(), hasErrored);
        }

        /// <summary>
        /// Gets all the files under a given folder.
        /// </summary>
        public async Task<(IList<Google.Apis.Drive.v3.Data.File>, bool)> GetFiles(string folderId)
        {
            bool hasErrored = false;

            Google.Apis.Drive.v3.Data.FileList response = new Google.Apis.Drive.v3.Data.FileList();

            try
            {
                FilesResource.ListRequest request = _Service.Files.List();
                request.Q = $"'{folderId}' in parents and mimeType != 'application/vnd.google-apps.folder'";
                request.Fields = "files(id, name, createdTime, modifiedTime)";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Search Query");
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Sending Request");

                response = await request.ExecuteAsync();
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from Google Drive for folder id {folderId}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (response.Files ?? new List<Google.Apis.Drive.v3.Data.File>(), hasErrored);
        }

        /// <summary>
        /// Creates a folder under the specified folder.
        /// </summary>
        public async Task<(Google.Apis.Drive.v3.Data.File, bool)> CreateFolder(string folderName, string parent)
        {
            bool hasErrored = false;

            Google.Apis.Drive.v3.Data.File response = null;

            try
            {
                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { parent }
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: \"{folderName}\", \"{fileMetaData.MimeType}\"");

                FilesResource.CreateRequest createRequest = _Service.Files.Create(fileMetaData);
                createRequest.Fields = "id";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Create Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                response = await createRequest.ExecuteAsync();
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to create {folderName} in Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (response ?? new Google.Apis.Drive.v3.Data.File(), hasErrored);
        }

        /// <summary>
        /// Uploads a new file.
        /// </summary>
        public async Task<(IUploadProgress, bool)> CreateFile(FileModel file, string parent)
        {
            bool hasErrored = false;

            IUploadProgress requestStatus = null;

            try
            {
                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    Name = $"{file.Name}.{file.Type}",
                    Parents = new List<string> { parent },
                    CreatedTime = file.Created,
                    ModifiedTime = file.LastModified
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {LoggerFunction.FormatFileMetaData(fileMetaData, "Create")}");

                Stream fileStream = _FileSystem.OpenRead(file.Id);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.CreateMediaUpload createRequest = _Service.Files.Create(fileMetaData, fileStream, GoogleDriveConverter.GetMimeType($".{file.Type}"));
                createRequest.Fields = "id, name";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Create Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                requestStatus = await createRequest.UploadAsync();

                fileStream.Close();

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (requestStatus, hasErrored);
        }

        /// <summary>
        /// Modifies the existing file.
        /// </summary>
        public async Task<(IUploadProgress, bool)> UpdateFile(FileModel file)
        {
            bool hasErrored = false;

            IUploadProgress requestStatus = null;

            try
            {
                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    ModifiedTime = file.LastModified
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {LoggerFunction.FormatFileMetaData(fileMetaData, "Update")}");

                Stream fileStream = _FileSystem.OpenRead(GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"));

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.UpdateMediaUpload updateRequest = _Service.Files.Update(fileMetaData, GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"), fileStream, GoogleDriveConverter.GetMimeType($".{file.Type}"));
                updateRequest.Fields = "id, name";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Create Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                requestStatus = await updateRequest.UploadAsync();

                fileStream.Close();

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to update {file.Name}.{file.Type} in Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (requestStatus, hasErrored);
        }

        /// <summary>
        /// Changes the location of the file.
        /// </summary>
        public async Task<(IUploadProgress, bool)> MoveFile(FileModel file, string oldParent, string newParent)
        {
            bool hasErrored = false;

            IUploadProgress requestStatus = null;

            try
            {
                Google.Apis.Drive.v3.Data.File fileMetaData = new Google.Apis.Drive.v3.Data.File
                {
                    ModifiedTime = file.LastModified
                };

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created File Meta Data: {LoggerFunction.FormatFileMetaData(fileMetaData, "Move")}");

                Stream fileStream = _FileSystem.OpenRead(GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"));

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Opened File");

                FilesResource.UpdateMediaUpload moveRequest = _Service.Files.Update(fileMetaData, GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"), fileStream, GoogleDriveConverter.GetMimeType($".{file.Type}"));
                moveRequest.RemoveParents = oldParent;
                moveRequest.AddParents = newParent;
                moveRequest.Fields = "id, name, parents";

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Created Google Drive Create Request");
                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                requestStatus = await moveRequest.UploadAsync();

                fileStream.Close();

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Closed File");
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to move {file.Name}.{file.Type} in Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (requestStatus, hasErrored);
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        public async Task<bool> DownloadFile(FileModel file, string filePath)
        {
            FolderFunction _folderFunction = new FolderFunction(_FileSystem);

            bool hasErrored = false;

            try
            {
                _folderFunction.CheckPath(filePath);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Directories Checked");

                FilesResource.GetRequest downloadRequest = _Service.Files.Get(GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"));
                MemoryStream stream = new MemoryStream();

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                downloadRequest.MediaDownloader.ProgressChanged += progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Download File: {progress.BytesDownloaded} bytes");
                            break;

                        case DownloadStatus.Completed:
                            _FileSystem.WriteDataToFile(filePath, stream.ToArray());
                            _FileSystem.SetCreatedTime(filePath, file.Created);
                            _FileSystem.SetModifiedTime(filePath, file.LastModified);

                            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Downloaded {file.Name}.{file.Type} to Local Drive");
                            break;

                        case DownloadStatus.Failed:
                            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to download {file.Name}.{file.Type} to Local Drive");
                            break;
                    }
                };

                await downloadRequest.DownloadAsync(stream, CancellationToken.None);
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to download {file.Name}.{file.Type} to Local Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return hasErrored;
        }

        /// <summary>
        /// Removes the file.
        /// </summary>
        public async Task<bool> DeleteFile(FileModel file)
        {
            bool hasErrored = false;

            try
            {
                FilesResource.DeleteRequest deleteRequest = _Service.Files.Delete(GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Left"));

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Sending Request");

                string result = await deleteRequest.ExecuteAsync();

                if (string.IsNullOrWhiteSpace(result))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Deleted {file.Name}.{file.Type} from Google Drive");
                }

                else
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to delete {file.Name}.{file.Type} from Google Drive. Returned {result}");
                }
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to delete {file.Name}.{file.Type} from Google Drive");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return hasErrored;
        }
    }
}
