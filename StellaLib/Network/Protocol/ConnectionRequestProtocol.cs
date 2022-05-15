using System;
using System.Collections.Generic;
using System.Text;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    public class ConnectionRequestProtocol
    {
        public const int BYTES_NEEDED = sizeof(byte) *2; // key, version

        public static ConnectionRequestMessage Deserialize(byte[] bytes, int startIndex)
        {
            return new ConnectionRequestMessage(bytes[startIndex], bytes[startIndex+1]);
        }
    }

    public class ConnectionRequestMessage
    {
        public byte Key { get; }
        public byte Version { get; }

        public ConnectionRequestMessage(byte key, byte version)
        {
            Key = key;
            Version = version;
        }
    }
}
