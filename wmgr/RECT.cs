using System.Runtime.InteropServices;

namespace WindowManager
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public int Left
        {
            get => _left;
            set => _left = value;
        }

        public int Top
        {
            get => _top;
            set => _top = value;
        }

        public int Right
        {
            get => _right;
            set => _right = value;
        }

        public int Bottom
        {
            get => _bottom;
            set => _bottom = value;
        }

        public RECT(int left, int right, int top, int bottom) : this()
        {
            Left =      left;
            Top =       top;
            Right =     right;
            Bottom =    bottom;
        }
    }
}
