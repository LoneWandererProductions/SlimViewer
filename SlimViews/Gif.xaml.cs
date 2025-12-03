/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/Gif.xaml.cs
 * PURPOSE:     Gif Editor Window
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using CommonControls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

namespace SlimViews
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Gif Window
    /// </summary>
    public sealed partial class Gif
    {
        private bool _allowClose = false;   // Allow the window to really close

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
            _view = new GifView(this, Thumb);

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
            if (!_allowClose)
            {
                e.Cancel = true;
                _view.CloseCommand.Execute(null);  // VM will decide if it's allowed
            }
        }
    }
}