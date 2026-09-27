/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        FillSettings.cs
 * PURPOSE:     Settings for the Fill area mode - a single flat fill color. See AreaModeSettings for what
 *              Enabled means and why it is no longer stored here.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    /// <summary>
    /// Settings for the "🪣 Fill" area mode.
    /// </summary>
    /// <seealso cref="SlimControls.AreaModeSettings" />
    public sealed class FillSettings : AreaModeSettings
    {
        /// <summary>
        /// The color
        /// </summary>
        private string _color = "#FFFFFF";

        /// <summary>
        /// Initializes a new instance of the <see cref="FillSettings" /> class.
        /// </summary>
        /// <param name="owner">The <see cref="DrawingState" /> this settings object belongs to.</param>
        public FillSettings(DrawingState owner) : base(owner, AreaMode.Fill)
        {
        }

        /// <summary>
        /// Gets or sets the fill color, as a hex string (e.g. "#FFFFFF") - matches DrawingState.BrushColor's convention.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }
}