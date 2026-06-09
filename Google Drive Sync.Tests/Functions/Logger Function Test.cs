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
                CreatedTime = new DateTime(1985, 6, 1, 10, 5, 12, DateTimeKind.Utc),
                ModifiedTime = new DateTime(1987, 9, 5, 12, 45, 0, DateTimeKind.Utc)
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
                ModifiedTime = new DateTime(1987, 9, 5, 12, 45, 0, DateTimeKind.Utc)
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
                ModifiedTime = new DateTime(1987, 9, 5, 12, 45, 0, DateTimeKind.Utc)
            };

            string result = LoggerFunction.FormatFileMetaData(testFile, "Move");

            Assert.AreEqual("\"1987-09-05T12:45:00.000Z\"", result);
        }
    }
}
