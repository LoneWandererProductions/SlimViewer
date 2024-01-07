/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Converter.xaml.cs
 * PURPOSE:     Basic Converter Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;

namespace SlimViews
{
    /// <summary>
    ///     Window for converting all Files
    /// </summary>
    /// <seealso cref="Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <inheritdoc cref="Window" />
    internal sealed partial class Converter
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Converter" /> class.
        /// </summary>
        public Converter()
        {
            InitializeComponent();
        }
    }
}