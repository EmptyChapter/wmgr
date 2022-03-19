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

                IntPtr myStyle = new(
                    (int)User32.WindowStyle.WS_CAPTION |
                    (int)User32.WindowStyle.WS_CLIPCHILDREN |
                    (int)User32.WindowStyle.WS_MINIMIZEBOX |
                    (int)User32.WindowStyle.WS_MAXIMIZEBOX |
                    (int)User32.WindowStyle.WS_SYSMENU |
                    (int)User32.WindowStyle.WS_SIZEBOX);

                SetWindowLongPtr(
                    new HandleRef(null, mWindowHandle), -16, myStyle);

                WindowCommandsManager.AddWindowCommands(window);
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

            var lPrimaryScreen = User32.MonitorFromPoint(new POINT(0, 0),
                User32.MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            var lCurrentScreen = User32.MonitorFromWindow(hwnd,
                User32.MonitorOptions.MONITOR_DEFAULTTONEAREST);

            var lPrimaryScreenGetInfoResult =
                User32.GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo);
            var lCurrentScreenGetInfoResult =
                User32.GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo);

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
            var hwnd = User32.FindWindow("Shell_TrayWnd", null);

            if (hwnd != null)
            {
                var taskBarMonitor = User32.MonitorFromWindow(hwnd,
                    User32.MonitorOptions.MONITOR_DEFAULTTONEAREST);

                if (appMonitor.Equals(taskBarMonitor))
                {
                    APPBARDATA abd =    new();
                    abd.cbSize =        Marshal.SizeOf(abd);
                    abd.hWnd =          hwnd;

                    bool autoHide = Convert.ToBoolean(
                        Shell32.SHAppBarMessage(
                            Shell32.ABMsg.ABM_GETSTATE, ref abd));

                    if (autoHide)
                    {
                        abd.cbSize =    Marshal.SizeOf(abd);
                        abd.hWnd =      hwnd;

                        Shell32.SHAppBarMessage(
                            Shell32.ABMsg.ABM_GETTASKBARPOS, ref abd);

                        switch (GetEdge(abd.rc))
                        {
                            case Shell32.ABEdge.ABE_LEFT:
                                lMmi.ptMaxPosition.X +=     2;
                                lMmi.ptMaxTrackSize.X -=    2;
                                lMmi.ptMaxSize.X -=         2;
                                break;
                            case Shell32.ABEdge.ABE_RIGHT:
                                lMmi.ptMaxSize.X -=         2;
                                lMmi.ptMaxTrackSize.X -=    2;
                                break;
                            case Shell32.ABEdge.ABE_TOP:
                                lMmi.ptMaxPosition.Y +=     2;
                                lMmi.ptMaxTrackSize.Y -=    2;
                                lMmi.ptMaxSize.Y -=         2;
                                break;
                            case Shell32.ABEdge.ABE_BOTTOM:
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

        private static Shell32.ABEdge GetEdge(RECT rc)
        {
            if (rc.Top == rc.Left &&
                rc.Bottom > rc.Right)       return Shell32.ABEdge.ABE_LEFT;

            else if (rc.Top == rc.Left &&
                     rc.Bottom < rc.Right)  return Shell32.ABEdge.ABE_TOP;

            else if (rc.Top > rc.Left)      return Shell32.ABEdge.ABE_BOTTOM;

            else                            return Shell32.ABEdge.ABE_RIGHT;
        }

        private static IntPtr SetWindowLongPtr(
            HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return User32.SetWindowLongPtr64(
                    hWnd, nIndex, dwNewLong);

            else return new IntPtr(
                User32.SetWindowLong32(
                    hWnd, nIndex, dwNewLong.ToInt32()));
        }

        #endregion
    }
}
