/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/SearchView.cs
 * PURPOSE:     View Model for Search
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Diagnostics;
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
        ///     Current folder being searched.
        /// </summary>
        private string _currentFolder;

        /// <summary>
        ///     Text to search for in file names.
        /// </summary>
        private string _searchString;

        /// <summary>
        ///     Search range for color comparison.
        /// </summary>
        private int _range = 3;

        /// <summary>
        ///     Red component for color comparison.
        /// </summary>
        private int _red;

        /// <summary>
        ///     Green component for color comparison.
        /// </summary>
        private int _green;

        /// <summary>
        ///     Blue component for color comparison.
        /// </summary>
        private int _blue;

        /// <summary>
        ///     Indicates whether subfolders should be included in the search.
        /// </summary>
        private bool _includeSubfolders;

        /// <summary>
        ///     ImageView instance for updating search results.
        /// </summary>
        private ImageView _imageView;

        /// <summary>
        ///     Command for performing text-based searches.
        /// </summary>
        private ICommand _searchByTextCommand;

        /// <summary>
        ///     Command for performing color-based searches.
        /// </summary>
        private ICommand _searchByColorCommand;

        /// <summary>
        ///     Gets or sets the search range for color comparison.
        /// </summary>
        /// <value>
        ///     The range of allowable color difference.
        /// </value>
        public int Range
        {
            get => _range;
            set => SetProperty(ref _range, value, nameof(Range));
        }

        /// <summary>
        ///     Gets or sets the red component for color comparison.
        /// </summary>
        /// <value>
        ///     The red component of the target color.
        /// </value>
        public int Red
        {
            get => _red;
            set => SetProperty(ref _red, value, nameof(Red));
        }

        /// <summary>
        ///     Gets or sets the green component for color comparison.
        /// </summary>
        /// <value>
        ///     The green component of the target color.
        /// </value>
        public int Green
        {
            get => _green;
            set => SetProperty(ref _green, value, nameof(Green));
        }

        /// <summary>
        ///     Gets or sets the blue component for color comparison.
        /// </summary>
        /// <value>
        ///     The blue component of the target color.
        /// </value>
        public int Blue
        {
            get => _blue;
            set => SetProperty(ref _blue, value, nameof(Blue));
        }

        /// <summary>
        ///     Gets or sets the text to search for in file names.
        /// </summary>
        /// <value>
        ///     The search string.
        /// </value>
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value, nameof(SearchString));
        }

        /// <summary>
        ///     Command for performing text-based searches.
        /// </summary>
        /// <value>
        ///     A command that triggers the text-based search action.
        /// </value>
        public ICommand SearchByTextCommand =>
            _searchByTextCommand ??= new DelegateCommand<object>(ExecuteTextSearch, CanExecute);

        /// <summary>
        ///     Command for performing color-based searches.
        /// </summary>
        /// <value>
        ///     A command that triggers the color-based search action.
        /// </value>
        public ICommand SearchByColorCommand =>
            _searchByColorCommand ??= new DelegateCommand<object>(ExecuteColorSearch, CanExecute);

        /// <summary>
        ///     Initializes the SearchView with the specified parameters.
        /// </summary>
        /// <param name="includeSubfolders">Whether to include subfolders in the search.</param>
        /// <param name="currentFolder">The folder to search within.</param>
        /// <param name="imageView">The ImageView instance to update with search results.</param>
        /// <param name="initialColor">Optional initial color for color-based searches.</param>
        /// <exception cref="ArgumentNullException">Thrown if currentFolder or imageView is null.</exception>
        public void Initialize(bool includeSubfolders, string currentFolder, ImageView imageView, ColorHsv initialColor = null)
        {
            _includeSubfolders = includeSubfolders;
            _currentFolder = currentFolder ?? throw new ArgumentNullException(nameof(currentFolder));
            _imageView = imageView ?? throw new ArgumentNullException(nameof(imageView));

            if (initialColor != null)
            {
                Red = initialColor.R;
                Green = initialColor.G;
                Blue = initialColor.B;
            }
        }

        /// <summary>
        ///     Validates whether commands can execute.
        /// </summary>
        /// <param name="obj">Optional command parameter.</param>
        /// <returns>
        ///     <c>true</c> if the command can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecute(object obj) => true;

        /// <summary>
        ///     Executes the text-based search action.
        /// </summary>
        /// <param name="obj">Optional command parameter.</param>
        private void ExecuteTextSearch(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchString)) return;

            var files = FileHandleSearch.GetFilesWithSubString(
                _currentFolder,
                ImagingResources.Appendix,
                _includeSubfolders,
                SearchString,
                true);

            if (files.IsNullOrEmpty()) return;

            _imageView.ChangeImage(files);
        }

        /// <summary>
        ///     Executes the color-based search action.
        /// </summary>
        /// <param name="obj">Optional command parameter.</param>
        private void ExecuteColorSearch(object obj)
        {
            try
            {
                var files = _imageAnalysis.FindImagesInColorRange(
                    Red, Green, Blue, Range, _currentFolder, _includeSubfolders, ImagingResources.Appendix);

                if (files.IsNullOrEmpty()) return;

                _imageView.ChangeImage(files);
            }
            catch (ArgumentException ex)
            {
                Trace.WriteLine(ex);
                MessageBox.Show(ex.Message,nameof(ArgumentException), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex);
                MessageBox.Show(ex.Message, nameof(InvalidOperationException), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
