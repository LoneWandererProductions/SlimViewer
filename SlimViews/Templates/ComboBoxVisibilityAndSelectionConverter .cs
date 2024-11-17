using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SlimViews.Templates
{
    public class ComboBoxVisibilityAndSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is Visibility visibility &&
                values[1] is int selectedIndex)
            {
                // Return Visible only if ComboBox is visible and has a valid selection
                return visibility == Visibility.Visible && selectedIndex >= 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}