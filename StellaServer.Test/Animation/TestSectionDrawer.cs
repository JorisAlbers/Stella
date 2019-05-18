using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
    }
}
