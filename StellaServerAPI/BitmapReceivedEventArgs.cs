using System;
using System.Drawing;

namespace StellaServerAPI
{
    public class BitmapReceivedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
