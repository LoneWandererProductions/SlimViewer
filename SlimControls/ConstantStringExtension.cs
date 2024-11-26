/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimControls/ConstantStringExtension.cs
 * PURPOSE:     Convert string into xaml string
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Windows.Markup;

namespace SlimControls
{
    /// <inheritdoc />
    /// <summary>
    ///     Convert string constant into xaml string.
    /// </summary>
    /// <seealso cref="T:System.Windows.Markup.MarkupExtension" />
    public sealed class ConstantStringExtension : MarkupExtension
    {
        /// <summary>
        ///     Gets or sets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        ///     When implemented in a derived class, returns an object that is provided as the value of the target property for
        ///     this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        ///     The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return typeof(ViewGuiResources).GetField(Key)?.GetValue(null);
        }
    }
}