﻿using System.Runtime.InteropServices;

namespace WindowManager
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class MONITORINFO
    {
        public int cbSize =         Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor =     new();
        public RECT rcWork =        new();
        public int dwFlags =        0;
    }
}
