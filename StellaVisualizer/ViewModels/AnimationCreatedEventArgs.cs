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

        public AnimationCreatedEventArgs(List<Frame> animation)
        {
            Animation = animation;
        }
    }
}
