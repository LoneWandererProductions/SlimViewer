using Imaging;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Drawing;

namespace SlimViews
{
	public class TextureConfigView : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     Gets the filter options.
		/// </summary>
		/// <value>
		///     The filter options.
		/// </value>
		public IEnumerable<TextureType> TextureOptions =>
			Enum.GetValues(typeof(TextureType)) as IEnumerable<TextureType>;

		private TextureType _selectedTexture;

		public TextureType SelectedTexture
		{
			get => _selectedTexture;
			set => SetProperty(ref _selectedTexture, value, nameof(SelectedTexture));
		}

		private int _minValue;
		public int MinValue
		{
			get => _minValue;
			set => SetProperty(ref _minValue, value, nameof(MinValue));
		}

		private int _maxValue;
		public int MaxValue
		{
			get => _maxValue;
			set => SetProperty(ref _maxValue, value, nameof(MaxValue));
		}

		private int _alpha;
		public int Alpha
		{
			get => _alpha;
			set => SetProperty(ref _alpha, value, nameof(Alpha));
		}

		private double _xPeriod;
		public double XPeriod
		{
			get => _xPeriod;
			set => SetProperty(ref _xPeriod, value, nameof(XPeriod));
		}

		private double _yPeriod;
		public double YPeriod
		{
			get => _yPeriod;
			set => SetProperty(ref _yPeriod, value, nameof(YPeriod));
		}

		private double _turbulencePower;
		public double TurbulencePower
		{
			get => _turbulencePower;
			set => SetProperty(ref _turbulencePower, value, nameof(TurbulencePower));
		}

		private double _turbulenceSize;
		public double TurbulenceSize
		{
			get => _turbulenceSize;
			set => SetProperty(ref _turbulenceSize, value, nameof(TurbulenceSize));
		}

		private Color _baseColor;
		public Color BaseColor
		{
			get => _baseColor;
			set => SetProperty(ref _baseColor, value, nameof(BaseColor));
		}

		private bool _isMonochrome;
		public bool IsMonochrome
		{
			get => _isMonochrome;
			set => SetProperty(ref _isMonochrome, value, nameof(IsMonochrome));
		}

		private bool _isTiled;
		public bool IsTiled
		{
			get => _isTiled;
			set => SetProperty(ref _isTiled, value, nameof(IsTiled));
		}

		private bool _useSmoothNoise;
		public bool UseSmoothNoise
		{
			get => _useSmoothNoise;
			set => SetProperty(ref _useSmoothNoise, value, nameof(UseSmoothNoise));
		}

		private bool _useTurbulence;
		public bool UseTurbulence
		{
			get => _useTurbulence;
			set => SetProperty(ref _useTurbulence, value, nameof(UseTurbulence));
		}

		private double _xyPeriod;
		public double XyPeriod
		{
			get => _xyPeriod;
			set => SetProperty(ref _xyPeriod, value, nameof(XyPeriod));
		}

		private int _lineSpacing;
		public int LineSpacing
		{
			get => _lineSpacing;
			set => SetProperty(ref _lineSpacing, value, nameof(LineSpacing));
		}

		private Color _lineColor;
		public Color LineColor
		{
			get => _lineColor;
			set => SetProperty(ref _lineColor, value, nameof(LineColor));
		}

		private int _lineThickness;
		public int LineThickness
		{
			get => _lineThickness;
			set => SetProperty(ref _lineThickness, value, nameof(LineThickness));
		}

		private double _angle1;
		public double Angle1
		{
			get => _angle1;
			set => SetProperty(ref _angle1, value, nameof(Angle1));
		}

		private double _angle2;
		public double Angle2
		{
			get => _angle2;
			set => SetProperty(ref _angle2, value, nameof(Angle2));
		}

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

		/// <summary>
		/// Initializes a new instance of the <see cref="TextureConfigView"/> class.
		/// </summary>
		public TextureConfigView()
		{
			var defaultConfig = new TextureConfiguration(); // Initialize with default values

			// Set properties from the default configuration
			MinValue = defaultConfig.MinValue;
			MaxValue = defaultConfig.MaxValue;
			Alpha = defaultConfig.Alpha;
			XPeriod = defaultConfig.XPeriod;
			YPeriod = defaultConfig.YPeriod;
			TurbulencePower = defaultConfig.TurbulencePower;
			TurbulenceSize = defaultConfig.TurbulenceSize;
			BaseColor = defaultConfig.BaseColor;
			IsMonochrome = defaultConfig.IsMonochrome;
			IsTiled = defaultConfig.IsTiled;
			UseSmoothNoise = defaultConfig.UseSmoothNoise;
			UseTurbulence = defaultConfig.UseTurbulence;
			XyPeriod = defaultConfig.XyPeriod;
			LineSpacing = defaultConfig.LineSpacing;
			LineColor = defaultConfig.LineColor;
			LineThickness = defaultConfig.LineThickness;
			Angle1 = defaultConfig.Angle1;
			Angle2 = defaultConfig.Angle2;

			UpdateActiveProperties();
		}

		/// <summary>
		/// Updates the active properties.
		/// </summary>
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

		/// <summary>
		/// Saves the settings.
		/// </summary>
		private void SaveSettings()
		{
			// Gather the current properties into a TextureConfig object
			var config = new TextureConfiguration
			{
				MinValue = MinValue,
				MaxValue = MaxValue,
				Alpha = Alpha,
				XPeriod = XPeriod,
				YPeriod = YPeriod,
				TurbulencePower = TurbulencePower,
				TurbulenceSize = TurbulenceSize,
				BaseColor = BaseColor,
				IsMonochrome = IsMonochrome,
				IsTiled = IsTiled,
				UseSmoothNoise = UseSmoothNoise,
				UseTurbulence = UseTurbulence,
				XyPeriod = XyPeriod,
				LineSpacing = LineSpacing,
				LineColor = LineColor,
				LineThickness = LineThickness,
				Angle1 = Angle1,
				Angle2 = Angle2
			};

			// Update the settings in ImageRegister
			ImageRegister.SetSettings(SelectedTexture, config);
		}

		/// <summary>
		/// Called when [property changed].
		/// </summary>
		/// <param name="name">The name.</param>
		protected void OnPropertyChanged(string name) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		/// <summary>
		/// Sets the property.
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
	}
}
