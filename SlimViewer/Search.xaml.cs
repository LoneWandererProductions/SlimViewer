/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/Search.xaml.cs
 * PURPOSE:     Search Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


// ReSharper disable MemberCanBeInternal

using Imaging;

namespace SlimViewer
{
    /// <inheritdoc cref="Search" />
    /// <summary>
    ///     Search Window
    /// </summary>
    public sealed partial class Search
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViewer.Search" /> class.
        /// </summary>
        public Search()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:SlimViewer.Search" /> class.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView">The image view.</param>
        /// <param name="color">The color.</param>
        public Search(bool subFolders, string currentFolder, ImageView imageView, ColorHsv color)
        {
            InitializeComponent();

            View.Initiate(subFolders, currentFolder, imageView, color);
        }
    }
}