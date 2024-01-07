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
        /// <param name="imageView"></param>
        public Compare(bool subFolders, string currentFolder, ImageView imageView)
        {
            InitializeComponent();
            _ = View.AsyncInitiate(subFolders, currentFolder, imageView);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Compare" /> class.
        /// </summary>
        /// <param name="subFolders">if set to <c>true</c> [sub folders].</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="imageView"></param>
        /// <param name="similarity">The similarity, in Percentages</param>
        public Compare(bool subFolders, string currentFolder, ImageView imageView, int similarity)
        {
            InitializeComponent();
            _ = View.AsyncInitiate(subFolders, currentFolder, imageView, similarity);
        }

        /// <summary>
        ///     Observers the first image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void ObserverFirst_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 0);
        }

        /// <summary>
        ///     Observers the second image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void ObserverSecond_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 1);
        }

        /// <summary>
        ///     Observers the third image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void ObserverThird_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 2);
        }

        /// <summary>
        ///     Observers the fourth image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void ObserverFourth_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 3);
        }

        /// <summary>
        ///     Observers the fifth image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void ObserverFifth_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 4);
        }

        /// <summary>
        ///     Observers the sixth image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ObserverSixth_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 5);
        }

        /// <summary>
        ///     Observers the seventh image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ObserverSeventh_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 6);
        }

        /// <summary>
        ///     Observers the eight image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ObserverEight_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 7);
        }

        /// <summary>
        ///     Observers the ninth image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ObserverNinth_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 8);
        }

        /// <summary>
        ///     Observers the tenth image clicked.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ObserverTenth_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id, 9);
        }
    }
}