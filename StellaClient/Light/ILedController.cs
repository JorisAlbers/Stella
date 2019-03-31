using System;
using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaClient.Light
{
    public interface ILedController
    {
        int FramesInBuffer { get; }
        int FramesInPendingBuffer { get; }

        void AddFrame(Frame frame);
        void AddFrames(IEnumerable<Frame> frames);
        void Dispose();
        void PrepareNextFrameSet(FrameSetMetadata metadata);
        void Run();
        event EventHandler<FramesNeededEventArgs> FramesNeeded;
    }
}