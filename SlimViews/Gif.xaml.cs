﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Gif.xaml.cs
 * PURPOSE:     Gif Editor Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System.ComponentModel;
using System.Windows;
using CommonControls;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Gif Window
    /// </summary>
    public sealed partial class Gif
    {
        public Gif()
        {
            InitializeComponent();
            View.Thumbnail = Thumb;
        }

        /// <summary>
        ///     Thumbs the image clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void Thumb_ImageClicked(object sender, ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Raises the <see cref="Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            View.CloseCommand.Execute(null);
        }
    }
}