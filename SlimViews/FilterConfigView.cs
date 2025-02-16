﻿/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/FilterConfigView.cs
 * PURPOSE:     View Model for Filter Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

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
    ///     Set Input Fields active or Inactive based on the used Filter
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    public sealed class FilterConfigView : ViewModelBase
    {
        /// <summary>
        ///     The current configuration
        /// </summary>
        private readonly FiltersConfig _currentConfig;

        /// <summary>
        ///     The base window size
        /// </summary>
        private int _baseWindowSize;

        /// <summary>
        ///     The bias
        /// </summary>
        private double _bias;

        /// <summary>
        ///     The cancel command
        /// </summary>
        private ICommand _cancelCommand;

        /// <summary>
        ///     The factor
        /// </summary>
        private double _factor;

        /// <summary>
        ///     The reset command
        /// </summary>
        private ICommand _resetCommand;

        /// <summary>
        ///     The save command
        /// </summary>
        private ICommand _saveCommand;

        /// <summary>
        ///     The scale
        /// </summary>
        private int _scale;

        /// <summary>
        ///     The selected filter
        /// </summary>
        private FiltersType _selectedFilter;

        /// <summary>
        ///     The sigma
        /// </summary>
        private double _sigma;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterConfigView" /> class.
        /// </summary>
        public FilterConfigView()
        {
            CurrentConfig = new FiltersConfig(); // Initialize with default values

            // Set properties from CurrentConfig
            Factor = CurrentConfig.Factor;
            Bias = CurrentConfig.Bias;
            Sigma = CurrentConfig.Sigma;
            BaseWindowSize = CurrentConfig.BaseWindowSize;
            Scale = CurrentConfig.Scale;

            UpdateActiveProperties();
        }

        /// <summary>
        ///     Gets or sets the factor.
        /// </summary>
        public double Factor
        {
            get => _factor;
            set => SetProperty(ref _factor, value, nameof(Factor));
        }

        /// <summary>
        ///     Gets or sets the bias.
        /// </summary>
        public double Bias
        {
            get => _bias;
            set => SetProperty(ref _bias, value, nameof(Bias));
        }

        /// <summary>
        ///     Gets or sets the sigma.
        /// </summary>
        public double Sigma
        {
            get => _sigma;
            set => SetProperty(ref _sigma, value, nameof(Sigma));
        }

        /// <summary>
        ///     Gets or sets the base window size.
        /// </summary>
        public int BaseWindowSize
        {
            get => _baseWindowSize;
            set => SetProperty(ref _baseWindowSize, value, nameof(BaseWindowSize));
        }

        /// <summary>
        ///     Gets or sets the scale.
        /// </summary>
        public int Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value, nameof(Scale));
        }

        /// <summary>
        ///     Gets or sets the selected filter.
        /// </summary>
        /// <value>
        ///     The selected filter.
        /// </value>
        public FiltersType SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter == value) return;

                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                UpdateActiveProperties();
            }
        }

        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<FiltersType> FilterOptions =>
            Enum.GetValues(typeof(FiltersType)) as IEnumerable<FiltersType>;

        /// <summary>
        ///     Gets or sets the current configuration.
        /// </summary>
        /// <value>
        ///     The current configuration.
        /// </value>
        public FiltersConfig CurrentConfig
        {
            get => _currentConfig;
            init
            {
                _currentConfig = value;
                OnPropertyChanged(nameof(CurrentConfig));
            }
        }

        // Local properties to control UI element states

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is factor active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is factor active; otherwise, <c>false</c>.
        /// </value>
        public bool IsFactorActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is bias active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is bias active; otherwise, <c>false</c>.
        /// </value>
        public bool IsBiasActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is sigma active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is sigma active; otherwise, <c>false</c>.
        /// </value>
        public bool IsSigmaActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is base window size active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is base window size active; otherwise, <c>false</c>.
        /// </value>
        public bool IsBaseWindowSizeActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is scale active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is scale active; otherwise, <c>false</c>.
        /// </value>
        public bool IsScaleActive { get; set; }

        /// <summary>
        ///     Gets the save command.
        /// </summary>
        /// <value>
        ///     The save command.
        /// </value>
        public ICommand SaveCommand => GetCommand(ref _saveCommand, SaveAction);

        /// <summary>
        ///     Gets the reset command.
        /// </summary>
        /// <value>
        ///     The reset command.
        /// </value>
        public ICommand ResetCommand => GetCommand(ref _resetCommand, ResetAction);

        /// <summary>
        ///     Gets the cancel command.
        /// </summary>
        /// <value>
        ///     The cancel command.
        /// </value>
        public ICommand CancelCommand => GetCommand(ref _cancelCommand, CancelAction);

        /// <summary>
        ///     Gets the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="execute">The execute.</param>
        /// <returns>The selected Command</returns>
        private ICommand GetCommand(ref ICommand command, Action<object> execute)
        {
            return command ??= new DelegateCommand<object>(execute, CanExecute);
        }

        /// <summary>
        ///     Updates the active properties.
        /// </summary>
        private void UpdateActiveProperties()
        {
            // Get the used properties for the selected filter
            var usedProperties = ImageProcessor.Render.ImageSettings.GetUsedProperties(SelectedFilter);

            // Update active state for each property based on the selected filter
            IsFactorActive = usedProperties.Contains(nameof(Factor));
            IsBiasActive = usedProperties.Contains(nameof(Bias));
            IsSigmaActive = usedProperties.Contains(nameof(Sigma));
            IsBaseWindowSizeActive = usedProperties.Contains(nameof(BaseWindowSize));
            IsScaleActive = usedProperties.Contains(nameof(Scale));

            // Retrieve the saved settings for the selected filter
            var savedSettings = ImageProcessor.Render.ImageSettings.GetSettings(SelectedFilter);

            // Set the properties from the saved settings, if the property is active
            if (IsFactorActive) Factor = savedSettings.Factor;
            if (IsBiasActive) Bias = savedSettings.Bias;
            if (IsSigmaActive) Sigma = savedSettings.Sigma;
            if (IsBaseWindowSizeActive) BaseWindowSize = savedSettings.BaseWindowSize;
            if (IsScaleActive) Scale = savedSettings.Scale;

            // Notify UI about changes
            OnPropertyChanged(nameof(IsFactorActive));
            OnPropertyChanged(nameof(IsBiasActive));
            OnPropertyChanged(nameof(IsSigmaActive));
            OnPropertyChanged(nameof(IsBaseWindowSizeActive));
            OnPropertyChanged(nameof(IsScaleActive));
        }

        /// <summary>
        ///     Saves the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveAction(object obj)
        {
            SaveSettings();
        }

        /// <summary>
        ///     Saves the settings.
        /// </summary>
        private void SaveSettings()
        {
            //create a new ImageFilterConfig object
            var config = new FiltersConfig();

            // Update or better say reset the settings in ImageRegister
            ImageProcessor.Render.ImageSettings.SetSettings(SelectedFilter, config);
            UpdateActiveProperties();
        }

        /// <summary>
        ///     Resets the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ResetAction(object obj)
        {
            var config = new FiltersConfig();
            // Update the settings in ImageRegister
            ImageProcessor.Render.ImageSettings.SetSettings(SelectedFilter, config);
        }

        /// <summary>
        ///     Cancels the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void CancelAction(object obj)
        {
            // Close the window
            if (obj is Window window) window.Close();
        }
    }
}