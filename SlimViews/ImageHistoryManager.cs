/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViewer
 * FILE:        ImageHistoryManager.cs
 * PURPOSE:     Histories the image changes, manages undo/redo, and safely replaces the active image in the UI.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Core.MemoryLog;
using Imaging;
using SlimViews.Contexts;

namespace SlimViews
{
    /// <summary>
    /// Manages the Undo/Redo history and safe image replacement for the active image.
    /// </summary>
    /// <remarks>
    /// Every bitmap swap goes through <see cref="_gate"/> so only one is ever in flight
    /// at a time. Without that, two rapid calls (holding Ctrl+Z, or an Undo racing a
    /// filter application) could let one call's Dispose() run on the exact Bitmap
    /// another call's background conversion was still reading - a real crash/corruption
    /// risk with GDI+ objects, not a theoretical one.
    /// </remarks>
    public sealed class ImageHistoryManager
    {
        /// <summary>
        /// The image context
        /// </summary>
        private readonly ImageContext _imageContext;

        /// <summary>
        /// The gate
        /// </summary>
        private readonly SemaphoreSlim _gate = new(1, 1);

        /// <summary>
        /// The on error
        /// </summary>
        private readonly Action<Exception> _onError;

        /// <summary>
        /// The library name
        /// </summary>
        private const string LibraryName = "SlimViewer.ImageHistoryManager";

        /// <summary>
        /// Gets the undo history manager.
        /// Instantiated with a hard limit of 5.
        /// </summary>
        /// <value>
        /// The history.
        /// </value>
        public UndoManager<Bitmap> History { get; } = new UndoManager<Bitmap>(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageHistoryManager"/> class.
        /// </summary>
        /// <param name="imageContext">The image context.</param>
        /// <param name="onError">
        /// Called if a swap fails (bad bitmap, disposed object, etc). Defaults to
        /// <see cref="InMemoryLogger"/> - pass your own handler here if you want swap
        /// failures routed somewhere else instead.
        /// </param>
        public ImageHistoryManager(ImageContext imageContext, Action<Exception>? onError = null)
        {
            _imageContext = imageContext;
            _onError = onError ?? (ex => InMemoryLogger.Instance.Log(LogLevel.Error, "Bitmap swap failed",
                libraryName: LibraryName, exception: ex));
        }

        /// <summary>
        /// Commits the image change from tools or filters.
        /// </summary>
        /// <param name="newGdiBitmap">
        /// The new image. Ownership stays with the caller: if you call this from a
        /// <c>using var btm = ...;</c> block (as ApplyFilter/ApplyTexture do), that's
        /// fine - this method takes its own copy before returning, specifically so
        /// your <c>using</c> disposing <paramref name="newGdiBitmap"/> the instant this
        /// call returns can never race the background conversion below.
        /// </param>
        internal void CommitImageChange(Bitmap? newGdiBitmap)
        {
            if (newGdiBitmap == null) return;

            var owned = (Bitmap)newGdiBitmap.Clone();

            // Fire-and-forget is intentional here: this method is called from
            // synchronous, non-awaited call sites (ApplyFilter, ApplyTexture, ...).
            // Errors are caught and reported inside GuardedReplaceAsync, so nothing
            // here becomes an unobserved task exception.
            _ = GuardedReplaceAsync(owned);
        }

        /// <summary>
        /// Undoes the last action.
        /// </summary>
        public async Task UndoAsync()
        {
            await _gate.WaitAsync().ConfigureAwait(true);
            try
            {
                if (!History.CanUndo || _imageContext.Bitmap == null) return;

                var previousBitmap = History.Undo(_imageContext.Bitmap);
                await ReplaceBitmapCoreAsync(previousBitmap).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _onError(ex);
            }
            finally
            {
                _gate.Release();
            }
        }

        /// <summary>
        /// Redoes the previously undone action.
        /// </summary>
        public async Task RedoAsync()
        {
            await _gate.WaitAsync().ConfigureAwait(true);
            try
            {
                if (!History.CanRedo || _imageContext.Bitmap == null) return;

                var nextBitmap = History.Redo(_imageContext.Bitmap);
                await ReplaceBitmapCoreAsync(nextBitmap).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _onError(ex);
            }
            finally
            {
                _gate.Release();
            }
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
                History.RecordState((Bitmap)_imageContext.Bitmap.Clone());
            }
        }

        /// <summary>
        /// Takes <see cref="_gate"/> and performs the swap on behalf of
        /// <see cref="CommitImageChange"/>, which - unlike Undo/Redo - is called from
        /// synchronous call sites that can't await it directly.
        /// </summary>
        private async Task GuardedReplaceAsync(Bitmap ownedBitmap)
        {
            await _gate.WaitAsync().ConfigureAwait(true);
            try
            {
                await ReplaceBitmapCoreAsync(ownedBitmap).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _onError(ex);
            }
            finally
            {
                _gate.Release();
            }
        }

        /// <summary>
        /// Actually swaps the bitmap in. Only ever called while holding <see cref="_gate"/>,
        /// so it's safe to assume no other swap is concurrently in flight.
        /// </summary>
        private async Task ReplaceBitmapCoreAsync(Bitmap newBitmap)
        {
            if (!ReferenceEquals(_imageContext.Bitmap, newBitmap))
            {
                _imageContext.Bitmap = newBitmap;
            }

            // Heavy encoding/decoding stays off the UI thread. Because of the gate,
            // there's never more than one of these in flight at once.
            var newWpfImage = await Task.Run(() =>
            {
                var wpfImg = newBitmap.ToBitmapImage();
                if (wpfImg.CanFreeze && !wpfImg.IsFrozen)
                {
                    wpfImg.Freeze();
                }

                return wpfImg;
            }).ConfigureAwait(true);

            _imageContext.BitmapImage = newWpfImage;
        }
    }
}