using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using TiqUtils.Wpf.AbstractClasses.Attributes;

namespace TiqUtils.Wpf.Controls.Extensions.DataGrid
{
    public static class DataGridExtensions
    {
        public static void AutoGeneratingColumnLogic(DataGridAutoGeneratingColumnEventArgs e, bool hideWithDoNotDisplayAttribute = true, params string[] onlyPropertiesToShow)
        {
            if (IsCollectionType(e.PropertyType))
            {
                e.Cancel = true;
            }
            else
            {
                var propertyDescriptor = e.PropertyDescriptor as PropertyDescriptor;
                if (propertyDescriptor != null)
                {
                    if (onlyPropertiesToShow.Length > 0)
                    {
                        if (!onlyPropertiesToShow.Contains(propertyDescriptor.Name))
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    if (hideWithDoNotDisplayAttribute)
                    {
                        var hideAttribute = GetPropertyAttributeFromColumnGeneration<DoNotDisplayAttribute>(e);
                        if (hideAttribute != null)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    var nameAttribute = GetPropertyAttributeFromColumnGeneration<LabelNameAttributeBase>(e);
                    if (nameAttribute != null)
                    {
                        e.Column.Header = nameAttribute.GetProperText();
                    }
                }
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                e.Column.MinWidth = 75;
            }
        }

        public static T GetPropertyAttributeFromColumnGeneration<T>(this DataGridAutoGeneratingColumnEventArgs e) where T:Attribute
        {
            var propertyDescriptor = e.PropertyDescriptor as PropertyDescriptor;
            return (T) propertyDescriptor?.Attributes[typeof(T)];
        }

        private static bool IsCollectionType(Type type)
        {
            return type.GetInterface(nameof(ICollection)) != null;
        }
    }
}