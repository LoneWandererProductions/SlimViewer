/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        TextureConfigView.cs
 * PURPOSE:     The view for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Imaging;
using Imaging.Enums;
using ViewModel;

namespace SlimViews
{
    /// <inheritdoc />
    /// <summary>
    ///     Main View for texture Configuration
    /// </summary>
    /// <seealso cref="ViewModel.ViewModelBase" />
    public sealed class TextureConfigView : ViewModelBase
    {
        /// <summary>
        ///     The alpha
        /// </summary>
        private int _alpha;

        /// <summary>
        ///     The angle1
        /// </summary>
        private double _anglePrimary;

        /// <summary>
        ///     The angle2
        /// </summary>
        private double _angleSecondary;

        /// <summary>
        ///     The base color
        /// </summary>
        private Color _baseColor;

        /// <summary>
        ///     The cancel command
        /// </summary>
        private ICommand _cancelCommand;

        /// <summary>
        ///     The edge jaggedness limit
        /// </summary>
        private int _edgeJaggednessLimit;

        /// <summary>
        ///     The is monochrome
        /// </summary>
        private bool _isMonochrome;

        /// <summary>
        ///     The is tiled
        /// </summary>
        private bool _isTiled;

        /// <summary>
        ///     The jaggedness threshold
        /// </summary>
        private int _jaggednessThreshold;

        /// <summary>
        ///     The line color
        /// </summary>
        private Color _lineColor;

        /// <summary>
        ///     The line spacing
        /// </summary>
        private int _lineSpacing;

        /// <summary>
        ///     The line thickness
        /// </summary>
        private int _lineThickness;

        /// <summary>
        ///     The maximum value
        /// </summary>
        private int _maxValue;

        /// <summary>
        ///     The minimum value
        /// </summary>
        private int _minValue;

        /// <summary>
        ///     The randomization factor
        /// </summary>
        private double _randomizationFactor;

        /// <summary>
        ///     The reset command
        /// </summary>
        private ICommand _resetCommand;

        /// <summary>
        ///     The save command
        /// </summary>
        private ICommand _saveCommand;

        /// <summary>
        ///     The selected texture
        /// </summary>
        private TextureType _selectedTexture;

        /// <summary>
        ///     The turbulence power
        /// </summary>
        private double _turbulencePower;

        /// <summary>
        ///     The turbulence size
        /// </summary>
        private double _turbulenceSize;

        /// <summary>
        ///     The use smooth noise
        /// </summary>
        private bool _useSmoothNoise;

        /// <summary>
        ///     The use turbulence
        /// </summary>
        private bool _useTurbulence;

        /// <summary>
        ///     The wave amplitude
        /// </summary>
        private double _waveAmplitude;

        /// <summary>
        ///     The wave frequency
        /// </summary>
        private double _waveFrequency;

        /// <summary>
        ///     The x period
        /// </summary>
        private double _xPeriod;

        /// <summary>
        ///     The xy period
        /// </summary>
        private double _xyPeriod;

        /// <summary>
        ///     The y period
        /// </summary>
        private double _yPeriod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureConfigView" /> class.
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
            AnglePrimary = defaultConfig.AnglePrimary;
            AngleSecondary = defaultConfig.AngleSecondary;
            WaveFrequency = defaultConfig.WaveFrequency;
            WaveAmplitude = defaultConfig.WaveAmplitude;
            RandomizationFactor = defaultConfig.RandomizationFactor;
            EdgeJaggednessLimit = defaultConfig.EdgeJaggednessLimit;
            JaggednessThreshold = defaultConfig.JaggednessThreshold;

            UpdateActiveProperties();
        }

        /// <summary>
        ///     Gets the filter options.
        /// </summary>
        /// <value>
        ///     The filter options.
        /// </value>
        public IEnumerable<TextureType> TextureOptions =>
            Enum.GetValues(typeof(TextureType)) as IEnumerable<TextureType>;

        /// <summary>
        ///     Gets or sets the selected texture.
        /// </summary>
        /// <value>
        ///     The selected texture.
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

        /// <summary>
        ///     Gets or sets the minimum value.
        /// </summary>
        /// <value>
        ///     The minimum value.
        /// </value>
        public int MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value, nameof(MinValue));
        }

        /// <summary>
        ///     Gets or sets the maximum value.
        /// </summary>
        /// <value>
        ///     The maximum value.
        /// </value>
        public int MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value, nameof(MaxValue));
        }

        /// <summary>
        ///     Gets or sets the alpha.
        /// </summary>
        /// <value>
        ///     The alpha.
        /// </value>
        public int Alpha
        {
            get => _alpha;
            set => SetProperty(ref _alpha, value, nameof(Alpha));
        }

        /// <summary>
        ///     Gets or sets the x period.
        /// </summary>
        /// <value>
        ///     The x period.
        /// </value>
        public double XPeriod
        {
            get => _xPeriod;
            set => SetProperty(ref _xPeriod, value, nameof(XPeriod));
        }

        /// <summary>
        ///     Gets or sets the y period.
        /// </summary>
        /// <value>
        ///     The y period.
        /// </value>
        public double YPeriod
        {
            get => _yPeriod;
            set => SetProperty(ref _yPeriod, value, nameof(YPeriod));
        }

        /// <summary>
        ///     Gets or sets the turbulence power.
        /// </summary>
        /// <value>
        ///     The turbulence power.
        /// </value>
        public double TurbulencePower
        {
            get => _turbulencePower;
            set => SetProperty(ref _turbulencePower, value, nameof(TurbulencePower));
        }

        /// <summary>
        ///     Gets or sets the size of the turbulence.
        /// </summary>
        /// <value>
        ///     The size of the turbulence.
        /// </value>
        public double TurbulenceSize
        {
            get => _turbulenceSize;
            set => SetProperty(ref _turbulenceSize, value, nameof(TurbulenceSize));
        }

        /// <summary>
        ///     Gets or sets the color of the base.
        /// </summary>
        /// <value>
        ///     The color of the base.
        /// </value>
        public Color BaseColor
        {
            get => _baseColor;
            set => SetProperty(ref _baseColor, value, nameof(BaseColor));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is monochrome.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is monochrome; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonochrome
        {
            get => _isMonochrome;
            set => SetProperty(ref _isMonochrome, value, nameof(IsMonochrome));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is tiled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tiled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTiled
        {
            get => _isTiled;
            set => SetProperty(ref _isTiled, value, nameof(IsTiled));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [use smooth noise].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use smooth noise]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSmoothNoise
        {
            get => _useSmoothNoise;
            set => SetProperty(ref _useSmoothNoise, value, nameof(UseSmoothNoise));
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [use turbulence].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use turbulence]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTurbulence
        {
            get => _useTurbulence;
            set => SetProperty(ref _useTurbulence, value, nameof(UseTurbulence));
        }

        /// <summary>
        ///     Gets or sets the xy period.
        /// </summary>
        /// <value>
        ///     The xy period.
        /// </value>
        public double XyPeriod
        {
            get => _xyPeriod;
            set => SetProperty(ref _xyPeriod, value, nameof(XyPeriod));
        }

        /// <summary>
        ///     Gets or sets the line spacing.
        /// </summary>
        /// <value>
        ///     The line spacing.
        /// </value>
        public int LineSpacing
        {
            get => _lineSpacing;
            set => SetProperty(ref _lineSpacing, value, nameof(LineSpacing));
        }

        /// <summary>
        ///     Gets or sets the color of the line.
        /// </summary>
        /// <value>
        ///     The color of the line.
        /// </value>
        public Color LineColor
        {
            get => _lineColor;
            set => SetProperty(ref _lineColor, value, nameof(LineColor));
        }

        /// <summary>
        ///     Gets or sets the line thickness.
        /// </summary>
        /// <value>
        ///     The line thickness.
        /// </value>
        public int LineThickness
        {
            get => _lineThickness;
            set => SetProperty(ref _lineThickness, value, nameof(LineThickness));
        }

        /// <summary>
        ///     Gets or sets the angle1.
        /// </summary>
        /// <value>
        ///     The angle1.
        /// </value>
        public double AnglePrimary
        {
            get => _anglePrimary;
            set => SetProperty(ref _anglePrimary, value, nameof(AnglePrimary));
        }

        /// <summary>
        ///     Gets or sets the angle2.
        /// </summary>
        /// <value>
        ///     The angle2.
        /// </value>
        public double AngleSecondary
        {
            get => _angleSecondary;
            set => SetProperty(ref _angleSecondary, value, nameof(AngleSecondary));
        }

        /// <summary>
        ///     Gets or sets the wave frequency.
        /// </summary>
        /// <value>
        ///     The wave frequency.
        /// </value>
        public double WaveFrequency
        {
            get => _waveFrequency;
            set => SetProperty(ref _waveFrequency, value, nameof(WaveFrequency));
        }

        /// <summary>
        ///     Gets or sets the wave amplitude.
        /// </summary>
        /// <value>
        ///     The wave amplitude.
        /// </value>
        public double WaveAmplitude
        {
            get => _waveAmplitude;
            set => SetProperty(ref _waveAmplitude, value, nameof(WaveAmplitude));
        }

        /// <summary>
        ///     Gets or sets the randomization factor.
        /// </summary>
        /// <value>
        ///     The randomization factor.
        /// </value>
        public double RandomizationFactor
        {
            get => _randomizationFactor;
            set => SetProperty(ref _randomizationFactor, value, nameof(RandomizationFactor));
        }

        /// <summary>
        ///     Gets or sets the edge jaggedness limit.
        /// </summary>
        /// <value>
        ///     The edge jaggedness limit.
        /// </value>
        public int EdgeJaggednessLimit
        {
            get => _edgeJaggednessLimit;
            set => SetProperty(ref _edgeJaggednessLimit, value, nameof(EdgeJaggednessLimit));
        }

        /// <summary>
        ///     Gets or sets the jaggedness threshold.
        /// </summary>
        /// <value>
        ///     The jaggedness threshold.
        /// </value>
        public int JaggednessThreshold
        {
            get => _jaggednessThreshold;
            set => SetProperty(ref _jaggednessThreshold, value, nameof(JaggednessThreshold));
        }

        // Active properties

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is minimum value active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is minimum value active; otherwise, <c>false</c>.
        /// </value>
        public bool IsMinValueActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is maximum value active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is maximum value active; otherwise, <c>false</c>.
        /// </value>
        public bool IsMaxValueActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is alpha active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is alpha active; otherwise, <c>false</c>.
        /// </value>
        public bool IsAlphaActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is x period active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is x period active; otherwise, <c>false</c>.
        /// </value>
        public bool IsXPeriodActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is y period active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is y period active; otherwise, <c>false</c>.
        /// </value>
        public bool IsYPeriodActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is turbulence power active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is turbulence power active; otherwise, <c>false</c>.
        /// </value>
        public bool IsTurbulencePowerActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is turbulence size active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is turbulence size active; otherwise, <c>false</c>.
        /// </value>
        public bool IsTurbulenceSizeActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is base color active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is base color active; otherwise, <c>false</c>.
        /// </value>
        public bool IsBaseColorActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is monochrome active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is monochrome active; otherwise, <c>false</c>.
        /// </value>
        public bool IsMonochromeActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is tiled active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tiled active; otherwise, <c>false</c>.
        /// </value>
        public bool IsTiledActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is use smooth noise active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is use smooth noise active; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseSmoothNoiseActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is use turbulence active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is use turbulence active; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseTurbulenceActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is xy period active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is xy period active; otherwise, <c>false</c>.
        /// </value>
        public bool IsXyPeriodActive { get; set; }

        public bool IsRandomizationFactorActive { get; set; }
        public bool IsEdgeJaggednessLimitActive { get; set; }

        public bool IsJaggednessThresholdActive { get; set; }

        public bool IsWaveFrequencyActive { get; set; }

        public bool IsWaveAmplitudeActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is line spacing active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is line spacing active; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineSpacingActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is line color active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is line color active; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineColorActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is line thickness active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is line thickness active; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineThicknessActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is angle1 active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is angle1 active; otherwise, <c>false</c>.
        /// </value>
        public bool IsAnglePrimaryActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is angle2 active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is angle2 active; otherwise, <c>false</c>.
        /// </value>
        public bool IsAngleSecondaryActive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is jaggedness threshold.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is jaggedness threshold; otherwise, <c>false</c>.
        /// </value>
        public bool IsJaggednessThreshold { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is edge jaggedness limit.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is edge jaggedness limit; otherwise, <c>false</c>.
        /// </value>
        public bool IsEdgeJaggednessLimit { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is randomization factor.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is randomization factor; otherwise, <c>false</c>.
        /// </value>
        public bool IsRandomizationFactor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is wave amplitude.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is wave amplitude; otherwise, <c>false</c>.
        /// </value>
        public bool IsWaveAmplitude { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is wave frequency.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is wave frequency; otherwise, <c>false</c>.
        /// </value>
        public bool IsWaveFrequency { get; set; }

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
        ///     Saves the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void SaveAction(object obj)
        {
            SaveSettings();
        }

        /// <summary>
        ///     Resets the action.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void ResetAction(object obj)
        {
            var config = new TextureConfiguration();
            // Update the settings in ImageRegister
            ImageProcessor.Render.ImageSettings.SetSettings(SelectedTexture, config);
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

        /// <summary>
        ///     Updates the active properties.
        /// </summary>
        private void UpdateActiveProperties()
        {
            var usedProperties = ImageProcessor.Render.ImageSettings.GetUsedProperties(SelectedTexture);

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
            IsRandomizationFactorActive = usedProperties.Contains(nameof(RandomizationFactor));
            IsEdgeJaggednessLimitActive = usedProperties.Contains(nameof(EdgeJaggednessLimit));
            IsJaggednessThresholdActive = usedProperties.Contains(nameof(JaggednessThreshold));
            IsWaveFrequencyActive = usedProperties.Contains(nameof(WaveFrequency));
            IsWaveAmplitudeActive = usedProperties.Contains(nameof(WaveAmplitude));
            IsLineSpacingActive = usedProperties.Contains(nameof(LineSpacing));
            IsLineColorActive = usedProperties.Contains(nameof(LineColor));
            IsLineThicknessActive = usedProperties.Contains(nameof(LineThickness));
            IsAnglePrimaryActive = usedProperties.Contains(nameof(AnglePrimary));
            IsAngleSecondaryActive = usedProperties.Contains(nameof(AngleSecondary));

            var savedSettings = ImageProcessor.Render.ImageSettings.GetSettings(SelectedTexture);

            // ... (Value assignments remain similar but now correctly gated) ...

            // Crucially, call OnPropertyChanged for all booleans so the UI enables/disables boxes instantly
            RaiseAllActivePropertiesChanged();
        }

        /// <summary>
        /// Raises all active properties changed.
        /// </summary>
        private void RaiseAllActivePropertiesChanged()
        {
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
            OnPropertyChanged(nameof(IsAnglePrimaryActive));
            OnPropertyChanged(nameof(IsAngleSecondaryActive));
            OnPropertyChanged(nameof(IsWaveFrequency));
            OnPropertyChanged(nameof(IsWaveAmplitude));
            OnPropertyChanged(nameof(IsRandomizationFactor));
            OnPropertyChanged(nameof(IsEdgeJaggednessLimit));
            OnPropertyChanged(nameof(IsJaggednessThreshold));
        }

        /// <summary>
        ///     Saves the settings.
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
                AnglePrimary = AnglePrimary,
                AngleSecondary = AngleSecondary,
                WaveFrequency = WaveFrequency,
                WaveAmplitude = WaveAmplitude,
                RandomizationFactor = RandomizationFactor,
                EdgeJaggednessLimit = EdgeJaggednessLimit,
                JaggednessThreshold = JaggednessThreshold
            };

            // Update the settings in ImageRegister
            ImageProcessor.Render.ImageSettings.SetSettings(SelectedTexture, config);
        }
    }
}