/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        ConverterView.cs
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
    ///     Converter View Model
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    internal sealed class ConverterView : ViewModelBase
    {
        /// <summary>
        /// The extension select
        /// </summary>
        private string _extensionSelect;

        /// <summary>
        /// The source
        /// </summary>
        private string _source;

        /// <summary>
        /// The source select
        /// </summary>
        private string _sourceSelect;

        /// <summary>
        /// The target
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
        public ICommand OkayCommand { get; }

        /// <summary>
        ///     Gets the available source extensions.
        /// </summary>
        public List<string> SelectedSource => ImagingResources.Appendix;

        /// <summary>
        ///     Gets the available target extensions.
        /// </summary>
        public List<string> ExtensionSource => ImagingResources.Appendix;

        /// <summary>
        ///     Gets or sets the target extension selection.
        /// </summary>
        public string ExtensionSelect
        {
            get => _extensionSelect;
            set => SetField(ref _extensionSelect, value, () => Target = value, nameof(ExtensionSelect));
        }

        /// <summary>
        ///     Gets or sets the source extension selection.
        /// </summary>
        public string SourceSelect
        {
            get => _sourceSelect;
            set => SetField(ref _sourceSelect, value, () => Source = value, nameof(SourceSelect));
        }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        public string Source
        {
            get => _source;
            set
            {
                SetProperty(ref _source, value, nameof(Source));
                // Force the UI to re-evaluate the Okay Button's CanExecute state
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public string Target
        {
            get => _target;
            set
            {
                SetProperty(ref _target, value, nameof(Target));
                // Force the UI to re-evaluate the Okay Button's CanExecute state
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        ///     Sets the property with an action.
        /// </summary>
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
        public new bool CanExecute(object obj)
        {
            // The button is only enabled if:
            // 1. Source is selected
            // 2. Target is selected
            // 3. Source and Target are NOT the exact same extension
            return !string.IsNullOrEmpty(_source) &&
                   !string.IsNullOrEmpty(_target) &&
                   !string.Equals(_source, _target, StringComparison.OrdinalIgnoreCase);
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