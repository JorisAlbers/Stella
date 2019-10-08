using System;
using System.Collections.Generic;
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


            PlayListSerializer serializer = new PlayListSerializer(new List<Storyboard>{storyboard});

            StreamReader mockStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(stringBuilder.ToString())));

            PlayList playList = serializer.Load(mockStream);

            Assert.AreEqual(1, playList.Items.Length);
            Assert.AreEqual(expectedName, playList.Name);
            
            Assert.AreEqual(storyboard, playList.Items[0].Storyboard);
            Assert.AreEqual(expectedStoryboardDuration, playList.Items[0].Duration);
        }

    }
}
