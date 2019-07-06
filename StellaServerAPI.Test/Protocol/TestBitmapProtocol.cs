using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NUnit.Framework;
using StellaServerAPI.Protocol;

namespace StellaServerAPI.Test.Protocol
{
    [TestFixture]
    public class TestBitmapProtocol
    {
        [Test]
        public void TryDeserialize_byteArray_ReturnsBitmap()
        {
            string expectedName = "A";
            byte[] nameAsBytes = Encoding.ASCII.GetBytes(expectedName);
            Color[] expectedColors1 = new Color[]{Color.FromArgb(255,255,255), Color.FromArgb(200,200,200)};
            Color[] expectedColors2 = new Color[]{Color.FromArgb(150,150,150), Color.FromArgb(0,0,0)};
            int width = 2;
            int height = 2;

            byte[] data = new byte[4 + 4 + 4 + nameAsBytes.Length+  4 * 3]; // nameLength + width + height + name + 4 * rgb
            int startIndex = 0;
            BitConverter.GetBytes(nameAsBytes.Length).CopyTo(data, startIndex);
            BitConverter.GetBytes(width) .CopyTo(data, startIndex += 4);
            BitConverter.GetBytes(height).CopyTo(data, startIndex += 4);
            // Name
            nameAsBytes.CopyTo(data,startIndex += 4);
            // Row 1
            data[startIndex += nameAsBytes.Length] = expectedColors1[0].R;
            data[startIndex += 1] = expectedColors1[0].G;
            data[startIndex += 1] = expectedColors1[0].B;
            data[startIndex += 1] = expectedColors1[1].R;
            data[startIndex += 1] = expectedColors1[1].G;
            data[startIndex += 1] = expectedColors1[1].B;
            // Row 2
            data[startIndex += 1] = expectedColors2[0].R;
            data[startIndex += 1] = expectedColors2[0].G;
            data[startIndex += 1] = expectedColors2[0].B;
            data[startIndex += 1] = expectedColors2[1].R;
            data[startIndex += 1] = expectedColors2[1].G;
            data[startIndex += 1] = expectedColors2[1].B;
            
            BitmapProtocol bitmapProtocol = new BitmapProtocol();
            bool received = bitmapProtocol.TryDeserialize(data, out Bitmap bitmap, out string name);

            Assert.IsTrue(received);
            Assert.AreEqual(expectedName,name);
            Assert.AreEqual(width, bitmap.Width);
            Assert.AreEqual(height, bitmap.Height);
            Assert.AreEqual(expectedColors1[0], bitmap.GetPixel(0,0));
            Assert.AreEqual(expectedColors1[1], bitmap.GetPixel(1,0));
            Assert.AreEqual(expectedColors2[0], bitmap.GetPixel(0,1));
            Assert.AreEqual(expectedColors2[1], bitmap.GetPixel(1,1));


        }
    }
}
