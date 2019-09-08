using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using TiqUtils.Wpf.Helpers;

namespace TiqUtils.Wpf.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<ValueDescription>> DescriptorCache = new ConcurrentDictionary<string, IEnumerable<ValueDescription>>();

        private static readonly Lazy<EnumToCollectionConverter> LazyInstance = new Lazy<EnumToCollectionConverter>(() => new EnumToCollectionConverter());

        public static EnumToCollectionConverter Instance => LazyInstance.Value;

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var skip = false;
            if (parameter is bool b)
            {
                skip = b;
            }
            return TryGetFromCache(value?.GetType(), skip);
        }

        protected IEnumerable<ValueDescription> TryGetFromCache(Type type, bool skip)
        {
            var key = GenerateKey(type, skip);
            if (DescriptorCache.ContainsKey(key))
            {
                return DescriptorCache[key];
            }
            var desc = EnumHelper.GetAllValuesAndDescriptions(type, skip).ToList();
            DescriptorCache.TryAdd(key, desc);
            return desc;
        }

        private string GenerateKey(Type type, bool skip)
        {
            return type.FullName + (skip ? "!" : "#");
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
