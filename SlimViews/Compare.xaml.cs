/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Compare.xaml.cs
 * PURPOSE:     Basic Compare Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Windows;
using CommonControls;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Result Set for Image Comparer
    /// </summary>
    internal sealed partial class Compare
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Compare" /> class.
        /// </summary>
        public Compare()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Compare" /> class.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView">Parent view</param>
        /// <param name="similarity">The similarity, in Percentages</param>
        public Compare(bool subFolders, string currentFolder, ImageView imageView, int similarity = 0)
        {
            InitializeComponent();
            _ = similarity == 0
                ? View.AsyncInitiate(subFolders, currentFolder, imageView)
                : View.AsyncInitiate(subFolders, currentFolder, imageView, similarity);
        }

        /// <summary>
        ///     Called when [image clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void OnImageClicked(object sender, ImageEventArgs e)
        {
            if (sender is FrameworkElement element && int.TryParse(element.Tag.ToString(), out var observerIndex))
                View.ChangeImage(e.Id, observerIndex);
        }
    }
}