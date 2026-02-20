/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        DetailCompare.xaml.cs
 * PURPOSE:     DetailCompare Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System.Windows;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Compares two images in detail
    /// </summary>
    internal sealed partial class DetailCompare : Window
    {
        private readonly DetailCompareView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="DetailCompare" /> class.
        /// </summary>
        public DetailCompare()
        {
            InitializeComponent();
            _viewModel = new DetailCompareView();
            DataContext = _viewModel;

            _viewModel.RtBoxInformation = Information;
            _viewModel.TxtBoxColorInformation = ColorInformation;
        }
    }
}