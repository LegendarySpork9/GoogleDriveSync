// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Converters
{
    public static class GoogleDriveConverter
    {
        /// <summary>
        /// Returns the google drive file type.
        /// </summary>
        public static string GetMimeType(string fileType)
        {
            switch (fileType)
            {
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png": return "image/png";
                case ".jpeg": return "image/jpeg";
                default: return "application/octet-stream";
            }
        }

        /// <summary>
        /// Returns the full file path of a file.
        /// </summary>
        public static string GetFilePath(string root, string sourcePath, string file) => $@"{root}\{sourcePath}\{file}";
    }
}
