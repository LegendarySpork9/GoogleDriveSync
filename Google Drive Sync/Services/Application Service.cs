// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Functions;
using GoogleDriveSync.Implementations;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleDriveSync.Services
{
    public class ApplicationService
    {
        private static readonly ILoggerService _Logger = new LoggerServiceWrapper();
        private static readonly IFileSystem _FileSystem = new FileSystemWrapper();
        private readonly GoogleAPIService _GoogleAPI = new GoogleAPIService(_Logger, new GoogleCredentialProvider(_Logger, _FileSystem), new GoogleDriveClientWrapper(_Logger, _FileSystem), new MessageBoxWrapper(), AppSettingsModel.DriveFolder);
        private readonly DocumentService _DocumentService = new DocumentService(_Logger, _FileSystem, new FileMetadataProvider(), new MessageBoxWrapper(), AppSettingsModel.LocalFolder);
        private readonly FileFunction _FileFunction = new FileFunction(_Logger, _FileSystem, new SystemClockProvider());

        public event Action<int> ProgressChanged;

        /// <summary>
        /// Increases the value of the pogress bar.
        /// </summary>
        private void OnProgressChanged(int progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        /// <summary>
        /// Runs the process of checking for updates.
        /// </summary>
        public async Task<(List<FileModel>, bool)> CheckUpdates()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Checking for updates in root folder");

            int progress = 0;
            int increaseValue = ProgressBarValueConverter.GetPBIncreaseValue(4);

            List<FileModel> googleDrive = await _GoogleAPI.GetData();
            bool hasErrored = _GoogleAPI.GetHasErrored();

            progress += increaseValue;
            OnProgressChanged(progress);

            List<FileModel> localDrive = _DocumentService.GetData();
            hasErrored = _DocumentService.GetHasErrored();

            progress += increaseValue;
            OnProgressChanged(progress);

            List<FileModel> files = _FileFunction.CompareForChanges(googleDrive, localDrive);

            progress += increaseValue;
            OnProgressChanged(progress);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Checked for updates in root folder");

            return (files, hasErrored);
        }

        /// <summary>
        /// Runs the process of updating the files selected.
        /// </summary>
        public async Task<bool> SyncChanges(List<FileModel> uploadFiles, List<FileModel> downloadFiles)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Syncing {uploadFiles.Count + downloadFiles.Count} file(s) to root folder");

            bool hasErrored = false;

            int progress = 0;
            int increaseValue = ProgressBarValueConverter.GetPBIncreaseValue(uploadFiles.Count + downloadFiles.Count + 1);

            _GoogleAPI.ResetHasErrored();

            foreach (FileModel file in uploadFiles)
            {
                if (file.Changes[0].OldValue == "Not Uploaded")
                {
                    await _GoogleAPI.CreateFile(file);
                }

                else
                {
                    if (file.Changes[0].Field == "Path")
                    {
                        await _GoogleAPI.MoveFile(file);
                    }

                    else
                    {
                        await _GoogleAPI.UpdateFile(file);
                    }
                }

                hasErrored = _GoogleAPI.GetHasErrored();

                progress += increaseValue;
                OnProgressChanged(progress);
            }

            foreach (FileModel file in downloadFiles)
            {
                _DocumentService.HideFile(GoogleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"), false);

                if (file.Changes[0].Field == "Path")
                {
                    await _GoogleAPI.DownloadFile(file);

                    _DocumentService.DeleteFile(GoogleDriveFunction.RemoveStringCharacters(file.Id, new char[] { ',' }, "Right"));
                }

                else
                {
                    await _GoogleAPI.DownloadFile(file);
                }

                hasErrored = _GoogleAPI.GetHasErrored();

                _DocumentService.UnblockFile(GoogleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"));
                _DocumentService.HideFile(GoogleDriveConverter.GetFilePath(AppSettingsModel.LocalFolder.Remove(AppSettingsModel.LocalFolder.LastIndexOf('\\')), GoogleDriveFunction.RemoveStringCharacters(file.Path, new char[] { ',' }, "Left"), $"{file.Name}.{file.Type}"), file.Hidden);

                progress += increaseValue;
                OnProgressChanged(progress);
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Synced {uploadFiles.Count + downloadFiles.Count} file(s) to root folder");

            return hasErrored;
        }
    }
}
