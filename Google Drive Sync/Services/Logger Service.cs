// Copyright © - 14/05/2025 - Toby Hunter
using log4net;

namespace GoogleDriveSync.Services
{
    internal class LoggerService
    {
        private readonly ILog Logger = LogManager.GetLogger("Logs");

        /// <summary>
        /// Sends a message to the specified logs.
        /// </summary>
        public void LogMessage(string level, string message)
        {
            switch (level)
            {
                case "Info": Logger.Info(message); break;
                case "Debug": Logger.Debug(message); break;
                case "Warn": Logger.Warn(message); break;
                case "Error": Logger.Error(message); break;
            }
        }
    }
}
