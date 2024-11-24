/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/ConverterView.cs
 * PURPOSE:     View Model for the Converter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Imaging;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Converter View
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    internal sealed class ConverterView : ViewModelBase
    {
        /// <summary>
        ///     The extension select
        /// </summary>
        private string _extensionSelect;

        /// <summary>
        ///     The source
        /// </summary>
        private string _source;

        /// <summary>
        ///     The source select
        /// </summary>
        private string _sourceSelect;

        /// <summary>
        ///     The target
        /// </summary>
        private string _target;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConverterView" /> class.
        /// </summary>
        public ConverterView()
        {
            OkayCommand = new DelegateCommand<Window>(OkayAction, CanExecute);
        }

        /// <summary>
        ///     Gets the okay command.
        /// </summary>
        /// <value>
        ///     The okay command.
        /// </value>
        public ICommand OkayCommand { get; }

        /// <summary>
        ///     Gets the selected source.
        /// </summary>
        /// <value>
        ///     The selected source.
        /// </value>
        public List<string> SelectedSource => ImagingResources.Appendix;

        /// <summary>
        ///     Gets the extension source.
        /// </summary>
        /// <value>
        ///     The extension source.
        /// </value>
        public List<string> ExtensionSource => ImagingResources.Appendix;

        /// <summary>
        ///     Gets or sets the extension select.
        /// </summary>
        /// <value>
        ///     The extension select.
        /// </value>
        public string ExtensionSelect
        {
            get => _extensionSelect;
            set => SetField(ref _extensionSelect, value, () => Target = value, nameof(ExtensionSelect));
        }

        /// <summary>
        ///     Gets or sets the source select.
        /// </summary>
        /// <value>
        ///     The source select.
        /// </value>
        public string SourceSelect
        {
            get => _sourceSelect;
            set => SetField(ref _sourceSelect, value, () => Source = value, nameof(SourceSelect));
        }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public string Source
        {
            get => _source;
            set => SetProperty(ref _source, value, nameof(Source));
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        public string Target
        {
            get => _target;
            set => SetProperty(ref _target, value, nameof(Target));
        }

        /// <summary>
        ///     Sets the property with an action.
        /// </summary>
        /// <typeparam name="T">Generic Parameter</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="updateAction">The action to perform when setting the value.</param>
        /// <param name="propertyName">Name of the property.</param>
        private void SetField<T>(ref T field, T value, Action updateAction, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            updateAction?.Invoke();
            OnPropertyChanged(propertyName);
        }

        /// <inheritdoc cref="CanExecute" />
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
        public new bool CanExecute(object obj)
        {
            return !string.IsNullOrEmpty(_source) && !string.IsNullOrEmpty(_target);
        }

        /// <summary>
        ///     Okays the action.
        /// </summary>
        /// <param name="window">The window to close.</param>
        private void OkayAction(Window window)
        {
            SlimViewerRegister.Source = Source;
            SlimViewerRegister.Target = Target;
            window.Close();
        }
    }
}