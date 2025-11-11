/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ResizerView.cs
 * PURPOSE:     View Model for Resizer
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global, if we make it private the Property Changed event will not be triggered in the Window
// ReSharper disable MemberCanBeInternal, must be public, else the View Model won't work

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommonDialogs;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
using Imaging.Enums;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     View for Resizer
    ///     TODO:
    ///     Add Resize Options
    ///     Add optional Filters
    ///     Add File Converter
    /// </summary>
    internal sealed class ResizerView : ViewModelBase
    {
        /// <summary>
        ///     The cancel command
        /// </summary>
        private ICommand _cancelCommand;

        /// <summary>
        ///     The height
        /// </summary>
        private int _height = 100;

        /// <summary>
        ///     The input
        /// </summary>
        private string _input;

        /// <summary>
        ///     The input command
        /// </summary>
        private ICommand _inputCommand;

        /// <summary>
        ///     The is percentages checked
        /// </summary>
        private bool _isPercentagesChecked = true;

        /// <summary>
        ///     The is relative size checked
        /// </summary>
        private bool _isRelativeSizeChecked;

        /// <summary>
        ///     The output
        /// </summary>
        private string _output;

        /// <summary>
        ///     The output command
        /// </summary>
        private ICommand _outputCommand;

        /// <summary>
        ///     The process command
        /// </summary>
        private ICommand _processCommand;

        /// <summary>
        ///     The selected extension
        /// </summary>
        private string _selectedExtension;

        /// <summary>
        ///     The selected filter option
        /// </summary>
        private FiltersType _selectedFilterOption = FiltersType.None;

        /// <summary>
        ///     The width
        /// </summary>
        private int _width = 100;

        /// <summary>
        /// The current path
        /// </summary>
        private string _currentPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizerView"/> class.
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        public ResizerView(string currentPath)
        {
            _currentPath = currentPath;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is percentages checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is percentages checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsPercentagesChecked
        {
            get => _isPercentagesChecked;
            set => SetProperty(ref _isPercentagesChecked, value, nameof(IsPercentagesChecked));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is relative size checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is relative size checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsRelativeSizeChecked
        {
            get => _isRelativeSizeChecked;
            set => SetProperty(ref _isRelativeSizeChecked, value, nameof(IsRelativeSizeChecked));
        }

        /// <summary>
        ///     Gets or sets the selected filter option.
        /// </summary>
        /// <value>
        ///     The selected filter option.
        /// </value>
        public FiltersType SelectedFilterOption
        {
            get => _selectedFilterOption;
            set => SetProperty(ref _selectedFilterOption, value, nameof(SelectedFilterOption));
        }

        /// <summary>
        ///     Gets or sets the selected extension.
        /// </summary>
        /// <value>
        ///     The selected extension.
        /// </value>
        public string SelectedExtension
        {
            get => _selectedExtension;
            set => SetProperty(ref _selectedExtension, value, nameof(SelectedExtension));
        }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height
        {
            get => _height;
            set
            {
                if (value < 0)
                {
                    _ = MessageBox.Show(string.Concat(ViewResources.ErrorMeasures, nameof(Height)),
                        ViewResources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SetProperty(ref _height, value, nameof(Height));
            }
        }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width
        {
            get => _width;
            set
            {
                if (value < 0)
                {
                    _ = MessageBox.Show(string.Concat(ViewResources.ErrorMeasures, nameof(Width)),
                        ViewResources.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SetProperty(ref _width, value, nameof(Width));
            }
        }

        /// <summary>
        ///     Gets or sets the output.
        /// </summary>
        /// <value>
        ///     The output.
        /// </value>
        public string Output
        {
            get => _output;
            set => SetProperty(ref _output, value, nameof(Output));
        }

        /// <summary>
        ///     Gets or sets the input.
        /// </summary>
        /// <value>
        ///     The input.
        /// </value>
        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value, nameof(Input));
        }

        /// <summary>
        ///     Gets the process command.
        /// </summary>
        /// <value>
        ///     The process command.
        /// </value>
        public ICommand ProcessCommand =>
            _processCommand ??= new DelegateCommand<object>(ProcessAction, CanExecute);

        /// <summary>
        ///     Gets the cancel command.
        /// </summary>
        /// <value>
        ///     The cancel command.
        /// </value>
        public ICommand CancelCommand =>
            _cancelCommand ??= new DelegateCommand<Window>(CancelAction, CanExecute);

        /// <summary>
        ///     Gets the output command.
        /// </summary>
        /// <value>
        ///     The output command.
        /// </value>
        public ICommand OutputCommand =>
            _outputCommand ??= new DelegateCommand<object>(OutputAction, CanExecute);

        /// <summary>
        ///     Gets the input command.
        /// </summary>
        /// <value>
        ///     The input command.
        /// </value>
        public ICommand InputCommand =>
            _inputCommand ??= new DelegateCommand<object>(InputAction, CanExecute);

        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<FiltersType> FilterOptions =>
            Enum.GetValues(typeof(FiltersType)) as IEnumerable<FiltersType>;

        /// <summary>
        ///     Gets the file extensions.
        /// </summary>
        /// <value>
        ///     The file extensions.
        /// </value>
        public IEnumerable<string> FileExtensions => ImagingResources.Appendix;

        /// <summary>
        ///     Cancels the action and closes the window
        /// </summary>
        /// <param name="window">The window.</param>
        private void CancelAction(Window window)
        {
            window.Close();
        }

        /// <summary>
        ///     Outputs the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OutputAction(object obj)
        {
            if (string.IsNullOrEmpty(_currentPath))
                _currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Output = DialogHandler.ShowFolder(Path.GetDirectoryName(_currentPath));
        }

        /// <summary>
        ///     Inputs the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void InputAction(object obj)
        {
            if (string.IsNullOrEmpty(_currentPath))
                _currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Input = DialogHandler.ShowFolder(Path.GetDirectoryName(_currentPath));
        }

        /// <summary>
        ///     Processes the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ProcessAction(object obj)
        {
            double height = _height;
            double width = _width;

            if (!Directory.Exists(_input) || !Directory.Exists(_output))
                // TODO: Show a message box indicating directories are missing
                return;

            var files = FileHandleSearch.GetFilesByExtensionFullPath(_input, ImagingResources.Appendix, false);

            if (files.IsNullOrEmpty())
                // TODO: Show a message box indicating no files found
                return;

            foreach (var filePath in files)
            {
                if (!File.Exists(filePath)) continue;

                var bitmap = ImageProcessor.LoadImage(filePath);

                if (bitmap == null) continue;

                // Apply selected filter option
                if (_selectedFilterOption != FiltersType.None)
                    bitmap = ImageProcessor.Filter(bitmap, _selectedFilterOption);

                // Resize the image based on percentage or absolute dimensions
                if (_isPercentagesChecked)
                {
                    height = bitmap.Height * height / 100;
                    width = bitmap.Width * width / 100;
                }

                var iHeight = (int)height;
                var iWidth = (int)width;

                if (iHeight == 0 || iWidth == 0) continue;

                bitmap = ImageProcessor.Resize(bitmap, iWidth, iHeight);

                // Determine the file extension is set, if not use the current one
                if (string.IsNullOrEmpty(SelectedExtension))
                {
                    SelectedExtension = Path.GetExtension(filePath);

                    if (string.IsNullOrEmpty(SelectedExtension)) continue;
                }

                var name = Path.GetFileName(filePath);
                var target = Path.Combine(_output, name);

                // Save the modified image with the determined file extension
                _ = ImageProcessor.SaveImage(target, SelectedExtension, bitmap);
            }
        }
    }
}