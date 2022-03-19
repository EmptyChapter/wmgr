using System.Runtime.InteropServices;

namespace WindowManager
{
    internal class Shell32
    {
        #region Externals

        [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        #endregion

        #region Enumerations

        public enum ABEdge
        {
            ABE_UNKNOWN = -1,
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3,
        }

        public enum ABMsg : uint
        {
            ABM_NEW = 0x00000000,
            ABM_REMOVE = 0x00000001,
            ABM_QUERYPOS = 0x00000002,
            ABM_SETPOS = 0x00000003,
            ABM_GETSTATE = 0x00000004,
            ABM_GETTASKBARPOS = 0x00000005,
            ABM_ACTIVATE = 0x00000006,
            ABM_GETAUTOHIDEBAR = 0x00000007,
            ABM_SETAUTOHIDEBAR = 0x00000008,
            ABM_WINDOWPOSCHANGED = 0x00000009,
            ABM_SETSTATE = 0x00000010,
        }

        #endregion
    }
}
