using System.Net.Mime;
using Zipper.Application;

namespace Zipper.Tests
{
    [TestClass]
    public class FileHelperTests
    {
        #region GetContentType
        [TestMethod]
        public void GetContentType_Empty_Returns_Octet()
        {
            const string input = "";
            const string expectedOutput = MediaTypeNames.Application.Octet;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void GetContentType_NoExtensions_Returns_Octet()
        {
            const string input = "fileName";
            const string expectedOutput = MediaTypeNames.Application.Octet;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void GetContentType_Xml_Returns_Xml()
        {
            const string input = "fileName.xml";
            const string expectedOutput = MediaTypeNames.Text.Xml;  //MediaTypeNames.Text.Xml;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void GetContentType_Txt_Returns_Txt()
        {
            const string input = "fileName.txt";
            const string expectedOutput = MediaTypeNames.Text.Plain;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void GetContentType_Zip_Returns_Zip()
        {
            const string input = "fileName.zip";
            const string expectedOutput = MediaTypeNames.Application.Zip;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void GetContentType_TxtZip_Returns_Zip()
        {
            const string input = "fileName.txt.zip";
            const string expectedOutput = MediaTypeNames.Application.Zip;

            string actualOutput = FileHelper.GetContentType(input);

            Assert.AreEqual(expectedOutput, actualOutput);
        }
        #endregion

        #region ValidateFileName
        [TestMethod]
        public void ValidateFileName_Empty_Throws_ArgumentNullException()
        {
            const string input = "";

            Assert.ThrowsException<ArgumentNullException>(() 
                => FileHelper.ValidateFileName(input));
        }

        [TestMethod]
        public void ValidateFileName_NULL_Throws_ArgumentNullException()
        {
            const string? input = null;

            Assert.ThrowsException<ArgumentNullException>(()
               => FileHelper.ValidateFileName(input));
        }

        [TestMethod]
        public void ValidateFileName_ContainsSlash_Throws_ArgumentException()
        {
            const string input = "someDirectory/SomeFile.txt";

            Assert.ThrowsException<ArgumentException>(()
               => FileHelper.ValidateFileName(input));
        }

        [TestMethod]
        public void ValidateFileName_ContainsBackSlash_Throws_ArgumentException()
        {
            const string input = "someDirectory\\SomeFile.txt";

            Assert.ThrowsException<ArgumentException>(()
              => FileHelper.ValidateFileName(input));
        }

        [TestMethod]
        public void ValidateFileName_NoExtension_DoesNotThrow()
        {
            const string input = "SomeFile";

            try
            {
                FileHelper.ValidateFileName(input);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
        #endregion
    }
}