/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        ImageEditQueue.cs
 * PURPOSE:     Serializes every bitmap edit (canvas tools and whole-image menu
 *              operations alike) through one ordered, copy-on-write pipeline.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Drawing;
using System.Threading.Channels;
using System.Threading.Tasks;
using SlimViews.Contexts;

namespace SlimViews
{
    /// <summary>
    /// Applies bitmap edits one at a time, strictly in the order they were
    /// submitted, each against its own private clone of the current image -
    /// never the live, on-screen <see cref="Bitmap"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This replaces the previous combination of a bare <c>lock</c> around the
    /// "compute the edit" step and a separate <c>SemaphoreSlim</c> gate around
    /// the "swap it into the UI" step, glued together with a fire-and-forget
    /// <c>CommitImageChange</c> call. That combination had a real gap: the
    /// method that computed an edit (<c>SelectedFrameAction</c>, etc.) returned
    /// - and the command that triggered it re-enabled - as soon as the swap had
    /// been *started*, not once it had actually *finished*. A second edit could
    /// then start reading/writing <see cref="ImageContext.Bitmap"/> while the
    /// first edit's background clone/dispose/WPF-conversion was still running.
    /// Most pixel operations (<c>FillAreaWithColor</c>, <c>EraseRectangle</c>,
    /// <c>CombineBitmap</c>) also draw directly onto whatever <see cref="Bitmap"/>
    /// they're given via <c>Graphics.FromImage</c>, so if that bitmap was still
    /// the live one, two threads could end up drawing into / disposing / reading
    /// the same GDI+ handle at once - which throws ("Object is currently in use
    /// elsewhere") or silently corrupts the image, and did so invisibly because
    /// nothing was listening for the exception.
    /// </para>
    /// <para>
    /// This class closes both gaps at once. Every edit function receives a
    /// private clone made *by the queue*, right before the edit runs and while
    /// nothing else can be touching it, so it's free to mutate that clone in
    /// place exactly the way the existing <c>ImageProcessor</c>/<c>ImageStream</c>
    /// helpers already do - nothing about those helpers needs to change. And
    /// because there is exactly one reader processing the channel, "read the
    /// current bitmap, clone it, edit it, commit it" is atomic with respect to
    /// every other edit, whether it came from a canvas drag or a menu command,
    /// without any caller needing its own lock.
    /// </para>
    /// </remarks>
    public sealed class ImageEditQueue
    {
        /// <summary>
        /// The channel
        /// </summary>
        private readonly Channel<(Func<Bitmap, Bitmap?> Job, bool RecordUndo)> _channel =
            Channel.CreateUnbounded<(Func<Bitmap, Bitmap?> Job, bool RecordUndo)>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });

        /// <summary>
        /// The history
        /// </summary>
        private readonly ImageHistoryManager _history;

        /// <summary>
        /// The image context
        /// </summary>
        private readonly ImageContext _imageContext;

        /// <summary>
        /// The on error
        /// </summary>
        private readonly Action<Exception> _onError;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEditQueue"/> class
        /// and starts its single background worker.
        /// </summary>
        /// <param name="history">Owns undo/redo and the actual bitmap swap.</param>
        /// <param name="imageContext">Holds the live, on-screen bitmap.</param>
        /// <param name="onError">
        /// Called (on top of any exception propagated to <see cref="SubmitAsync"/>
        /// callers that await it) whenever an edit throws - including edits
        /// submitted fire-and-forget, which would otherwise fail silently.
        /// </param>
        public ImageEditQueue(ImageHistoryManager history, ImageContext imageContext, Action<Exception> onError)
        {
            _history = history;
            _imageContext = imageContext;
            _onError = onError;
            _ = RunAsync();
        }

        /// <summary>
        /// Queues an edit and returns a task that completes once this edit - and
        /// everything queued before it - has been fully applied and committed.
        /// </summary>
        /// <param name="edit">
        /// Receives a private clone of the current image, safe to mutate in
        /// place or discard. Return the resulting bitmap to commit it, or
        /// <see langword="null"/> to make this a no-op (e.g. an unrecognized
        /// filter/texture name) without recording an undo step.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a result was committed, <see langword="false"/>
        /// if <paramref name="edit"/> returned <see langword="null"/>. Faults with
        /// whatever <paramref name="edit"/> threw if it threw.
        /// </returns>
        /// <param name="recordUndo">
        /// <see langword="false"/> to apply the edit without pushing a new undo state - used for the
        /// 2nd..nth batch of one pencil/eraser stroke, so a whole stroke is a single undo step instead
        /// of one per flush (which also used to push real history out of the 5-deep undo buffer).
        /// </param>
        public Task<bool> SubmitAsync(Func<Bitmap, Bitmap?> edit, bool recordUndo = true)
        {
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Func<Bitmap, Bitmap?> job = bitmap =>
            {
                try
                {
                    var result = edit(bitmap);
                    completion.SetResult(result != null);
                    return result;
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                    throw;
                }
            };

            var accepted = _channel.Writer.TryWrite((job, recordUndo));

            if (!accepted)
            {
                // Can't happen with an unbounded channel that's never completed,
                // but don't leave a caller awaiting forever if it ever does.
                completion.SetException(new InvalidOperationException(
                    "The image edit queue is no longer accepting edits."));
            }

            return completion.Task;
        }

        /// <summary>
        /// The single consumer loop: takes jobs strictly in submission order, one
        /// at a time, so nothing else needs to coordinate between them.
        /// </summary>
        private async Task RunAsync()
        {
            await foreach (var (job, recordUndo) in _channel.Reader.ReadAllAsync().ConfigureAwait(true))
            {
                try
                {
                    var current = _imageContext.Bitmap;
                    if (current == null) continue;

                    // Snapshot for undo *before* the edit touches anything.
                    if (recordUndo)
                    {
                        _history.SaveUndoState();
                    }

                    // The edit's own private copy. Whatever the job does to this
                    // - draw on it in place, hand back a different object entirely
                    // - the live, on-screen bitmap is never touched.
                    using var privateCopy = (Bitmap)current.Clone();

                    var result = job(privateCopy);

                    if (result != null)
                    {
                        await _history.CommitImageChangeAsync(result).ConfigureAwait(true);
                    }
                }
                catch (Exception ex)
                {
                    // The submitting caller (if awaiting) already saw this via its
                    // own TaskCompletionSource; this is what makes fire-and-forget
                    // submissions (whole-image menu commands) visible too.
                    _onError(ex);
                }
            }
        }
    }
}