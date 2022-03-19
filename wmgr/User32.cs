using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowManager
{
    internal class User32
    {
        #region Externals

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, MonitorOptions flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #region Enumerations

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL =     0x00000000,
            MONITOR_DEFAULTTOPRIMARY =  0x00000001,
            MONITOR_DEFAULTTONEAREST =  0x00000002,
        }

        public enum WindowStyle : long
        {
            WS_BORDER =                 0x00800000L,
            WS_CAPTION =                0x00C00000L,
            WS_CHILD =                  0x40000000L,
            WS_CHILDWINDOW =            0x40000000L,
            WS_CLIPCHILDREN =           0x02000000L,
            WS_CLIPSIBLINGS =           0x04000000L,
            WS_DISABLED =               0x08000000L,
            WS_DLGFRAME =               0x00400000L,
            WS_GROUP =                  0x00020000L,
            WS_HSCROLL =                0x00100000L,
            WS_ICONIC =                 0x20000000L,
            WS_MAXIMIZE =               0x01000000L,
            WS_MAXIMIZEBOX =            0x00010000L,
            WS_MINIMIZE =               0x20000000L,
            WS_MINIMIZEBOX =            0x00020000L,
            WS_OVERLAPPED =             0x00000000L,
            WS_OVERLAPPEDWINDOW =       WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP =                  0x80000000L,
            WS_POPUPWINDOW =            WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEBOX =                0x00040000L,
            WS_SYSMENU =                0x00080000L,
            WS_TABSTOP =                0x00010000L,
            WS_THICKFRAME =             0x00040000L,
            WS_TILED =                  0x00000000L,
            WS_TILEDWINDOW =            WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_VISIBLE =                0x10000000L,
            WS_VSCROLL =                0x00200000L,
        }

        #endregion
    }
}
