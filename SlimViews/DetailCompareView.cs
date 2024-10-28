/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/DetailCompareView.cs
 * PURPOSE:     View Model for the Detail Viewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommonDialogs;
using ImageCompare;
using Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     View for Detail Window
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    internal sealed class DetailCompareView : ViewModelBase
    {
        /// <summary>
        ///     The chunk size
        ///     Define a reasonable chunk size for text appending
        /// </summary>
        private const int ChunkSize = 1024;

        /// <summary>
        ///     The analysis
        /// </summary>
        private readonly ImageAnalysis _analysis;

        /// <summary>
        ///     The green icon
        /// </summary>
        private readonly string _greenIcon;

        /// <summary>
        ///     The red icon
        /// </summary>
        private readonly string _redIcon;

        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        private readonly string _root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     The first BitmapImage
        /// </summary>
        private BitmapImage _bmpOne;

        /// <summary>
        ///     The first BitmapImage
        /// </summary>
        private BitmapImage _bmpTwo;

        /// <summary>
        ///     The first bitmap
        /// </summary>
        private Bitmap _btmOne;

        /// <summary>
        ///     The first bitmap
        /// </summary>
        private Bitmap _btmTwo;

        /// <summary>
        ///     The color
        /// </summary>
        private string _color;

        /// <summary>
        ///     The color one
        /// </summary>
        private string _colorOne;

        /// <summary>
        ///     The color two
        /// </summary>
        private string _colorTwo;

        /// <summary>
        ///     The difference
        /// </summary>
        private Bitmap _difference;

        /// <summary>
        ///     The information one
        /// </summary>
        private string _informationOne;

        /// <summary>
        ///     The information two
        /// </summary>
        private string _informationTwo;

        /// <summary>
        ///     The path one
        /// </summary>
        private string _pathOne;

        /// <summary>
        ///     The path two
        /// </summary>
        private string _pathTwo;

        /// <summary>
        ///     The similarity
        /// </summary>
        private string _similarity;

        /// <summary>
        ///     The status image
        /// </summary>
        private string _statusImage;

        /// <summary>
        ///     The information RichTextBox
        /// </summary>
        public RichTextBox RtBoxInformation;

        /// <summary>
        ///     The color information TextBox
        /// </summary>
        public TextBox TxtBoxColorInformation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DetailCompareView" /> class.
        /// </summary>
        public DetailCompareView()
        {
            _analysis = new ImageAnalysis();
            _greenIcon = Path.Combine(_root, SlimViewerResources.IconPathGreen);
            _redIcon = Path.Combine(_root, SlimViewerResources.IconPathRed);

            // Initialize commands in the constructor
            OpenOneCommand = new DelegateCommand<object>(OpenOneAction, CanExecute);
            OpenTwoCommand = new DelegateCommand<object>(OpenTwoAction, CanExecute);
            DifferenceCommand = new DelegateCommand<object>(DifferenceAction, CanExecute);
            ExportCommand = new DelegateCommand<object>(ExportAction, CanExecute);
        }

        /// <summary>
        ///     Gets or sets the first BitmapImage.
        /// </summary>
        public BitmapImage BmpOne
        {
            get => _bmpOne;
            set => SetProperty(ref _bmpOne, value, nameof(BmpOne));
        }

        /// <summary>
        ///     Gets or sets the second BitmapImage.
        /// </summary>
        public BitmapImage BmpTwo
        {
            get => _bmpTwo;
            set => SetProperty(ref _bmpTwo, value, nameof(BmpTwo));
        }

        /// <summary>
        ///     Gets or sets the path of the first image.
        /// </summary>
        public string PathOne
        {
            get => _pathOne;
            set => SetProperty(ref _pathOne, value, nameof(PathOne));
        }

        /// <summary>
        ///     Gets or sets the path of the second image.
        /// </summary>
        public string PathTwo
        {
            get => _pathTwo;
            set => SetProperty(ref _pathTwo, value, nameof(PathTwo));
        }

        /// <summary>
        ///     Gets or sets the status image.
        /// </summary>
        public string StatusImage
        {
            get => _statusImage;
            set => SetProperty(ref _statusImage, value, nameof(StatusImage));
        }

        /// <summary>
        ///     Gets or sets the color used for difference highlighting.
        /// </summary>
        public string Colors
        {
            get => _color;
            set => SetProperty(ref _color, value, nameof(Colors));
        }

        /// <summary>
        ///     Command to open the first image.
        /// </summary>
        public ICommand OpenOneCommand { get; }

        /// <summary>
        ///     Command to open the second image.
        /// </summary>
        public ICommand OpenTwoCommand { get; }

        /// <summary>
        ///     Command to compute the difference between images.
        /// </summary>
        public ICommand DifferenceCommand { get; }

        /// <summary>
        ///     Command to export the comparison results.
        /// </summary>
        public ICommand ExportCommand { get; }

        /// <summary>
        ///     Sets the property and raises the PropertyChanged event.
        /// </summary>
        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Determines whether the commands can execute.
        /// </summary>
        public bool CanExecute(object obj)
        {
            return _btmOne != null && _btmTwo != null;
        }

        /// <summary>
        ///     Action to open the first image.
        /// </summary>
        private async void OpenOneAction(object obj)
        {
            var pathObj = OpenFile();

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            _similarity = null;
            _difference = null;
            _colorOne = null;

            // Check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                MessageBox.Show($"{SlimViewerResources.MessageFileNotSupported}{pathObj.Extension}",
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = Helper.GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathOne = pathObj.FilePath;

            _btmOne = btm;
            BmpOne = btm.ToBitmapImage();

            _informationOne = SlimViewerResources.BuildImageInformationLine(pathObj.FilePath, pathObj.FileName, BmpOne);

            Compare();

            StatusImage = _redIcon;

            var text = await ComputeText(btm);
            await AppendTextAsync(TxtBoxColorInformation, text);

            _colorOne = text;
        }

        /// <summary>
        ///     Action to open the second image.
        /// </summary>
        private async void OpenTwoAction(object obj)
        {
            var pathObj = OpenFile();

            _similarity = null;
            _difference = null;
            _colorTwo = null;

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            // Check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                MessageBox.Show($"{SlimViewerResources.MessageFileNotSupported}{pathObj.Extension}",
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = Helper.GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathTwo = pathObj.FilePath;
            _btmTwo = btm;
            BmpTwo = btm.ToBitmapImage();

            _informationTwo = SlimViewerResources.BuildImageInformationLine(pathObj.FilePath, pathObj.FileName, BmpTwo);
            RtBoxInformation.AppendText(_informationTwo);
            RtBoxInformation.ScrollToEnd();

            Compare();

            StatusImage = _redIcon;

            var text = await ComputeText(btm);
            await AppendTextAsync(TxtBoxColorInformation, text);

            _colorTwo = text;
        }

        /// <summary>
        ///     Appends text to a TextBoxBase control asynchronously.
        /// </summary>
        private static async Task AppendTextAsync(TextBoxBase textBox, string text)
        {
            var offset = 0;
            while (offset < text.Length)
            {
                var length = Math.Min(ChunkSize, text.Length - offset);
                var chunk = text.Substring(offset, length);

                await textBox.Dispatcher.InvokeAsync(() => textBox.AppendText(chunk));
                offset += length;

                await Task.Yield();
            }

            textBox.ScrollToEnd();
        }

        /// <summary>
        ///     Action to compute the difference between the two images.
        /// </summary>
        private void DifferenceAction(object obj)
        {
            if (_btmOne == null || _btmTwo == null) return;

            if (string.IsNullOrEmpty(Colors)) return;

            var col = Color.FromName(Colors);

            _difference = _analysis.DifferenceImage(_btmOne, _btmTwo, col);

            BmpOne = _difference.ToBitmapImage();
        }

        /// <summary>
        ///     Action to export the comparison results.
        /// </summary>
        private void ExportAction(object obj)
        {
            if (string.IsNullOrEmpty(_informationOne) && string.IsNullOrEmpty(_informationTwo)) return;

            _ = Helper.GenerateExportAsync(_informationOne, _informationTwo, _colorOne, _colorTwo, _similarity,
                _difference);
        }

        /// <summary>
        ///     Computes the text for color information.
        /// </summary>
        private async Task<string> ComputeText(Bitmap btm)
        {
            var str = new StringBuilder();
            await Task.Run(() =>
            {
                foreach (var (color, count) in _analysis.GetColors(btm))
                    str.AppendLine(
                        $"{SlimViewerResources.InformationColor}{color}{SlimViewerResources.InformationCount}{count}");
            });
            StatusImage = _greenIcon;
            return str.ToString();
        }

        /// <summary>
        ///     Compares the two images and updates similarity information.
        /// </summary>
        private void Compare()
        {
            if (_btmOne == null || _btmTwo == null) return;

            var data = _analysis.CompareImages(_btmOne, _btmTwo);

            _similarity = $"{SlimViewerResources.Similarity}{data.Similarity}{Environment.NewLine}";
            RtBoxInformation.AppendText(_similarity);
            RtBoxInformation.ScrollToEnd();
        }

        /// <summary>
        ///     Opens a file dialog to select an image file.
        /// </summary>
        private static PathObject OpenFile()
        {
            return FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen);
        }
    }
}