/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/FilterConfigView.cs
 * PURPOSE:     View Model for Filter Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Imaging;

namespace SlimViews
{
	/// <summary>
	/// Set Input Fields active or Inactive based on the used Filter
	/// </summary>
	/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
	public class FilterConfigView : INotifyPropertyChanged
    {
		/// <summary>
		/// The current configuration
		/// </summary>
		private ImageFilterConfig _currentConfig;

		/// <summary>
		/// The selected filter
		/// </summary>
		private ImageFilters _selectedFilter;

		/// <summary>
		/// The factor
		/// </summary>
		private double _factor;

		/// <summary>
		/// The bias
		/// </summary>
		private double _bias;

		/// <summary>
		/// The sigma
		/// </summary>
		private double _sigma;

		/// <summary>
		/// The base window size
		/// </summary>
		private int _baseWindowSize;

		/// <summary>
		/// The scale
		/// </summary>
		private int _scale;

		/// <summary>
		/// Gets or sets the factor.
		/// </summary>
		public double Factor
		{
			get => _factor;
			set => SetProperty(ref _factor, value, nameof(Factor));
		}

		/// <summary>
		/// Gets or sets the bias.
		/// </summary>
		public double Bias
		{
			get => _bias;
			set => SetProperty(ref _bias, value, nameof(Bias));
		}

		/// <summary>
		/// Gets or sets the sigma.
		/// </summary>
		public double Sigma
		{
			get => _sigma;
			set => SetProperty(ref _sigma, value, nameof(Sigma));
		}

		/// <summary>
		/// Gets or sets the base window size.
		/// </summary>
		public int BaseWindowSize
		{
			get => _baseWindowSize;
			set => SetProperty(ref _baseWindowSize, value, nameof(BaseWindowSize));
		}

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		public int Scale
		{
			get => _scale;
			set => SetProperty(ref _scale, value, nameof(Scale));
		}


		/// <summary>
		/// Gets or sets the selected filter.
		/// </summary>
		/// <value>
		/// The selected filter.
		/// </value>
		public ImageFilters SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                    UpdateActiveProperties();
                }
            }
        }

        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<ImageFilters> FilterOptions =>
            Enum.GetValues(typeof(ImageFilters)) as IEnumerable<ImageFilters>;

		/// <summary>
		/// Gets or sets the current configuration.
		/// </summary>
		/// <value>
		/// The current configuration.
		/// </value>
		public ImageFilterConfig CurrentConfig
        {
            get => _currentConfig;
            set
            {
                _currentConfig = value;
                OnPropertyChanged(nameof(CurrentConfig));
            }
        }

		// Local properties to control UI element states

		/// <summary>
		/// Gets or sets a value indicating whether this instance is factor active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is factor active; otherwise, <c>false</c>.
		/// </value>
		public bool IsFactorActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is bias active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is bias active; otherwise, <c>false</c>.
		/// </value>
		public bool IsBiasActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is sigma active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is sigma active; otherwise, <c>false</c>.
		/// </value>
		public bool IsSigmaActive { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is base window size active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is base window size active; otherwise, <c>false</c>.
		/// </value>
		public bool IsBaseWindowSizeActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is scale active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is scale active; otherwise, <c>false</c>.
		/// </value>
		public bool IsScaleActive { get; set; }

		/// <summary>
		///     Sets the property.
		/// </summary>
		/// <typeparam name="T">Generic Parameter</typeparam>
		/// <param name="field">The field.</param>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">Name of the property.</param>
		private void SetProperty<T>(ref T field, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return;

			field = value;
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="FilterConfigView"/> class.
		/// </summary>
		public FilterConfigView()
		{
			CurrentConfig = new ImageFilterConfig(); // Initialize with default values

			// Set properties from CurrentConfig
			Factor = CurrentConfig.Factor;
			Bias = CurrentConfig.Bias;
			Sigma = CurrentConfig.Sigma;
			BaseWindowSize = CurrentConfig.BaseWindowSize;
			Scale = CurrentConfig.Scale;

			UpdateActiveProperties();
		}

		/// <summary>
		/// Updates the active properties.
		/// </summary>
		private void UpdateActiveProperties()
        {
            var usedProperties = ImageRegister.GetUsedProperties(SelectedFilter);

            IsFactorActive = usedProperties.Contains(nameof(Factor));
            IsBiasActive = usedProperties.Contains(nameof(Bias));
            IsSigmaActive = usedProperties.Contains(nameof(Sigma));
            IsBaseWindowSizeActive = usedProperties.Contains(nameof(BaseWindowSize));
            IsScaleActive = usedProperties.Contains(nameof(Scale));

            // Notify UI about changes
            OnPropertyChanged(nameof(IsFactorActive));
            OnPropertyChanged(nameof(IsBiasActive));
            OnPropertyChanged(nameof(IsSigmaActive));
            OnPropertyChanged(nameof(IsBaseWindowSizeActive));
            OnPropertyChanged(nameof(IsScaleActive));
        }

		/// <summary>
		/// Saves the settings.
		/// </summary>
		private void SaveSettings()
		{
			// Gather the current properties into an ImageFilterConfig object
			var config = new ImageFilterConfig
			{
				Factor = Factor, // Set default value if not active
				Bias = Bias,
				Sigma = Sigma,
				BaseWindowSize = BaseWindowSize,
				Scale = Scale
			};

			// Update the settings in ImageRegister
			ImageRegister.SetSettings(SelectedFilter, config);
		}

		/// <summary>
		/// Called when [property changed].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}