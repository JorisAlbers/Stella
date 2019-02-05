using System;
using System.Collections.Generic;
using System.Linq;

namespace StellaClient.Time
{
    public class TimeSetter
    {
        const int MAX_MEASUREMENTS = 3;

        private ISystemTimeSetter _systemTimeSetter;
        private List<long[]> _measurements;

        public TimeSetter(ISystemTimeSetter systemTimeSetter)
        {
            _systemTimeSetter = systemTimeSetter;
            
        }

        public void AddMeasurements(long clientSendTime, long serverReceivedTime)
        {
            if(_measurements == null)
            {
                long timeDelta = CalculateOffSet(clientSendTime, serverReceivedTime, DateTime.Now.Ticks);
                _systemTimeSetter.AdjustTimeWithTicks(timeDelta);
                _measurements = new List<long[]>();
            }
            else 
            {
                _measurements.Add(new long[]{clientSendTime,serverReceivedTime, DateTime.Now.Ticks});
                if(NeedsMoreData)
                {
                    // TODO set time
                }
            }
        }

        public bool NeedsMoreData { get { return _measurements.Count < MAX_MEASUREMENTS + 1; } }


        /// <summary>
        /// Calculates the timeoffset with the server
        /// </summary>
        /// <param name="clientSendTime"></param>
        /// <param name="serverSendTime"></param>
        /// <param name="clientReceivedTime"></param>
        /// <returns></returns>
        private long CalculateOffSet(long clientSendTime, long serverSendTime, long clientReceivedTime)
        {
            long timeDelta =  clientReceivedTime - clientSendTime; // How long it took for the message to return from the server
            long latency = timeDelta/2; // Latency from A to B
            
            long offset = (serverSendTime - clientReceivedTime) + latency;
            return offset;
        }

        private long ParseFinalTime(long[] measurements)
        {
            // Remove outliers, get the median, set the time.
            // Calulate deltas

            // A measurement = clientSendTime, serverSendTime, clientReceivedTime
            
            long[] deltas = new long[MAX_MEASUREMENTS];
            long total = 0;
            for(int i=0; i<MAX_MEASUREMENTS; i++)
            {
                long clientSendTime = measurements[i];
                long serverTime = measurements[i + 1];
                long clientReceivedTime = measurements[i+2];
                deltas[i] = CalculateOffSet(clientSendTime, serverTime, clientReceivedTime);
                total += deltas[i];
            }
            // Remove outliers
            double average = total / deltas.Length;
            double sumOfSquaresOfDifferences = deltas.Select(x => (x - average) * (x - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / deltas.Length);
            // Get median

            // Adjust time           
            
            deltas.OrderBy(x=>x).ToArray();
            return 0;
        }

    }
}