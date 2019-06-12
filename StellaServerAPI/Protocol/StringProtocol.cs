using System;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaServerAPI.Protocol
{
    public static class StringProtocol
    {
        /// <summary>
        /// The number of bytes needed for the header of the package.
        /// Represents:
        /// - Number of packages if it's the first package.
        /// - Package index if it's an subsequent package.
        /// </summary>
        const int PACKAGE_HEADER_BYTES = sizeof(int);

        // Convert the string to an array of byte arrays
        public static byte[][] Serialize(string message, int maxPackageSize)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            int bytesAvailablePerPackage = maxPackageSize - PACKAGE_HEADER_BYTES;
            int packagesNeeded = (data.Length + bytesAvailablePerPackage - 1) / bytesAvailablePerPackage;
            
            byte[][] returnData = new byte[packagesNeeded][];
            // Create first package
            returnData[0] = CreatePackage(data, 0, maxPackageSize, packagesNeeded);
            for (int i = 1; i < packagesNeeded; i++)
            {
                returnData[i] = CreatePackage(data, i * bytesAvailablePerPackage, maxPackageSize, i);
            }
            return returnData;
        }

        /// <summary>
        ///  Create a subsequent package with a normal package header
        /// </summary>
        private static byte[] CreatePackage(byte[] data, int startIndex, int maxPackageSize, int headerCounter)
        {
            byte[] buffer = new byte[Math.Min(data.Length - startIndex + PACKAGE_HEADER_BYTES, maxPackageSize + PACKAGE_HEADER_BYTES)];
            BitConverter.GetBytes(headerCounter).CopyTo(buffer, 0);
            Array.Copy(data,startIndex,buffer,4,buffer.Length - 4);
            return buffer;
        }
    }
}
