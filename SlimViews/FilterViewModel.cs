using Imaging;
using System.ComponentModel;

namespace SlimViews
{
	public class FilterViewModel : INotifyPropertyChanged
	{
		private ImageFilters _selectedFilter;
		private ImageFilterConfig _currentConfig;

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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}