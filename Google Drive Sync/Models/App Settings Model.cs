// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Models
{
    /// <summary>
    /// Stores the settings used by the application.
    /// </summary>
    public static class AppSettingsModel
    {
        public static string Credentials { get; set; }
        public static string DriveFolder { get; set; }
        public static string LocalFolder { get; set; }
        public static string[] IgnoreFolders { get; set; }
        public static string[] IgnoreFiles { get; set; }
    }
}
