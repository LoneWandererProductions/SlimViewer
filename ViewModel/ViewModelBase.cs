/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     ViewModel
 * FILE:        ViewModel/ViewModelBase.cs
 * PURPOSE:     Basics for my View Model, can be reused as needed and extended
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    /// <inheritdoc />
    /// <summary>
    ///     Basic stuff for my View Models
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <inheritdoc />
        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property and raises PropertyChanged if the value changes.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the field storing the property value.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="propertyName">Name of the property (optional, automatically provided by caller).</param>
        /// <returns>If property was changed.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the nested property.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>If property was changed.</returns>
        protected bool SetNestedProperty<T>(Func<T> getter, Action<T> setter, T value,
            [CallerMemberName] string? propertyName = null)
        {
            var current = getter();
            if (EqualityComparer<T>.Default.Equals(current, value))
                return false;

            setter(value);
            OnPropertyChanged(propertyName);
            return true;
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
        protected bool CanExecute(object obj)
        {
            // check if executing is allowed, not used right now
            return true;
        }

        /// <summary>
        ///     Determines whether this instance can execute the specified object.
        /// </summary>
        /// <typeparam name="T">Generic Parameter</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can execute the specified object; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanExecute<T>(T obj)
        {
            // check if executing is allowed, not used right now
            return true;
        }
    }
}