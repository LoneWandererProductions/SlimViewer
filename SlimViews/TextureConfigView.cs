/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimViews/TextureConfigView.cs
 * PURPOSE:     The view for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using Imaging;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Drawing;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    /// Main View for texture Configuration
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public sealed class TextureConfigView : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<TextureType> TextureOptions =>
            Enum.GetValues(typeof(TextureType)) as IEnumerable<TextureType>;

        /// <summary>
        /// The selected texture
        /// </summary>
        private TextureType _selectedTexture;

        /// <summary>
        /// Gets or sets the selected texture.
        /// </summary>
        /// <value>
        /// The selected texture.
        /// </value>
        public TextureType SelectedTexture
        {
            get => _selectedTexture;
            set
            {
                SetProperty(ref _selectedTexture, value, nameof(SelectedTexture));
                UpdateActiveProperties();
            }
        }

        private double _angle2;
        private double _angle1;
        private int _lineThickness;
        private Color _lineColor;
        private int _lineSpacing;
        private double _xyPeriod;
        private bool _useTurbulence;
        private bool _useSmoothNoise;
        private bool _isTiled;
        private bool _isMonochrome;
        private Color _baseColor;
        private double _turbulenceSize;
        private double _turbulencePower;
        private double _yPeriod;
        private double _xPeriod;
        private int _alpha;
        private int _maxValue;
        private int _minValue;

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public int MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value, nameof(MinValue));
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public int MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value, nameof(MaxValue));
        }

        /// <summary>
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>
        /// The alpha.
        /// </value>
        public int Alpha
        {
            get => _alpha;
            set => SetProperty(ref _alpha, value, nameof(Alpha));
        }

        /// <summary>
        /// Gets or sets the x period.
        /// </summary>
        /// <value>
        /// The x period.
        /// </value>
        public double XPeriod
        {
            get => _xPeriod;
            set => SetProperty(ref _xPeriod, value, nameof(XPeriod));
        }

        /// <summary>
        /// Gets or sets the y period.
        /// </summary>
        /// <value>
        /// The y period.
        /// </value>
        public double YPeriod
        {
            get => _yPeriod;
            set => SetProperty(ref _yPeriod, value, nameof(YPeriod));
        }

        /// <summary>
        /// Gets or sets the turbulence power.
        /// </summary>
        /// <value>
        /// The turbulence power.
        /// </value>
        public double TurbulencePower
        {
            get => _turbulencePower;
            set => SetProperty(ref _turbulencePower, value, nameof(TurbulencePower));
        }

        /// <summary>
        /// Gets or sets the size of the turbulence.
        /// </summary>
        /// <value>
        /// The size of the turbulence.
        /// </value>
        public double TurbulenceSize
        {
            get => _turbulenceSize;
            set => SetProperty(ref _turbulenceSize, value, nameof(TurbulenceSize));
        }

        /// <summary>
        /// Gets or sets the color of the base.
        /// </summary>
        /// <value>
        /// The color of the base.
        /// </value>
        public Color BaseColor
        {
            get => _baseColor;
            set => SetProperty(ref _baseColor, value, nameof(BaseColor));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is monochrome.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is monochrome; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonochrome
        {
            get => _isMonochrome;
            set => SetProperty(ref _isMonochrome, value, nameof(IsMonochrome));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tiled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is tiled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTiled
        {
            get => _isTiled;
            set => SetProperty(ref _isTiled, value, nameof(IsTiled));
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use smooth noise].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use smooth noise]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSmoothNoise
        {
            get => _useSmoothNoise;
            set => SetProperty(ref _useSmoothNoise, value, nameof(UseSmoothNoise));
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use turbulence].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use turbulence]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTurbulence
        {
            get => _useTurbulence;
            set => SetProperty(ref _useTurbulence, value, nameof(UseTurbulence));
        }

        /// <summary>
        /// Gets or sets the xy period.
        /// </summary>
        /// <value>
        /// The xy period.
        /// </value>
        public double XyPeriod
        {
            get => _xyPeriod;
            set => SetProperty(ref _xyPeriod, value, nameof(XyPeriod));
        }

        /// <summary>
        /// Gets or sets the line spacing.
        /// </summary>
        /// <value>
        /// The line spacing.
        /// </value>
        public int LineSpacing
        {
            get => _lineSpacing;
            set => SetProperty(ref _lineSpacing, value, nameof(LineSpacing));
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>
        /// The color of the line.
        /// </value>
        public Color LineColor
        {
            get => _lineColor;
            set => SetProperty(ref _lineColor, value, nameof(LineColor));
        }

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        /// <value>
        /// The line thickness.
        /// </value>
        public int LineThickness
        {
            get => _lineThickness;
            set => SetProperty(ref _lineThickness, value, nameof(LineThickness));
        }

        /// <summary>
        /// Gets or sets the angle1.
        /// </summary>
        /// <value>
        /// The angle1.
        /// </value>
        public double Angle1
        {
            get => _angle1;
            set => SetProperty(ref _angle1, value, nameof(Angle1));
        }


        /// <summary>
        /// Gets or sets the angle2.
        /// </summary>
        /// <value>
        /// The angle2.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line color active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is line color active; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineColorActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is line thickness active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is line thickness active; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineThicknessActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is angle1 active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is angle1 active; otherwise, <c>false</c>.
        /// </value>
        public bool IsAngle1Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is angle2 active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is angle2 active; otherwise, <c>false</c>.
        /// </value>
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
            // Get the used properties for the selected texture
            var usedProperties = ImageRegister.GetUsedProperties(SelectedTexture);

            // Update active state for each property based on the selected texture
            IsMinValueActive = usedProperties.Contains(nameof(MinValue));
            IsMaxValueActive = usedProperties.Contains(nameof(MaxValue));
            IsAlphaActive = usedProperties.Contains(nameof(Alpha));
            IsXPeriodActive = usedProperties.Contains(nameof(XPeriod));
            IsYPeriodActive = usedProperties.Contains(nameof(YPeriod));
            IsTurbulencePowerActive = usedProperties.Contains(nameof(TurbulencePower));
            IsTurbulenceSizeActive = usedProperties.Contains(nameof(TurbulenceSize));
            IsBaseColorActive = usedProperties.Contains(nameof(BaseColor));
            IsMonochromeActive = usedProperties.Contains(nameof(IsMonochrome));
            IsTiledActive = usedProperties.Contains(nameof(IsTiled));
            IsUseSmoothNoiseActive = usedProperties.Contains(nameof(UseSmoothNoise));
            IsUseTurbulenceActive = usedProperties.Contains(nameof(UseTurbulence));
            IsXyPeriodActive = usedProperties.Contains(nameof(XyPeriod));

            // Retrieve the saved settings for the selected texture
            var savedSettings = ImageRegister.GetSettings(SelectedTexture);

            // Set the properties from the saved settings, if the property is active
            if (IsMinValueActive) MinValue = savedSettings.MinValue;
            if (IsMaxValueActive) MaxValue = savedSettings.MaxValue;
            if (IsAlphaActive) Alpha = savedSettings.Alpha;
            if (IsXPeriodActive) XPeriod = savedSettings.XPeriod;
            if (IsYPeriodActive) YPeriod = savedSettings.YPeriod;
            if (IsTurbulencePowerActive) TurbulencePower = savedSettings.TurbulencePower;
            if (IsTurbulenceSizeActive) TurbulenceSize = savedSettings.TurbulenceSize;
            if (IsBaseColorActive) BaseColor = savedSettings.BaseColor;
            if (IsMonochromeActive) IsMonochrome = savedSettings.IsMonochrome;
            if (IsTiledActive) IsTiled = savedSettings.IsTiled;
            if (IsUseSmoothNoiseActive) UseSmoothNoise = savedSettings.UseSmoothNoise;
            if (IsUseTurbulenceActive) UseTurbulence = savedSettings.UseTurbulence;
            if (IsXyPeriodActive) XyPeriod = savedSettings.XyPeriod;

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
        private void OnPropertyChanged(string name) =>
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
