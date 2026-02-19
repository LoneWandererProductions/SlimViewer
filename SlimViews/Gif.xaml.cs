/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        Gif.xaml.cs
 * PURPOSE:     Gif Editor Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using CommonControls.Images;
using SlimViews.Interfaces;
using System;
using System.ComponentModel;
using System.Windows;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Gif Window
    /// </summary>
    public sealed partial class Gif : IClosableByCommand
    {
        /// <summary>
        /// Gets or sets the request close action.
        /// </summary>
        /// <value>
        /// The request close action.
        /// </value>
        public Action? RequestCloseAction { get; set; }

        /// <summary>
        ///     The view
        /// </summary>
        private GifView _view;


        /// <summary>
        /// Initializes a new instance of the <see cref="Gif"/> class.
        /// </summary>
        public Gif()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _view = new GifView(Thumb);

            DataContext = _view;
        }

        /// <summary>
        ///     Thumbs the image clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void Thumb_ImageClicked(object sender, ImageEventArgs itemId)
        {
            _view.ChangeImage(itemId.Id);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Raises the <see cref="Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            _view.CloseCommand.Execute(null); // VM will decide if it's allowed
        }

        /// <summary>
        /// Called when [close click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            RequestCloseAction?.Invoke(); // ask command class to close
        }
    }
}