using System;

namespace TiqUtils.Wpf.UIBuilders
{
    public class SliderLimitsAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }

        public SliderLimitsAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}