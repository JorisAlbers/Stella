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
    // 6. Client sets its time to the median of the 3, measurements 1-sd from the median are discared,attemps to remove network latency.
    public class TimeSyncProtocol
    {
        const char SEPARATOR = ';';
        
        public static string CreateMessage()
        {
            return $"{DateTime.Now.Ticks}";
        }

        // Adds the system time to the message
        public static string CreateMessage(string previousMessage)
        {
            return $"{previousMessage}{SEPARATOR}{DateTime.Now.Ticks}";
        }

        public static long[] ParseMessage(string message)
        {
            string[] split = message.Split(SEPARATOR);
            return split.Select(x=> long.Parse(x)).ToArray();
        }  
    }
}