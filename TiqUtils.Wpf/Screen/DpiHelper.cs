using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TiqUtils.Wpf.Screen
{
    public class DpiHelper
    {
        public const float DefaultDpi = 96;

        [DllImport("Shcore")]
        private static extern int GetProcessDpiAwareness(IntPtr hprocess, out ProcessDpiAwareness awareness);

        public static int GetDpiForWindow(IntPtr hwnd)
        {
            try
            {
                IntPtr hmonitor = MonitorFromWindow(hwnd, MonitorFromWindowFlags.DefaultToNearest);
                if (GetDpiForMonitor(hmonitor, MonitorDpiTypes.EffectiveDPI, out int newDpiX, out int newDpiY) != 0)
                {
                    return 96;
                }
                return newDpiX;
            }
            catch
            {
                Graphics graphics = Graphics.FromHwnd(hwnd);
                float dpiXX = graphics.DpiX;
                return Convert.ToInt32(dpiXX);
            }
        }

        [DllImport("Shcore")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, MonitorDpiTypes dpiType, out int dpiX,
            out int dpiY);

        private enum MonitorDpiTypes
        {
            EffectiveDPI = 0,
            AngularDPI = 1,
            RawDPI = 2,
        }

        public static int GetSystemDpi()
        {
            int newDpiX = 0;
            IntPtr dc = GetDC(IntPtr.Zero);
            newDpiX = GetDeviceCaps(dc, LOGPIXELSX);
            ReleaseDC(IntPtr.Zero, dc);
            return newDpiX;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private const int LOGPIXELSX = 88;

        public static IntPtr GetMonitorFromWindow(Window wnd)
        {
            var source = (HwndSource) PresentationSource.FromVisual(wnd);
            return MonitorFromWindow(source.Handle, MonitorFromWindowFlags.DefaultToNearest);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFromWindowFlags dwFlags);

        [Flags]
        private enum MonitorFromWindowFlags
        {
            DefaultToNull = 0,
            DefaultToPrimary = 1,
            DefaultToNearest = 2,
        }
    }
}