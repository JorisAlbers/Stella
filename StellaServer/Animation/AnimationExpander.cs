using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StellaLib.Animation;

namespace StellaServer.Animation
{
    /// <summary>
    /// Repeats an animation
    /// </summary>
    public class AnimationExpander
    {
        private readonly List<Frame> _originalAnimation;

        public AnimationExpander(List<Frame> animation)
        {
            _originalAnimation = animation;
        }

        public List<Frame> Expand(int times)
        {
            if (times == 1)
            {
                return _originalAnimation;
            }

            List<Frame> expanded = new List<Frame>(_originalAnimation);

            int interval = _originalAnimation[1].TimeStampRelative - _originalAnimation[0].TimeStampRelative; // TODO be able to work with different intervals

            int index = _originalAnimation.Count;
            int timestampRelative = _originalAnimation.Last().TimeStampRelative;
            for (int i = 0; i < times -1; i++) // -1 as there is already one cycle done (the _originalAnimation is a single cycle on its own)
            {
                for (int j = 0; j < _originalAnimation.Count; j++)
                {
                    timestampRelative += interval;
                    Frame frame = new Frame(index++, timestampRelative);
                    frame.AddRange(_originalAnimation[j]);
                    expanded.Add(frame);
                }
            }

            return expanded;
        }
    }
}
