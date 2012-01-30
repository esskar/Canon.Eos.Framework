using Canon.Eos.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Canon.Eos.UnitTests
{
    [TestClass]
    public class EosImageQualityTests
    {
        private static void AssertConversion(long expectedValue, EosImageQuality expectedQuality)
        {
            var actualQuality = EosImageQuality.Create(expectedValue);
            Assert.AreEqual(expectedQuality.PrimaryCompressLevel, actualQuality.PrimaryCompressLevel, "PrimaryCompressLevel");
            Assert.AreEqual(expectedQuality.PrimaryImageFormat, actualQuality.PrimaryImageFormat, "PrimaryImageFormat");
            Assert.AreEqual(expectedQuality.PrimaryImageSize, actualQuality.PrimaryImageSize, "PrimaryImageSize");
            Assert.AreEqual(expectedQuality.SecondaryCompressLevel, actualQuality.SecondaryCompressLevel, "SecondaryCompressLevel");
            Assert.AreEqual(expectedQuality.SecondaryImageFormat, actualQuality.SecondaryImageFormat, "SecondaryImageFormat");
            Assert.AreEqual(expectedQuality.SecondaryImageSize, actualQuality.SecondaryImageSize, "SecondaryImageSize");

            var actualValue = actualQuality.ToBitMask();
            Assert.AreEqual(expectedValue, actualValue, "ToBitMask");
        }

        [TestMethod]
        public void CheckLargeCrwTest()
        {
            AssertConversion(0x002f000f, new EosImageQuality
            {
                PrimaryCompressLevel = EosCompressLevel.Unknown,
                PrimaryImageFormat = EosImageFormat.Crw,
                PrimaryImageSize = EosImageSize.Large,
                SecondaryCompressLevel = EosCompressLevel.Unknown,
                SecondaryImageFormat = EosImageFormat.Unknown,
                SecondaryImageSize = EosImageSize.Large,
            });
        }

        [TestMethod]
        public void CheckLargeJpegTest()
        {
            AssertConversion(0x0010ff0f, new EosImageQuality
            {
                SecondaryCompressLevel = EosCompressLevel.Unknown,
                SecondaryImageFormat = EosImageFormat.Unknown,
                SecondaryImageSize = EosImageSize.Unknown,
                PrimaryCompressLevel = EosCompressLevel.JpegUncompressed,
                PrimaryImageFormat = EosImageFormat.Jpeg,
                PrimaryImageSize = EosImageSize.Large,                                                
            });
        }
    }
}
