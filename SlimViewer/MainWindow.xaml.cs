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

            View = new ImageView(obj.MainSubFolders, obj.MainCompressCif, obj.MainSimilarity,
                obj.MainAutoClean) { Main = this, Thumb = Thumbnail, Picker = ColorPick, ImageZoom = ImageZoom };

            DataContext = View;

            ImageZoom.AutoplayGifImage = obj.MainAutoPlayGif;
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

            if (files == null || files.Length == 0) return;

            View.IsActive = true;

            if (files.Length == 1) View.ChangeImage(files[0]);
            else View.ChangeImage(files);
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