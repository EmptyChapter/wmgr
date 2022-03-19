using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace WindowManager
{
    internal static class WindowCommandsManager
    {
        #region Public Methods

        public static void AddWindowCommands(Window window)
        {
            List<CommandBinding> defaultCommands = new()
            {
                new CommandBinding(SystemCommands.CloseWindowCommand,
                    OnCloseWindow),
                new CommandBinding(SystemCommands.MaximizeWindowCommand,
                    OnMaximizeWindow, OnCanResizeWindow),
                new CommandBinding(SystemCommands.MinimizeWindowCommand,
                    OnMinimizeWindow, OnCanMinimizeWindow),
                new CommandBinding(SystemCommands.RestoreWindowCommand,
                    OnRestoreWindow, OnCanResizeWindow),
                new CommandBinding(SystemCommands.ShowSystemMenuCommand,
                    OnShowSystemMenu),
            };

            window.CommandBindings.AddRange(defaultCommands);
        }

        #endregion

        #region Handled Events

        private static void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = sender as Window;

            e.CanExecute =
                window.ResizeMode == ResizeMode.CanResize ||
                window.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private static void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            var window = sender as Window;

            e.CanExecute = window.ResizeMode != ResizeMode.NoResize;
        }

        private static void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = target as Window;

            SystemCommands.CloseWindow(window);
        }

        private static void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = target as Window;

            SystemCommands.MaximizeWindow(window);
        }

        private static void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = target as Window;

            SystemCommands.MinimizeWindow(window);
        }

        private static void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            var window = target as Window;

            SystemCommands.RestoreWindow(window);
        }

        private static void OnShowSystemMenu(object target, ExecutedRoutedEventArgs e)
        {
            var window = target as Window;

            SystemCommands.ShowSystemMenu(window,
                Mouse.GetPosition(
                    Mouse.PrimaryDevice.Target));
        }

        #endregion
    }
}
