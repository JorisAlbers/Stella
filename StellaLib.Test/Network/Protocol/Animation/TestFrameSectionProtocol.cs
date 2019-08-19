using System;
using System.Collections.Generic;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Packages;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
    [TestFixture]
    public class TestFrameSectionProtocol
    {
        [Test]
        public void Serialize_FrameSectionPackage_CorrectlySerialized()
        {
            FrameSectionPackageWithoutDelta package = new FrameSectionPackageWithoutDelta()
            {
                FrameSequenceIndex = 100,
                Index = 10,
                pixelInstructions = new List<PixelInstructionWithoutDelta>
                {
                    new PixelInstructionWithoutDelta(10,20,30),
                    new PixelInstructionWithoutDelta(40,50,60)
                }
            };

            int header_bytes_needed = sizeof(int) + sizeof(int) + sizeof(int);
            int pixelInstructions_bytes_needed = PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED * 2;

            byte[] expectedBytes = new byte[header_bytes_needed + pixelInstructions_bytes_needed];
            BitConverter.GetBytes(package.FrameSequenceIndex).CopyTo(expectedBytes, 0);
            BitConverter.GetBytes(package.Index).CopyTo(expectedBytes, 4);
            BitConverter.GetBytes(package.pixelInstructions.Count).CopyTo(expectedBytes, 8);
            PixelInstructionWithoutDeltaProtocol.Serialize(package.pixelInstructions[0], expectedBytes, 12);
            PixelInstructionWithoutDeltaProtocol.Serialize(package.pixelInstructions[1], expectedBytes, 12 + PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED);

            byte[] returnBytes = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED + pixelInstructions_bytes_needed];
            FrameSectionProtocol.Serialize(package, returnBytes, 0);
            Assert.AreEqual(expectedBytes, returnBytes);
        }

        [Test]
        public void Deserialize_bytes_CorrectlyDeserializes()
        {
            FrameSectionPackageWithoutDelta expectedPackage = new FrameSectionPackageWithoutDelta()
            {
                FrameSequenceIndex = 100,
                Index = 10,
                pixelInstructions = new List<PixelInstructionWithoutDelta>
                {
                    new PixelInstructionWithoutDelta(10,20,30),
                    new PixelInstructionWithoutDelta(40,50,60)
                }
            };

            int header_bytes_needed = sizeof(int) + sizeof(int) + sizeof(int);
            int pixelInstructions_bytes_needed = PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED * 2;

            byte[] expectedBytes = new byte[header_bytes_needed + pixelInstructions_bytes_needed];
            BitConverter.GetBytes(expectedPackage.FrameSequenceIndex).CopyTo(expectedBytes, 0);
            BitConverter.GetBytes(expectedPackage.Index).CopyTo(expectedBytes, 4);
            BitConverter.GetBytes(expectedPackage.pixelInstructions.Count).CopyTo(expectedBytes, 8);
            PixelInstructionWithoutDeltaProtocol.Serialize(expectedPackage.pixelInstructions[0], expectedBytes, 12);
            PixelInstructionWithoutDeltaProtocol.Serialize(expectedPackage.pixelInstructions[1], expectedBytes, 12 + PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED);

            FrameSectionPackageWithoutDelta package = FrameSectionProtocol.Deserialize(expectedBytes, 0);
            Assert.AreEqual(expectedPackage.FrameSequenceIndex, package.FrameSequenceIndex);
            Assert.AreEqual(expectedPackage.Index, package.Index);
            Assert.AreEqual(expectedPackage.NumberOfPixelInstructions, package.NumberOfPixelInstructions);
            Assert.AreEqual(expectedPackage.pixelInstructions[0], package.pixelInstructions[0]);
            Assert.AreEqual(expectedPackage.pixelInstructions[1], package.pixelInstructions[1]);
        }
    }
}