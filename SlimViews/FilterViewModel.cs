using Imaging;
using System.Collections.Generic;
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
					_selectedFilter = value;
					OnPropertyChanged();
					CurrentConfig = ImageRegister.GetSettings(_selectedFilter);
					UsedProperties = ImageRegister.GetUsedProperties(_selectedFilter);
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

		public HashSet<string> UsedProperties { get; set; }

		public bool IsPropertyUsed(string propertyName) => UsedProperties.Contains(propertyName);

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
