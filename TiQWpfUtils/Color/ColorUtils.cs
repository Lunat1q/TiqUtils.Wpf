using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TiQWpfUtils
{
    public static class ColorUtils
    {
        public static Color GetColorFromLevel(int level)
        {
            Color clr;
            switch (level)
            {
                case 0:
                    clr = Color.FromRgb(255, 0, 0);
                    break;
                case 1:
                    clr = Color.FromRgb(0, 0, 0);
                    break;
                case 2:
                    clr = Color.FromRgb(0, 20, 93);
                    break;
                default:
                    clr = Color.FromRgb(0, 0, 0);
                    break;
            }
            return clr;
        }
    }
}
