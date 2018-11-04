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
        private List<Frame> _frames;

        public FrameSet()
        {
            _frames = new List<Frame>();
        }

        public void Add(Frame frame)
        {
            _frames.Add(frame);
        }
        
        public Frame this[int index] =>  _frames[index];

        public int Count => _frames.Count;

        public IEnumerator<Frame> GetEnumerator()
        {
             for (int i = 0; i < _frames.Count; i++) 
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