using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StellaServerAPI
{
    public class BitmapReceivedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
