/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ScaleView.cs
 * PURPOSE:     View Model for Scale
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     The View for Rename
    /// </summary>
    /// <seealso cref="T:ViewModel.ViewModelBase" />
    internal sealed class ScaleView : ViewModelBase
    {
        /// <summary>
        ///     The Degrees
        /// </summary>
        private int _degree;

        /// <summary>
        ///     The add command
        /// </summary>
        private ICommand _okayCommand;

        /// <summary>
        ///     The height
        /// </summary>
        private float _scaling = 1;

        /// <summary>
        ///     Gets or sets the Degree.
        /// </summary>
        /// <value>
        ///     The Degree.
        /// </value>
        public int Degree
        {
            get => _degree;
            set
            {
                if (_degree == value) return;

                //only certain Numbers are allowed
                if (value > 360 || _degree < -360) return;

                _degree = value;
                OnPropertyChanged(nameof(Degree));
            }
        }

        /// <summary>
        ///     Gets or sets the Height.
        /// </summary>
        /// <value>
        ///     The Height.
        /// </value>
        public float Scaling
        {
            get => _scaling;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator,it is close enough here
                if (_scaling == value) return;

                _scaling = value;
                OnPropertyChanged(nameof(Scaling));
            }
        }

        /// <summary>
        ///     Gets the explorer command.
        /// </summary>
        /// <value>
        ///     The explorer command.
        /// </value>
        public ICommand OkayCommand =>
            _okayCommand ??= new DelegateCommand<Window>(OkayAction, CanExecute);

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
        ///     Okays the action.
        /// </summary>
        /// <param name="window">The object.</param>
        private void OkayAction(Window window)
        {
            SlimViewerRegister.Scaling = Scaling;
            SlimViewerRegister.Degree = Degree;

            window.Close();
        }
    }
}