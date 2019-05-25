using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StellaServer.Animation.Mapping;

namespace StellaServer.Test.Animation.Mapping
{
    [TestFixture]
    public class TestPiMaskCalculator
    {
        [Test]
        public void Calculate_SingleMappingWithoutSections_CorrectlyCreatesMask()
        {
            int expectedPiIndex = 9;
            int expectedLength = 5;
            int expectedStartIndex = 0;
            int expectedStartIndexOnPi = 500;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedStartIndex,expectedLength,expectedStartIndexOnPi,new int[]{},false)
            };
            
            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate();

            Assert.AreEqual(expectedLength, piMaskItems.Count);

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
            int expectedPiIndex = 9;
            int expectedLength = 5;
            int expectedStartIndex = 0;
            int expectedStartIndexOnPi = 500;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex,expectedStartIndex,expectedLength,expectedStartIndexOnPi,new int[]{},true)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate();

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
            int expectedPiIndex1 = 9;
            int expectedPiIndex2 = 1;
            int expectedLength = 2;
            int expectedStartIndex1 = 0;
            int expectedStartIndex2 = 0;
            int expectedStartIndexOnPi1 = 500;
            int expectedStartIndexOnPi2 = 100;

            List<PiMapping> mappings = new List<PiMapping>()
            {
                new PiMapping(expectedPiIndex1,expectedStartIndex1,expectedLength,expectedStartIndexOnPi1,new int[]{},false),
                new PiMapping(expectedPiIndex2,expectedStartIndex2,expectedLength,expectedStartIndexOnPi2, new int[]{},false)
            };

            PiMaskCalculator maskCalculator = new PiMaskCalculator(mappings);
            List<PiMaskItem> piMaskItems = maskCalculator.Calculate();

            Assert.AreEqual(4, piMaskItems.Count);

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

    }
}
