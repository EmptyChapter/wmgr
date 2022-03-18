using System.Runtime.InteropServices;

namespace WindowManager
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        private int _x;
        private int _y;

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public POINT(int x, int y) : this()
        {
            X = x;
            Y = y;
        }
    }
}
