/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        ImageProcessingCommands.cs
 * PURPOSE:     Mostly image processing related commands, such as filters, textures, and transformations.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using Imaging;
using SlimControls;
using System.Drawing;
using System.Reflection.Metadata;

namespace SlimViews
{
    /// <summary>
    ///     Provides command-based access to various image processing operations.
    /// </summary>
    internal class ImageProcessingCommands
    {
        /// <summary>
        ///     Applies the specified filter to the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="filterName">The name of the filter to apply.</param>
        internal void ApplyFilter(ImageView owner, string filterName)
        {
            if (owner?.Image?.Bitmap == null || string.IsNullOrWhiteSpace(filterName))
                return;

            owner.SaveUndoState();

            var filter = Translator.GetFilterFromString(filterName);
            using var btm = ImageProcessor.Filter(owner.Image.Bitmap, filter);
            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Applies a texture overlay or effect to the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="textureName">The name of the texture to apply.</param>
        internal void ApplyTexture(ImageView owner, string textureName)
        {
            if (owner?.Image?.Bitmap == null || string.IsNullOrWhiteSpace(textureName))
                return;

            owner.SaveUndoState();

            var texture = Translator.GetTextureFromString(textureName);
            using var btm = ImageProcessor.Texture(owner.Image.Bitmap, texture);

            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Brightens the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Brighten(ImageView owner, object obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            owner.SaveUndoState();

            using var btm = ImageProcessor.Brighten(owner.Image.Bitmap);

            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Darkens the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Darken(ImageView owner, string obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            owner.SaveUndoState();

            using var btm = ImageProcessor.Darken(owner.Image.Bitmap);

            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Mirrors the owner's image horizontally.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Mirror(ImageView owner, object? obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            // Create a new bitmap based on the original
            var original = owner.Image.Bitmap;

            using var clone = (Bitmap)original.Clone();
            clone.RotateFlip(RotateFlipType.RotateNoneFlipX);

            owner.Image.Bitmap = clone;
            owner.Bmp = clone.ToBitmapImage();
        }


        /// <summary>
        ///     Pixelates the owner's image based on the view's pixel width.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="parameter">Unused parameter (reserved for future use).</param>
        internal void Pixelate(ImageView owner, object parameter)
        {
            if (owner?.Image?.Bitmap == null)
                return;
            var pixelWidth = 2;

            if (parameter != null && int.TryParse(parameter.ToString(), out var result))
            {
                pixelWidth = result;
            }

            owner.SaveUndoState();

            using var btm = ImageProcessor.Pixelate(owner.Image.Bitmap, pixelWidth);

            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Rotates the owner's image by -90 degrees (counterclockwise).
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void RotateBackward(ImageView owner, object obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            owner.SaveUndoState();

            var btm = ImageProcessor.RotateImage(owner.Image.Bitmap, -90);
            owner.CommitImageChange(btm);
        }

        /// <summary>
        ///     Rotates the owner's image by +90 degrees (clockwise).
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void RotateForward(ImageView owner, object obj)
        {
            if (owner?.Image?.Bitmap == null)
                return;

            owner.SaveUndoState();

            var btm = ImageProcessor.RotateImage(owner.Image.Bitmap, 90);
            owner.CommitImageChange(btm);
        }
    }
}