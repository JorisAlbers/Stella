using System;
using Moq;
using NUnit.Framework;
using StellaClient.Time;

namespace StellaClient.Test.Time
{
    [TestFixture]
    public class TestTimeSetter
    {
        [Test]
        public void AddMeasurements_initialMeasurements_setsTime()
        {
            // CS  SS  CR
            // -20     now  | latency = 10 min
            //     -60      | offset  = 60 - 10 = 50 min terug in de tijd
            long clientSendTime =  DateTime.Now.Ticks - TimeSpan.FromMinutes(20).Ticks;
            long serverSendTime =  DateTime.Now.Ticks - TimeSpan.FromMinutes(60).Ticks;
            long expectedTimeDelta = 0 - TimeSpan.FromMinutes(50).Ticks;

            var mock = new Mock<ISystemTimeSetter>();

            double time = 0;
            mock.Setup(x=> x.AdjustTimeWithTicks(It.IsAny<long>())).Callback<long>((t) =>
            {
                time = t;
            });

            TimeSetter timeSetter = new TimeSetter(mock.Object);
            timeSetter.AddMeasurements(clientSendTime,serverSendTime);

            Assert.IsTrue(time != 0);
            Assert.IsTrue(time - expectedTimeDelta < TimeSpan.FromMilliseconds(500).Ticks, "Time delta is off"); // Margin of 500 miliseconds
            Assert.IsTrue(expectedTimeDelta - time < TimeSpan.FromMilliseconds(500).Ticks, "Time delta is off"); // Margin of 500 miliseconds
        }
        
    }
}