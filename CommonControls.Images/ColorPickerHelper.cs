/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonControls.Images
 * FILE:        ColorPickerHelper.cs
 * PURPOSE:     Helper functions for  ColorPicker
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows.Media;
using System.Windows.Shapes;
using Imaging;

namespace CommonControls.Images
{
    /// <summary>
    ///     Helper Class for the ColorPicker
    /// </summary>
    internal static class ColorPickerHelper
    {
        /// <summary>
        ///     Gets the color preview.
        /// </summary>
        /// <param name="colorHsv">Color Converter class, ColorHsv.</param>
        /// <returns>A Rectangle</returns>
        internal static Rectangle GetColorPreview(ColorHsv colorHsv)
        {
            var rectangle = new Rectangle { Width = 25, Height = 25 };

            var rgb = HsvToRgb(colorHsv.H, colorHsv.S, colorHsv.V);

            var col = Color.FromArgb((byte)colorHsv.A, rgb.R, rgb.G, rgb.B);
            rectangle.Fill = new SolidColorBrush(col);

            return rectangle;
        }

        /// <summary>
        /// H HSV in degrees (0-360), S and V in 0-1
        /// </summary>
        /// <param name="h">The h.</param>
        /// <param name="s">The s.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        private static (byte R, byte G, byte B) HsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;
            double r1 = 0, g1 = 0, b1 = 0;

            if (h < 60) { r1 = c; g1 = x; b1 = 0; }
            else if (h < 120) { r1 = x; g1 = c; b1 = 0; }
            else if (h < 180) { r1 = 0; g1 = c; b1 = x; }
            else if (h < 240) { r1 = 0; g1 = x; b1 = c; }
            else if (h < 300) { r1 = x; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = x; }

            byte R = (byte)((r1 + m) * 255);
            byte G = (byte)((g1 + m) * 255);
            byte B = (byte)((b1 + m) * 255);

            return (R, G, B);
        }
    }
}
