/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        ImageProcessingCommands.cs
 * PURPOSE:     Mostly image processing related commands, such as filters, textures, and transformations.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;
using SlimControls;

namespace SlimViews
{
    /// <summary>
    ///     Provides command-based access to various image processing operations.
    /// </summary>
    /// <remarks>
    ///     Every method here submits through <see cref="ImageView.EditQueue"/>
    ///     instead of calling <c>SaveUndoState</c>/<c>CommitImageChange</c>
    ///     directly. That used to mean each whole-image operation (Filter,
    ///     Texture, Brighten, ...) raced independently against every other one -
    ///     including against a canvas tool mid-drag - because nothing serialized
    ///     "read the current bitmap, edit it, commit it" as a single step. Routing
    ///     through the same queue that the canvas tools use puts every mutation
    ///     of the image, regardless of where it came from, through one ordered
    ///     pipeline. The submissions here are fire-and-forget (these commands are
    ///     synchronous <c>ICommand</c>s, not awaited by their callers), but that's
    ///     safe now: ordering and copy-on-write safety come from the queue itself,
    ///     not from the caller awaiting it, and <see cref="ImageEditQueue"/> still
    ///     reports a failure via its <c>onError</c> callback even when nobody
    ///     awaits the returned task.
    /// </remarks>
    internal class ImageProcessingCommands
    {
        /// <summary>
        ///     Applies the specified filter to the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="filterName">The name of the filter to apply.</param>
        internal void ApplyFilter(ImageView? owner, string filterName)
        {
            if (owner?.Image.Bitmap == null || string.IsNullOrWhiteSpace(filterName))
                return;

            var filter = Translator.GetFilterFromString(filterName);
            _ = owner.EditQueue.SubmitAsync(bitmap => ImageProcessor.Filter(bitmap, filter));
        }

        /// <summary>
        ///     Applies a texture overlay or effect to the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="textureName">The name of the texture to apply.</param>
        internal void ApplyTexture(ImageView? owner, string textureName)
        {
            if (owner?.Image.Bitmap == null || string.IsNullOrWhiteSpace(textureName))
                return;

            var texture = Translator.GetTextureFromString(textureName);
            _ = owner.EditQueue.SubmitAsync(bitmap => ImageProcessor.Texture(bitmap, texture));
        }

        /// <summary>
        ///     Brightens the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Brighten(ImageView? owner, object obj)
        {
            if (owner?.Image.Bitmap == null)
                return;

            _ = owner.EditQueue.SubmitAsync(ImageProcessor.Brighten);
        }

        /// <summary>
        ///     Darkens the owner's image.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Darken(ImageView? owner, object obj)
        {
            if (owner?.Image.Bitmap == null)
                return;

            _ = owner.EditQueue.SubmitAsync(ImageProcessor.Darken);
        }

        /// <summary>
        ///     Mirrors the owner's image horizontally.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void Mirror(ImageView? owner, object? obj)
        {
            if (owner?.Image.Bitmap == null)
                return;

            // Previously bypassed SaveUndoState/CommitImageChange entirely (it set
            // Image.Bitmap and Image.BitmapImage directly), so Mirror had no undo
            // support and could itself race a concurrent edit. Routing it through
            // the queue like everything else fixes both at once.
            _ = owner.EditQueue.SubmitAsync(bitmap =>
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                return bitmap;
            });
        }

        /// <summary>
        ///     Pixelates the owner's image based on the view's pixel width.
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="parameter">Unused parameter (reserved for future use).</param>
        internal void Pixelate(ImageView? owner, object? parameter)
        {
            if (owner?.Image.Bitmap == null)
                return;

            var pixelWidth = 2;

            if (parameter is { } && int.TryParse(parameter.ToString(), out var result))
            {
                pixelWidth = result;
            }

            _ = owner.EditQueue.SubmitAsync(bitmap => ImageProcessor.Pixelate(bitmap, pixelWidth));
        }

        /// <summary>
        ///     Rotates the owner's image by -90 degrees (counterclockwise).
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void RotateBackward(ImageView? owner, object obj)
        {
            if (owner?.Image.Bitmap == null)
                return;

            _ = owner.EditQueue.SubmitAsync(bitmap => ImageProcessor.RotateImage(bitmap, -90));
        }

        /// <summary>
        ///     Rotates the owner's image by +90 degrees (clockwise).
        /// </summary>
        /// <param name="owner">The image view to modify.</param>
        /// <param name="obj">Unused parameter (reserved for future use).</param>
        internal void RotateForward(ImageView? owner, object obj)
        {
            if (owner?.Image.Bitmap == null)
                return;

            _ = owner.EditQueue.SubmitAsync(bitmap => ImageProcessor.RotateImage(bitmap, 90));
        }
    }
}