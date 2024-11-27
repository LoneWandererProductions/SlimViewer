/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        SlimControls/TextureConfigView.cs
 * PURPOSE:     The view for Texture Configuration
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace SlimControls
{
    /// <summary>
    /// The Tools we support right now
    /// </summary>
    public enum ImageTools
    {
        /// <summary>
        /// The move
        /// </summary>
        Move = 0,

        /// <summary>
        ///     The paint
        /// </summary>
        Paint = 1,

        /// <summary>
        ///     The erase
        /// </summary>
        Erase = 2,

        /// <summary>
        ///     The color select
        /// </summary>
        ColorSelect = 3,

        /// <summary>
        ///     The area
        /// </summary>
        Area = 4
    }
}