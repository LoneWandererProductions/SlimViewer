/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Search.xaml.cs
 * PURPOSE:     Search Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using Imaging;
using System.Windows;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Search Window
    /// </summary>
    public sealed partial class Search
    {
        /// <summary>
        /// The view model
        /// </summary>
        private readonly SearchView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.Search" /> class.
        /// </summary>
        public Search()
        {
            InitializeComponent();
            _viewModel = new SearchView();
            DataContext = _viewModel;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.Search" /> class.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView">The image view.</param>
        /// <param name="color">The color.</param>
        public Search(bool subFolders, string currentFolder, ImageView imageView, ColorHsv color)
        {
            InitializeComponent();
            _viewModel = new SearchView();
            DataContext = _viewModel;

            _viewModel.Initialize(subFolders, currentFolder, imageView, color);
        }
    }
}