/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Templates/TextureConfigView.cs
 * PURPOSE:     The view for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using System.Windows.Controls;

namespace SlimViews.Templates
{
    /// <inheritdoc />
    /// <summary>
    ///     Tool Template Selector
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.DataTemplateSelector" />
    public sealed class ToolOptionsTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        ///     Gets or sets the paint tool template.
        /// </summary>
        /// <value>
        ///     The paint tool template.
        /// </value>
        public DataTemplate PaintToolTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the erase tool options.
        /// </summary>
        /// <value>
        ///     The erase tool options.
        /// </value>
        public DataTemplate EraseToolTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the color select tool template.
        /// </summary>
        /// <value>
        ///     The color select tool template.
        /// </value>
        public DataTemplate ColorSelectToolTemplate { get; set; }

        /// <summary>
        ///     Gets or sets the area select tool template.
        /// </summary>
        /// <value>
        ///     The area select tool template.
        /// </value>
        public DataTemplate AreaSelectToolTemplate { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        ///     Returns a <see cref="T:System.Windows.DataTemplate" /> or <see langword="null" />. The default value is
        ///     <see langword="null" />.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ImageTools toolType)
                return toolType switch
                {
                    ImageTools.Paint => PaintToolTemplate,
                    ImageTools.Erase => EraseToolTemplate,
                    ImageTools.ColorSelect => ColorSelectToolTemplate,
                    ImageTools.Area => AreaSelectToolTemplate,
                    _ => null
                };

            return null;
        }

        public enum ImageTools
        {
            /// <summary>
            ///     The paint
            /// </summary>
            Paint = 0,

            /// <summary>
            ///     The erase
            /// </summary>
            Erase = 1,

            /// <summary>
            ///     The cut
            /// </summary>
            Cut = 4,

            /// <summary>
            ///     The color select
            /// </summary>
            ColorSelect = 2,

            /// <summary>
            ///     The area
            /// </summary>
            Area = 3
        }
    }
}