/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/MainWindow.xaml.cs
 * PURPOSE:     Main Window for the SlimViewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBeInternal

using System.ComponentModel;
using System.Windows;
using SlimViews;

namespace SlimViewer
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Main Window
    /// </summary>
    public sealed partial class MainWindow
    {
        /// <summary>
        ///     The view
        /// </summary>
        private ImageView _view;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = Config.GetConfig();

            SlimViewerRegister.SetRegister(obj);

            //TODO rework and set:
            //_view = new ImageView(obj.MainSubFolders, obj.MainCompressCif, obj.MainSimilarity,
            //    obj.MainAutoClean, ImageControl) { UiState = { Main = this, Thumb = Thumbnail }, Picker = ColorPick };

            _view = new ImageView(obj.MainSubFolders, obj.MainCompressCif, obj.MainSimilarity,
                obj.MainAutoClean, ImageControl, this, Thumbnail, ColorPick);
           
            DataContext = _view;

            ImageControl.AutoplayGifImage = obj.MainAutoPlayGif;
        }

        /// <summary>
        ///     Handles the Drop event of the Image control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragEventArgs" /> instance containing the event data.</param>
        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length == 0 || _view == null) return;

            _view.IsImageActive = true;

            if (files.Length == 1) _view.ChangeImage(files[0]);
            else _view.ChangeImage(files);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Raises the <see cref="Window.Closing" /> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            if (_view == null) return;

            _view.Commands.Close.Execute(null);
        }
    }
}