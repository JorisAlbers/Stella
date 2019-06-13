using System;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaServerAPI.Protocol
{
    public class StringProtocol
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


        private StringBuilder _stringBuilder;
        private int _numberOfPackages;
        private int _packagesReceived;

        public StringProtocol()
        {
            _stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Returns true when the string has been fully deserialized
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool TryDeserialize(byte[] package, out string message)
        {
            message = null;
            if (_packagesReceived == 0)
            {
                // First package.
                _numberOfPackages = BitConverter.ToInt32(package, 0);
            }

            _stringBuilder.Append(Encoding.ASCII.GetString(package, 4, package.Length - 4));
            _packagesReceived++;

            if (_packagesReceived == _numberOfPackages)
            {
                message = _stringBuilder.ToString();
                return true;
            }

            return false;
        }
    }
}
