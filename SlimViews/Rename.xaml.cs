/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Rename.xaml.cs
 * PURPOSE:     View Model for Rename
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
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Rename" /> class.
        /// </summary>
        public Rename()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViews.Rename" /> class.
        /// </summary>
        /// <param name="observer">The Dictionary of files.</param>
        public Rename(Dictionary<int, string> observer)
        {
            InitializeComponent();
            View.Observer = new ConcurrentDictionary<int, string>(observer);
        }

        /// <summary>
        ///     Gets the observer. If we Change the Filename we can change them in the Main View too.
        /// </summary>
        /// <value>
        ///     The observer.
        /// </value>
        internal Dictionary<int, string> Observer => new(View.Observer);
    }
}