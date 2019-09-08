using System;

namespace TiqUtils.Wpf.UIBuilders
{
    public class SliderLimitsAttribute : Attribute
    {
        public float Min { get; }

        public float Max { get; }

        public int ToolTipPrecision { get; }

        public float TickFrequency { get; }

        public float LargeChange { get; }

        public SliderLimitsAttribute(float min, float max, int toolTipPrecision = 1, float tickFrequency = 0.1f, float largeChange = 0.1f)
        {
            this.Min = min;
            this.Max = max;
            this.ToolTipPrecision = toolTipPrecision;
            this.TickFrequency = tickFrequency;
            this.LargeChange = largeChange;
        }
    }
}