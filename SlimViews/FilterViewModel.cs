using System;
using System.Collections.Generic;
using System.ComponentModel;
using Imaging;

namespace SlimViews
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        private ImageFilterConfig _currentConfig;
        private ImageFilters _selectedFilter;

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
        public bool IsFactorActive { get; set; }
        public bool IsBiasActive { get; set; }
        public bool IsSigmaActive { get; set; }
        public bool IsBaseWindowSizeActive { get; set; }
        public bool IsScaleActive { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateActiveProperties()
        {
            var usedProperties = ImageRegister.GetUsedProperties(SelectedFilter);

            IsFactorActive = usedProperties.Contains(nameof(ImageFilterConfig.Factor));
            IsBiasActive = usedProperties.Contains(nameof(ImageFilterConfig.Bias));
            IsSigmaActive = usedProperties.Contains(nameof(ImageFilterConfig.Sigma));
            IsBaseWindowSizeActive = usedProperties.Contains(nameof(ImageFilterConfig.BaseWindowSize));
            IsScaleActive = usedProperties.Contains(nameof(ImageFilterConfig.Scale));

            // Notify UI about changes
            OnPropertyChanged(nameof(IsFactorActive));
            OnPropertyChanged(nameof(IsBiasActive));
            OnPropertyChanged(nameof(IsSigmaActive));
            OnPropertyChanged(nameof(IsBaseWindowSizeActive));
            OnPropertyChanged(nameof(IsScaleActive));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}