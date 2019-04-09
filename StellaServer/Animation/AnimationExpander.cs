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
            List<Frame> expanded = new List<Frame>();
            int index = _originalAnimation.Count;
            int timestampRelative = _originalAnimation.Sum(x => x.TimeStampRelative);
            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < _originalAnimation.Count; j++)
                {
                    timestampRelative += _originalAnimation[j].TimeStampRelative;
                    Frame frame = new Frame(index++, timestampRelative);
                    frame.AddRange(_originalAnimation[j]);
                    expanded.Add(frame);
                }
            }

            return expanded;
        }
    }
}
