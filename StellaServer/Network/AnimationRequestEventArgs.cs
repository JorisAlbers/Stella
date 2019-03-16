using System;

namespace StellaServer.Network
{
    public class AnimationRequestEventArgs: EventArgs
    {
        /// <summary>
        /// Event args
        /// </summary>
        /// <param name="clientID">The id of the client making the request</param>
        /// <param name="startIndex">The start index of the first frame the client has requested</param>
        /// <param name="count">The number of frames the client has requested</param>
        public AnimationRequestEventArgs(string clientID, int startIndex, int count)
        {
            ClientID = clientID;
            StartIndex = startIndex;
            Count = count;
        }
        public string ClientID { get; private set; }
        public int StartIndex { get; private set; }
        public int Count { get; private set; }
    }
}