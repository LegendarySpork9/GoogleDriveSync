// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Converters
{
    public static class LocalDriveConverter
    {
        /// <summary>
        /// Returns the name of the object from the full path.
        /// </summary>
        public static string GetObjectName(string objectPath) => objectPath.Remove(0, objectPath.LastIndexOf('\\') + 1);
    }
}
