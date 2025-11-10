using Imaging;
using SlimControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimViews
{
    internal class ImageProcessingCommands
    {
        /// <summary>
        ///     Gets the render.
        /// </summary>
        /// <value>
        ///     The render.
        /// </value>
        internal static ImageRender Render { get; } = new();

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="filterName">Name of the filter.</param>
        internal void ApplyFilter(ImageView owner, string filterName)
        {
            var filter = Translator.GetFilterFromString(filterName);

            var btm = ImageProcessor.Filter(owner.Image.Bitmap, filter);
            owner.Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        /// Applies the texture.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void ApplyTexture(ImageView owner, string textureName)
        {
            var texture = Translator.GetTextureFromString(textureName);

            var btm = ImageProcessor.Texture(owner.Image.Bitmap, texture);
            owner.Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        /// Brigthens the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void Brigthen(ImageView owner, string obj)
        {
            var btm = ImageProcessor.Brighten(owner.Image.Bitmap);
            owner.Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        /// Darkens the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void Darken(ImageView owner, string obj)
        {
            var btm = ImageProcessor.Darken(owner.Image.Bitmap);
            owner.Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        /// Mirrors the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void Mirror(ImageView owner, object obj)
        {
            if (owner.Image.Bitmap == null) return;

            owner.Image.Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            owner.Bmp = owner.Image.BitmapSource;
        }

        /// <summary>
        /// Pixelates the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void Pixelate(ImageView owner, object obj)
        {
            var btm = ImageProcessor.Pixelate(owner.Image.Bitmap, owner.PixelWidth);
            owner.Bmp = btm.ToBitmapImage();
        }

        /// <summary>
        /// Rotates the backward.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void RotateBackward(ImageView owner, object obj)
        {
            {
                owner.Image.Bitmap = ImageProcessor.RotateImage(owner.Image.Bitmap, -90);
                owner.Bmp = owner.Image.BitmapSource;
            }
        }

        /// <summary>
        /// Rotates the forward.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="obj">The object.</param>
        internal void RotateForward(ImageView owner, object obj)
        {
            owner.Image.Bitmap = ImageProcessor.RotateImage(owner.Image.Bitmap, 90);
            owner.Bmp = owner.Image.BitmapSource;
        }
    }
}
