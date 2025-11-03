using System;
using System.Globalization;
using System.Windows.Data;

namespace SlimControls
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return null;
            return (bool)value ? Enum.Parse(targetType, parameter.ToString()) : Binding.DoNothing;
        }
    }
}
