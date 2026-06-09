// Copyright © - 16/03/2026 - Toby Hunter
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the Google Drive credentials.
    /// </summary>
    public interface ICredentialProvider
    {
        Task<(UserCredential, bool)> GetCredentials();
    }
}
