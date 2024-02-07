using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TiqUtils.Wpf.UIBuilders.Proxy;

namespace TiqUtils.Wpf.UIBuilders
{
    // ReSharper disable once InconsistentNaming
    public static class AutoUIExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static Button NewAutoUIButton(this object obj, string caption = "Open")
        {
            return SettingsAutoUI.CreateSettingsClassController(obj, caption);
        }

        // ReSharper disable once InconsistentNaming
        public static void OpenAutoUISettingsDialog(this object obj)
        {
            var settingsPage = CreateAutoUISettingsDialog(obj);
            settingsPage.ShowDialog();
        }

        // ReSharper disable once InconsistentNaming
        public static SettingsAutoUI CreateAutoUISettingsDialog(this object obj)
        {
            var builderName = GetBuilderClassName(obj);

            if (!string.IsNullOrWhiteSpace(builderName))
            {
                return TypeHelper.CreateClassOfTypeByName<SettingsAutoUI>(builderName, obj);
            }

            var settingsPage = new SettingsAutoUI(obj)
            {
                Topmost = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            return settingsPage;
        }

        // ReSharper disable once InconsistentNaming
        public static SettingsAutoControl CreateAutoSettingsControl(this object obj, Panel parentControl)
        {
            var builderName = GetBuilderClassName(obj);

            if (!string.IsNullOrWhiteSpace(builderName))
            {
                return TypeHelper.CreateClassOfTypeByName<SettingsAutoControl>(builderName, obj);
            }

            var settingsPage = new SettingsAutoControl(obj, parentControl);
            return settingsPage;
        }

        private static string GetBuilderClassName(object o)
        {
            if (o is NotifyPropertyChangedProxy p)
            {
                o = p.WrappedObject;
            }

            var uiBuilderAttribute = o.GetType().GetCustomAttribute<UIBuilderAttribute>();

            return uiBuilderAttribute?.BuilderTypeName ?? "";
        }
    }
}
