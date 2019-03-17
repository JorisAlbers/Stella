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
            long now = DateTime.Now.Ticks;
            // CS  SS  CR
            // -20     now  | latency = 10 min
            //     -60      | offset  = 60 - 10 = 50 min terug in de tijd
            long clientSendTime =  now - TimeSpan.FromMinutes(20).Ticks;
            long serverSendTime =  now - TimeSpan.FromMinutes(60).Ticks;
            long clientReceivedTime = now;
            long expectedTimeDelta = 0 - TimeSpan.FromMinutes(50).Ticks;

            var mock = new Mock<ISystemTimeSetter>();

            double time = 0;
            mock.Setup(x=> x.AdjustTimeWithTicks(It.IsAny<long>())).Callback<long>((t) =>
            {
                time = t;
            });

            TimeSetter timeSetter = new TimeSetter(mock.Object,3);
            timeSetter.AddMeasurements(clientSendTime,serverSendTime,clientReceivedTime);

            Assert.AreEqual(expectedTimeDelta,time);
        }

        [Test]
        public void AddMeasurements_AllMeasurements_setsTime()
        {
            long now = DateTime.Now.Ticks;

            // CS  SS  CR
            // -20     now  | latency = 10 min
            //     -60      | offset  = 60 - 10 = 50 min terug in de tijd
            long clientSendTime1 =  now - TimeSpan.FromSeconds(20).Ticks;
            long serverSendTime1 =  now - TimeSpan.FromSeconds(60).Ticks;
            long clientReceivedTime1 = now;

            // Second measurement
            // CS  SS  CR
            // -10     now  | latency = 5 min
            //     -70      | offset  = 70 - 5 = 65 min terug in de tijd
            long clientSendTime2 =  now - TimeSpan.FromSeconds(10).Ticks;
            long serverSendTime2 =  now - TimeSpan.FromSeconds(70).Ticks;
            long clientReceivedTime2 = now;


            // Third and final measurement
            // CS  SS  CR
            // -10     now       | latency = 5 min
            //     -100000         | outlier, should be removed
            long clientSendTime3 =  now - TimeSpan.FromSeconds(10).Ticks;
            long serverSendTime3 =  now - TimeSpan.FromSeconds(10000).Ticks;
            long clientReceivedTime3 = now;

            var mock = new Mock<ISystemTimeSetter>();

            double time = 0;
            mock.Setup(x=> x.AdjustTimeWithTicks(It.IsAny<long>())).Callback<long>((t) =>
            {
                time = t;

            });

            TimeSetter timeSetter = new TimeSetter(mock.Object,3);
            timeSetter.AddMeasurements(0,0,0); // Set the first measurements which is not used for the final time adjustment
            timeSetter.AddMeasurements(clientSendTime1,serverSendTime1,clientReceivedTime1); // Second measurement
            timeSetter.AddMeasurements(clientSendTime2,serverSendTime2,clientReceivedTime2); // Third measurement
            timeSetter.AddMeasurements(clientSendTime3,serverSendTime3,clientReceivedTime3); // Final measurement
            
            // The median of the deltas (50 , 65) = 57.5.
            long expectedTimeDelta = 0 - (TimeSpan.FromSeconds(57).Ticks + TimeSpan.FromMilliseconds(500).Ticks);
            Assert.AreEqual(expectedTimeDelta, time);
        }
        
    }
}