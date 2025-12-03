/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Contexts
 * FILE:        ImageContext.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

#nullable enable
using Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace SlimViews.Contexts
{
    /// <summary>
    /// Holds all image-related state and operations used by <see cref="ImageView"/>.
    /// Pure data + a few helpers; no UI dependencies.
    /// </summary>
    public sealed class ImageContext
    {
        // Core image data
        public Bitmap? Bitmap { get; set; }

        /// <summary>
        /// Gets or sets the custom image format.
        /// Only used internal no data Binding
        /// </summary>
        /// <value>
        /// The custom image format.
        /// </value>
        internal CustomImageFormat? CustomImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the bitmap image.
        /// Only used internal no data Binding
        /// </summary>
        /// <value>
        /// The bitmap image.
        /// </value>
        internal BitmapImage? BitmapImage { get; set; }

        // Filters, textures, and processing

        /// <summary>
        /// Gets or sets the similarity.
        /// </summary>
        /// <value>
        /// The similarity.
        /// </value>
        public int Similarity { get; set; } = 90;

        /// <summary>
        /// Gets or sets a value indicating whether [compress cif].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [compress cif]; otherwise, <c>false</c>.
        /// </value>
        public bool CompressCif { get; set; }

        // Display and zoom

        /// <summary>
        /// Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive { get; set; }

        // Cached flags

        public int PixelWidth { get; internal set; }

        internal bool HasImage => Bitmap != null || BitmapSource != null;

        // Internal helpers

        /// <summary>
        /// Gets the bitmap source.
        /// </summary>
        /// <value>
        /// The bitmap source.
        /// </value>
        internal BitmapImage? BitmapSource => Bitmap?.ToBitmapImage();

        public int BrushSize { get; internal set; }

        public string Information { get; internal set; }

        /// <summary>
        /// Optional helper: reset without reallocating the object
        /// </summary>
        internal void Clear()
        {
            Bitmap?.Dispose();
            Bitmap = null;
            IsImageActive = false;
        }
    }
}