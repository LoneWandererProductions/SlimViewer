/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        ImageHistoryManager.cs
 * PURPOSE:     Histories the image changes, manages undo/redo, and safely replaces the active image in the UI.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */


using System.Drawing;
using System.Threading.Tasks;
using Imaging;
using SlimViews.Contexts;

namespace SlimViews
{
    /// <summary>
    /// Manages the Undo/Redo history and safe image replacement for the active image.
    /// </summary>
    public sealed class ImageHistoryManager
    {
        private readonly ImageContext _imageContext;

        /// <summary>
        /// Gets the undo history manager. 
        /// Instantiated with a hard limit of 5.
        /// </summary>
        public UndoManager<Bitmap> History { get; } = new UndoManager<Bitmap>(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHistoryManager"/> class.
        /// </summary>
        /// <param name="imageContext">The image context.</param>
        public ImageHistoryManager(ImageContext imageContext)
        {
            _imageContext = imageContext;
        }

        /// <summary>
        /// Commits the image change from tools or filters.
        /// </summary>
        /// <param name="newGdiBitmap">The new image.</param>
        internal void CommitImageChange(Bitmap newGdiBitmap)
        {
            if (newGdiBitmap == null) return;
            ReplaceBitmap(newGdiBitmap);
        }

        /// <summary>
        /// Safely replaces the current bitmap, handles memory disposal, and forces the UI to update.
        /// </summary>
        /// <param name="newBitmap">The new bitmap.</param>
        private async void ReplaceBitmap(Bitmap newBitmap)
        {
            if (newBitmap == null) return;

            if (!ReferenceEquals(_imageContext.Bitmap, newBitmap))
            {
                var old = _imageContext.Bitmap;
                _imageContext.Bitmap = newBitmap;
                old?.Dispose();
            }

            // Push the heavy encoding/decoding to a background thread!
            var newWpfImage = await Task.Run(() =>
            {
                var wpfImg = newBitmap.ToBitmapImage(); // Assumes your extension method is available
                if (wpfImg.CanFreeze && !wpfImg.IsFrozen)
                {
                    wpfImg.Freeze(); // Crucial: Freezing allows it to be sent back to the UI thread
                }
                return wpfImg;
            });

            // Back on the UI thread, instantly swap the image
            _imageContext.BitmapImage = newWpfImage;
        }

        /// <summary>
        /// Undoes the last action.
        /// </summary>
        public void Undo()
        {
            if (!History.CanUndo || _imageContext.Bitmap == null) return;

            var previousBitmap = History.Undo(_imageContext.Bitmap);
            ReplaceBitmap(previousBitmap);
        }

        /// <summary>
        /// Redoes the previously undone action.
        /// </summary>
        public void Redo()
        {
            if (!History.CanRedo || _imageContext.Bitmap == null) return;

            var nextBitmap = History.Redo(_imageContext.Bitmap);
            ReplaceBitmap(nextBitmap);
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        public void ClearHistory()
        {
            History.Clear();
        }

        /// <summary>
        /// Saves the state of the current image for undoing.
        /// </summary>
        internal void SaveUndoState()
        {
            if (_imageContext.Bitmap != null)
            {
                // Note: Consider offloading this clone to a Task.Run if it freezes the UI
                History.RecordState((Bitmap)_imageContext.Bitmap.Clone());
            }
        }
    }
}