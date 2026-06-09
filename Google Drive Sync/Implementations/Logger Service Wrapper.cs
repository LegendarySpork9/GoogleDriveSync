// Copyright © - 16/03/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Services;

namespace GoogleDriveSync.Implementations
{
    public class LoggerServiceWrapper : ILoggerService
    {
        readonly LoggerService _Logger = new LoggerService();

        /// <summary>
        /// Logs the given message to the log file.
        /// </summary>
        public void LogMessage(string level, string message)
        {
            _Logger.LogMessage(level, message);
        }
    }
}
