/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ScaleView.cs
 * PURPOSE:     View Model for Scale
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

using System.Windows;
using System.Windows.Input;
using ViewModel;

namespace SlimViews
{
    /// <summary>
    ///     View model for scaling operations.
    /// </summary>
    internal sealed class ScaleView : ViewModelBase
    {
        private int _degree;
        private ICommand _okayCommand;
        private float _scaling = 1;

        /// <summary>
        ///     Gets or sets the rotation degree. Valid range: -360 to 360.
        /// </summary>
        public int Degree
        {
            get => _degree;
            set
            {
                if (_degree != value && IsValidDegree(value))
                {
                    _degree = value;
                    OnPropertyChanged(nameof(Degree));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the scaling factor.
        /// </summary>
        public float Scaling
        {
            get => _scaling;
            set
            {
                if (!Equals(_scaling, value))
                {
                    _scaling = value;
                    OnPropertyChanged(nameof(Scaling));
                }
            }
        }

        /// <summary>
        ///     Command for confirming the scaling and rotation values.
        /// </summary>
        public ICommand OkayCommand =>
            _okayCommand ??= new DelegateCommand<Window>(OkayAction, CanExecute);

        /// <summary>
        ///     Determines whether the command can execute. Currently always returns true.
        /// </summary>
        /// <param name="obj">The parameter passed to the command.</param>
        /// <returns>True if the command can execute; otherwise, false.</returns>
        public bool CanExecute(object obj)
        {
            return true;
        }

        /// <summary>
        ///     Executes the confirmation action.
        /// </summary>
        /// <param name="window">The window to close after confirming values.</param>
        private void OkayAction(Window window)
        {
            SlimViewerRegister.Scaling = Scaling;
            SlimViewerRegister.Degree = Degree;
            window.Close();
        }

        /// <summary>
        ///     Validates the degree value.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the degree is within the valid range; otherwise, false.</returns>
        private static bool IsValidDegree(int value)
        {
            return value is >= -360 and <= 360;
        }
    }
}