using System;
using System.Collections.Generic;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    public interface ILedController
    {
        void RenderFrame(FrameWithoutDelta frame);
    }
}