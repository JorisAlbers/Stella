using System.Collections.Generic;
using NUnit.Framework;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Test.Animation.Mapping
{
    [TestFixture]
    public class TestPiMaskCalculator
    {
        [Test]
        public void Calculate_SingleMappingWithoutSections_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 0;
            int expectedLength = 5;
            int expectedStartIndexOnPi = 500;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedLength,expectedStartIndexOnPi,null,false)
            };
            
            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);

            Assert.AreEqual(expectedLength, piMaskItems.Count);
            Assert.AreEqual(1,stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength,stripLengthPerPi[0]);

            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex, item1.PiIndex);
            Assert.AreEqual(500, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex, item2.PiIndex);
            Assert.AreEqual(501, item2.PixelIndex);
            // Item 3
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex, item3.PiIndex);
            Assert.AreEqual(502, item3.PixelIndex);
            // Item 4
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex, item4.PiIndex);
            Assert.AreEqual(503, item4.PixelIndex);
            // Item 5
            PiMaskItem item5 = piMaskItems[4];
            Assert.AreEqual(expectedPiIndex, item5.PiIndex);
            Assert.AreEqual(504, item5.PixelIndex);
        }

        [Test]
        public void Calculate_SingleReversedMappingWithoutSections_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 0;
            int expectedLength = 5;
            int expectedStartIndexOnPi = 500;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedLength,expectedStartIndexOnPi,new int[]{},true)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);
            Assert.AreEqual(1, stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);

            Assert.AreEqual(expectedLength, piMaskItems.Count);

            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex, item1.PiIndex);
            Assert.AreEqual(504, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex, item2.PiIndex);
            Assert.AreEqual(503, item2.PixelIndex);
            // Item 3
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex, item3.PiIndex);
            Assert.AreEqual(502, item3.PixelIndex);
            // Item 4
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex, item4.PiIndex);
            Assert.AreEqual(501, item4.PixelIndex);
            // Item 5
            PiMaskItem item5 = piMaskItems[4];
            Assert.AreEqual(expectedPiIndex, item5.PiIndex);
            Assert.AreEqual(500, item5.PixelIndex);
        }

        [Test]
        public void Calculate_TwoForwardMappingsWithoutSections_CorrectlyCreatesMask()
        {
            int expectedPiIndex1 = 0;
            int expectedPiIndex2 = 1;
            int expectedLength = 2;
            int expectedStartIndexOnPi1 = 500;
            int expectedStartIndexOnPi2 = 100;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex1,expectedLength,expectedStartIndexOnPi1,new int[]{},false),
                new PiMapping(expectedPiIndex2,expectedLength,expectedStartIndexOnPi2, new int[]{},false)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);

            Assert.AreEqual(4, piMaskItems.Count);
            Assert.AreEqual(2, stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);

            // Mapping 1
            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex1, item1.PiIndex);
            Assert.AreEqual(500, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex1, item2.PiIndex);
            Assert.AreEqual(501, item2.PixelIndex);

            // Mapping 2
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex2, item3.PiIndex);
            Assert.AreEqual(100, item3.PixelIndex);
            // Item 2
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex2, item4.PiIndex);
            Assert.AreEqual(101, item4.PixelIndex);
        }

        [Test]
        public void Calculate_SingleMappingWithTwoSectionsFirstSectionInverted_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 0;
            int expectedLength = 5;
            int expectedStartIndexOnPi = 500;

            int[] sections = new int[] { 2 };

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedLength,expectedStartIndexOnPi,sections,true)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);

            Assert.AreEqual(expectedLength, piMaskItems.Count);
            Assert.AreEqual(1, stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);

            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex, item1.PiIndex);
            Assert.AreEqual(501, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex, item2.PiIndex);
            Assert.AreEqual(500, item2.PixelIndex);
            // Item 3
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex, item3.PiIndex);
            Assert.AreEqual(502, item3.PixelIndex);
            // Item 4
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex, item4.PiIndex);
            Assert.AreEqual(503, item4.PixelIndex);
            // Item 5
            PiMaskItem item5 = piMaskItems[4];
            Assert.AreEqual(expectedPiIndex, item5.PiIndex);
            Assert.AreEqual(504, item5.PixelIndex);
        }

        [Test]
        public void Calculate_SingleMappingWithThreeSectionsFirstSectionInverted_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 0;
            int expectedLength = 6;
            int expectedStartIndexOnPi = 500;

            int[] sections = new int[] { 2,4 };

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedLength,expectedStartIndexOnPi,sections,true)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);

            Assert.AreEqual(expectedLength, piMaskItems.Count);
            Assert.AreEqual(1, stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);

            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex, item1.PiIndex);
            Assert.AreEqual(501, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex, item2.PiIndex);
            Assert.AreEqual(500, item2.PixelIndex);
            // Item 3
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex, item3.PiIndex);
            Assert.AreEqual(502, item3.PixelIndex);
            // Item 4
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex, item4.PiIndex);
            Assert.AreEqual(503, item4.PixelIndex);
            // Item 5
            PiMaskItem item5 = piMaskItems[4];
            Assert.AreEqual(expectedPiIndex, item5.PiIndex);
            Assert.AreEqual(505, item5.PixelIndex);
            // Item 6
            PiMaskItem item6 = piMaskItems[5];
            Assert.AreEqual(expectedPiIndex, item6.PiIndex);
            Assert.AreEqual(504, item6.PixelIndex);
        }

        [Test]
        public void Calculate_SingleMappingWithThreeSectionsFirstSectionNotInverted_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 0;
            int expectedLength = 6;
            int expectedStartIndexOnPi = 500;

            int[] sections = new int[] { 2, 4 };

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedLength,expectedStartIndexOnPi,sections,false)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate(out int[] stripLengthPerPi);

            Assert.AreEqual(expectedLength, piMaskItems.Count);
            Assert.AreEqual(1, stripLengthPerPi.Length);
            Assert.AreEqual(expectedLength, stripLengthPerPi[0]);

            // Item 1
            PiMaskItem item1 = piMaskItems[0];
            Assert.AreEqual(expectedPiIndex, item1.PiIndex);
            Assert.AreEqual(500, item1.PixelIndex);
            // Item 2
            PiMaskItem item2 = piMaskItems[1];
            Assert.AreEqual(expectedPiIndex, item2.PiIndex);
            Assert.AreEqual(501, item2.PixelIndex);
            // Item 3
            PiMaskItem item3 = piMaskItems[2];
            Assert.AreEqual(expectedPiIndex, item3.PiIndex);
            Assert.AreEqual(503, item3.PixelIndex);
            // Item 4
            PiMaskItem item4 = piMaskItems[3];
            Assert.AreEqual(expectedPiIndex, item4.PiIndex);
            Assert.AreEqual(502, item4.PixelIndex);
            // Item 5
            PiMaskItem item5 = piMaskItems[4];
            Assert.AreEqual(expectedPiIndex, item5.PiIndex);
            Assert.AreEqual(504, item5.PixelIndex);
            // Item 6
            PiMaskItem item6 = piMaskItems[5];
            Assert.AreEqual(expectedPiIndex, item6.PiIndex);
            Assert.AreEqual(505, item6.PixelIndex);
        }

    }
}
