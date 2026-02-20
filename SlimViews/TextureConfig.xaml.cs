/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        TextureConfig.xaml.cs
 * PURPOSE:     The main Xaml for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using Imaging.Enums;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Texture config Window
    /// </summary>
    public sealed partial class TextureConfig
    {
        private readonly TextureConfigView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.TextureConfig" /> class.
        /// </summary>
        public TextureConfig()
        {
            InitializeComponent();
            _viewModel = new TextureConfigView();
            DataContext = _viewModel;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureConfig" /> class with a specific texture type.
        /// </summary>
        /// <param name="texture">The texture type to select upon opening.</param>
        public TextureConfig(TextureType texture)
        {
            InitializeComponent();
            _viewModel = new TextureConfigView();
            DataContext = _viewModel;

            // Now we set the property on our explicit ViewModel instance
            _viewModel.SelectedTexture = texture;
        }
    }
}