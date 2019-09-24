using System.Drawing;
using System.IO;
using System.Text;
using NUnit.Framework;
using SharpYaml.Serialization;
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
            int expectedTimeUnitsPerFrame = 30;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !MovingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.Append    ($"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine($"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            MovingPatternAnimationSettings settings = (MovingPatternAnimationSettings) storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex,settings.StartIndex);
            Assert.AreEqual(expectedStripLength,settings.StripLength);
            Assert.AreEqual(expectedTimeUnitsPerFrame,settings.TimeUnitsPerFrame);
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
            int expectedTimeUnitsPerFrame = 30;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !SlidingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.Append    ($"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine($"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            SlidingPatternAnimationSettings settings = (SlidingPatternAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedTimeUnitsPerFrame, settings.TimeUnitsPerFrame);
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
            int expectedTimeUnitsPerFrame = 30;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RepeatingPattern");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.Append(    $"    Patterns: [[[{expectedPatterns[0][0].R},{expectedPatterns[0][0].G},{expectedPatterns[0][0].B}],");
            stringBuilder.Append(         $"[{expectedPatterns[0][1].R},{expectedPatterns[0][1].G},{expectedPatterns[0][1].B}]],");
            stringBuilder.Append(         $"[[{expectedPatterns[1][0].R},{expectedPatterns[1][0].G},{expectedPatterns[1][0].B}],");
            stringBuilder.Append(         $"[{expectedPatterns[1][1].R},{expectedPatterns[1][1].G},{expectedPatterns[1][1].B}]]]");


            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            RepeatingPatternAnimationSettings settings = (RepeatingPatternAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedTimeUnitsPerFrame, settings.TimeUnitsPerFrame);
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
            int expectedTimeUnitsPerFrame = 30;
            int expectedFadeSteps = 5;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !RandomFade");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"    FadeSteps:  {expectedFadeSteps}");
            stringBuilder.Append(    $"    Pattern: [[{expectedPattern[0].R},{expectedPattern[0].G},{expectedPattern[0].B}],");
            stringBuilder.AppendLine(    $"[{expectedPattern[1].R},{expectedPattern[1].G},{expectedPattern[1].B}]]");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            RandomFadeAnimationSettings settings = (RandomFadeAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedFadeSteps, settings.FadeSteps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedTimeUnitsPerFrame, settings.TimeUnitsPerFrame);
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
            int expectedTimeUnitsPerFrame = 30;
            int expectedFadeSteps = 5;
            int expectedRelativeStart = 99;
            

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !FadingPulse");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"    FadeSteps:  {expectedFadeSteps}");
            stringBuilder.AppendLine($"    Color:  [{expectedColor.R},{expectedColor.G},{expectedColor.B}]");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            FadingPulseAnimationSettings settings = (FadingPulseAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedFadeSteps, settings.FadeSteps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedTimeUnitsPerFrame, settings.TimeUnitsPerFrame);
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            Assert.AreEqual(expectedColor, settings.Color);
        }

        [Test]
        public void Load_StoryBoardWithBitmapAnimation_LoadsCorrectly()
        {
            string expectedName = "Storyboard name";
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedTimeUnitsPerFrame = 30;
            int expectedRelativeStart = 99;
            bool expectedWraps = true;
            string expectedImageName = "fakeImage"; 


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!Storyboard");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Animations:");
            stringBuilder.AppendLine($"  - !Bitmap");
            stringBuilder.AppendLine($"    StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"    StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  {expectedTimeUnitsPerFrame}");
            stringBuilder.AppendLine($"    RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"    ImageName:  {expectedImageName}");
            stringBuilder.AppendLine($"    Wraps:  {expectedWraps}");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(1, storyboard.AnimationSettings.Length);
            BitmapAnimationSettings settings = (BitmapAnimationSettings)storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedImageName, settings.ImageName);
            Assert.AreEqual(expectedWraps, settings.Wraps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            Assert.AreEqual(expectedTimeUnitsPerFrame, settings.TimeUnitsPerFrame);
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
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  1");
            stringBuilder.AppendLine($"    FadeSteps:  1");
            stringBuilder.AppendLine($"    Color:  [1,1,1]");
            stringBuilder.AppendLine("  - !MovingPattern");
            stringBuilder.AppendLine($"    StartIndex:  2");
            stringBuilder.AppendLine($"    StripLength:  2");
            stringBuilder.AppendLine($"    TimeUnitsPerFrame:  2");
            stringBuilder.AppendLine($"    Pattern: [[2,2,2]]");

            StoryboardSerializer serializer = new StoryboardSerializer();

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            Storyboard storyboard = serializer.Load(mockStream);

            Assert.AreEqual(2, storyboard.AnimationSettings.Length);
        }

        [Test]
        public void Save_StoryBoardWithMultipleAnimations_SavesCorrectly()
        {
            string expectedName = "CoolName";
            int expectedFadeSteps = 4;
            int expectedTimeUnitsPerFrame = 30;


            Storyboard storyboard = new Storyboard();
            storyboard.Name = expectedName;
            storyboard.AnimationSettings = new IAnimationSettings[]
            {
                new FadingPulseAnimationSettings
                {
                    StartIndex = 1,
                    StripLength = 2,
                    TimeUnitsPerFrame = 3,
                    FadeSteps = 4,
                    Color = Color.FromArgb(1,1,1)
                },
                new MovingPatternAnimationSettings
                {
                    StartIndex = 10,
                    StripLength = 20,
                    TimeUnitsPerFrame = 30,
                    Pattern = new Color[]{
                        Color.FromArgb(2,2,2)
                    }
                }
            };

            StoryboardSerializer serializer = new StoryboardSerializer();
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            serializer.Save(storyboard, streamWriter);
            streamWriter.Flush();
            string s = Encoding.UTF8.GetString(memoryStream.ToArray());

            // Test by deserializing
            SerializerSettings settings = new SerializerSettings();
            settings.EmitDefaultValues = true;
            settings.RegisterAssembly(typeof(Storyboard).Assembly);
            Serializer yamlSerializer = new Serializer(settings);
            Storyboard returnStoryboard = (Storyboard) yamlSerializer.Deserialize(s, typeof(Storyboard));

            Assert.AreEqual(2, returnStoryboard.AnimationSettings.Length);
            Assert.AreEqual(expectedName, returnStoryboard.Name);
            Assert.AreEqual(expectedFadeSteps, ((FadingPulseAnimationSettings)returnStoryboard.AnimationSettings[0]).FadeSteps);
            Assert.AreEqual(expectedTimeUnitsPerFrame, ((MovingPatternAnimationSettings)returnStoryboard.AnimationSettings[1]).TimeUnitsPerFrame);
        }
    }
}
