using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SlimViews.Templates
{
    public sealed class FillTypeVisibilityConverter : IValueConverter
    {
        /// <inheritdoc />
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedIndex && parameter is string targetFillType)
            {
                switch (targetFillType)
                {
                    case "SolidColor":
                        return selectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
                    case "Texture":
                        return selectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
                    case "Filter":
                        return selectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
                    case "TextureConfig":
                        // Check if a texture is selected in the TextureCombobox
                        return selectedIndex >= 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}