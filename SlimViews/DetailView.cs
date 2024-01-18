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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommonControls;
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
        ///     The analysis
        /// </summary>
        private readonly ImageAnalysis _analysis;

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
        ///     The open one command
        /// </summary>
        private ICommand _openOneCommand;

        /// <summary>
        ///     The open two command
        /// </summary>
        private ICommand _openTwoCommand;

        /// <summary>
        /// The difference command
        /// </summary>
        private ICommand _differenceCommand;

        /// <summary>
        ///     The path one
        /// </summary>
        private string _pathOne;

        /// <summary>
        ///     The path two
        /// </summary>
        private string _pathTwo;

        /// <summary>
        /// The status image
        /// </summary>
        private string _statusImage;

        /// <summary>
        ///     Gets or sets the root.
        /// </summary>
        /// <value>
        ///     The root.
        /// </value>
        private readonly string _root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     The information
        /// </summary>
        public ScrollingTextBoxes Information;

        /// <summary>
        /// The color information
        /// </summary>
        public ScrollingTextBoxes ColorInformation;

        /// <summary>
        /// The green icon
        /// </summary>
        private readonly string _greenIcon;

        /// <summary>
        /// The red icon
        /// </summary>
        private readonly string _redIcon;

        /// <summary>
        /// The color
        /// </summary>
        private object _color;

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
            set
            {
                if (_bmpOne == value) return;

                _bmpOne = value;
                OnPropertyChanged(nameof(BmpOne));
            }
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
            set
            {
                if (_bmpTwo == value) return;

                _bmpTwo = value;
                OnPropertyChanged(nameof(BmpTwo));
            }
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
            set
            {
                _pathOne = value;
                OnPropertyChanged(nameof(PathOne));
            }
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
            set
            {
                _pathTwo = value;
                OnPropertyChanged(nameof(PathTwo));
            }
        }


        /// <summary>
        /// Gets or sets the status image.
        /// </summary>
        /// <value>
        /// The status image.
        /// </value>
        public string StatusImage
        {
            get => _statusImage;
            set
            {
                if (_statusImage == value) return;

                _statusImage = value;
                OnPropertyChanged(nameof(StatusImage));
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public object Colors
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                OnPropertyChanged(nameof(Colors));
            }
        }

        /// <summary>
        ///     Gets the open one command.
        /// </summary>
        /// <value>
        ///     The open one command.
        /// </value>
        public ICommand OpenOneCommand =>
            _openOneCommand ??= new DelegateCommand<object>(OpenOneAction, CanExecute);

        /// <summary>
        ///     Gets the open two command.
        /// </summary>
        /// <value>
        ///     The open two command.
        /// </value>
        public ICommand OpenTwoCommand =>
            _openTwoCommand ??= new DelegateCommand<object>(OpenTwoAction, CanExecute);


        /// <summary>
        /// Gets the difference command.
        /// </summary>
        /// <value>
        /// The difference command.
        /// </value>
        public ICommand DifferenceCommand =>
            _differenceCommand ??= new DelegateCommand<object>(DifferenceAction, CanExecute);

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
            // check if executing is allowed, not used right now
            return true;
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

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageFileNotSupported, pathObj.Extension),
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathOne = pathObj.FilePath;

            _btmOne = btm;
            BmpOne = btm.ToBitmapImage();

            Compare();

            SetInformation(pathObj.FilePath, pathObj.FileName, btm);

            StatusImage = _redIcon;

            ColorInformation.Text += await _ComputeText(btm);
        }

        /// <summary>
        ///     Opens the two action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void OpenTwoAction(object obj)
        {
            var pathObj = OpenFile();

            if (string.IsNullOrEmpty(pathObj?.FilePath)) return;

            //check if file extension is supported
            if (!ImagingResources.Appendix.Contains(pathObj.Extension.ToLower()))
            {
                _ = MessageBox.Show(string.Concat(SlimViewerResources.MessageFileNotSupported, pathObj.Extension),
                    SlimViewerResources.MessageError);
                return;
            }

            var btm = GenerateImage(pathObj.FilePath);
            if (btm == null) return;

            PathTwo = pathObj.FilePath;
            _btmTwo = btm;
            BmpTwo = btm.ToBitmapImage();

            Compare();

            SetInformation(pathObj.FilePath, pathObj.FileName, btm);

            StatusImage = _redIcon;

            ColorInformation.Text += await _ComputeText(btm);
        }

        private void DifferenceAction(object obj)
        {
            var color = Colors;

            throw new NotImplementedException();
        }


        /// <summary>
        /// Computes the text.
        /// </summary>
        /// <param name="btm">The BTM.</param>
        /// <returns>The color Infos</returns>
        private async Task<string> _ComputeText(Bitmap btm)
        {
            var str = new StringBuilder();

            _ = await Task.Run(() =>
            {
                foreach (var (color, count) in _analysis.GetColors(btm))
                {
                    var cache = string.Concat(SlimViewerResources.InformationColor, color,
                        SlimViewerResources.InformationCount, count, Environment.NewLine);
                    str.Append(cache);
                    Thread.Sleep(1);
                }

                StatusImage = _greenIcon;
                return true;
            });

            return str.ToString();
        }

        /// <summary>
        ///     Sets the information
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="btm">The BTM.</param>
        private void SetInformation(string filePath, string fileName, Bitmap btm)
        {
            Information.Append(SlimViewerResources.BuildImageInformationLine(filePath, fileName, btm.ToBitmapImage()));
        }

        /// <summary>
        ///     Compares this instance.
        /// </summary>
        private void Compare()
        {
            if (_btmOne == null || _btmTwo == null) return;

            var data = _analysis.CompareImages(_btmOne, _btmTwo);

            Information.Append(string.Concat(SlimViewerResources.Similarity, data.Similarity));
        }

        /// <summary>
        ///     Opens the file.
        /// </summary>
        /// <returns>Path object with all needed file information</returns>
        private static PathObject OpenFile()
        {
            return FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen, null);
        }

        /// <summary>
        ///     Generates the image.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Bitmap from the Image in Question</returns>
        private static Bitmap GenerateImage(string filePath)
        {
            try
            {
                return Helper.Render.GetOriginalBitmap(filePath);
            }
            catch (IOException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (NotSupportedException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.ToString(), SlimViewerResources.MessageError);
            }

            return null;
        }
    }
}