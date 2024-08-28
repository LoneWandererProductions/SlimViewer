using Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SlimViews
{
	public class FilterViewModel : INotifyPropertyChanged
	{
		private ImageFilters _selectedFilter;
		private ImageFilterConfig _currentConfig;

		public ObservableCollection<ImageFilters> Filters { get; set; }

		public ImageFilters SelectedFilter
		{
			get => _selectedFilter;
			set
			{
				if (_selectedFilter != value)
				{
					// Save the current config before switching
					ImageRegister.FilterSettings[_selectedFilter] = _currentConfig;
					_selectedFilter = value;
					OnPropertyChanged();
					CurrentConfig = ImageRegister.GetSettings(_selectedFilter);
				}
			}
		}

		public ImageFilterConfig CurrentConfig
		{
			get => _currentConfig;
			set
			{
				_currentConfig = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public FilterViewModel()
		{
			Filters = new ObservableCollection<ImageFilters>(ImageRegister.GetAvailableFilters());
			SelectedFilter = Filters[0]; // Set default filter
		}
	}
}
