using Imaging;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace SlimViews
{
	public class TextureConfigView : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Gets or sets the selected filter.
		/// </summary>
		/// <value>
		/// The selected filter.
		/// </value>
		public TextureType SelectedTexture
		{
			get => _selectedTexture;
			set
			{
				if (_selectedTexture != value)
				{
					_selectedTexture = value;
					OnPropertyChanged(nameof(SelectedTexture));
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
		public IEnumerable<TextureType> TextureOptions =>
			Enum.GetValues(typeof(TextureType)) as IEnumerable<TextureType>;

		private TextureType _selectedTexture;

		// Active properties
		public bool IsMinValueActive { get; set; }
		public bool IsMaxValueActive { get; set; }
		public bool IsAlphaActive { get; set; }
		public bool IsXPeriodActive { get; set; }
		public bool IsYPeriodActive { get; set; }
		public bool IsTurbulencePowerActive { get; set; }
		public bool IsTurbulenceSizeActive { get; set; }
		public bool IsBaseColorActive { get; set; }
		public bool IsMonochromeActive { get; set; }
		public bool IsTiledActive { get; set; }
		public bool IsUseSmoothNoiseActive { get; set; }
		public bool IsUseTurbulenceActive { get; set; }
		public bool IsXyPeriodActive { get; set; }

		public bool IsLineSpacingActive { get; set; }
		public bool IsLineColorActive { get; set; }
		public bool IsLineThicknessActive { get; set; }
		public bool IsAngle1Active { get; set; }
		public bool IsAngle2Active { get; set; }

		private void UpdateActiveProperties()
		{
			var usedProperties = ImageRegister.GetUsedProperties(SelectedTexture);

			IsMinValueActive = usedProperties.Contains(nameof(IsMinValueActive));
			IsMaxValueActive = usedProperties.Contains(nameof(IsMaxValueActive));
			IsAlphaActive = usedProperties.Contains(nameof(IsAlphaActive));
			IsXPeriodActive = usedProperties.Contains(nameof(IsXPeriodActive));
			IsYPeriodActive = usedProperties.Contains(nameof(IsYPeriodActive));
			IsTurbulencePowerActive = usedProperties.Contains(nameof(IsTurbulencePowerActive));
			IsTurbulenceSizeActive = usedProperties.Contains(nameof(IsTurbulenceSizeActive));
			IsBaseColorActive = usedProperties.Contains(nameof(IsBaseColorActive));
			IsMonochromeActive = usedProperties.Contains(nameof(IsMonochromeActive));
			IsTiledActive = usedProperties.Contains(nameof(IsTiledActive));
			IsUseSmoothNoiseActive = usedProperties.Contains(nameof(IsUseSmoothNoiseActive));
			IsUseTurbulenceActive = usedProperties.Contains(nameof(IsUseTurbulenceActive));
			IsXyPeriodActive = usedProperties.Contains(nameof(IsXyPeriodActive));

			IsLineSpacingActive = usedProperties.Contains(nameof(IsLineSpacingActive));
			IsLineColorActive = usedProperties.Contains(nameof(IsLineColorActive));
			IsLineThicknessActive = usedProperties.Contains(nameof(IsLineThicknessActive));
			IsAngle1Active = usedProperties.Contains(nameof(IsAngle1Active));
			IsAngle2Active = usedProperties.Contains(nameof(IsAngle2Active));

			// Notify UI about changes
			OnPropertyChanged(nameof(IsMinValueActive));
			OnPropertyChanged(nameof(IsMaxValueActive));
			OnPropertyChanged(nameof(IsAlphaActive));
			OnPropertyChanged(nameof(IsXPeriodActive));
			OnPropertyChanged(nameof(IsYPeriodActive));
			OnPropertyChanged(nameof(IsTurbulencePowerActive));
			OnPropertyChanged(nameof(IsTurbulenceSizeActive));
			OnPropertyChanged(nameof(IsBaseColorActive));
			OnPropertyChanged(nameof(IsMonochromeActive));
			OnPropertyChanged(nameof(IsTiledActive));
			OnPropertyChanged(nameof(IsUseSmoothNoiseActive));
			OnPropertyChanged(nameof(IsUseTurbulenceActive));
			OnPropertyChanged(nameof(IsXyPeriodActive));

			OnPropertyChanged(nameof(IsLineSpacingActive));
			OnPropertyChanged(nameof(IsLineColorActive));
			OnPropertyChanged(nameof(IsLineThicknessActive));
			OnPropertyChanged(nameof(IsAngle1Active));
			OnPropertyChanged(nameof(IsAngle2Active));
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
