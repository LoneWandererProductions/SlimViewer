using CommonControls;
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
        public Bitmap Bitmap { get; set; }

        public BitmapImage BitmapImage { get; set; }

        public BitmapImage BitmapSource => Bitmap.ToBitmapImage();

        public CustomImageFormat CustomImageFormat { get; set; }

        // Filters, textures, and processing
        public string SelectedFilter { get; set; }
        public string SelectedTexture { get; set; }
        public int Similarity { get; set; } = 90;
        public bool CompressCif { get; set; }

        // Display and zoom

        /// <summary>
        /// Gets or sets the reference to Image Zoom custom Control itself.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        public ImageZoom Zoom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive { get; set; }

        // Cached flags
        public bool HasImage => Bitmap != null || BitmapSource != null;
        public bool IsModified { get; set; }
        public int PixelWidth { get; internal set; }

        // Optional helper: reset without reallocating the object
        public void Clear()
        {
            Bitmap?.Dispose();
            Bitmap = null;
            SelectedFilter = null;
            SelectedTexture = null;
            IsModified = false;
            IsImageActive = false;
        }
    }
}