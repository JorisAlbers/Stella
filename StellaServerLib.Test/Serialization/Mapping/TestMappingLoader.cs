using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Serialization.Mapping;

namespace StellaServerLib.Test.Serialization.Mapping
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
            bool inverseDirection = true;
            string expectedMac = "04:e9:e5:0b:f0:fa";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Mappings");
            stringBuilder.AppendLine("Clients:");
            stringBuilder.AppendLine($"  - Index: {expectedPiIndex}");
            stringBuilder.AppendLine($"    Mac:  {expectedMac}");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex}");
            stringBuilder.AppendLine($"    Length:  {expectedLength}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi}");
            stringBuilder.AppendLine($"    InverseDirection:  {inverseDirection}");



            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            var mappings = loader.Load(mockStream);

            Assert.AreEqual(1, mappings.ClientMappings.Count);
            Assert.AreEqual(expectedMac, mappings.ClientMappings[0].Mac);
            Assert.AreEqual(expectedPiIndex, mappings.ClientMappings[0].Index);

            Assert.AreEqual(1,mappings.RegionMappings.Count);
            RegionMapping mapping = mappings.RegionMappings[0];
            Assert.AreEqual(expectedPiIndex,mapping.PiIndex);
            Assert.AreEqual(expectedLength,mapping.Length);
            Assert.AreEqual(expectedStartIndexOnPi,mapping.StartIndexOnPi);
            Assert.AreEqual(inverseDirection, mapping.InverseDirection);
        }

        [Test]
        public void Load_TwoMappings_CorrectlyLoads()
        {
            int expectedPiIndex1 = 1;
            int expectedPiIndex2 = 2;
            int expectedLength1 = 11;
            int expectedLength2 = 22;
            int expectedStartIndexOnPi1 = 111;
            int expectedStartIndexOnPi2 = 222;
            bool expectedInverseDirection1 = true;
            bool expectedInverseDirection2 = false;
            string expectedMac1 = "04:e9:e5:0b:f0:fa";
            string expectedMac2 = "05:e9:e5:0b:f0:fa";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Mappings");
            stringBuilder.AppendLine("Clients:");
            stringBuilder.AppendLine($"  - Index: {expectedPiIndex1}");
            stringBuilder.AppendLine($"    Mac:  {expectedMac1}");
            stringBuilder.AppendLine($"  - Index: {expectedPiIndex2}");
            stringBuilder.AppendLine($"    Mac:  {expectedMac2}");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex1}");
            stringBuilder.AppendLine($"    Length:  {expectedLength1}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi1}");
            stringBuilder.AppendLine($"    InverseDirection:  {expectedInverseDirection1}");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex2}");
            stringBuilder.AppendLine($"    Length:  {expectedLength2}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi2}");
            stringBuilder.AppendLine($"    InverseDirection:  {expectedInverseDirection2}");


            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            var mappings = loader.Load(mockStream);
            Assert.AreEqual(2, mappings.RegionMappings.Count);

            // Mapping 1
            RegionMapping mapping1 = mappings.RegionMappings[0];
            Assert.AreEqual(expectedPiIndex1, mapping1.PiIndex);
            Assert.AreEqual(expectedLength1, mapping1.Length);
            Assert.AreEqual(expectedStartIndexOnPi1, mapping1.StartIndexOnPi);
            Assert.AreEqual(expectedInverseDirection1, mapping1.InverseDirection);
            // Mapping 2
            RegionMapping mapping2 = mappings.RegionMappings[1];
            Assert.AreEqual(expectedPiIndex2, mapping2.PiIndex);
            Assert.AreEqual(expectedLength2, mapping2.Length);
            Assert.AreEqual(expectedStartIndexOnPi2, mapping2.StartIndexOnPi);
            Assert.AreEqual(expectedInverseDirection2, mapping2.InverseDirection);
        }

    }
}
