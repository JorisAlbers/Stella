using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Mathematics;

namespace StellaClient.Time
{
    public class TimeSetter
    {
        const int MAX_MEASUREMENTS = 6;
        private ISystemTimeSetter _systemTimeSetter;
        private List<long[]> _measurements; // A measurement = clientSendTime, serverSendTime, clientReceivedTime

        public TimeSetter(ISystemTimeSetter systemTimeSetter)
        {
            _systemTimeSetter = systemTimeSetter;
        }

        /// <summary>
        /// Add a TimeSync measurement.
        /// </summary>
        /// <param name="clientSendTime">The moment the client initiated the TimeSync protocol</param>
        /// <param name="serverReceivedTime">The moment the server received the data</param>
        /// <param name="clientReceivedTime">The moment the client received the data from the server</param>
        public void AddMeasurements(long clientSendTime, long serverReceivedTime, long clientReceivedTime)
        {
            if(_measurements == null)
            {
                long timeDelta = CalculateOffSet(clientSendTime, serverReceivedTime, clientReceivedTime);
                _systemTimeSetter.AdjustTimeWithTicks(timeDelta);
                _measurements = new List<long[]>();
            }
            else 
            {
                _measurements.Add(new long[]{clientSendTime,serverReceivedTime, clientReceivedTime});
                if(!NeedsMoreData)
                {
                    _systemTimeSetter.AdjustTimeWithTicks(ParseFinalTime(_measurements));
                }
            }
        }

        /// <summary>
        /// True if this TimeSetter instances is waiting for more measurements to arrive in order to set the time.
        /// </summary>
        /// <value></value>
        public bool NeedsMoreData { get { return _measurements.Count < MAX_MEASUREMENTS; } }

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
            double latency = timeDelta/2; // Latency from A to B
            
            long offset = (long)((serverSendTime - clientReceivedTime) + latency);
            return offset;
        }

        private long ParseFinalTime(List<long[]> measurements)
        {
            long[] deltas = new long[MAX_MEASUREMENTS];
            for(int i=0; i<MAX_MEASUREMENTS; i++)
            {
                long clientSendTime = measurements[i][0];
                long serverTime = measurements[i][1];
                long clientReceivedTime = measurements[i][2];
                deltas[i] = (long)CalculateOffSet(clientSendTime, serverTime, clientReceivedTime);
            }
            
            deltas = RemoveOutliers(deltas);
            
            return (long)Calculation.Median(deltas);
        }

        private long[] RemoveOutliers(long[] deltas)
        {
            // Calculate the IQR and the maximum range
            Array.Sort(deltas);
            double q1 = Calculation.Percentile(deltas,25);
            double q2 = Calculation.Percentile(deltas,50);
            double q3 = Calculation.Percentile(deltas,75);
            double iqr;

            if(q1 > q3)
            {
                iqr = q1- q3;
            }
            else
            {
                iqr = q3 - q1;
            }
            long maxRange = (long)Math.Floor(1.2 * iqr);

            // Remove outliers
            double average = deltas.Average();
            deltas = deltas.OrderByDescending(d => Math.Abs(d - average)).ToArray();


            int outlierIndex = 0;
            while(outlierIndex < deltas.Length -1)
            {
                // Remove Object if its value is more than 1.5*IQR from the Mean.
                double absolute = Math.Abs(deltas[outlierIndex] - average);
                if (absolute<= maxRange)
                {
                    // No outlier found, we're finished.
                    break;
                }
                outlierIndex ++;
            }

            if(outlierIndex == 0)
            {
                return deltas;
            }
            if(outlierIndex == deltas.Length-1)
            {
                return new long[0];
            }

            long[] newArray = new long[deltas.Length-outlierIndex];
            for(int i=0;i<newArray.Length;i++)
            {
                newArray[i] = deltas[i + outlierIndex];
            }
            return newArray;
        }
    }
}