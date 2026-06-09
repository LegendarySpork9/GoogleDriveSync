// Copyright © - 16/03/2026 - Toby Hunter
using GoogleDriveSync.Abstractions;
using System.Windows.Forms;

namespace GoogleDriveSync.Implementations
{
    public class MessageBoxWrapper : IUserNotifier
    {
        /// <summary>
        /// Displays a message box to the user with a warning.
        /// </summary>
        public void ShowMessage(string message, string title) => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
