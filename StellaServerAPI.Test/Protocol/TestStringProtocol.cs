using System;
using System.Text;
using NUnit.Framework;
using StellaServerAPI.Protocol;

namespace StellaServerAPI.Test.Protocol
{
    [TestFixture]
    public class TestStringProtocol
    {
        [Test]
        public void Deserialize_SmallString_ReturnsASinglePackage()
        {
            int maxPackageSize = 300;
            string message = "ThisIsAMessage";
            byte[] expectedBytes = Encoding.ASCII.GetBytes(message);
            int expectedNumberOfPackages = 1;

            byte[][] packages = StringProtocol.Serialize(message,maxPackageSize);

            Assert.AreEqual(1,packages.Length);
            // number of packages
            Assert.AreEqual(expectedNumberOfPackages,BitConverter.ToInt32(packages[0],0));

            byte[] messageBuffer = new byte[expectedBytes.Length];
            Array.Copy(packages[0],4, messageBuffer,0, expectedBytes.Length);
            Assert.AreEqual(expectedBytes, messageBuffer);
        }

        [Test]
        public void Deserialize_LargeString_ReturnsTwoPackages()
        {
            int maxPackageSize = 40;
            string message = "ThisIsALargeMessageThatDoesNotFitInASinglePackage";
            byte[] expectedBytes = Encoding.ASCII.GetBytes(message); // 49 bytes
            byte[] expectedStringData1 = new byte[36]; // 40 (max package size) -4 (header of the package)
            Array.Copy(expectedBytes, 0,expectedStringData1,0, expectedStringData1.Length);
            byte[] expectedStringData2 = new byte[13]; //  13 (49 - 36)
            Array.Copy(expectedBytes, 36, expectedStringData2, 0, expectedStringData2.Length);
            
            int expectedNumberOfPackages = 2;
            int expectedPackageIndex = 1;

            byte[][] packages = StringProtocol.Serialize(message, maxPackageSize);

            Assert.AreEqual(2, packages.Length);
            // First package
            // number of packages
            byte[] package = packages[0];
            Assert.AreEqual(expectedNumberOfPackages, BitConverter.ToInt32(package, 0));

            byte[] messageBuffer = new byte[expectedStringData1.Length];
            Array.Copy(package, 4, messageBuffer, 0, expectedStringData1.Length);
            Assert.AreEqual(expectedStringData1, messageBuffer);

            // Second package
            package = packages[1];
            Assert.AreEqual(expectedPackageIndex, BitConverter.ToInt32(package, 0));

            messageBuffer = new byte[expectedStringData2.Length];
            Array.Copy(package, 4, messageBuffer, 0, expectedStringData2.Length);
            Assert.AreEqual(expectedStringData2, messageBuffer);

        }
    }
}
