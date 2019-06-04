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

        [Test]
        public void Load_StoryBoardWithRepeatingPatternsAnimation_LoadsCorrectly()
        {
            Color[][] expectedPatterns = new Color[][]
            {
                new Color[]
                {
                    Color.FromArgb(1, 2, 3),
                    Color.FromArgb(4, 5, 6)
                },
                new Color[]
                {
                    Color.FromArgb(7, 8, 9),
                    Color.FromArgb(10, 11, 12)
                }, 
                
            };

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RepeatingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.Append(    $"    Patterns: [[[{expectedPatterns[0][0].R},{expectedPatterns[0][0].G},{expectedPatterns[0][0].B}],");
            stringBuilder.Append(         $"[{expectedPatterns[0][1].R},{expectedPatterns[0][1].G},{expectedPatterns[0][1].B}]],");
            stringBuilder.Append(         $"[[{expectedPatterns[1][0].R},{expectedPatterns[1][0].G},{expectedPatterns[1][0].B}],");
            stringBuilder.Append(         $"[{expectedPatterns[1][1].R},{expectedPatterns[1][1].G},{expectedPatterns[1][1].B}]]]");


            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            RepeatingPatternAnimationSettings settings = (RepeatingPatternAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedFrameWaitMs, settings.FrameWaitMs);
            CollectionAssert.AreEqual(expectedPatterns, settings.Patterns);
        }

        [Test]
        public void Load_StoryBoardWithRandomFadeAnimation_LoadsCorrectly()
        {
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            };

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedFadeSteps = 5;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RandomFade");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    FadeSteps:  {expectedFadeSteps}");
            stringBuilder.Append(    $"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine(    $"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            RandomFadeAnimationSettings settings = (RandomFadeAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedFadeSteps, settings.FadeSteps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedFrameWaitMs, settings.FrameWaitMs);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }

        [Test]
        public void Load_StoryBoardWithFadingPulseAnimation_LoadsCorrectly()
        {
            Color expectedColor = Color.FromArgb(1, 2, 3);
            
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedFadeSteps = 5;


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !FadingPulse");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    FadeSteps:  {expectedFadeSteps}");
            stringBuilder.AppendLine($"    Color:  [{expectedColor.R},{expectedColor.G},{expectedColor.B}]");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            FadingPulseAnimationSettings settings = (FadingPulseAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedFadeSteps, settings.FadeSteps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedFrameWaitMs, settings.FrameWaitMs);
            Assert.AreEqual(expectedColor, settings.Color);
        }
    }
}
