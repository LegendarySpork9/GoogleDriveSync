// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Converters;

namespace GoogleDriveSync.Tests.Converters
{
    [TestClass]
    public class LocalDriveConverterTest
    {
        /// <summary>
        /// Checks whether the GetObjectName method returns the expected file name.
        /// </summary>
        [TestMethod]
        public void TestGetObjectName()
        {
            string expected = "Test.txt";

            string actual = LocalDriveConverter.GetObjectName(@"C:\GDSTests\Book Tests\Test.txt");

            Assert.AreEqual(expected, actual);
        }
    }
}
