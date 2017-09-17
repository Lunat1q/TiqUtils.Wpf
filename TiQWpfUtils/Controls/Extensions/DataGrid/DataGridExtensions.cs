using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;
using TiQWpfUtils.AbstractClasses.Attributes;

namespace TiQWpfUtils.Controls.Extensions.DataGrid
{
    public static class DataGridExtensions
    {
        public static void AutoGeneratingColumnLogic(DataGridAutoGeneratingColumnEventArgs e, bool hideWithDoNotDisplayAttribute = true)
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
                    if (hideWithDoNotDisplayAttribute)
                    {
                        var hideAttribute =
                            propertyDescriptor.Attributes[typeof(DoNotDisplayAttribute)] as DoNotDisplayAttribute;
                        if (hideAttribute != null)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    var nameAttribute = propertyDescriptor.Attributes[typeof(LabelNameAttributeBase)] as LabelNameAttributeBase;
                    if (nameAttribute != null)
                    {
                        e.Column.Header = nameAttribute.GetProperText();
                    }
                }
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private static bool IsCollectionType(Type type)
        {
            return type.GetInterface(nameof(ICollection)) != null;
        }
    }
}