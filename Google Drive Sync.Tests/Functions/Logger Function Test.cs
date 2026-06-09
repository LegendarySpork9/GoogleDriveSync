// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Functions;
using File = Google.Apis.Drive.v3.Data.File;

namespace GoogleDriveSync.Tests.Functions
{
    [TestClass]
    public class LoggerFunctionTest
    {
        /// <summary>
        /// Checks whether the FormatFileMetaData method returns the expected string values.
        /// </summary>
        [TestMethod]
        public void TestFileMetaDataCreate()
        {
            File testFile = new()
            {
                Name = "Test.txt",
                Parents = new List<string> { "q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0" },
                CreatedTime = DateTime.Parse("01/06/1985 11:05:12"),
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            };

            string[] result = LoggerFunction.FormatFileMetaData(testFile, "Create").Split(',');

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("\"Test.txt\"", result[0].Trim());
            Assert.AreEqual("\"q2iEH0smrmudiaBCzkQkn2lbrGqKGL2M0\"", result[1].Trim());
            Assert.AreEqual("\"1985-06-01T10:05:12.000Z\"", result[2].Trim());
            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result[3].Trim());
        }

        /// <summary>
        /// Checks whether the FormatFileMetaData method returns the expected string values.
        /// </summary>
        [TestMethod]
        public void TestFileMetaDataUpdate()
        {
            File testFile = new()
            {
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            };

            string result = LoggerFunction.FormatFileMetaData(testFile, "Update");

            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result);
        }

        /// <summary>
        /// Checks whether the FormatFileMetaData method returns the expected string values.
        /// </summary>
        [TestMethod]
        public void TestFileMetaDataMove()
        {
            File testFile = new()
            {
                ModifiedTime = DateTime.Parse("05/09/1987 13:45:00")
            };

            string result = LoggerFunction.FormatFileMetaData(testFile, "Move");

            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result);
        }
    }
}
