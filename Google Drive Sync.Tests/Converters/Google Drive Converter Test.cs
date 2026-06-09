// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class GoogleDriveConverterTest
    {
        #region GetMimeType

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeText()
        {
            string expected = "text/plain";

            string actual = GoogleDriveConverter.GetMimeType(".txt");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypePDF()
        {
            string expected = "application/pdf";

            string actual = GoogleDriveConverter.GetMimeType(".pdf");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeDoc()
        {
            string expected = "application/msword";

            string actual = GoogleDriveConverter.GetMimeType(".doc");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeDocx()
        {
            string expected = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            string actual = GoogleDriveConverter.GetMimeType(".docx");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeXLS()
        {
            string expected = "application/vnd.ms-excel";

            string actual = GoogleDriveConverter.GetMimeType(".xls");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeXLSX()
        {
            string expected = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            string actual = GoogleDriveConverter.GetMimeType(".xlsx");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypePNG()
        {
            string expected = "image/png";

            string actual = GoogleDriveConverter.GetMimeType(".png");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeJPEG()
        {
            string expected = "image/jpeg";

            string actual = GoogleDriveConverter.GetMimeType(".jpeg");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks whether the GetMimeType method returns the correct value with the given input.
        /// </summary>
        [TestMethod]
        public void TestGetMimeTypeDefault()
        {
            string expected = "application/octet-stream";

            string actual = GoogleDriveConverter.GetMimeType(".xml");

            Assert.AreEqual(expected, actual);
        }

        #endregion

        /// <summary>
        /// Checks whether the GetFilePath method returns the expected file location. 
        /// </summary>
        [TestMethod]
        public void TestGetFilePath()
        {
            string expected = @"C:\GDSTests\Book Tests\Test.txt";

            string actual = GoogleDriveConverter.GetFilePath(@"C:\GDSTests", "Book Tests", "Test.txt");

            Assert.AreEqual(expected, actual);
        }
    }
}
