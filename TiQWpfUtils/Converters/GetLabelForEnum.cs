using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using TiQWpfUtils.Controls.Extensions.DataGrid;

namespace TiQWpfUtils.Converters
{
    public sealed class GetLabelForEnum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as Enum;
            if (val != null)
            {
                var bVal = val;
                var type = bVal.GetType();
                var memInfo = type.GetMember(bVal.ToString());
                var attribute = memInfo[0].GetCustomAttribute(typeof(LabelNameAttributeBase), false) as LabelNameAttributeBase;
                if (attribute != null)
                {
                    return attribute.GetProperText();
                }
            }
            return value?.ToString() ?? "%empty_label%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
