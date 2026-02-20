/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Compare.xaml.cs
 * PURPOSE:     Basic Compare Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using CommonControls.Images;
using System.Windows;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Result Set for Image Comparer
    /// </summary>
    internal sealed partial class Compare : Window
    {
        private readonly CompareView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Compare" /> class.
        /// </summary>
        public Compare()
        {
            InitializeComponent();
            _viewModel = new CompareView();
            DataContext = _viewModel;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Compare" /> class.
        /// </summary>
        public Compare(bool subFolders, string currentFolder, ImageView imageView, int similarity = 0)
        {
            InitializeComponent();
            _viewModel = new CompareView();
            DataContext = _viewModel;

            _ = _viewModel.AsyncInitiate(subFolders, currentFolder, imageView, similarity);
        }

        /// <summary>
        ///     Called when [image clicked].
        /// </summary>
        private void OnImageClicked(object sender, ImageEventArgs e)
        {
            if (sender is FrameworkElement element && int.TryParse(element.Tag.ToString(), out var observerIndex))
            {
                _viewModel.ChangeImage(e.Id, observerIndex);
            }
        }
    }
}