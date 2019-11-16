using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation
{
    public class PlayListItem
    {
        public Storyboard Storyboard { get; }
        public int Duration { get; }

        public PlayListItem(Storyboard storyboard, int duration)
        {
            Storyboard = storyboard;
            Duration = duration;
        }
    }
}
