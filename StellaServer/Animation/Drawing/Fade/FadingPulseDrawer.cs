using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing.Fade
{
    public class FadingPulseDrawer : IDrawer

    {
        private readonly int _stripLength;
        private readonly int _frameWaitMs;
        private readonly Color _color;
        private readonly int _pulsePosition;
        private readonly int _fadeSteps;

        public FadingPulseDrawer(int stripLength, int frameWaitMS, Color color, int pulsePosition, int fadeSteps)
        {
            _stripLength = stripLength;
            _frameWaitMs = frameWaitMS;
            _color = color;
            _pulsePosition = pulsePosition;
            _fadeSteps = fadeSteps;
        }

        public List<Frame> Create()
        {
            List<Frame> frames = new List<Frame>();
            Create(frames, _pulsePosition,0);
            return frames;
        }

        public void Create(List<Frame> frames, int pulsePosition, int frameStartIndex)
        {
            Color[] fadeColors = new Color[_fadeSteps];

            Color[][] fadedPatterns = FadeCalculation.CalculateFadedPatterns(new Color[] { _color }, _fadeSteps);

            for (int i = 0; i < _fadeSteps; i++)
            {
                fadeColors[i] = fadedPatterns[i][0];
            }

            for (int i = 0; i < _fadeSteps; i++)
            {
                Color color = fadeColors[fadeColors.Length - 1 - i];
                int missingFrames = frameStartIndex + _fadeSteps - frames.Count;
                if (missingFrames > 0)
                {
                    for (int j = 0; j < missingFrames; j++)
                    {
                        frames.Add(new Frame(frames.Count,frames.Count * _frameWaitMs));
                    }
                }

                Frame frame = frames[frameStartIndex + i];
                for (int j = pulsePosition - i; j < pulsePosition + i; j++)
                {
                    if (j < 0 || j > _stripLength - 1)
                    {
                        continue;
                    }
                    frame.Add(new PixelInstruction((uint)j, color));
                }
                
            }

        }

    }
}
