/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        AreaModeSettings.cs
 * PURPOSE:     Common base for FillSettings/TextureSettings/FilterSettings/EraseSettings. Each area mode
 *              (Imaging.Enums.AreaMode) has its own settings object with its own payload (a fill color, a
 *              texture name, ...), but all four share exactly one other thing: an Enabled flag that is true
 *              only while DrawingState.ActiveAreaMode equals the mode this object represents. Previously
 *              each of the four classes carried its own independent Enabled bool, and DrawingState.
 *              UpdateSubStates() set all four by hand every time the tool or area mode changed - four
 *              near-identical classes, and one more place to remember to update if a fifth mode is ever
 *              added. Here Enabled is instead computed straight from the owner, so it can never drift out
 *              of sync and there is nothing left for UpdateSubStates() to do for it at all.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.ComponentModel;
using Imaging.Enums;

namespace SlimControls
{
    /// <summary>
    ///     Base class for a single area mode's settings (see <see cref="FillSettings" />,
    ///     <see cref="TextureSettings" />, <see cref="FilterSettings" />, <see cref="EraseSettings" />).
    ///     Subclasses add whatever payload their mode needs (or none, like <see cref="EraseSettings" />) and
    ///     get <see cref="Enabled" /> for free.
    /// </summary>
    public abstract class AreaModeSettings : INotifyPropertyChanged
    {
        private readonly DrawingState _owner;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AreaModeSettings" /> class, wiring it to raise
        ///     <see cref="Enabled" /> change notifications whenever <paramref name="owner" />'s
        ///     <see cref="DrawingState.ActiveAreaMode" /> changes.
        /// </summary>
        /// <param name="owner">The <see cref="DrawingState" /> this settings object belongs to.</param>
        /// <param name="mode">The area mode this settings object is enabled for.</param>
        protected AreaModeSettings(DrawingState owner, AreaMode mode)
        {
            _owner = owner;
            Mode = mode;
            _owner.PropertyChanged += OnOwnerPropertyChanged;
        }

        /// <summary>Gets the area mode this settings object represents.</summary>
        public AreaMode Mode { get; }

        /// <summary>
        ///     Gets a value indicating whether this is the currently active area mode - i.e. whether the
        ///     matching options panel in the toolbar should be shown. Derived from the owner, not stored, so
        ///     it can never disagree with <see cref="DrawingState.ActiveAreaMode" />.
        /// </summary>
        public bool Enabled => _owner.ActiveAreaMode == Mode;

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnOwnerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DrawingState.ActiveAreaMode)) OnPropertyChanged(nameof(Enabled));
        }

        /// <summary>Raises <see cref="PropertyChanged" /> for the named property.</summary>
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
