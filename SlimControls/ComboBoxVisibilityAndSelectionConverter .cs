﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimControls/ComboBoxVisibilityAndSelectionConverter.cs
 * PURPOSE:     A bit of helper to handle the dynamic Menu.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SlimControls
{
    /// <inheritdoc />
    /// <summary>
    ///     Manages Visibility of the config Button.
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IMultiValueConverter" />
    public sealed class ComboBoxVisibilityAndSelectionConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        /// <summary>
        ///     Converts source values to a value for the binding target. The data binding engine calls this method when it
        ///     propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">
        ///     The array of values that the source bindings in the
        ///     <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value
        ///     <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to
        ///     provide for conversion.
        /// </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     A converted value.
        ///     If the method returns <see langword="null" />, the valid <see langword="null" /> value is used.
        ///     A return value of <see cref="T:System.Windows.DependencyProperty" />.
        ///     <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value,
        ///     and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is
        ///     available, or else will use the default value.
        ///     A return value of <see cref="T:System.Windows.Data.Binding" />.
        ///     <see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or
        ///     use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is Visibility visibility &&
                values[1] is int selectedIndex)
                // Return Visible only if ComboBox is visible and has a valid selection
                return visibility == Visibility.Visible && selectedIndex >= 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">
        ///     The array of types to convert to. The array length indicates the number and types of values
        ///     that are suggested for the method to return.
        /// </param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        ///     An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}