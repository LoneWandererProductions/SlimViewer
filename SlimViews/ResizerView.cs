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
using System.Windows.Input;
using Imaging;
using ViewModel;

namespace SlimViews
{
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
        /// The height
        /// </summary>
        private int _height;

        /// <summary>
        /// The width
        /// </summary>
        private int _width;

        /// <summary>
        /// The input
        /// </summary>
        private string _input;

        /// <summary>
        /// The output
        /// </summary>
        private string _output;

        /// <summary>
        /// The output command
        /// </summary>
        private ICommand _outputCommand;

        /// <summary>
        /// The cancel command
        /// </summary>
        private ICommand _cancelCommand;

        /// <summary>
        /// The input command
        /// </summary>
        private ICommand _inputCommand;

        /// <summary>
        ///     The percentage command
        /// </summary>
        private ICommand _percentageCommand;

        /// <summary>
        /// The process command
        /// </summary>
        private ICommand _processCommand;

        /// <summary>
        /// The relative command
        /// </summary>
        private ICommand _relativeCommand;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
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
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
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
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
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
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
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

        public ICommand ProcessCommand =>
            _processCommand ??= new DelegateCommand<object>(ProcessAction, CanExecute);

        public ICommand CancelCommand =>
            _cancelCommand ??= new DelegateCommand<object>(CancelAction, CanExecute);

        public ICommand OutputCommand =>
            _outputCommand ??= new DelegateCommand<object>(OutputAction, CanExecute);

        public ICommand InputCommand =>
            _inputCommand ??= new DelegateCommand<object>(InputAction, CanExecute);

        public ICommand RelativeCommand =>
            _relativeCommand ??= new DelegateCommand<object>(RelativeAction, CanExecute);

        /// <summary>
        ///     Gets the percentage command.
        /// </summary>
        /// <value>
        ///     The percentage command.
        /// </value>
        public ICommand PercentageCommand =>
            _percentageCommand ??= new DelegateCommand<object>(Percentagection, CanExecute);


        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<ImageFilter> FilterOptions =>
            Enum.GetValues(typeof(ImageFilter)) as IEnumerable<ImageFilter>;

        /// <inheritdoc />
        /// <summary>
        ///     Triggers if an Attribute gets changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Processes the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ProcessAction(object obj)
        {
        }

        /// <summary>
        /// Cancels the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CancelAction(object obj)
        {
        }

        /// <summary>
        /// Outputs the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OutputAction(object obj)
        {
        }

        /// <summary>
        /// Inputs the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void InputAction(object obj)
        {
        }

        /// <summary>
        /// Relatives the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void RelativeAction(object obj)
        {
        }

        /// <summary>
        /// Percentagections the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void Percentagection(object obj)
        {
        }

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
    }
}