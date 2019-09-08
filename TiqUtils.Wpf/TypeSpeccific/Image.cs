using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TiqUtils.Wpf.TypeSpeccific
{
    public static class Image
    {
        public static System.Windows.Controls.Image ToImage(this Bitmap source, string toolTipText = "no_text")
        {
            var img = new System.Windows.Controls.Image { Source = source.LoadBitmap() };
            var tt = new ToolTip { Content = toolTipText };
            img.ToolTip = tt;
            return img;
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource LoadBitmap(this Bitmap source)
        {
            var ip = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                    IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
    }
}
