using System;
using System.Runtime.InteropServices;

namespace Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ws2811_t
	{
		public long render_wait_time;
		public IntPtr device;
		public IntPtr rpi_hw;
		public uint freq;
		public int dmanum;

		public ws2811_channel_t channel_0;
		// TODO channel_1 is used somewere.
		// TODO If we remove the line below, the program segmentation faults.
		public ws2811_channel_t channel_1;
	}
}
