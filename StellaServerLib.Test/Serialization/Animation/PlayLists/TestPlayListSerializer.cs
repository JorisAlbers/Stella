using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using NUnit.Framework;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Test.Serialization.Animation.PlayLists
{
    public class TestPlayListSerializer
    {
        [Test]
        public void Deserialize_SingleStoryboard_CorrectlyDeserializes()
        {
            string expectedName = "PlayList name";
            string expectedStoryboardName = "Storyboard name";
            int expectedStoryboardDuration = 999;

            Storyboard storyboard = new Storyboard();
            storyboard.Name = expectedStoryboardName;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!PlayList");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Storyboards:");
            stringBuilder.AppendLine($"  - Name:  {expectedStoryboardName}");
            stringBuilder.AppendLine($"    Duration:  {expectedStoryboardDuration}");


            PlayListSerializer serializer = new PlayListSerializer(new List<Storyboard> {storyboard});

            StreamReader mockStream =
                new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            PlayList playList = serializer.Load(mockStream);

            Assert.AreEqual(1, playList.Items.Length);
            Assert.AreEqual(expectedName, playList.Name);

            Assert.AreEqual(storyboard, playList.Items[0].Storyboard);
            Assert.AreEqual(expectedStoryboardDuration, playList.Items[0].Duration);
        }

        [Test]
        public void Deserialize_SingleStoryboardDefinedByAnimationSettings_CorrectlyDeserializes()
        {
            string expectedName = "PlayList name";
            int expectedStoryboardDuration = 999;
            // storyboard
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedRelativeStart = 99;
            bool expectedWraps = true;
            string expectedImageName = "fakeImage";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!PlayList");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Storyboards:");
            stringBuilder.AppendLine($"   - Duration:  {expectedStoryboardDuration}");
            stringBuilder.AppendLine("     Animations:");
            stringBuilder.AppendLine($"        - !Bitmap");
            stringBuilder.AppendLine($"           StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"           StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"           RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"           ImageName:  {expectedImageName}");
            stringBuilder.AppendLine($"           Wraps:  {expectedWraps}");



            PlayListSerializer serializer = new PlayListSerializer(new List<Storyboard>());

            StreamReader mockStream =
                new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            PlayList playList = serializer.Load(mockStream);

            Assert.AreEqual(1, playList.Items.Length);
            Assert.AreEqual(expectedName, playList.Name);
            Assert.AreEqual(expectedStoryboardDuration, playList.Items[0].Duration);

            Assert.AreEqual(1, playList.Items[0].Storyboard.AnimationSettings.Length);
            BitmapAnimationSettings settings =
                (BitmapAnimationSettings) playList.Items[0].Storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedImageName, settings.ImageName);
            Assert.AreEqual(expectedWraps, settings.Wraps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
        }

        [Test]
        public void Deserialize_TwoStoryboardDefinedByNameAndBySettings_CorrectlyDeserializes()
        {
            string expectedName = "PlayList name";
            // By setttings
            int expectedStoryboardDuration1 = 999;
            int expectedStartIndex = 10;
            int expectedStripLength = 20;
            int expectedRelativeStart = 99;
            bool expectedWraps = true;
            string expectedImageName = "fakeImage";
            // By name
            int expectedStoryboardDuration2 = 888;
            string expectedStoryboardName = "storyboardName";

            Storyboard expectedStoryboard = new Storyboard(){Name= expectedStoryboardName};

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("!PlayList");
            stringBuilder.AppendLine($"Name: {expectedName}");
            stringBuilder.AppendLine("Storyboards:");
            stringBuilder.AppendLine($"   - Duration:  {expectedStoryboardDuration1}");
            stringBuilder.AppendLine("     Animations:");
            stringBuilder.AppendLine($"        - !Bitmap");
            stringBuilder.AppendLine($"           StartIndex:  {expectedStartIndex}");
            stringBuilder.AppendLine($"           StripLength:  {expectedStripLength}");
            stringBuilder.AppendLine($"           RelativeStart:  {expectedRelativeStart}");
            stringBuilder.AppendLine($"           ImageName:  {expectedImageName}");
            stringBuilder.AppendLine($"           Wraps:  {expectedWraps}");
            stringBuilder.AppendLine($"   - Duration:  {expectedStoryboardDuration2}");
            stringBuilder.AppendLine($"     Name:     {expectedStoryboardName}");
            

            PlayListSerializer serializer = new PlayListSerializer(new List<Storyboard>(){expectedStoryboard});

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            PlayList playList = serializer.Load(mockStream);

            Assert.AreEqual(2, playList.Items.Length);
            Assert.AreEqual(expectedName, playList.Name);
            Assert.AreEqual(expectedStoryboardDuration1, playList.Items[0].Duration);
            
            Assert.AreEqual(1, playList.Items[0].Storyboard.AnimationSettings.Length);
            // By settings
            BitmapAnimationSettings settings = (BitmapAnimationSettings)playList.Items[0].Storyboard.AnimationSettings[0];
            Assert.AreEqual(expectedStartIndex, settings.StartIndex);
            Assert.AreEqual(expectedImageName, settings.ImageName);
            Assert.AreEqual(expectedWraps, settings.Wraps);
            Assert.AreEqual(expectedStripLength, settings.StripLength);
            Assert.AreEqual(expectedRelativeStart, settings.RelativeStart);
            // By name
            Assert.AreEqual(expectedStoryboard, playList.Items[1].Storyboard );
        }
    }
}
