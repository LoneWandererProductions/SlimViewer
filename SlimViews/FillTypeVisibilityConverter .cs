using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SlimViews
{
    public class FillTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && parameter is string targetFillType)
                switch (targetFillType)
                {
                    case "SolidColor":
                        return selectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
                    case "Texture":
                        return selectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
                    case "Filter":
                        return selectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
                }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}