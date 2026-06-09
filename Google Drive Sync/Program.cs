// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Converters;
using GoogleDriveSync.Implementations;
using GoogleDriveSync.Models;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            ILoggerService logger = new LoggerServiceWrapper();

            logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");

            if (!Setup())
            {
                logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
                Environment.Exit(0);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainSyncPage());
        }

        /// <summary>
        /// Checks the application settings are present.
        /// </summary>
        private static bool Setup()
        {
            ILoggerService logger = new LoggerServiceWrapper();

            bool configured = true;

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CredentialsLocation"]))
            {
                logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid credentials location not found. Please provide one in the app settings with the tag \"CredentialsLocation\"");

                configured = false;
            }

            else
            {
                AppSettingsModel.Credentials = ConfigurationManager.AppSettings["CredentialsLocation"];

                logger.LogMessage(StandardValues.LoggerValues.Debug, $"Credentials Location: {AppSettingsModel.Credentials}");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["GoogleDriveFolder"]))
            {
                logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid drive folder not found. Please provide one in the app settings with the tag \"GoogleDriveFolder\"");

                configured = false;
            }

            else
            {
                AppSettingsModel.DriveFolder = ConfigurationManager.AppSettings["GoogleDriveFolder"];

                logger.LogMessage(StandardValues.LoggerValues.Debug, $"Google Drive Folder: {AppSettingsModel.DriveFolder}");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LocalDirectory"]))
            {
                logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid local folder not found. Please provide one in the app settings with the tag \"LocalDirectory\"");

                configured = false;
            }

            else
            {
                AppSettingsModel.LocalFolder = ConfigurationManager.AppSettings["LocalDirectory"];

                logger.LogMessage(StandardValues.LoggerValues.Debug, $"Local Folder: {AppSettingsModel.LocalFolder}");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IgnoreFolders"]))
            {
                logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid ignore folders token not found. Please provide then in the app settings with the tag \"IgnoreFolders\" if required");
            }

            else
            {
                AppSettingsModel.IgnoreFolders = ConfigurationManager.AppSettings["IgnoreFolders"].Split(',');

                logger.LogMessage(StandardValues.LoggerValues.Debug, $"Ignore Folder(s): {string.Join(",", AppSettingsModel.IgnoreFolders)}");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IgnoreFiles"]))
            {
                logger.LogMessage(StandardValues.LoggerValues.Warning, "Valid ignore files not found. Please provide them in the app settings with the tag \"IgnoreFiles\" if required");
            }

            else
            {
                AppSettingsModel.IgnoreFiles = ConfigurationManager.AppSettings["IgnoreFiles"].Split(',');

                logger.LogMessage(StandardValues.LoggerValues.Debug, $"Ignore File(s): {string.Join(",", AppSettingsModel.IgnoreFiles)}");
            }

            return configured;
        }
    }
}
