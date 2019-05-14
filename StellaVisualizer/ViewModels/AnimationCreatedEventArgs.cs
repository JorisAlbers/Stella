using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaServer.Animation;

namespace StellaVisualizer.ViewModels
{
    public class AnimationCreatedEventArgs : EventArgs
    {
        public IAnimator Animator { get; }
        public int StripLength { get; }

        public AnimationCreatedEventArgs(IAnimator animator, int stripLength)
        {
            Animator = animator;
            StripLength = stripLength;
        }
    }
}
