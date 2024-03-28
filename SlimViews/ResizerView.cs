/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ResizerView.cs
 * PURPOSE:     View Model for Resizer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global, if we make it private the Property Changed event will not be triggered in the Window
// ReSharper disable MemberCanBeInternal, must be public, else the View Model won't work

using System.Collections.Generic;
using System;
using Imaging;
using System.ComponentModel;

namespace SlimViews
{
    /// <summary>
    /// View for Resizer
    /// TODO:
    /// Add Resize Options
    /// Add optional Filters
    /// Add File Converter
    /// </summary>
    internal sealed class ResizerView : INotifyPropertyChanged
    {
        /// <summary>
        /// The size format
        /// </summary>
        private bool _sizeFormat;

        /// <summary>
        ///     Gets or sets a value indicating whether which [Size Format] to use.
        /// </summary>
        /// <value>s
        ///     <c>true</c> if [Percentage]; otherwise, <c>false</c> [Relative size].
        /// </value>
        public bool SizeFormat
        {
            get => _sizeFormat;
            set
            {
                if (_sizeFormat == value) return;

                _sizeFormat = value;
                OnPropertyChanged(nameof(SizeFormat));
            }
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <value>
        /// The filter options.
        /// </value>
        public IEnumerable<ImageFilter> FilterOptions => Enum.GetValues(typeof(ImageFilter)) as IEnumerable<ImageFilter>;

        /// <inheritdoc />
        /// <summary>
        ///     Triggers if an Attribute gets changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
