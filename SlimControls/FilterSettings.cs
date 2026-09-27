/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        FilterSettings.cs
 * PURPOSE:     Settings for the Filter area mode - which filter effect to apply. See AreaModeSettings for
 *              what Enabled means and why it is no longer stored here.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Imaging.Enums;

namespace SlimControls
{
    /// <summary>Settings for the "🪄 Filter" area mode.</summary>
    public sealed class FilterSettings : AreaModeSettings
    {
        private string? _filterName;

        /// <summary>Initializes a new instance of the <see cref="FilterSettings" /> class.</summary>
        /// <param name="owner">The <see cref="DrawingState" /> this settings object belongs to.</param>
        public FilterSettings(DrawingState owner) : base(owner, AreaMode.Filter)
        {
        }

        /// <summary>Gets or sets the name of the selected filter effect (see <see cref="DrawingState.Filters" />).</summary>
        public string? FilterName
        {
            get => _filterName;
            set
            {
                if (_filterName == value) return;
                _filterName = value;
                OnPropertyChanged(nameof(FilterName));
            }
        }
    }
}
