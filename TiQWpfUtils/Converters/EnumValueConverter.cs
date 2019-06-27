using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TiQWpfUtils.Helpers;

namespace TiQWpfUtils.Converters
{
    [ValueConversion(typeof(Enum), typeof(ValueDescription))]
    public class EnumValueConverter : EnumToCollectionConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var skip = false;
            if (parameter is bool b)
            {
                skip = b;
            }
            var enumCollection = TryGetFromCache(value?.GetType(), skip);

            var valueDescription = enumCollection.FirstOrDefault(x => Equals(x.Value, value));

            return valueDescription;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ValueDescription vd))
            {
                return value;
            }

            return vd.Value;
        }
    }
}