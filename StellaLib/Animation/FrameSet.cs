using System.Collections;
using System.Collections.Generic;

namespace StellaLib.Animation
{
    /// <summary>
    /// Collection of Frames.
    /// </summary>
    /// <typeparam name="Frame"></typeparam>
    public class FrameSet : IEnumerable<Frame>
    {
        private Frame[] _frames;

        public FrameSet(Frame[] frames)
        {
            _frames = frames;
        }

        public IEnumerator<Frame> GetEnumerator()
        {
             for (int i = 0; i < _frames.Length; i++) 
             {
                 yield return _frames[i];
             }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}