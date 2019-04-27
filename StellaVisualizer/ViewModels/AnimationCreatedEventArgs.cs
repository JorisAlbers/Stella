using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;

namespace StellaVisualizer.ViewModels
{
    public class AnimationCreatedEventArgs : EventArgs
    {
        public List<Frame> Animation { get; }
        public int StripLength { get; }

        public AnimationCreatedEventArgs(List<Frame> animation, int stripLength)
        {
            Animation = animation;
            StripLength = stripLength;
        }
    }
}
