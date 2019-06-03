using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StellaServer.Animation.Mapping;
using StellaServer.Serialization.Mapping;

namespace StellaServer.Test.Serialization.Mapping
{
    [TestFixture]
    public class TestMappingLoader
    {
        [Test]
        public void Load_SingleMapping_CorrectlyLoads()
        {
            int  expectedPiIndex = 1;
            int  expectedLength = 20;
            int  expectedStartIndexOnPi = 100;
            bool expectedFirstSectionIsInverted = true;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!MappingSettings");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex}");
            stringBuilder.AppendLine($"    Length:  {expectedLength}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi}");
            stringBuilder.AppendLine($"    FirstSectionIsInverted:  {expectedFirstSectionIsInverted}");

            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            List<PiMapping> mappings = loader.Load(mockStream);

            Assert.AreEqual(1,mappings.Count);
            PiMapping mapping = mappings[0];
            Assert.AreEqual(expectedPiIndex,mapping.PiIndex);
            Assert.AreEqual(expectedLength,mapping.Length);
            Assert.AreEqual(expectedStartIndexOnPi,mapping.StartIndexOnPi);
            Assert.AreEqual(expectedFirstSectionIsInverted, mapping.FirstSectionIsInverted);

        }

    }
}
