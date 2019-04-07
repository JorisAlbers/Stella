using System;
using System.Linq;
using System.Net;

namespace StellaLib.Network.Protocol
{
    // Time sync flow:
    // 1. Client send time sync request
    // 2. Server sends it's time
    // 3. Client updates its time and sends its new time
    // 4. Server sends its time
    // 5. Repeat 3x
    // 6. Client sets its time to the median of the 3, measurements 1-sd from the median are discarded, attempts to remove network latency.
    public static class TimeSyncProtocol
    {
        private const int BYTES_PER_MEASUREMENT = sizeof(long);
        
        public static byte[] CreateMessage(DateTime now)
        {
            return BitConverter.GetBytes(now.Ticks);
        }

        // Adds the system time to the message
        public static byte[] CreateMessage(DateTime now, byte[] previousMessage)
        {
            byte[] bytes = new byte[previousMessage.Length + BYTES_PER_MEASUREMENT];
            previousMessage.CopyTo(bytes,0);
            BitConverter.GetBytes(now.Ticks).CopyTo(bytes,previousMessage.Length);
            return bytes;
        }

        public static long[] ParseMessage(byte[] message)
        {
            long[] measurements = new long[message.Length / BYTES_PER_MEASUREMENT];
            int measurementsInMessage = message.Length / BYTES_PER_MEASUREMENT;
            for (int i = 0; i < measurementsInMessage; i++)
            {
                measurements[i] = BitConverter.ToInt64(message, i * BYTES_PER_MEASUREMENT);
            }
            return measurements;
        }  
    }
}