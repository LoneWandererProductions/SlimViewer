/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonControls
 * FILE:        CommonControls/FocusExtension.cs
 * PURPOSE:     Helper Extension to check if something has focus.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;

namespace CommonControls
{
    /// <summary>
    /// Helper to check where the focus is
    /// </summary>
    public static class FocusExtension
    {
        /// <summary>
        /// The is input focused property
        /// </summary>
        public static readonly DependencyProperty IsInputFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsInputFocused",
                typeof(bool),
                typeof(FocusExtension),
                new FrameworkPropertyMetadata(false, OnIsInputFocusedChanged));

        /// <summary>
        /// Gets the is input focused.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetIsInputFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsInputFocusedProperty);
        }

        /// <summary>
        /// Sets the is input focused.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsInputFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsInputFocusedProperty, value);
        }

        /// <summary>
        /// Called when [is input focused changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsInputFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element && (bool)e.NewValue)
            {
                element.Focus(); // Set focus
            }
        }
    }
}