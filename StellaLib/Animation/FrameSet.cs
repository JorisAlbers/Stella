using System.Collections;
using System.Collections.Generic;

namespace StellaLib.Animation
{
    /// <summary>
    /// Collection of Frames.
    /// </summary>
    /// <typeparam name="Frame"></typeparam>
    public class FrameSet : IList<Frame>
    {
        private List<Frame> _frames;

        public FrameSet()
        {
            _frames = new List<Frame>();
        }

        # region IList interface
        public Frame this[int index] { get => ((IList<Frame>)_frames)[index]; set => ((IList<Frame>)_frames)[index] = value; }

        public int Count => ((IList<Frame>)_frames).Count;

        public bool IsReadOnly => ((IList<Frame>)_frames).IsReadOnly;

        public void Add(Frame item)
        {
            ((IList<Frame>)_frames).Add(item);
        }

        public void Clear()
        {
            ((IList<Frame>)_frames).Clear();
        }

        public bool Contains(Frame item)
        {
            return ((IList<Frame>)_frames).Contains(item);
        }

        public void CopyTo(Frame[] array, int arrayIndex)
        {
            ((IList<Frame>)_frames).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            return ((IList<Frame>)_frames).GetEnumerator();
        }

        public int IndexOf(Frame item)
        {
            return ((IList<Frame>)_frames).IndexOf(item);
        }

        public void Insert(int index, Frame item)
        {
            ((IList<Frame>)_frames).Insert(index, item);
        }

        public bool Remove(Frame item)
        {
            return ((IList<Frame>)_frames).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Frame>)_frames).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Frame>)_frames).GetEnumerator();
        }
        #endregion
    }
}