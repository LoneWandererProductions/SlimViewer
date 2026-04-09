/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews.Contexts
 * FILE:        ImageContext.cs
 * PURPOSE:     Image-related state and operations for ImageView, including core image data, filters, display settings, and cached flags.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Imaging;
using Imaging.Cifs;
using System.Drawing;
using System.Windows.Media.Imaging;
using ViewModel;

namespace SlimViews.Contexts
{
    /// <summary>
    /// Holds all image-related state and operations used by <see cref="ImageView"/>.
    /// Pure data + a few helpers; no UI dependencies.
    /// </summary>
    public sealed class ImageContext : ViewModelBase
    {
        /// <summary>
        /// The bitmap
        /// </summary>
        private Bitmap? _bitmap;

        /// <summary>
        /// The bitmap image
        /// </summary>
        private BitmapImage? _bitmapImage;

        /// <summary>
        /// The GIF path
        /// </summary>
        private string? _gifPath;

        /// <summary>
        /// The compress cif
        /// </summary>
        private bool _compressCif;

        /// <summary>
        /// The information
        /// </summary>
        private string? _information;

        // Core image data
        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public Bitmap? Bitmap
        {
            get => _bitmap;
            set
            {
                if (!ReferenceEquals(_bitmap, value))
                {
                    _bitmap?.Dispose();

                    if (SetProperty(ref _bitmap, value))
                    {
                        OnPropertyChanged(nameof(IsImageActive));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the GIF path.
        /// Now  directly data-bound, but still part of the core image data since it represents an alternative way to load/display an image.
        /// </summary>
        /// <value>
        /// The GIF path.
        /// </value>
        public string? GifPath
        {
            get => _gifPath;
            set
            {
                if (SetProperty(ref _gifPath, value))
                {
                    // Manually tell the UI that IsImageActive also changed
                    OnPropertyChanged(nameof(IsImageActive));
                }
            }
        }

        /// <summary>
        /// Gets or sets the bitmap image.
        /// Only used internal no data Binding
        /// </summary>
        /// <value>
        /// The bitmap image.
        /// </value>
        public BitmapImage? BitmapImage
        {
            get => _bitmapImage;
            set
            {
                if (SetProperty(ref _bitmapImage, value))
                {
                    // Manually tell the UI that IsImageActive also changed
                    OnPropertyChanged(nameof(IsImageActive));
                    OnPropertyChanged(nameof(BitmapImage));
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom image format.
        /// Only used internal no data Binding
        /// </summary>
        /// <value>
        /// The custom image format.
        /// </value>
        internal CustomImageFormat? CustomImageFormat { get; set; }

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
        public bool CompressCif
        {
            get => _compressCif;
            set
            {
                if (SetProperty(ref _compressCif, value))
                {
                    // Manually tell the UI that IsImageActive also changed
                    OnPropertyChanged(nameof(CompressCif));
                }
            }
        }

        // Display and zoom

        /// <summary>
        /// Gets or sets a value indicating whether this instance is image active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is image active; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageActive => HasImage;

        // Cached flags

        /// <summary>
        /// Gets a value indicating whether this instance has image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has image; otherwise, <c>false</c>.
        /// </value>
        public bool HasImage => Bitmap != null || BitmapImage != null || !string.IsNullOrEmpty(GifPath);

        // Internal helpers

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public string? Information
        {
            get => _information;
            set
            {
                if (SetProperty(ref _information, value))
                {
                    // Manually tell the UI that IsImageActive also changed
                    OnPropertyChanged(nameof(Information));
                }
            }
        }

        /// <summary>
        /// Gets the bitmap source.
        /// </summary>
        /// <value>
        /// The bitmap source.
        /// </value>
        internal BitmapImage? BitmapSource => Bitmap?.ToBitmapImage();

        /// <summary>
        /// Optional helper: reset without reallocating the object
        /// </summary>
        internal void Clear()
        {
            Bitmap?.Dispose();
            Bitmap = null;
            GifPath = string.Empty;
            Information = null;
        }
    }
}