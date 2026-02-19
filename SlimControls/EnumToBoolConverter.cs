/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        EnumToBoolConverter.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Globalization;
using System.Windows.Data;

namespace SlimControls
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.ToString() == parameter?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Enum.Parse(targetType, parameter.ToString()) : Binding.DoNothing;
    }
}