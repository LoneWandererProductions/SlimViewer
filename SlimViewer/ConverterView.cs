/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViewer/ConverterView.cs
 * PURPOSE:     View Model for the Converter
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Imaging;
using ViewModel;

namespace SlimViewer
{
    /// <inheritdoc />
    /// <summary>
    ///     Converter View
    /// </summary>
    internal sealed class ConverterView : INotifyPropertyChanged
    {
        /// <summary>
        ///     The extension select
        /// </summary>
        private string _extensionSelect;

        /// <summary>
        ///     The add command
        /// </summary>
        private ICommand _okayCommand;

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
        ///     Gets the explorer command.
        /// </summary>
        /// <value>
        ///     The explorer command.
        /// </value>
        public ICommand OkayCommand =>
            _okayCommand ??= new DelegateCommand<Window>(OkayAction, CanExecute);

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
            set
            {
                if (_extensionSelect == value) return;

                _extensionSelect = value;
                Target = value;
                OnPropertyChanged(nameof(ExtensionSelect));
            }
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
            set
            {
                if (_sourceSelect == value) return;

                _sourceSelect = value;
                Source = value;
                OnPropertyChanged(nameof(SourceSelect));
            }
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
            set
            {
                if (_source == value) return;

                _source = value;
                OnPropertyChanged(nameof(Source));
            }
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
            set
            {
                if (_target == value) return;

                _target = value;
                OnPropertyChanged(nameof(Target));
            }
        }

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
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Okays the action.
        /// </summary>
        /// <param name="window">The object.</param>
        private void OkayAction(Window window)
        {
            SlimViewerRegister.Source = Source;
            SlimViewerRegister.Target = Target;
            window.Close();
        }
    }
}