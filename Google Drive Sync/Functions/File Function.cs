// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleDriveSync.Functions
{
    public class FileFunction
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;
        private readonly IClock _Clock;

        /// <summary>
        /// Sets the class's gloabl variables.
        /// </summary>
        public FileFunction(
            ILoggerService _logger,
            IFileSystem _fileSystem,
            IClock _clock)
        {
            _Logger = _logger;
            _FileSystem = _fileSystem;
            _Clock = _clock;
        }

        /// <summary>
        /// Compares the files and records the changes.
        /// </summary>
        public List<FileModel> CompareForChanges(List<FileModel> googleDrive, List<FileModel> localDrive)
        {
            _Logger.LogMessage(StandardValues.LoggerValues.Info, "Comparing Google Drive and the Local Drive for changes");

            List<FileModel> files = new List<FileModel>();
            int differences = 0;

            foreach (FileModel localFile in localDrive)
            {
                List<ChangeModel> changes = new List<ChangeModel>();
                FileModel googleFile = googleDrive.Find(c => c.Name == localFile.Name && c.Type == localFile.Type);

                if (googleFile != null)
                {
                    DateTime lastModified = _Clock.DefaultDate;
                    
                    if (localFile.Path != googleFile.Path)
                    {
                        changes.Add(new ChangeModel
                        {
                            Field = "Path",
                            OldValue = localFile.Path,
                            NewValue = googleFile.Path,
                            Stream = "Up"
                        });

                        differences++;
                    }

                    if (localFile.LastModified.ToString("dd/MM/yyyy HH:mm:ss") != googleFile.LastModified.ToString("dd/MM/yyyy HH:mm:ss"))
                    {
                        if (googleFile.LastModified > localFile.LastModified)
                        {
                            lastModified = googleFile.LastModified;

                            changes.Add(new ChangeModel
                            {
                                Field = "Last Modified",
                                OldValue = localFile.LastModified.ToString(),
                                NewValue = googleFile.LastModified.ToString(),
                                Stream = "Up"
                            });
                        }

                        else
                        {
                            lastModified = localFile.LastModified;

                            changes.Add(new ChangeModel
                            {
                                Field = "Last Modified",
                                OldValue = googleFile.LastModified.ToString(),
                                NewValue = localFile.LastModified.ToString(),
                                Stream = "Down"
                            });
                        }

                        differences++;
                    }

                    if (lastModified == _Clock.DefaultDate)
                    {
                        files.Add(new FileModel
                        {
                            Id = $"{googleFile.Id},{localFile.Id}",
                            Name = googleFile.Name,
                            Type = googleFile.Type,
                            PathIds = googleFile.PathIds,
                            Path = $"{googleFile.Path},{localFile.Path}",
                            Hidden = localFile.Hidden,
                            Created = googleFile.Created,
                            LastModified = googleFile.LastModified,
                            Changes = changes
                        });
                    }

                    else
                    {
                        files.Add(new FileModel
                        {
                            Id = $"{googleFile.Id},{localFile.Id}",
                            Name = googleFile.Name,
                            Type = googleFile.Type,
                            PathIds = googleFile.PathIds,
                            Path = $"{googleFile.Path},{localFile.Path}",
                            Hidden = localFile.Hidden,
                            Created = googleFile.Created,
                            LastModified = lastModified,
                            Changes = changes
                        });
                    }
                }

                else
                {
                    changes.Add(new ChangeModel
                    {
                        Field = "Path",
                        OldValue = "Not Uploaded",
                        NewValue = localFile.Path,
                        Stream = "Down"
                    });

                    changes.Add(new ChangeModel
                    {
                        Field = "Last Modified",
                        OldValue = "Not Uploaded",
                        NewValue = localFile.LastModified.ToString(),
                        Stream = "Down"
                    });

                    files.Add(new FileModel
                    {
                        Id = localFile.Id,
                        Name = localFile.Name,
                        Type = localFile.Type,
                        Path = localFile.Path,
                        Hidden = localFile.Hidden,
                        Created = localFile.Created,
                        LastModified = localFile.LastModified,
                        Changes = changes
                    });

                    differences += 2;
                }
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Compared Local Drive");

            foreach (FileModel googleFile in googleDrive)
            {
                List<ChangeModel> changes = new List<ChangeModel>();
                FileModel file = files.Find(c => c.Name == googleFile.Name && c.Type == googleFile.Type);

                if (file == null)
                {
                    changes.Add(new ChangeModel
                    {
                        Field = "Path",
                        OldValue = "Not Downloaded",
                        NewValue = googleFile.Path,
                        Stream = "Up"
                    });

                    changes.Add(new ChangeModel
                    {
                        Field = "Last Modified",
                        OldValue = "Not Downloaded",
                        NewValue = googleFile.LastModified.ToString(),
                        Stream = "Up"
                    });

                    files.Add(new FileModel
                    {
                        Id = googleFile.Id,
                        Name = googleFile.Name,
                        Type = googleFile.Type,
                        PathIds = googleFile.PathIds,
                        Path = googleFile.Path,
                        Created = googleFile.Created,
                        LastModified = googleFile.LastModified,
                        Changes = changes
                    });

                    differences += 2;
                }
            }

            _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Compared Google Drive");
            _Logger.LogMessage(StandardValues.LoggerValues.Info, $"Found {differences} change(s) between Google Drive and the Local Drive");

            return files;
        }

        /// <summary>
        /// Checks if the file is in use.
        /// </summary>
        public bool IsFileLocked(FileInfo file) => !_FileSystem.TryOpenRead(file.FullName);
    }
}
