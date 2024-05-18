/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/DetailCompare.xaml.cs
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
    public sealed partial class DetailCompare
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="DetailCompare" /> class.
        /// </summary>
        public DetailCompare()
        {
            InitializeComponent();
            View.RtBoxInformation = Information;
            View.TxtBoxColorInformation = ColorInformation;
        }
    }
}