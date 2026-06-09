// Copyright © - 14/05/2025 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Upload;
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleDriveSync.Services
{
    public class GoogleAPIService
    {
        private readonly ILoggerService _Logger;
        private readonly ICredentialProvider _CredentialProvider;
        private readonly IGoogleDriveClient _GoogleDriveClient;
        private readonly IUserNotifier _UserNotifier;

        private readonly string FolderId;
        private string FolderName;
        private bool HasErrored = false;
        private bool _Initialized = false;
        private List<KeyValuePair<string, string>> FolderStore = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public GoogleAPIService(
            ILoggerService _logger,
            ICredentialProvider _credentialProvider,
            IGoogleDriveClient _googleDriveClient,
            IUserNotifier _userNotifier,
            string folderId)
        {
            _Logger = _logger;
            _CredentialProvider = _credentialProvider;
            _GoogleDriveClient = _googleDriveClient;
            _UserNotifier = _userNotifier;
            FolderId = folderId;
        }

        /// <summary>
        /// Initializes the Google Drive service and folder name.
        /// </summary>
        public async Task InitializeAsync()
        {
            if (!_Initialized)
            {
                _GoogleDriveClient.CreateGoogleDriveService(await GetCredentials());
                FolderName = await GetFolderName(FolderId);
                _Initialized = true;
            }
        }

        /// <summary>
        /// Returns the value of the HasErrored variable.
        /// </summary>
        /// <returns></returns>
        public bool GetHasErrored() => HasErrored;

        /// <summary>
        /// Resets the HasErrored variable.
        /// </summary>
        public void ResetHasErrored() => HasErrored = false;

        /// <summary>
        /// Gets the name of the given folder id.
        /// </summary>
        private async Task<string> GetFolderName(string folderId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder name for folder id {folderId}");

            string folderName = string.Empty;

            (IList<Google.Apis.Drive.v3.Data.File> driveFolders, bool hasErrored) = await _GoogleDriveClient.GetFolders();

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage("An error occured when trying to get the folders from Google Drive", "Warning");
            }

            if (driveFolders != null && driveFolders.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                foreach (Google.Apis.Drive.v3.Data.File folder in driveFolders)
                {
                    if (folder.Id == folderId)
                    {
                        folderName = folder.Name;

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {folderName}");

                        FolderStore.Add(new KeyValuePair<string, string>(folderId, folderName));
                    }
                }
            }

            if (!string.IsNullOrEmpty(folderName))
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained folder name for folder id {folderId}");
            }

            else
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to obtain folder name for folder id {folderId}");
            }

            return folderName;
        }

        /// <summary>
        /// Generates credentials from the specified json file.
        /// </summary>
        private async Task<UserCredential> GetCredentials()
        {
            (UserCredential credential, bool hasErrored) = await _CredentialProvider.GetCredentials();

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage("An error occured when trying to generate user credentials", "Warning");
            }

            return credential;
        }

        /// <summary>
        /// Obtains all the files and folders of the specified directory.
        /// </summary>
        public async Task<List<FileModel>> GetData()
        {
            await InitializeAsync();

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file(s) under folder {FolderName ?? FolderId}");

            FolderStore = new List<KeyValuePair<string, string>>();

            List<FileModel> googleDrive = new List<FileModel>();
            (string[] folderIds, string[] folderNames) = await GetFolders(FolderId);

            googleDrive = await GetFiles(FolderId, FolderId, FolderName);

            string pathIds = FolderId;
            string path = FolderName;

            for (int i = 0; i < folderIds.Length; i++)
            {
                pathIds += $@"\{folderIds[i]}";
                path += $@"\{folderNames[i]}";

                await TraverseFolders(googleDrive, folderIds[i], pathIds, path);

                pathIds = pathIds.Replace($@"\{folderIds[i]}", "");
                path = path.Replace($@"\{folderNames[i]}", "");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {googleDrive.Count} file(s) under folder {FolderName ?? FolderId}");

            return googleDrive;
        }

        /// <summary>
        /// Obtains all the folders under a given folder.
        /// </summary>
        private async Task<(string[], string[])> GetFolders(string folderId)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {folderId}");

            string[] folderIds = Array.Empty<string>();
            string[] folderNames = Array.Empty<string>();

            (IList<Google.Apis.Drive.v3.Data.File> driveFolders, bool hasErrored) = await _GoogleDriveClient.GetFolders(folderId);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to get the sub folders from Google Drive for folder id {folderId}", "Warning");
            }

            if (driveFolders != null && driveFolders.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                foreach (Google.Apis.Drive.v3.Data.File folder in driveFolders)
                {
                    if (AppSettingsModel.IgnoreFolders == null || !AppSettingsModel.IgnoreFolders.Contains(folder.Name))
                    {
                        folderIds = folderIds.Append(folder.Id).ToArray();
                        folderNames = folderNames.Append(folder.Name).ToArray();

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Id: {folder.Id}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {folder.Name}");

                        FolderStore.Add(new KeyValuePair<string, string>(folder.Id, $"{folder.Name} ({folder.Parents[0].ToString()})"));
                    }
                }
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folderIds.Length} folder(s) under folder {folderId}");

            return (folderIds, folderNames);
        }

        /// <summary>
        /// Obtains all the files under a given folder.
        /// </summary>
        private async Task<List<FileModel>> GetFiles(string folderId, string pathIds, string path)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {folderId}");

            List<FileModel> googleDrive = new List<FileModel>();

            (IList<Google.Apis.Drive.v3.Data.File> driveFiles, bool hasErrored) = await _GoogleDriveClient.GetFiles(folderId);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to get the files from Google Drive for folder id {folderId}", "Warning");
            }

            if (driveFiles != null && driveFiles.Count > 0)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, "Received Response");

                foreach (Google.Apis.Drive.v3.Data.File file in driveFiles)
                {
                    if (AppSettingsModel.IgnoreFiles == null || !AppSettingsModel.IgnoreFiles.Contains(file.Name))
                    {
                        string[] nameSplit = file.Name.Split('.');

                        googleDrive.Add(new FileModel()
                        {
                            Id = file.Id,
                            Name = nameSplit[0],
                            Type = nameSplit[1],
                            PathIds = pathIds,
                            Path = path,
                            Created = file.CreatedTime.Value,
                            LastModified = file.ModifiedTime.Value
                        });

                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Id: {file.Id}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Name: {nameSplit[0]}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Type: {nameSplit[1]}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path Ids: {pathIds}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path: {path}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created Date: {file.CreatedTimeRaw}");
                        _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Modified Date: {file.ModifiedTimeRaw}");
                    }
                }
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {googleDrive.Count} file(s) under folder {folderId}");

            return googleDrive;
        }

        /// <summary>
        /// Loops through all folders and sub folders to obtain all files.
        /// </summary>
        private async Task TraverseFolders(List<FileModel> googleDrive, string folderId, string pathIds, string path)
        {
            (string[] folderIds, string[] folderNames) = await GetFolders(folderId);

            googleDrive.AddRange(await GetFiles(folderId, pathIds, path));

            for (int i = 0; i < folderIds.Length; i++)
            {
                pathIds += $@"\{folderIds[i]}";
                path += $@"\{folderNames[i]}";

                await TraverseFolders(googleDrive, folderIds[i], pathIds, path);

                pathIds = pathIds.Replace($@"\{folderIds[i]}", "");
                path = path.Replace($@"\{folderNames[i]}", "");
            }
        }

        /// <summary>
        /// Creates a folder under the specified folder.
        /// </summary>
        private async Task<string> CreateFolder(string folderName, string parent)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Creating {folderName} folder in Google Drive");

            (Google.Apis.Drive.v3.Data.File folder, bool hasErrored) = await _GoogleDriveClient.CreateFolder(folderName, parent);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to create {folderName} in Google Drive", "Warning");
            }

            if (!string.IsNullOrWhiteSpace(folder.Id))
            {
                FolderStore.Add(new KeyValuePair<string, string>(folder.Id, $"{folderName} ({parent})"));

                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Created {folderName} in Google Drive");
            }

            return folder.Id;
        }

        /// <summary>
        /// Checks if the required folders exist.
        /// </summary>
        private async Task<string> CheckFolders(string[] folders)
        {
            string parent = AppSettingsModel.DriveFolder;

            for (int x = 1; x < folders.Length; x++)
            {
                string folderStoreValue = $"{folders[x]} ({parent})";
                string folderId = FolderStore.Find(c => c.Value == folderStoreValue).Key;

                if (string.IsNullOrWhiteSpace(folderId))
                {
                    _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"{folders[x]} folder not found in Google Drive");

                    string newFolderId = await CreateFolder(folders[x], parent);

                    if (x != (folders.Length - 1))
                    {
                        parent = newFolderId;
                    }
                }

                else if (x != (folders.Length - 1))
                {
                    parent = folderId;
                }
            }

            return parent;
        }

        /// <summary>
        /// Uploads a new file.
        /// </summary>
        public async Task CreateFile(FileModel file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploading {file.Name}.{file.Type} to Google Drive");

            string folderParent = await CheckFolders(file.Path.Remove(0, file.Path.IndexOf(',') + 1).Split('\\'));
            string folderStoreValue = $"{GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { '\\' }, "Right")} ({folderParent})";
            string parent = FolderStore.Find(c => c.Value == folderStoreValue).Key ?? AppSettingsModel.DriveFolder;

            (IUploadProgress requestStatus, bool hasErrored) = await _GoogleDriveClient.CreateFile(file, parent);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to upload {file.Name}.{file.Type} to Google Drive", "Warning");
            }

            if (requestStatus.Status == UploadStatus.Completed)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Uploaded {file.Name}.{file.Type} to Google Drive");
            }

            else
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to upload {file.Name}.{file.Type} to Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        /// <summary>
        /// Modifies the existing file.
        /// </summary>
        public async Task UpdateFile(FileModel file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Updating {file.Name}.{file.Type} in Google Drive");

            (IUploadProgress requestStatus, bool hasErrored) = await _GoogleDriveClient.UpdateFile(file);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to update {file.Name}.{file.Type} in Google Drive", "Warning");
            }

            if (requestStatus.Status == UploadStatus.Completed)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Updated {file.Name}.{file.Type} in Google Drive");
            }

            else
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to update {file.Name}.{file.Type} in Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        /// <summary>
        /// Changes the location of the file.
        /// </summary>
        public async Task MoveFile(FileModel file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Moving {file.Name}.{file.Type} in Google Drive");

            string folderParent = await CheckFolders(file.Path.Remove(0, file.Path.IndexOf(',') + 1).Split('\\'));
            string folderStoreValue = $"{GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',', '\\' }, "Right")} ({folderParent})";

            string oldParent = GoogleDriveFunction.RemoveStringCharacters(file.PathIds, new char[] { '\\' }, "Right");
            string newParent = FolderStore.Find(c => c.Value == folderStoreValue).Key ?? AppSettingsModel.DriveFolder;

            (IUploadProgress requestStatus, bool hasErrored) = await _GoogleDriveClient.MoveFile(file, oldParent, newParent);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to move {file.Name}.{file.Type} in Google Drive", "Warning");
            }

            if (requestStatus != null && requestStatus.Status == UploadStatus.Completed)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Moved {file.Name}.{file.Type} in Google Drive");
            }

            else
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Failed to move {file.Name}.{file.Type} in Google Drive. Returned Code {requestStatus.Status}");
            }
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        public async Task DownloadFile(FileModel file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Downloading {file.Name}.{file.Type} to Local Drive");

            string filePath = GoogleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}");

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Path: {filePath}");

            bool hasErrored = await _GoogleDriveClient.DownloadFile(file, filePath);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to download {file.Name}.{file.Type} to Local Drive", "Warning");
            }
        }

        /// <summary>
        /// Removes the file.
        /// </summary>
        public async Task DeleteFile(FileModel file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Deleting {file.Name}.{file.Type} from Google Drive");

            bool hasErrored = await _GoogleDriveClient.DeleteFile(file);

            HasErrored = hasErrored;

            if (HasErrored)
            {
                _UserNotifier.ShowMessage($"An error occured when trying to delete {file.Name}.{file.Type} from Google Drive", "Warning");
            }
        }
    }
}
