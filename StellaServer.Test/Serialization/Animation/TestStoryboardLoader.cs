using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using NUnit.Framework;
using StellaServer.Animation;
using StellaServer.Serialization.Animation;

namespace StellaServer.Test.Serialization.Animation
{
    [TestFixture]
    public class TestStoryboardLoader
    {
        [Test]
        public void Load_StoryBoardWithMovingPatternAnimation_LoadsCorrectly()
        {
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            }; 

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !MovingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.Append    ($"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine($"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            MovingPatternAnimationSettings settings = (MovingPatternAnimationSettings) storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex,settings.StartIndex);
            Assert.AreEqual(expectedStripLength,settings.StripLength);
            Assert.AreEqual(expectedFrameWaitMs,settings.FrameWaitMs);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }

        [Test]
        public void Load_StoryBoardWithSlidingPatternAnimation_LoadsCorrectly()
        {
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            };

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !SlidingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.Append    ($"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine($"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            SlidingPatternAnimationSettings settings = (SlidingPatternAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedFrameWaitMs, settings.FrameWaitMs);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }
    }
}
