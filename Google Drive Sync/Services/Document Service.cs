// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleDriveSync.Services
{
    public class DocumentService
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IFileMetadata _FileMetadata;
        private readonly IUserNotifier _UserNotifier;

        private readonly string FolderPath;
        private readonly string FolderName;
        private bool HasErrored = false;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public DocumentService(
            ILoggerService _logger,
            IFileSystem _fileSystem,
            IFileMetadata _fileMetadata,
            IUserNotifier _userNotifier,
            string folder)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _FileMetadata = _fileMetadata;
            _UserNotifier = _userNotifier;
            FolderPath = folder;
            FolderName = folder.Remove(0, folder.LastIndexOf('\\') + 1);
        }

        /// <summary>
        /// Returns the value of the HasErrored variable.
        /// </summary>
        public bool GetHasErrored() => HasErrored;

        /// <summary>
        /// Obtains all the files and folders of the specified directory.
        /// </summary>
        public List<FileModel> GetData()
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder(s) and file(s) under folder {FolderName}");

            List<FileModel> localDrive = new List<FileModel>();
            (string[] folderPaths, string[] folderNames) = GetFolders(FolderPath);
            
            localDrive = GetFiles(FolderPath, FolderName);

            string path = FolderName;

            for (int i = 0; i < folderPaths.Length; i++)
            {
                path += $@"\{folderNames[i]}";

                TraverseFolders(localDrive, folderPaths[i], path);

                path = path.Replace($@"\{folderNames[i]}", "");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {localDrive.Count} file(s) under folder {FolderName}");

            return localDrive;
        }

        /// <summary>
        /// Obtains all the folders under a given folder.
        /// </summary>
        private (string[], string[]) GetFolders(string folder)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining folder information for folder(s) under folder {LocalDriveConverter.GetObjectName(folder)}");

            string[] folderPaths = Array.Empty<string>();
            string[] folderNames = Array.Empty<string>();

            try
            {
                string[] folders = _FileSystem.GetDirectories(folder);

                if (folders.Length > 0)
                {
                    foreach (string folderPath in folders)
                    {
                        if (AppSettingsModel.IgnoreFolders == null || !AppSettingsModel.IgnoreFolders.Contains(LocalDriveConverter.GetObjectName(folderPath)))
                        {
                            folderPaths = folderPaths.Append(folderPath).ToArray();
                            folderNames = folderNames.Append(LocalDriveConverter.GetObjectName(folderPath)).ToArray();

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Name: {LocalDriveConverter.GetObjectName(folderPath)}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Folder Path: {folderPath}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                HasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the sub folders from the Local Drive under folder {LocalDriveConverter.GetObjectName(folder)}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                _UserNotifier.ShowMessage($"An error occured when trying to get the sub folders from the Local Drive under folder {LocalDriveConverter.GetObjectName(folder)}", "Warning");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {folderPaths.Length} folder(s) under folder {LocalDriveConverter.GetObjectName(folder)}");

            return (folderPaths, folderNames);
        }

        /// <summary>
        /// Obtains all the files under a given folder.
        /// </summary>
        private List<FileModel> GetFiles(string folder, string path)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file(s) under folder {LocalDriveConverter.GetObjectName(folder)}");

            List<FileModel> localDrive = new List<FileModel>();

            try
            {
                string[] filePaths = _FileSystem.GetFiles(folder);

                if (filePaths.Length > 0)
                {
                    foreach (string filePath in filePaths)
                    {
                        if (AppSettingsModel.IgnoreFiles == null || !AppSettingsModel.IgnoreFiles.Contains(LocalDriveConverter.GetObjectName(filePath)) && !filePath.Contains("~$e"))
                        {
                            (DateTime created, DateTime modified, bool hidden) = GetFileInformation(filePath);

                            string[] nameSplit = LocalDriveConverter.GetObjectName(filePath).Split('.');

                            localDrive.Add(new FileModel()
                            {
                                Id = filePath,
                                Name = nameSplit[0],
                                Type = nameSplit[1],
                                Path = path,
                                Hidden = hidden,
                                Created = created,
                                LastModified = modified
                            });

                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Id: {filePath}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Name: {nameSplit[0]}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"File Type: {nameSplit[1]}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Path: {path}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Hidden: {hidden}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Created Date: {created}");
                            _Logger.LogMessage(StandardValues.LoggerValues.Debug, $"Modified Date: {modified}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogMessage(StandardValues.LoggerValues.Warning, $"An error occured when trying to get the files from the Local Drive under folder {LocalDriveConverter.GetObjectName(folder)}");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());

                _UserNotifier.ShowMessage($"An error occured when trying to get the files from the Local Drive under folder {LocalDriveConverter.GetObjectName(folder)}", "Warning");
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained {localDrive.Count} file(s) under folder {LocalDriveConverter.GetObjectName(folder)}");

            return localDrive;
        }

        /// <summary>
        /// Obtains specific information about the file.
        /// </summary>
        private (DateTime, DateTime, bool) GetFileInformation(string file)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtaining file information for file {LocalDriveConverter.GetObjectName(file)}");

            (DateTime created, DateTime modified, bool hidden) = _FileMetadata.GetFileInformation(file);

            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Obtained file information for file {LocalDriveConverter.GetObjectName(file)}");

            return (created, modified, hidden);
        }

        /// <summary>
        /// Loops through all folders and sub folders to obtain all files.
        /// </summary>
        private void TraverseFolders(List<FileModel> localDrive, string folder, string path)
        {
            (string[] folderPaths, string[] folderNames) = GetFolders(folder);

            localDrive.AddRange(GetFiles(folder, path));

            for (int i = 0; i < folderPaths.Length; i++)
            {
                path += $@"\{folderNames[i]}";

                TraverseFolders(localDrive, folderPaths[i], path);

                path = path.Replace($@"\{folderNames[i]}", "");
            }
        }

        /// <summary>
        /// Delets the given file.
        /// </summary>
        public void DeleteFile(string file) => _FileSystem.DeleteFile(file);

        /// <summary>
        /// Unblocks the given file.
        /// </summary>
        public void UnblockFile(string file)
        {
            string adsPath = $"{file}:Zone.Identifier";

            if (_FileSystem.FileExists(adsPath))
            {
                _FileSystem.DeleteFile(adsPath);
            }
        }

        /// <summary>
        /// Hides the given file if it was already hidden.
        /// </summary>
        public void HideFile(string file, bool hidden)
        {
            if (_FileSystem.FileExists(file))
            {
                if (hidden)
                {
                    _FileSystem.SetAttributes(file, _FileSystem.GetAttributes(file) | FileAttributes.Hidden);
                }

                else
                {
                    _FileSystem.SetAttributes(file, _FileSystem.GetAttributes(file) & ~FileAttributes.Hidden);
                }
            }
        }
    }
}
