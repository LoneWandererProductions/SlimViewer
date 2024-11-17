/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Templates/FillTypeVisibilityConverter.cs
 * PURPOSE:     A bit of helper to handle the dynamic Menu.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

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
            if (value is not int selectedIndex || parameter is not string expectedType) return Visibility.Collapsed;

            switch (expectedType)
            {
                case "Texture" when selectedIndex == 1:
                case "Filter" when selectedIndex == 2:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}