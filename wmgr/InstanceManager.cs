using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WindowManager
{
    public static class InstanceManager
    {
        #region Public Methods

        public static void CheckInstanceDuplication()
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            Mutex mutex = new(true, appName, out var canCreate);

            if (!canCreate)
            {
                var current = Process.GetCurrentProcess();
                var procName = current.ProcessName;

                var mainProc = (from proc in Process.GetProcessesByName(procName)
                                where proc.Id != current.Id
                                select proc).FirstOrDefault();

                if (mainProc != null)
                {
                    User32.SetForegroundWindow(mainProc.MainWindowHandle);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Exit += (s, e) => mutex.Close();
            }
        }

        #endregion
    }
}
