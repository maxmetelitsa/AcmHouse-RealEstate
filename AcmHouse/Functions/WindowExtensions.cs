using System.Windows;
using System.Windows.Input;

namespace AcmHouse.Functions
{
    /// <summary>
    /// Shared behaviour for the application's borderless windows (WindowStyle="None"),
    /// which draw their own title bar with minimize / maximize / close buttons.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>Switches the window between maximized and normal state.</summary>
        public static void ToggleMaximize(this Window window)
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        /// <summary>Toggles maximize on a double click of the custom title bar.</summary>
        public static void ToggleMaximizeOnDoubleClick(this Window window, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                window.ToggleMaximize();
            }
        }

        public static void Minimize(this Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>Lets the user drag the window by its custom title bar.</summary>
        public static void DragWithLeftButton(this Window window, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                window.DragMove();
            }
        }

        /// <summary>Closes every window and exits the application.</summary>
        public static void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
