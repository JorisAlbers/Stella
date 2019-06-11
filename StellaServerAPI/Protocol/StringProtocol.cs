using System;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaServerAPI.Protocol
{
    public static class StringProtocol
    {
        const int FIRST_PACKAGE_HEADER_BYTES = sizeof(int); // numberOfPackages
        const int PACKAGE_HEADER_BYTES = sizeof(int); // PackageIndex

        // Convert the string to an array of byte arrays
        public static byte[][] Serialize(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            if (data.Length < PacketProtocol<MessageType>.BUFFER_SIZE -  FIRST_PACKAGE_HEADER_BYTES + PACKAGE_HEADER_BYTES)
            {
                // just one package needed
                byte[] buffer = new byte[data.Length + FIRST_PACKAGE_HEADER_BYTES + PACKAGE_HEADER_BYTES];
                BitConverter.GetBytes(1).CopyTo(buffer,0);
                BitConverter.GetBytes(0).CopyTo(buffer,4);
                data.CopyTo(buffer,8);
                return new byte[1][]{buffer};
            }
            
            // Multiple packages are needed to be able to send the message.

            throw new NotImplementedException();


        }



    }
}
