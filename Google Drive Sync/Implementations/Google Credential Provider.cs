// Copyright © - 16/03/2026 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleDriveSync.Implementations
{
    public class GoogleCredentialProvider : ICredentialProvider
    {
        private readonly ILoggerService _Logger;
        private readonly IFileSystem _FileSystem;

        /// <summary>
        /// Sets the class's global variables.
        /// </summary>
        public GoogleCredentialProvider(
            ILoggerService _logger,
            IFileSystem fileSystem)
        {
            _Logger = _logger;
            _FileSystem = fileSystem;
        }

        /// <summary>
        /// Generates credentials from the specified json file.
        /// </summary>
        public async Task<(UserCredential, bool)> GetCredentials()
        {
            UserCredential credential = null;
            bool hasErrored = false;

            try
            {
                Stream credentialsStream = _FileSystem.OpenRead(AppSettingsModel.Credentials);

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Opened stream to Google Drive OAuth Credentials");

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(credentialsStream).Secrets,
                    new[] { DriveService.Scope.Drive },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)
                );

                _Logger.LogMessage(StandardValues.LoggerValues.Debug, "Generated User Credentials");
            }

            catch (Exception ex)
            {
                hasErrored = true;

                _Logger.LogMessage(StandardValues.LoggerValues.Warning, "An error occured when trying to generate user credentials");
                _Logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
            }

            return (credential, hasErrored);
        }
    }
}
