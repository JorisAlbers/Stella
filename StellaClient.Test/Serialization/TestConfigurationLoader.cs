﻿using System.IO;
using System.Text;
using NUnit.Framework;
using StellaClientLib.Serialization;

namespace StellaClient.Test.Serialization
{
    [TestFixture]
    public class TestConfigurationLoader
    {
        [Test]
        public void Load_Configuration_LoadCorrectly()
        {
            int expectedId = 1;
            string expectedIp = "192.168.1.11";
            int expectedPort = 20060;
            int expectedUdpPort = 20065;
            int expectedLedCount = 1200;
            int expectedPwmPin = 18;
            byte expectedBrightness = 250;
            int expectedDmaChannel = 10;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Configuration");
            stringBuilder.AppendLine($"Id: {expectedId}");
            stringBuilder.AppendLine($"Ip: {expectedIp}");
            stringBuilder.AppendLine($"Port: {expectedPort}");
            stringBuilder.AppendLine($"UdpPort: {expectedUdpPort}");
            stringBuilder.AppendLine($"LedCount: {expectedLedCount}");
            stringBuilder.AppendLine($"PwmPin: {expectedPwmPin}");
            stringBuilder.AppendLine($"Brightness: {expectedBrightness}");
            stringBuilder.AppendLine($"DmaChannel: {expectedDmaChannel}");

            ConfigurationLoader loader = new ConfigurationLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Configuration configuration = loader.Load(mockStream);

            Assert.AreEqual(expectedId, configuration.Id);
            Assert.AreEqual(expectedIp, configuration.Ip);
            Assert.AreEqual(expectedPort, configuration.Port);
            Assert.AreEqual(expectedLedCount, configuration.LedCount);
            Assert.AreEqual(expectedPwmPin, configuration.PwmPin);
            Assert.AreEqual(expectedBrightness, configuration.Brightness);
            Assert.AreEqual(expectedDmaChannel, configuration.DmaChannel);

        }
    }
}
