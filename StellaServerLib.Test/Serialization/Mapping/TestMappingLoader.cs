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

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!MappingSettings");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex}");
            stringBuilder.AppendLine($"    Length:  {expectedLength}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi}");
            stringBuilder.AppendLine($"    InverseDirection:  {inverseDirection}");



            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            List<RegionMapping> mappings = loader.Load(mockStream);

            Assert.AreEqual(1,mappings.Count);
            RegionMapping mapping = mappings[0];
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

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!MappingSettings");
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

            List<RegionMapping> mappings = loader.Load(mockStream);
            Assert.AreEqual(2, mappings.Count);

            // Mapping 1
            RegionMapping mapping1 = mappings[0];
            Assert.AreEqual(expectedPiIndex1, mapping1.PiIndex);
            Assert.AreEqual(expectedLength1, mapping1.Length);
            Assert.AreEqual(expectedStartIndexOnPi1, mapping1.StartIndexOnPi);
            Assert.AreEqual(expectedInverseDirection1, mapping1.InverseDirection);
            // Mapping 2
            RegionMapping mapping2 = mappings[1];
            Assert.AreEqual(expectedPiIndex2, mapping2.PiIndex);
            Assert.AreEqual(expectedLength2, mapping2.Length);
            Assert.AreEqual(expectedStartIndexOnPi2, mapping2.StartIndexOnPi);
            Assert.AreEqual(expectedInverseDirection2, mapping2.InverseDirection);
        }

    }
}
