/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        ScaleView.cs
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
        /// <summary>
        /// The degree
        /// </summary>
        private int _degree;

        /// <summary>
        /// The okay command
        /// </summary>
        private ICommand _okayCommand;

        /// <summary>
        /// The scaling
        /// </summary>
        private float _scaling = 1;

        /// <summary>
        ///     Gets or sets the rotation degree. Valid range: -360 to 360.
        /// </summary>
        public int Degree
        {
            get => _degree;
            set
            {
                if (IsValidDegree(value))
                {
                    _degree = value;
                }

                // Always raise property changed, even if validation fails.
                // This forces the UI TextBox to revert to the previous valid number
                // instead of leaving the invalid text on the screen.
                OnPropertyChanged(nameof(Degree));
            }
        }

        /// <summary>
        ///     Gets or sets the scaling factor. Must be greater than 0.
        /// </summary>
        public float Scaling
        {
            get => _scaling;
            set
            {
                // Basic validation: You can't scale an image to 0 or negative size
                if (value > 0)
                {
                    _scaling = value;
                }

                OnPropertyChanged(nameof(Scaling));
            }
        }

        /// <summary>
        ///     Command for confirming the scaling and rotation values.
        /// </summary>
        public ICommand OkayCommand =>
            _okayCommand ??= new DelegateCommand<Window>(OkayAction, CanExecute);

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