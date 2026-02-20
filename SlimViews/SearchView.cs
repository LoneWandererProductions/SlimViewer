/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SearchView.cs
 * PURPOSE:     View Model for Search
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExtendedSystemObjects;
using FileHandler;
using ImageCompare;
using Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Search View Model responsible for handling search operations
    ///     including text-based and color-based image searches.
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    internal sealed class SearchView : ViewModelBase
    {
        /// <summary>
        ///     Image analysis utility for performing color-based searches.
        /// </summary>
        private readonly ImageAnalysis _imageAnalysis = new();

        /// <summary>
        /// The blue
        /// </summary>
        private int _blue;

        /// <summary>
        /// The current folder
        /// </summary>
        private string _currentFolder;

        /// <summary>
        /// The green
        /// </summary>
        private int _green;

        /// <summary>
        /// The image view
        /// </summary>
        private ImageView _imageView;

        /// <summary>
        /// The include subfolders
        /// </summary>
        private bool _includeSubfolders;

        /// <summary>
        /// The range
        /// </summary>
        private int _range = 3;

        /// <summary>
        /// The red
        /// </summary>
        private int _red;

        /// <summary>
        /// The search string
        /// </summary>
        private string _searchString;

        /// <summary>
        /// The is working
        /// </summary>
        private bool _isWorking;

        /// <summary>
        /// The search by color command
        /// </summary>
        private ICommand _searchByColorCommand;

        /// <summary>
        /// The search by text command
        /// </summary>
        private ICommand _searchByTextCommand;

        /// <summary>
        ///     Gets or sets the search range for color comparison.
        /// </summary>
        public int Range
        {
            get => _range;
            set
            {
                if (value >= 0) SetProperty(ref _range, value, nameof(Range));
            }
        }

        /// <summary>
        ///     Gets or sets the red component for color comparison (0-255).
        /// </summary>
        public int Red
        {
            get => _red;
            set
            {
                if (value >= 0 && value <= 255) SetProperty(ref _red, value, nameof(Red));
            }
        }

        /// <summary>
        ///     Gets or sets the green component for color comparison (0-255).
        /// </summary>
        public int Green
        {
            get => _green;
            set
            {
                if (value >= 0 && value <= 255) SetProperty(ref _green, value, nameof(Green));
            }
        }

        /// <summary>
        ///     Gets or sets the blue component for color comparison (0-255).
        /// </summary>
        public int Blue
        {
            get => _blue;
            set
            {
                if (value >= 0 && value <= 255) SetProperty(ref _blue, value, nameof(Blue));
            }
        }

        /// <summary>
        ///     Gets or sets the text to search for in file names.
        /// </summary>
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value, nameof(SearchString));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is actively processing a search.
        /// </summary>
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                SetProperty(ref _isWorking, value, nameof(IsWorking));
                OnPropertyChanged(nameof(IsNotWorking));
            }
        }

        public bool IsNotWorking => !IsWorking;

        /// <summary>
        ///     Command for performing text-based searches.
        /// </summary>
        public ICommand SearchByTextCommand =>
            _searchByTextCommand ??= new AsyncDelegateCommand<object>(ExecuteTextSearchAsync, CanExecute);

        /// <summary>
        ///     Command for performing color-based searches.
        /// </summary>
        public ICommand SearchByColorCommand =>
            _searchByColorCommand ??= new AsyncDelegateCommand<object>(ExecuteColorSearchAsync, CanExecute);

        /// <summary>
        ///     Initializes the SearchView with the specified parameters.
        /// </summary>
        public void Initialize(bool includeSubfolders, string currentFolder, ImageView imageView, ColorHsv initialColor = null)
        {
            _includeSubfolders = includeSubfolders;

            // FIXED: Check the passed parameter, not the private field
            if (string.IsNullOrEmpty(currentFolder))
            {
                _ = MessageBox.Show(ViewResources.ErrorDirectoryMessage, nameof(ArgumentException), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentFolder = currentFolder;

            if (imageView == null)
            {
                _ = MessageBox.Show(ViewResources.ErrorObjectMessage, nameof(ArgumentException), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _imageView = imageView;

            if (initialColor == null) return;

            Red = initialColor.R;
            Green = initialColor.G;
            Blue = initialColor.B;
        }

        /// <summary>
        ///     Executes the text-based search action asynchronously.
        /// </summary>
        private async Task ExecuteTextSearchAsync(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchString)) return;

            IsWorking = true;
            try
            {
                var files = await Task.Run(() => FileHandleSearch.GetFilesWithSubString(
                    _currentFolder,
                    ImagingResources.Appendix,
                    _includeSubfolders,
                    SearchString,
                    true));

                if (files.IsNullOrEmpty())
                {
                    MessageBox.Show("No files found matching that text.", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Dispatch back to the UI thread to safely update the Main Window
                Application.Current.Dispatcher.Invoke(() => _imageView.ChangeImage(files));
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        ///     Executes the color-based search action asynchronously.
        /// </summary>
        private async Task ExecuteColorSearchAsync(object obj)
        {
            IsWorking = true;
            try
            {
                var files = await Task.Run(() => _imageAnalysis.FindImagesInColorRange(
                    Red, Green, Blue, Range, _currentFolder, _includeSubfolders, ImagingResources.Appendix));

                if (files.IsNullOrEmpty())
                {
                    MessageBox.Show("No files found matching that color.", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Dispatch back to the UI thread to safely update the Main Window
                Application.Current.Dispatcher.Invoke(() => _imageView.ChangeImage(files));
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.Message, nameof(ArgumentException), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex);
                _ = MessageBox.Show(ex.Message, nameof(InvalidOperationException), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsWorking = false;
            }
        }
    }
}