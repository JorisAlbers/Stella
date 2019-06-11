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
            string message = "ThisIsAMessage";
            byte[] expectedBytes = Encoding.ASCII.GetBytes(message);
            int expectedNumberOfPackages = 1;
            int expectedPackageIndex = 0;

            byte[][] packages = StringProtocol.Serialize(message);

            Assert.AreEqual(1,packages.Length);
            // number of packages
            Assert.AreEqual(expectedNumberOfPackages,BitConverter.ToInt32(packages[0],0));
            Assert.AreEqual(expectedPackageIndex,BitConverter.ToInt32(packages[0],4));

            byte[] messageBuffer = new byte[expectedBytes.Length];
            Array.Copy(packages[0],8, messageBuffer,0, expectedBytes.Length);
            Assert.AreEqual(expectedBytes, messageBuffer);
        }
    }
}
