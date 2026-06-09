// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Functions;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class GoogleDriveFunctionTest
    {
        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersRightBackSlash()
        {
            string input = "Book Tests\\Test Folder\\Test Folder Two";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, ['\\'], "Right");

            Assert.AreEqual("Test Folder Two", result);
        }

        // /hecks whether the RemoveStringCharacters method returns the expected string.
        [TestMethod]
        public void TestRemoveStringCharactersRightComma()
        {
            string input = "C:\\GDSTests\\Book Tests,rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, [','], "Right");

            Assert.AreEqual("rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersRightBoth()
        {
            string input = "C:\\GDSTests\\Book Tests,Book Tests\\Test Folder\\Test Folder Two";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, [',', '\\'], "Right");

            Assert.AreEqual("Test Folder Two", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersLeft()
        {
            string input = "C:\\GDSTests\\Book Tests,Book Tests\\Test Folder\\Test Folder Two";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, [','], "Left");

            Assert.AreEqual("C:\\GDSTests\\Book Tests", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersRightNoBackSlash()
        {
            string input = "Book Tests";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, ['\\'], "Right");

            Assert.AreEqual("Book Tests", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersLeftNoBackSlash()
        {
            string input = "Book Tests";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, ['\\'], "Left");

            Assert.AreEqual("Book Tests", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersRightNoComma()
        {
            string input = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, [','], "Right");

            Assert.AreEqual("rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE", result);
        }

        /// <summary>
        /// Checks whether the RemoveStringCharacters method returns the expected string.
        /// </summary>
        [TestMethod]
        public void TestRemoveStringCharactersLeftNoComma()
        {
            string input = "rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE";
            string result = GoogleDriveFunction.RemoveStringCharacters(input, [','], "Left");

            Assert.AreEqual("rqfqjFATIC6bSvmuTxvov0BD3kVvh0UYE", result);
        }
    }
}
