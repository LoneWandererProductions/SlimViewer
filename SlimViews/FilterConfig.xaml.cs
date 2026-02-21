/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        FilterConfig.xaml.cs
 * PURPOSE:     Configuration for our Filters
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using Imaging.Enums;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Configuration for our Filters
    /// </summary>
    public partial class FilterConfig
    {
        /// <summary>
        /// The view model
        /// </summary>
        private readonly FilterConfigView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterConfig" /> class.
        /// </summary>
        public FilterConfig()
        {
            InitializeComponent();
            _viewModel = new FilterConfigView();
            DataContext = _viewModel;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterConfig" /> class.
        /// </summary>
        /// <param name="filter">The filter type to pre-select.</param>
        public FilterConfig(FiltersType filter)
        {
            InitializeComponent();
            _viewModel = new FilterConfigView();
            DataContext = _viewModel;

            // Explicitly set the filter on the instance we just created
            _viewModel.SelectedFilter = filter;
        }
    }
}