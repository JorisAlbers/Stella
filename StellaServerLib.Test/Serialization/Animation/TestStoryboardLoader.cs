﻿using System.Drawing;
using System.IO;
using System.Text;
using NUnit.Framework;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Test.Serialization.Animation
{
    [TestFixture]
    public class TestStoryboardLoader
    {
        [Test]
        public void Load_StoryBoardWithMovingPatternAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            }; 

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !MovingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
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
            Assert.AreEqual(expectedRelativeStart,settings.RelativeStart);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }

        [Test]
        public void Load_StoryBoardWithSlidingPatternAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            };

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !SlidingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
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
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }

        [Test]
        public void Load_StoryBoardWithRepeatingPatternsAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
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
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RepeatingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
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
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            CollectionAssert.AreEqual(expectedPatterns, settings.Patterns);
        }

        [Test]
        public void Load_StoryBoardWithRandomFadeAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            Color[] expectedPattern = new Color[]
            {
                Color.FromArgb(1, 2, 3),
                Color.FromArgb(4, 5, 6)
            };

            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedFadeSteps = 5;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RandomFade");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
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
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            CollectionAssert.AreEqual(expectedPattern, settings.Pattern);
        }

        [Test]
        public void Load_StoryBoardWithFadingPulseAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            Color expectedColor = Color.FromArgb(1, 2, 3);
            
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedFadeSteps = 5;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !FadingPulse");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
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
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            Assert.AreEqual(expectedColor, settings.Color);
        }

        [Test]
        public void Load_StoryBoardWithBitmapAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedFrameWaitMs = 30;
            int expectedRelativeStart = 99;
            string expectedImagePath = Path.Combine(TestContext.CurrentContext.TestDirectory,"TestData","TestStoryBoardLoader","fakeImage.png"); 


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !Bitmap");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    FrameWaitMs:  {expectedFrameWaitMs}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"    ImagePath:  {expectedImagePath}");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            BitmapAnimationSettings settings = (BitmapAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedImagePath, settings.ImagePath);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            Assert.AreEqual(expectedFrameWaitMs, settings.FrameWaitMs);
        }

        [Test]
        public void Load_StoryBoardWithMultipleAnimations_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !FadingPulse");
            stringBuilder.AppendLine($"    StartIndex:  1");
            stringBuilder.AppendLine($"    StripLength:  1");
            stringBuilder.AppendLine($"    FrameWaitMs:  1");
            stringBuilder.AppendLine($"    FadeSteps:  1");
            stringBuilder.AppendLine($"    Color:  [1,1,1]");
            stringBuilder.AppendLine("  - !MovingPattern");
            stringBuilder.AppendLine($"    StartIndex:  2");
            stringBuilder.AppendLine($"    StripLength:  2");
            stringBuilder.AppendLine($"    FrameWaitMs:  2");
            stringBuilder.AppendLine($"    Pattern: [[2,2,2]]");

            StoryboardLoader loader = new StoryboardLoader();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = loader.Load(mockStream);

            Assert.AreEqual(2, storyboard.AnimationSettings.Length);
        }
    }
}
