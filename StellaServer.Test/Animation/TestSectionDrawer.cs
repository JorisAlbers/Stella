using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Test.Animation
{
    [TestFixture]
    public class TestSectionDrawer
    {
        [Test]
        public void TimeStamp_SingleDrawer_CorrectlySet()
        {
            var mockDrawer = new Mock<IDrawer>();
            DateTime dateTime = DateTime.Now;
            SectionDrawer sectionDrawer = new SectionDrawer(new IDrawer[]{mockDrawer.Object}, new DateTime[]{dateTime});
            Assert.AreEqual(dateTime,sectionDrawer.Timestamp);
        }

        [Test]
        public void TimeStamp_MultipleDrawers_CorrectlySet()
        {
            var mockDrawer = new Mock<IDrawer>();
            DateTime dateTime1 = DateTime.Now;
            DateTime dateTime2 = DateTime.Now + TimeSpan.FromMinutes(1);
            DateTime dateTime3 = DateTime.Now + TimeSpan.FromMinutes(2);
            SectionDrawer sectionDrawer = new SectionDrawer(new IDrawer[] { mockDrawer.Object,mockDrawer.Object,mockDrawer.Object }, new DateTime[] { dateTime1,dateTime2,dateTime3 });
            Assert.AreEqual(dateTime1, sectionDrawer.Timestamp);
        }

        [Test]
        public void IEnumerable_TwoDrawersAtDelayFromEachOther_CorrectlyIteratesFrames()
        {
            List<Frame> frames1 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(0, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(2, 1, 2, 3)
                },
                new Frame(2, 200)
                {
                    new PixelInstruction(3, 1, 2, 3)
                }
            };

            List<Frame> frames2 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(7, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(8, 1, 2, 3)
                },
                new Frame(2, 100)
                {
                    new PixelInstruction(9, 1, 2, 3)
                }
            };

            // EXPECTED
            Frame expectedFrame1 = new Frame(0, 0)   { frames1[0][0] };
            Frame expectedFrame2 = new Frame(1, 50)  { frames2[0][0] };
            Frame expectedFrame3 = new Frame(2, 100) { frames1[1][0] };
            Frame expectedFrame4 = new Frame(3, 150) { frames2[1][0] };


            var mockDrawer1 = new Mock<IDrawer>();
            mockDrawer1.Setup(x => x.GetEnumerator()).Returns(frames1.GetEnumerator());

            var mockDrawer2 = new Mock<IDrawer>();
            mockDrawer2.Setup(x => x.GetEnumerator()).Returns(frames2.GetEnumerator());

            DateTime dateTime1 = DateTime.Now;
            DateTime dateTime2 = dateTime1 + TimeSpan.FromMilliseconds(50); // 50ms to make sure they get out of frame
            SectionDrawer sectionDrawer = new SectionDrawer(new IDrawer[] { mockDrawer1.Object, mockDrawer2.Object }, new DateTime[] { dateTime1, dateTime2 });

            List<Frame> receivedFrames = sectionDrawer.Take(4).ToList();



            Assert.AreEqual(expectedFrame1, receivedFrames[0]);
            Assert.AreEqual(expectedFrame2, receivedFrames[1]);
            Assert.AreEqual(expectedFrame3, receivedFrames[2]);
            Assert.AreEqual(expectedFrame4, receivedFrames[3]);
        }





    }
}
