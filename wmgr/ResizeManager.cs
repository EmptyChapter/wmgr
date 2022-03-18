using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WindowManager
{
    public static class ResizeManager
    {
        #region Private Fields

        private static Int32Size _minWindowSize;

        #endregion

        #region Public Methods

        public static void AddResizeHook(Window window)
        {
            if (window != null)
            {
                _minWindowSize =        new(window.MinWidth, window.MinHeight);
                var mWindowHandle =     new WindowInteropHelper(window).Handle;

                HwndSource.FromHwnd(mWindowHandle).AddHook(
                    new HwndSourceHook(WindowProc));
            }
            else throw new ArgumentNullException();
        }

        #endregion

        #region Private Methods

        private static IntPtr WindowProc(
            IntPtr hwnd, int msg, IntPtr wParam,
            IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var lMmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            MONITORINFO lPrimaryScreenInfo = new();
            MONITORINFO lCurrentScreenInfo = new();

            var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0),
                MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            var lCurrentScreen = MonitorFromWindow(hwnd,
                MonitorOptions.MONITOR_DEFAULTTONEAREST);

            var lPrimaryScreenGetInfoResult =
                GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo);
            var lCurrentScreenGetInfoResult =
                GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo);

            if (lPrimaryScreenGetInfoResult &&
                lCurrentScreenGetInfoResult)
            {
                if (lPrimaryScreen.Equals(lCurrentScreen))
                {
                    lMmi.ptMaxPosition.X =  lPrimaryScreenInfo.rcWork.Left;
                    lMmi.ptMaxPosition.Y =  lPrimaryScreenInfo.rcWork.Top;

                    lMmi.ptMaxSize.X =      lPrimaryScreenInfo.rcWork.Right -
                                            lPrimaryScreenInfo.rcWork.Left;
                    lMmi.ptMaxSize.Y =      lPrimaryScreenInfo.rcWork.Bottom -
                                            lPrimaryScreenInfo.rcWork.Top;
                }
                else
                {
                    lMmi.ptMaxPosition.X =  lPrimaryScreenInfo.rcMonitor.Left;
                    lMmi.ptMaxPosition.Y =  lPrimaryScreenInfo.rcMonitor.Top;

                    lMmi.ptMaxSize.X =      lPrimaryScreenInfo.rcMonitor.Right -
                                            lPrimaryScreenInfo.rcMonitor.Left;
                    lMmi.ptMaxSize.Y =      lPrimaryScreenInfo.rcMonitor.Bottom -
                                            lPrimaryScreenInfo.rcMonitor.Top;
                }

                lMmi.ptMaxTrackSize.X =     lMmi.ptMaxSize.X;
                lMmi.ptMaxTrackSize.Y =     lMmi.ptMaxSize.Y;

                lMmi.ptMinTrackSize.X =     (int)_minWindowSize.Width;
                lMmi.ptMinTrackSize.Y =     (int)_minWindowSize.Height;

                lMmi = AdjustForTaskBar(lCurrentScreen, lMmi);
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }

        private static MINMAXINFO AdjustForTaskBar(IntPtr appMonitor, MINMAXINFO lMmi)
        {
            var hwnd = FindWindow("Shell_TrayWnd", null);

            if (hwnd != null)
            {
                var taskBarMonitor = MonitorFromWindow(hwnd,
                    MonitorOptions.MONITOR_DEFAULTTONEAREST);

                if (appMonitor.Equals(taskBarMonitor))
                {
                    APPBARDATA abd =    new();
                    abd.cbSize =        Marshal.SizeOf(abd);
                    abd.hWnd =          hwnd;

                    bool autoHide = Convert.ToBoolean(
                        SHAppBarMessage(ABMsg.ABM_GETSTATE, ref abd));

                    if (autoHide)
                    {
                        abd.cbSize =    Marshal.SizeOf(abd);
                        abd.hWnd =      hwnd;

                        SHAppBarMessage(ABMsg.ABM_GETTASKBARPOS, ref abd);

                        switch (GetEdge(abd.rc))
                        {
                            case ABEdge.ABE_LEFT:
                                lMmi.ptMaxPosition.X +=     2;
                                lMmi.ptMaxTrackSize.X -=    2;
                                lMmi.ptMaxSize.X -=         2;
                                break;
                            case ABEdge.ABE_RIGHT:
                                lMmi.ptMaxSize.X -=         2;
                                lMmi.ptMaxTrackSize.X -=    2;
                                break;
                            case ABEdge.ABE_TOP:
                                lMmi.ptMaxPosition.Y +=     2;
                                lMmi.ptMaxTrackSize.Y -=    2;
                                lMmi.ptMaxSize.Y -=         2;
                                break;
                            case ABEdge.ABE_BOTTOM:
                                lMmi.ptMaxSize.Y -=         2;
                                lMmi.ptMaxTrackSize.Y -=    2;
                                break;
                            default:
                                return lMmi;
                        }
                    }
                }
            }

            return lMmi;
        }

        private static ABEdge GetEdge(RECT rc)
        {
            if (rc.Top == rc.Left &&
                rc.Bottom > rc.Right)       return ABEdge.ABE_LEFT;

            else if (rc.Top == rc.Left &&
                     rc.Bottom < rc.Right)  return ABEdge.ABE_TOP;

            else if (rc.Top > rc.Left)      return ABEdge.ABE_BOTTOM;

            else                            return ABEdge.ABE_RIGHT;
        }

        #endregion

        #region Externals

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, MonitorOptions flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        #endregion

        #region Enumerators

        private enum ABEdge
        {
            ABE_UNKNOWN =       -1,
            ABE_LEFT =           0,
            ABE_TOP =            1,
            ABE_RIGHT =          2,
            ABE_BOTTOM =         3,
        }

        private enum ABMsg : uint
        {
            ABM_NEW =                       0x00000000,
            ABM_REMOVE =                    0x00000001,
            ABM_QUERYPOS =                  0x00000002,
            ABM_SETPOS =                    0x00000003,
            ABM_GETSTATE =                  0x00000004,
            ABM_GETTASKBARPOS =             0x00000005,
            ABM_ACTIVATE =                  0x00000006,
            ABM_GETAUTOHIDEBAR =            0x00000007,
            ABM_SETAUTOHIDEBAR =            0x00000008,
            ABM_WINDOWPOSCHANGED =          0x00000009,
            ABM_SETSTATE =                  0x00000010,
        }

        private enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL =         0x00000000,
            MONITOR_DEFAULTTOPRIMARY =      0x00000001,
            MONITOR_DEFAULTTONEAREST =      0x00000002,
        }

        #endregion
    }
}
