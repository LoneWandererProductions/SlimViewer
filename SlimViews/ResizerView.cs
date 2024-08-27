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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommonDialogs;
using ExtendedSystemObjects;
using FileHandler;
using Imaging;
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
    internal sealed class ResizerView : INotifyPropertyChanged
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
        private ImageFilters _selectedFilterOption;

        /// <summary>
        ///     The width
        /// </summary>
        private int _width = 100;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is percentages checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is percentages checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsPercentagesChecked
        {
            get => _isPercentagesChecked;
            set
            {
                _isPercentagesChecked = value;
                OnPropertyChanged(nameof(IsPercentagesChecked));
            }
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
            set
            {
                _isRelativeSizeChecked = value;
                OnPropertyChanged(nameof(IsRelativeSizeChecked));
            }
        }

        /// <summary>
        ///     Gets or sets the selected filter option.
        /// </summary>
        /// <value>
        ///     The selected filter option.
        /// </value>
        public ImageFilters SelectedFilterOption
        {
            get => _selectedFilterOption;
            set
            {
                if (_selectedFilterOption == value) return;

                _selectedFilterOption = value;
                OnPropertyChanged(nameof(SelectedFilterOption));
            }
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
            set
            {
                if (_selectedExtension == value) return;

                _selectedExtension = value;
                OnPropertyChanged(nameof(SelectedExtension));
            }
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
                if (_height == value) return;

                _height = value;
                OnPropertyChanged(nameof(Height));
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
            get => _height;
            set
            {
                if (_width == value) return;

                _width = value;
                OnPropertyChanged(nameof(Width));
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
            set
            {
                if (_output == value) return;

                _output = value;
                OnPropertyChanged(nameof(Output));
            }
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
            set
            {
                if (_input == value) return;

                _input = value;
                OnPropertyChanged(nameof(Input));
            }
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
        public IEnumerable<ImageFilters> FilterOptions =>
            Enum.GetValues(typeof(ImageFilters)) as IEnumerable<ImageFilters>;

        /// <summary>
        ///     Gets the file extensions.
        /// </summary>
        /// <value>
        ///     The file extensions.
        /// </value>
        public IEnumerable<string> FileExtensions => ImagingResources.Appendix;

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
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Output = FileIoHandler.ShowFolder(Path.GetDirectoryName(SlimViewerRegister.CurrentFolder));
        }

        /// <summary>
        ///     Inputs the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void InputAction(object obj)
        {
            if (string.IsNullOrEmpty(SlimViewerRegister.CurrentFolder))
                SlimViewerRegister.CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Input = FileIoHandler.ShowFolder(Path.GetDirectoryName(SlimViewerRegister.CurrentFolder));
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

                var bitmap = Helper.LoadImage(filePath);

                if (bitmap == null) continue;

                // Apply selected filter option
                if (_selectedFilterOption != ImageFilters.None) bitmap = Helper.Filter(bitmap, _selectedFilterOption);

                // Resize the image based on percentage or absolute dimensions
                if (_isPercentagesChecked)
                {
                    height = bitmap.Height * height / 100;
                    width = bitmap.Width * width / 100;
                }

                var iHeight = (int)height;
                var iWidth = (int)width;

                if (iHeight == 0 || iWidth == 0) continue;

                bitmap = Helper.Resize(bitmap, iWidth, iHeight);

                // Determine the file extension is set, if not use the current one
                if (string.IsNullOrEmpty(SelectedExtension))
                {
                    SelectedExtension = Path.GetExtension(filePath);

                    if (string.IsNullOrEmpty(SelectedExtension)) continue;
                }

                var name = Path.GetFileName(filePath);
                var target = Path.Combine(_output, name);

                // Save the modified image with the determined file extension
                _ = Helper.SaveImage(target, SelectedExtension, bitmap);
            }
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}