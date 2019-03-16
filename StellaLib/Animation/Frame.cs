using System.Collections;
using System.Collections.Generic;

namespace StellaLib.Animation
{
    /// <summary>
    /// A frame contains:
    ///     1. All pixel instructions of a single moment in time.
    ///     2. The display time 
    /// 
    /// </summary>
    public class Frame : IList<PixelInstruction>
    {   
        private List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();

        /// <summary>
        /// The index of the Frame in the frameSet.
        /// </summary>
        public int Index;
        public PixelInstruction this[int index] { get => pixelInstructions[index]; set => pixelInstructions[index] = value;}

        /// <summary>
        /// The number of miliseconds after the start of the FrameSet (animation).
        /// </summary>
        /// <value></value>
        public int TimeStampRelative {get;private set;}

        public Frame(int index, int timeStampRelative)
        {
            Index = index;
            TimeStampRelative = timeStampRelative;
        }

        #region List interface 
        public int Count => pixelInstructions.Count;

        public bool IsReadOnly => false;

        public void Add(PixelInstruction item)
        {
            pixelInstructions.Add(item);
        }

        public void Clear()
        {
            pixelInstructions.Clear();
        }

        public bool Contains(PixelInstruction item)
        {
            return pixelInstructions.Contains(item);
        }

        public void CopyTo(PixelInstruction[] array, int arrayIndex)
        {
            pixelInstructions.CopyTo(array,arrayIndex);
        }

        public IEnumerator<PixelInstruction> GetEnumerator()
        {
            return pixelInstructions.GetEnumerator();
        }

        public int IndexOf(PixelInstruction item)
        {
            return pixelInstructions.IndexOf(item);
        }

        public void Insert(int index, PixelInstruction item)
        {
            pixelInstructions.Insert(index,item);
        }

        public bool Remove(PixelInstruction item)
        {
            return pixelInstructions.Remove(item);
        }

        public void RemoveAt(int index)
        {
            pixelInstructions.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pixelInstructions.GetEnumerator();
        }
        #endregion
    }
}