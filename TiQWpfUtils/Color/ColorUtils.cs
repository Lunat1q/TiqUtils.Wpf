namespace TiQWpfUtils.Color
{
    public static class ColorUtils
    {
        public static System.Windows.Media.Color GetColorFromLevel(int level)
        {
            System.Windows.Media.Color clr;
            switch (level)
            {
                case 0:
                    clr = System.Windows.Media.Color.FromRgb(255, 0, 0);
                    break;
                case 1:
                    clr = System.Windows.Media.Color.FromRgb(0, 0, 0);
                    break;
                case 2:
                    clr = System.Windows.Media.Color.FromRgb(0, 20, 93);
                    break;
                default:
                    clr = System.Windows.Media.Color.FromRgb(0, 0, 0);
                    break;
            }
            return clr;
        }
    }
}
