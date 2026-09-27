/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        EraseSettings.cs
 * PURPOSE:     Settings for the "Clear area" (Erase) area mode. It needs no payload beyond Enabled -
 *              clearing an area takes no color/pattern/effect - so this class exists only to give that
 *              mode a place in DrawingState.Erase, matching the shape of Fill/Texture/Filter. See
 *              AreaModeSettings for what Enabled means and why it is no longer stored here.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    /// <summary>
    /// Settings for the "✂️ Clear" area mode.
    /// </summary>
    /// <seealso cref="SlimControls.AreaModeSettings" />
    public sealed class EraseSettings : AreaModeSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EraseSettings" /> class.
        /// </summary>
        /// <param name="owner">The <see cref="DrawingState" /> this settings object belongs to.</param>
        public EraseSettings(DrawingState owner) : base(owner, AreaMode.Erase)
        {
        }
    }
}