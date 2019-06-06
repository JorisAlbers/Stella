using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Serialization.Mapping;

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
            int expectedFirstSection = 30;
            int expectedSecondSection = 31;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!MappingSettings");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex}");
            stringBuilder.AppendLine($"    Length:  {expectedLength}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi}");
            stringBuilder.AppendLine($"    FirstSectionIsInverted:  {expectedFirstSectionIsInverted}");
            stringBuilder.AppendLine($"    SectionStarts:");
            stringBuilder.AppendLine($"      - {expectedFirstSection}");
            stringBuilder.AppendLine($"      - {expectedSecondSection}");


            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            List<PiMapping> mappings = loader.Load(mockStream);

            Assert.AreEqual(1,mappings.Count);
            PiMapping mapping = mappings[0];
            Assert.AreEqual(expectedPiIndex,mapping.PiIndex);
            Assert.AreEqual(expectedLength,mapping.Length);
            Assert.AreEqual(expectedStartIndexOnPi,mapping.StartIndexOnPi);
            Assert.AreEqual(expectedFirstSectionIsInverted, mapping.FirstSectionIsInverted);
            Assert.AreEqual(expectedFirstSection,  mapping.SectionStarts[0]);
            Assert.AreEqual(expectedSecondSection, mapping.SectionStarts[1]);


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
            bool expectedFirstSectionIsInverted1 = true;
            bool expectedFirstSectionIsInverted2 = false;
            int expectedFirstSection1 = 30;
            int expectedFirstSection2 = 31;
            int expectedSecondSection1 = 40;
            int expectedSecondSection2 = 41;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!MappingSettings");
            stringBuilder.AppendLine("Mappings:");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex1}");
            stringBuilder.AppendLine($"    Length:  {expectedLength1}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi1}");
            stringBuilder.AppendLine($"    FirstSectionIsInverted:  {expectedFirstSectionIsInverted1}");
            stringBuilder.AppendLine($"    SectionStarts:");
            stringBuilder.AppendLine($"      - {expectedFirstSection1}");
            stringBuilder.AppendLine($"      - {expectedSecondSection1}");
            stringBuilder.AppendLine($"  - PiIndex: {expectedPiIndex2}");
            stringBuilder.AppendLine($"    Length:  {expectedLength2}");
            stringBuilder.AppendLine($"    StartIndexOnPi:  {expectedStartIndexOnPi2}");
            stringBuilder.AppendLine($"    FirstSectionIsInverted:  {expectedFirstSectionIsInverted2}");
            stringBuilder.AppendLine($"    SectionStarts:");
            stringBuilder.AppendLine($"      - {expectedFirstSection2}");
            stringBuilder.AppendLine($"      - {expectedSecondSection2}");

            MappingLoader loader = new MappingLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            List<PiMapping> mappings = loader.Load(mockStream);

            Assert.AreEqual(2, mappings.Count);
            // Mapping 1
            PiMapping mapping1 = mappings[0];
            Assert.AreEqual(expectedPiIndex1, mapping1.PiIndex);
            Assert.AreEqual(expectedLength1, mapping1.Length);
            Assert.AreEqual(expectedStartIndexOnPi1, mapping1.StartIndexOnPi);
            Assert.AreEqual(expectedFirstSectionIsInverted1, mapping1.FirstSectionIsInverted);
            Assert.AreEqual(expectedFirstSection1,  mapping1.SectionStarts[0]);
            Assert.AreEqual(expectedSecondSection1, mapping1.SectionStarts[1]);
            // Mapping 2
            PiMapping mapping2 = mappings[1];
            Assert.AreEqual(expectedPiIndex2, mapping2.PiIndex);
            Assert.AreEqual(expectedLength2, mapping2.Length);
            Assert.AreEqual(expectedStartIndexOnPi2, mapping2.StartIndexOnPi);
            Assert.AreEqual(expectedFirstSectionIsInverted2, mapping2.FirstSectionIsInverted);
            Assert.AreEqual(expectedFirstSection2, mapping2.SectionStarts[0]);
            Assert.AreEqual(expectedSecondSection2, mapping2.SectionStarts[1]);

        }

    }
}
