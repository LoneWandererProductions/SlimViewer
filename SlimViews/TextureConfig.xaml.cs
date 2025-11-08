/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/TextureConfig.xaml.cs
 * PURPOSE:     The main Xaml for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using Imaging;
using Imaging.Enums;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Texture config
    /// </summary>
    public sealed partial class TextureConfig
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.TextureConfig" /> class.
        /// </summary>
        public TextureConfig()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureConfig" /> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public TextureConfig(TextureType texture)
        {
            InitializeComponent();
            TextureView.SelectedTexture = texture;
        }
    }
}