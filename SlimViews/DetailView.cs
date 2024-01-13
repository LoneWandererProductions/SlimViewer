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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommonControls;
using Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    /// View for Detail Window
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    internal sealed class DetailView : INotifyPropertyChanged
    {
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
        /// Gets or sets the BMP two.
        /// </summary>
        /// <value>
        /// The BMP two.
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
        /// The first BitmapImage
        /// </summary>
        private BitmapImage _bmpOne;

        /// <summary>
        /// The first bitmap
        /// </summary>
        private Bitmap _btmOne;

        /// <summary>
        /// The first BitmapImage
        /// </summary>
        private BitmapImage _bmpTwo;

        /// <summary>
        /// The first bitmap
        /// </summary>
        private Bitmap _btmTwo;

        /// <summary>
        /// The open one command
        /// </summary>
        private ICommand _openOneCommand;

        /// <summary>
        /// The open two command
        /// </summary>
        private ICommand _openTwoCommand;

        /// <summary>
        /// Gets the open one command.
        /// </summary>
        /// <value>
        /// The open one command.
        /// </value>
        public ICommand OpenOneCommand =>
            _openOneCommand ??= new DelegateCommand<object>(OpenOneAction, CanExecute);

        /// <summary>
        /// Gets the open two command.
        /// </summary>
        /// <value>
        /// The open two command.
        /// </value>
        public ICommand OpenTwoCommand =>
            _openTwoCommand ??= new DelegateCommand<object>(OpenTwoAction, CanExecute);

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
        /// Opens the one action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenOneAction(object obj)
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
            _btmOne = btm;
            BmpOne = btm.ToBitmapImage();
        }

        /// <summary>
        /// Opens the two action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenTwoAction(object obj)
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
            _btmTwo = btm;
            BmpTwo = btm.ToBitmapImage();
        }

        /// <summary>
        /// Opens the file.
        /// </summary>
        /// <returns>Path object with all needed file information</returns>
        private static PathObject OpenFile()
        {
            return FileIoHandler.HandleFileOpen(SlimViewerResources.FileOpen, null);
        }

        /// <summary>
        /// Generates the image.
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