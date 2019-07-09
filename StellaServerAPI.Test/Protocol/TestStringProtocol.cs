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
        public void Serialize_SmallString_ReturnsASinglePackage()
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
        public void Serialize_LargeString_ReturnsTwoPackages()
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
            Array.Copy(package, 4, messageBuffer, 0, package.Length - 4);
            Assert.AreEqual(expectedStringData1, messageBuffer);

            // Second package
            package = packages[1];
            Assert.AreEqual(expectedPackageIndex, BitConverter.ToInt32(package, 0));

            messageBuffer = new byte[expectedStringData2.Length];
            Array.Copy(package, 4, messageBuffer, 0, package.Length - 4);
            Assert.AreEqual(expectedStringData2, messageBuffer);

            // Check the full message
            messageBuffer = new byte[expectedStringData1.Length + expectedStringData2.Length];
            Array.Copy(packages[0], 4, messageBuffer, 0, packages[0].Length - 4);
            Array.Copy(packages[1], 4, messageBuffer, packages[0].Length - 4, packages[1].Length -4);
            Assert.AreEqual(message, Encoding.ASCII.GetString(messageBuffer));
        }

        [Test]
        public void Deserialize_SinglePackage_ReturnsCorrectString()
        {
            string expectedMessage = "ThisIsAMessage";
            byte[] stringAsBytes = Encoding.ASCII.GetBytes(expectedMessage); // 49 bytes
            byte[] package = new byte[stringAsBytes.Length + 4];
            BitConverter.GetBytes(1).CopyTo(package,0);
            Array.Copy(stringAsBytes,0,package,4, stringAsBytes.Length);

            StringProtocol stringProtocol = new StringProtocol();
            bool completed = stringProtocol.TryDeserialize(package, out string message);
            Assert.IsTrue(completed);
            Assert.AreEqual(expectedMessage,message);
        }

        [Test]
        public void Deserialize_TwoPackages_ReturnsCorrectString()
        {
            string expectedMessage = "ThisIsALargeMessageThatDoesNotFitInASinglePackage";
            byte[] expectedBytes = Encoding.ASCII.GetBytes(expectedMessage); // 49 bytes
            // Package 1
            byte[] package1 = new byte[40]; // 36 (data) + 4 (header of the package)
            BitConverter.GetBytes(2).CopyTo(package1,0); // Number of frames
            Array.Copy(expectedBytes, 0, package1, 4, 36);

            // Package 2
            byte[] package2 = new byte[17]; //  13 (data) + 4 (header of the package)
            BitConverter.GetBytes(1).CopyTo(package2, 0); // Frame index
            Array.Copy(expectedBytes, 36, package2, 4, 13);

            StringProtocol stringProtocol = new StringProtocol();
            bool completed = stringProtocol.TryDeserialize(package1, out string message);
            Assert.IsFalse(completed);
            Assert.IsNull(message);
            completed = stringProtocol.TryDeserialize(package2, out message);
            Assert.IsTrue(completed);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}
