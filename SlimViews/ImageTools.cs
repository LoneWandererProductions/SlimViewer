/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        SlimViews/ImageTools.xaml.cs
 * PURPOSE:     Image View Control, that can handle some tools
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

namespace SlimViews
{
    /// <summary>
    ///     Tools that are available in the gui
    /// </summary>
    public enum ImageTools
    {
        /// <summary>
        ///     The paint
        /// </summary>
        Paint = 0,

        /// <summary>
        ///     The erase
        /// </summary>
        Erase = 1,

        /// <summary>
        ///     The select
        /// </summary>
        Select = 2,

        /// <summary>
        ///     The texture
        /// </summary>
        Texture = 3,

        /// <summary>
        ///     The filter
        /// </summary>
        Filter = 4,

        /// <summary>
        ///     The cut
        /// </summary>
        Cut = 5
    }
}