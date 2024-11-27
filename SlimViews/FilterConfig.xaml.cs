﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/FilterConfig.xaml.cs
 * PURPOSE:     Basic Converter Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using Imaging;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Configuration for our Filters
    /// </summary>
    public partial class FilterConfig
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.FilterConfig" /> class.
        /// </summary>
        public FilterConfig()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterConfig" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public FilterConfig(ImageFilters filter)
        {
            InitializeComponent();
            FilterView.SelectedFilter = filter;
        }
    }
}