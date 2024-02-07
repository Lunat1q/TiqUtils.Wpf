using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interop;
using TiqUtils.Wpf.Converters;
using TiqUtils.Wpf.Helpers;
using TiqUtils.Wpf.UIBuilders.Proxy;

namespace TiqUtils.Wpf.UIBuilders
{
    // ReSharper disable once InconsistentNaming
    public class SettingsAutoControl : FrameworkElement
    {
        protected readonly object Settings;
        private Grid _settingsGrid;

        protected internal SettingsAutoControl(object settingsClass, Panel targetContainerElement)
        {
            this.Settings = settingsClass;
            this.DataContext = settingsClass;
            this.InitializeComponent(targetContainerElement);
            this.GenerateBindingControllers();
        }

        private void InitializeComponent(Panel targetContainerElement)
        {
            this.InitControlSettings();

            var baseGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            baseGrid.RowDefinitions.Add(new RowDefinition());
            baseGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            this._settingsGrid = new Grid();
            baseGrid.Children.Add(this._settingsGrid);

            targetContainerElement.Children.Add(baseGrid);
        }

        private Type GetObjectType()
        {
            if (this.Settings is NotifyPropertyChangedProxy proxy)
            {
                return proxy.GetWrappedType();
            }

            return this.Settings.GetType();
        }

        private void GenerateBindingControllers()
        {
            var properties = this.GetObjectType().GetProperties();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var mainGridIdx = 0;
            foreach (var propGroup in properties
                .Where(x => x.GetCustomAttribute<PropertyMemberAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<PropertyOrderAttribute>()?.Order ?? 999)
                .GroupBy(x => x.GetCustomAttribute<PropertyGroupAttribute>()?.GroupName))
            {
                var curIdx = 0;
                var isGroup = !string.IsNullOrWhiteSpace(propGroup.Key);
                var parentGrid = grid;
                if (isGroup)
                {
                    var gb = new GroupBox
                    {
                        Header = propGroup.Key,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    var groupGrid = new Grid();
                    gb.Content = groupGrid;
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.Children.Add(gb);
                    Grid.SetColumn(gb, 0);
                    Grid.SetRow(gb, mainGridIdx++);
                    Grid.SetColumnSpan(gb, 2);
                    groupGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    groupGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    parentGrid = groupGrid;
                }
                else
                {
                    curIdx = mainGridIdx;
                }

                foreach (var prop in propGroup)
                {
                    parentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    var nameAttribute = prop.GetCustomAttribute<DisplayNameAttribute>();
                    var name = nameAttribute?.DisplayName ?? this.GetBeautyPropName(prop);
                    var text = CreateLabel(name);
                    parentGrid.Children.Add(text);
                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, curIdx);

                    var e = this.CreatePropertyUiElement(text, prop);

                    Grid.SetColumn(e, 1);
                    Grid.SetRow(e, curIdx);
                    parentGrid.Children.Add(e);
                    curIdx++;
                }

                if (!isGroup)
                {
                    mainGridIdx = curIdx;
                }
            }

            this._settingsGrid.Children.Add(grid);
            this.AfterBuild(grid);
        }

        protected static TextBlock CreateLabel(string name)
        {
            return new TextBlock { Text = name, Margin = new Thickness(5) };
        }

        protected virtual void AfterBuild(Grid grid)
        {
            // to be overriden
        }

        protected virtual UIElement CreatePropertyUiElement(TextBlock labelTextBlock, PropertyInfo prop)
        {
            UIElement e;

            switch (prop.PropertyType)
            {
                case Type _ when prop.PropertyType == typeof(bool):
                    e = this.CreateBooleanController(prop);
                    break;
                case Type _ when prop.PropertyType.IsEnum:
                    e = this.CreateEnumController(prop);
                    break;
                case Type _ when prop.PropertyType == typeof(double):
                case Type _ when prop.PropertyType == typeof(float):
                    e = this.CreateDoubleController(prop);
                    break;
                case Type _ when prop.PropertyType == typeof(int):
                    e = this.CreateIntController(prop);
                    break;
                case Type _ when prop.PropertyType == typeof(char):
                case Type _ when prop.PropertyType == typeof(string):
                    e = this.CreateTextBlockController(prop);
                    break;
                case Type _ when prop.PropertyType.IsClass:
                    e = CreateSettingsClassController(this.GetPropertyValue(prop));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return e;
        }

        protected virtual TextBox CreateTextBlockController(PropertyInfo prop)
        {
            var ret = new TextBox
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            ret.Text = (string)prop.GetValue(this.Settings);

            ret.TextChanged += (sender, args) =>
            {
                prop.SetValue(this.Settings, ((TextBox) sender).Text);
            };

            //var valueBinding = new Binding
            //{
            //    Path = new PropertyPath(prop.Name, Array.Empty<object>())
            //};
            //ret.SetBinding(TextBox.TextProperty, valueBinding);
            return ret;
        }

        private object GetPropertyValue(PropertyInfo prop)
        {
            if (this.Settings is NotifyPropertyChangedProxy proxy)
            {
                return prop.GetValue(proxy.WrappedObject);
            }
            return prop.GetValue(this.Settings);
        }

        protected virtual string GetBeautyPropName(PropertyInfo prop)
        {
            var name = prop.Name;
            var sb = new StringBuilder();
            
            foreach (var c in name)
            {
                var curChar = c;
                if (char.IsUpper(curChar))
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(' ');
                        curChar = char.ToLower(c);
                    }
                }
                sb.Append(curChar);
            }
            return sb.ToString();
        }

        protected virtual void InitControlSettings()
        {
            //var type = GetObjectType();
            //var formName = type.GetCustomAttribute<DisplayNameAttribute>();
            //WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //Topmost = true;
            //Title = formName?.DisplayName ?? type.Name;
            //MinWidth = 300.0;
            //MinHeight = 100.0;
            //SizeToContent = SizeToContent.WidthAndHeight;
            //ResizeMode = ResizeMode.NoResize;
            //WindowStyle = WindowStyle.ToolWindow;
            //Loaded += SettingsAutoUI_Loaded;
        }

        protected static Button CreateButton(string buttonText)
        {
            return new Button
            {
                Content = buttonText,
                Margin = new Thickness(5)
            };
        }

        public static Button CreateSettingsClassController(object obj, string buttonText = "Open")
        {
            var button = CreateButton(buttonText);

            button.Click += (sender, args) =>
            {
                if (!(obj is INotifyPropertyChanged))
                {
                    obj = new NotifyPropertyChangedProxy(obj);
                }
                var settingsPage = new SettingsAutoUI(obj)
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                settingsPage.ShowDialog();

            };

            return button;
        }

        protected virtual ComboBox CreateEnumController(PropertyInfo prop)
        {
            var cb = new ComboBox
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                DisplayMemberPath = nameof(ValueDescription.Description),
                SelectedValuePath = nameof(ValueDescription.Value)
            };
            var collectionBinding = new Binding
            {
                Path = new PropertyPath(prop.Name),
                Converter = EnumToCollectionConverter.Instance
            };
            cb.SetBinding(ItemsControl.ItemsSourceProperty, collectionBinding);
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name),
                Converter = EnumValueConverter.Instance
            };
            cb.SetBinding(Selector.SelectedItemProperty, valueBinding);
            return cb;
        }

        protected virtual CheckBox CreateBooleanController(PropertyInfo prop)
        {
            var cb = new CheckBox
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            cb.IsChecked = (bool)prop.GetValue(this.Settings);

            cb.Checked += (sender, args) =>
            {
                prop.SetValue(this.Settings, true);
            };

            cb.Unchecked += (sender, args) =>
            {
                prop.SetValue(this.Settings, false);
            };

            //var valueBinding = new Binding
            //{
            //    Path = new PropertyPath(prop.Name)
            //};
            //cb.SetBinding(ToggleButton.IsCheckedProperty, valueBinding);
            return cb;
        }

        protected virtual Slider CreateDoubleController(PropertyInfo prop)
        {
            var limits = prop.GetCustomAttribute<SliderLimitsAttribute>();
            var ret = new Slider
            {
                AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
                Maximum = limits?.Max ?? 10,
                Minimum = limits?.Min ?? 1,
                TickFrequency = 0.05,
                AutoToolTipPrecision = 2,
                VerticalAlignment = VerticalAlignment.Center
            };
            ret.Value = (double)prop.GetValue(this.Settings);
            ret.ValueChanged += (sender, args) => { prop.SetValue(this.Settings, ((Slider) sender).Value); };

            //var valueBinding = new Binding
            //{
            //    Path = new PropertyPath(prop.Name)
            //};
            //ret.SetBinding(RangeBase.ValueProperty, valueBinding);
            return ret;
        }

        protected virtual UIElement CreateIntController(PropertyInfo prop)
        {
            var limits = prop.GetCustomAttribute<SliderLimitsAttribute>();
            UIElement result;
            if (limits != null)
            {
                var slider = new Slider
                {
                    AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
                    Maximum = limits.Max,
                    Minimum = limits.Min,
                    TickFrequency = (int)limits.TickFrequency,
                    LargeChange = (int)limits.LargeChange,
                    AutoToolTipPrecision = 0,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var valueBinding = new Binding
                {
                    Path = new PropertyPath(prop.Name)
                };
                slider.SetBinding(RangeBase.ValueProperty, valueBinding);
                result = slider;
            }
            else
            {
                result = this.CreateTextBlockController(prop);
            }
            return result;
        }

        #region PInvoke
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GwlStyle = -16;

        private const int WsSysMenu = 0x80000;
        #endregion
    }
}
