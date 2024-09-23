/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/DetailView.cs
 * PURPOSE:     View Model for the Detail Viewer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <seealso cref="INotifyPropertyChanged" />
    internal sealed class DetailView : INotifyPropertyChanged
    {

        /// <summary>
        /// The chunk size
        /// Define a reasonable chunk size for text appending
        /// </summary>
        private const int _chunkSize = 1024;

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
        ///     The difference command
        /// </summary>
        private ICommand _differenceCommand;

        /// <summary>
        ///     The export command
        /// </summary>
        private ICommand _exportCommand;

        /// <summary>
        ///     The information one
        /// </summary>
        private string _informationOne;

        /// <summary>
        ///     The information two
        /// </summary>
        private string _informationTwo;

        /// <summary>
        ///     The open one command
        /// </summary>
        private ICommand _openOneCommand;

        /// <summary>
        ///     The open two command
        /// </summary>
        private ICommand _openTwoCommand;

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
        ///     The information
        /// </summary>
        public RichTextBox RtBoxInformation;

        /// <summary>
        ///     The color information
        /// </summary>
        public TextBox TxtBoxColorInformation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DetailView" /> class.
        /// </summary>
        public DetailView()
        {
            _analysis = new ImageAnalysis();
            _greenIcon = Path.Combine(_root, SlimViewerResources.IconPathGreen);
            _redIcon = Path.Combine(_root, SlimViewerResources.IconPathRed);
        }

        /// <summary>
        ///     Gets or sets the BitmapImage.
        /// </summary>
        /// <value>
        ///     The BitmapImage.
        /// </value>
        public BitmapImage BmpOne
        {
            get => _bmpOne;
            set => SetProperty(ref _bmpOne, value, nameof(BmpOne));
        }

        /// <summary>
        ///     Gets or sets the BMP two.
        /// </summary>
        /// <value>
        ///     The BMP two.
        /// </value>
        public BitmapImage BmpTwo
        {
            get => _bmpTwo;
            set => SetProperty(ref _bmpTwo, value, nameof(BmpTwo));
        }

        /// <summary>
        ///     Gets or sets the path one.
        /// </summary>
        /// <value>
        ///     The path one.
        /// </value>
        public string PathOne
        {
            get => _pathOne;
            set => SetProperty(ref _pathOne, value, nameof(PathOne));
        }

        /// <summary>
        ///     Gets or sets the path two.
        /// </summary>
        /// <value>
        ///     The path two.
        /// </value>
        public string PathTwo
        {
            get => _pathTwo;
            set => SetProperty(ref _pathTwo, value, nameof(PathTwo));
        }

        /// <summary>
        ///     Gets or sets the status image.
        /// </summary>
        /// <value>
        ///     The status image.
        /// </value>
        public string StatusImage
        {
            get => _statusImage;
            set => SetProperty(ref _statusImage, value, nameof(StatusImage));
        }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public string Colors
        {
            get => _color;
            set => SetProperty(ref _color, value, nameof(Colors));
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">Generic Parameter</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Gets the open one command.
        /// </summary>
        /// <value>
        ///     The open one command.
        /// </value>
        public ICommand OpenOneCommand => GetCommand(ref _openOneCommand, OpenOneAction);

        /// <summary>
        ///     Gets the open two command.
        /// </summary>
        /// <value>
        ///     The open two command.
        /// </value>
        public ICommand OpenTwoCommand => GetCommand(ref _openTwoCommand, OpenTwoAction);

        /// <summary>
        ///     Gets the difference command.
        /// </summary>
        /// <value>
        ///     The difference command.
        /// </value>
        public ICommand DifferenceCommand => GetCommand(ref _differenceCommand, DifferenceAction);

        /// <summary>
        ///     Gets the difference command.
        /// </summary>
        /// <value>
        ///     The difference command.
        /// </value>
        public ICommand ExportCommand => GetCommand(ref _exportCommand, ExportAction);


        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        private ICommand GetCommand(ref ICommand command, Action<object> execute)
        {
            return command ??= new DelegateCommand<object>(execute, CanExecute);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Triggers if an Attribute gets changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        /// <value>
        ///     <c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute(object obj)
        {
            return _btmOne != null && _btmTwo != null;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Opens the one action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OpenOneAction(object obj)
        {
            var pathObj = OpenFile();

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            _similarity = null;
            _difference = null;
            _colorOne = null;

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageFileNotSupported, pathObj.Extension),
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = Helper.GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathOne = pathObj.FilePath;

            _btmOne = btm;
            BmpOne = btm.ToBitmapImage();

            _informationOne =
                SlimViewerResources.BuildImageInformationLine(pathObj.FilePath, pathObj.FileName, btm.ToBitmapImage());

            Compare();

            StatusImage = _redIcon;

            var text = await ComputeText(btm);
            await AppendTextAsync(TxtBoxColorInformation, text);

            _colorOne = text;
        }

        /// <summary>
        ///     Opens the two action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OpenTwoAction(object obj)
        {
            var pathObj = OpenFile();

            _similarity = null;
            _difference = null;
            _colorTwo = null;

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageFileNotSupported, pathObj.Extension),
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = Helper.GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathTwo = pathObj.FilePath;
            _btmTwo = btm;
            BmpTwo = btm.ToBitmapImage();

            _informationTwo =
                SlimViewerResources.BuildImageInformationLine(pathObj.FilePath, pathObj.FileName, btm.ToBitmapImage());
            RtBoxInformation.AppendText(_informationTwo);
            // Scroll to the end of the RichTextBox
            RtBoxInformation.ScrollToEnd();

            Compare();

            StatusImage = _redIcon;

            var text = await ComputeText(btm);
            await AppendTextAsync(TxtBoxColorInformation, text);

            _colorTwo = text;
        }

        /// <summary>
        ///     Appends the text asynchronous.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="text">The text.</param>
        private static async Task AppendTextAsync(TextBoxBase richTextBox, string text)
        {
            var offset = 0;
            while (offset < text.Length)
            {
                var length = Math.Min(_chunkSize, text.Length - offset);
                var chunk = text.Substring(offset, length);

                await richTextBox.Dispatcher.InvokeAsync(() => richTextBox.AppendText(chunk));

                offset += length;

                // Optionally yield control to the UI thread occasionally
                await Task.Yield();
            }

            richTextBox.ScrollToEnd(); // Scroll after all chunks are appended
        }

        /// <summary>
        ///     Differences the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void DifferenceAction(object obj)
        {
            if (_btmOne == null || _btmTwo == null) return;

            var color = Colors;
            if (string.IsNullOrEmpty(color)) return;

            var col = Color.FromName(color);

            _difference = _analysis.DifferenceImage(_btmOne, _btmTwo, col);

            BmpOne = _difference.ToBitmapImage();
        }

        /// <summary>
        ///     Exports the Information.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ExportAction(object obj)
        {
            if (string.IsNullOrEmpty(_informationOne) && string.IsNullOrEmpty(_informationTwo)) return;

            _ = Helper.GenerateExportAsync(_informationOne, _informationTwo, _colorOne, _colorTwo, _similarity,
                _difference);
        }

        /// <summary>
        ///     Computes the text.
        /// </summary>
        /// <param name="btm">The bitmap.</param>
        /// <returns>The color Infos</returns>
        private async Task<string> ComputeText(Bitmap btm)
        {
            var str = new StringBuilder();
            await Task.Run(() =>
            {
                foreach (var (color, count) in _analysis.GetColors(btm))
                {
                    str.AppendLine($"{SlimViewerResources.InformationColor}{color}{SlimViewerResources.InformationCount}{count}");
                }
            });
            StatusImage = _greenIcon;
            return str.ToString();
        }

        /// <summary>
        ///     Compares this instance.
        /// </summary>
        private void Compare()
        {
            if (_btmOne == null || _btmTwo == null) return;

            var data = _analysis.CompareImages(_btmOne, _btmTwo);

            _similarity = $"{SlimViewerResources.Similarity}{data.Similarity}{Environment.NewLine}";
            RtBoxInformation.AppendText(_similarity);

            // Scroll to the end of the RichTextBox
            RtBoxInformation.ScrollToEnd();
        }

        /// <summary>
        ///     Opens the file.
        /// </summary>
        /// <returns>Path object with all needed file information</returns>
        private static PathObject OpenFile()
        {
            return FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen);
        }
    }
}