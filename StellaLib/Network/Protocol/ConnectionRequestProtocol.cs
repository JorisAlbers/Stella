using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    public class ConnectionRequestProtocol
    {
        public const int BYTES_NEEDED = sizeof(byte) *2 + sizeof(byte) *6; // key, version, mac

        public static ConnectionRequestMessage Deserialize(byte[] bytes, int startIndex)
        {
            byte[] mac = new[]
            {
                bytes[startIndex + 2],
                bytes[startIndex + 3],
                bytes[startIndex + 4],
                bytes[startIndex + 5],
                bytes[startIndex + 6],
                bytes[startIndex + 7],
            };

            return new ConnectionRequestMessage(bytes[startIndex], bytes[startIndex+1], mac);
        }
    }

    public class ConnectionRequestMessage
    {
        public byte Key { get; }
        public byte Version { get; }

        public MacAddress Mac { get; }

        public ConnectionRequestMessage(byte key, byte version, byte[] mac)
        {
            Key = key;
            Version = version;
            Mac = new MacAddress(mac);
        }
    }

    public class MacAddress
    {
        public byte[] Mac { get; }

        public MacAddress(byte[] mac)
        {
            Mac = mac;
        }

        public override string ToString()
        {
            return string.Join(':', Mac.Select(x=> x.ToString("X2")));
        }
    }
}
