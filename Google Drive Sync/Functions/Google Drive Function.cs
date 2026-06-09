// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Functions
{
    public static class GoogleDriveFunction
    {
        /// <summary>
        /// Returns the final folder in the given string.
        /// </summary>
        public static string RemoveStringCharacters(string value, char[] characters, string direction)
        {
            string modifiedValue = value;

            if (direction == "Right")
            {
                foreach (char character in characters)
                {
                    if (modifiedValue.IndexOf(character) != -1)
                    {
                        modifiedValue = modifiedValue.Remove(0, modifiedValue.LastIndexOf(character) + 1);
                    }
                }
            }

            else
            {
                foreach (char character in characters)
                {
                    if (modifiedValue.IndexOf(character) != -1)
                    {
                        modifiedValue = modifiedValue.Remove(modifiedValue.IndexOf(character));
                    }
                }
            }

            return modifiedValue;
        }
    }
}
