// Copyright © - 14/05/2025 - Toby Hunter
using Google.Apis.Drive.v3.Data;

namespace GoogleDriveSync.Functions
{
    public static class LoggerFunction
    {
        /// <summary>
        /// Returns a sting of the meta data for a Google Drive file.
        /// </summary>
        public static string FormatFileMetaData(File fileMetaData, string method)
        {
            string formattedFMD = string.Empty;

            if (fileMetaData != null)
            {
                if (method == "Create")
                {
                    formattedFMD = $"\"{fileMetaData.Name}\", \"{fileMetaData.Parents[0]}\", \"{fileMetaData.CreatedTimeRaw}\", \"{fileMetaData.ModifiedTimeRaw}\"";
                }

                else if (method == "Update" || method == "Move")
                {
                    formattedFMD = $"\"{fileMetaData.ModifiedTimeRaw}\"";
                }
            }

            return formattedFMD;
        }
    }
}
