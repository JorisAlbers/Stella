using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation;

namespace StellaServer.Test.Animators
{
    [TestFixture]
    public class TestAnimationExpander
    {
        [Test]
        public void Expand_Frames_CorrectlyExpandsFrames()
        {
            List<Frame> frames = new List<Frame>();
            frames.Add(new Frame(0,0));
            frames.Add(new Frame(1,100));
            frames.Add(new Frame(2,200));

            AnimationExpander expander = new AnimationExpander(frames);
            List<Frame> expandedFrames = expander.Expand(5); 
            Assert.AreEqual(15, expandedFrames.Count);
            // Check indexes
            int[] expectedIndexes = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
            int[] expectedTimestampRelatives = new int[] {0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400};
            CollectionAssert.AreEqual(expectedIndexes, expandedFrames.Select(x=>x.Index).ToArray(), "Indexes are incorrect");
            CollectionAssert.AreEqual(expectedTimestampRelatives, expandedFrames.Select(x=>x.TimeStampRelative).ToArray(), "Timestamps are incorrect");
        }

    }
}
