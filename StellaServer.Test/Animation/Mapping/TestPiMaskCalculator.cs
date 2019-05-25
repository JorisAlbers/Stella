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

    }
}
