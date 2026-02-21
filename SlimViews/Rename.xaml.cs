/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Rename.xaml.cs
 * PURPOSE:     Code-behind for Rename Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Basic Rename Window
    /// </summary>
    internal sealed partial class Rename
    {
        // 1. Create a strongly-typed reference to your ViewModel
        private readonly RenameView _viewModel;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Rename" /> class.
        /// </summary>
        public Rename()
        {
            InitializeComponent();

            // 2. Instantiate and assign the DataContext manually
            _viewModel = new RenameView();
            DataContext = _viewModel;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.Rename" /> class.
        /// </summary>
        /// <param name="observer">The Dictionary of files.</param>
        public Rename(Dictionary<int, string> observer)
        {
            InitializeComponent();

            // 2. Instantiate, inject the data, and assign the DataContext
            _viewModel = new RenameView
            {
                Observer = new ConcurrentDictionary<int, string>(observer)
            };
            DataContext = _viewModel;
        }

        /// <summary>
        ///     Gets the observer. If we Change the Filename we can change them in the Main View too.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        internal Dictionary<int, string> Observer => new(_viewModel.Observer);
    }
}