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
using CommonControls;
using Imaging;
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
            View.Main = this;
            View.Thumb = Thumbnail;
            View.Picker = ColorPick;
            View.ImageZoom = ImageZoom;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var obj = Config.GetConfig();

            SlimViewerRegister.SetRegister(obj);

            ImageZoom.AutoplayGifImage = obj.MainAutoPlayGif;
        }

        /// <summary>
        ///     Thumbs the image clicked. Easier to handle than in the view model.
        /// </summary>
        /// <param name="itemId">The <see cref="ImageEventArgs" /> instance containing the event data.</param>
        private void Thumb_ImageClicked(ImageEventArgs itemId)
        {
            View.ChangeImage(itemId.Id);
        }

        /// <summary>
        ///     Zoom selected frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        private void ImageZoom_SelectedFrame(SelectionFrame frame)
        {
            if (View.SelectedTool == SelectionTools.SelectRectangle) View.CutImage(frame);
            if (View.SelectedTool == SelectionTools.Erase) View.EraseImage(frame);
        }

        /// <summary>
        ///     Get Color of Selected Point
        /// </summary>
        /// <param name="point">The selected Point.</param>
        private void ImageZoom_SelectedPointColor(Point point)
        {
            View.GetPointColor(point);
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

        /// <summary>
        ///     Colors the picker menu color changed.
        ///     Creates a circle between ColorPicker and the Popup, to set Colors from outside
        /// </summary>
        /// <param name="colorHsv">The color HSV.</param>
        private void ColorPickerMenu_ColorChanged(ColorHsv colorHsv)
        {
            View.Color = colorHsv;
        }

        /// <summary>
        ///     Thumbnails the on image loaded.
        /// </summary>
        private void Thumbnail_OnImageLoaded()
        {
            View.Loaded();
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