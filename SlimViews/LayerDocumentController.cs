/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimViews
 * FILE:        LayerDocumentController.cs
 * PURPOSE:     Bridges the pure-data Imaging.Objects.Documents model to the rest of the app: owns one
 *              Document plus its DocumentHistory, and knows how to draw pencil/eraser strokes and shapes
 *              onto the currently active layer, flatten for display, and save/load .slimdoc files.
 *              Deliberately WPF-free (System.Drawing.Point/Color only) so it can be unit tested directly;
 *              ImageView is the (WPF) adapter that converts to/from System.Windows types around it, and
 *              Common.Images.ImageZoom (via its LayeredDocument property) is what actually displays it.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using Imaging;
using Imaging.Objects;
using Imaging.Objects.Commands;
using Imaging.Objects.Documents;
using Imaging.Objects.Interfaces;
using Imaging.Objects.Shapes;

namespace SlimViews
{
    /// <summary>
    ///     Owns a layered <see cref="Document" /> and mediates every edit to it. One instance per open,
    ///     layered image; calls are serialized internally, so it is safe to call from the UI thread the
    ///     way <see cref="ImageEditQueue" /> is used for the legacy single-bitmap path.
    /// </summary>
    public sealed class LayerDocumentController : IDisposable
    {
        private readonly SemaphoreSlim _gate = new(1, 1);

        private readonly IShapeRasterizer _rasterizer;

        private Guid _strokeLayerId;

        private RasterEditRecorder? _strokeRecorder;

        private LayerDocumentController(Document document, IShapeRasterizer rasterizer, int undoLimit)
        {
            Document = document;
            History = new DocumentHistory(document, undoLimit);
            _rasterizer = rasterizer;

            ActiveLayerId = document.Layers.Count > 0 ? document.Layers[^1].Id : null;

            Document.Changed += (_, e) => DocumentChanged?.Invoke(this, e);
            History.Changed += (_, e) => HistoryChanged?.Invoke(this, e);
        }

        /// <summary>Gets the document. Treat as read-only from the outside; edit through this controller instead.</summary>
        public Document Document { get; }

        /// <summary>Gets the undo/redo stack.</summary>
        public DocumentHistory History { get; }

        /// <summary>Gets the layer new strokes and shapes are added to, or null if the document has none.</summary>
        public Guid? ActiveLayerId { get; private set; }

        /// <summary>
        ///     Raised for every change to the document: structural edits, layer property changes, shape edits,
        ///     and pixel edits (including one raise per flushed batch of a still-in-progress stroke, so a live
        ///     view - such as <see cref="Common.Images.ImageZoom" />'s <c>LayeredDocument</c> - can redraw
        ///     before the stroke is committed to history).
        /// </summary>
        public event EventHandler<DocumentChangedEventArgs>? DocumentChanged;

        /// <summary>Raised whenever CanUndo/CanRedo/IsDirty may have changed.</summary>
        public event EventHandler? HistoryChanged;

        /// <summary>Creates a controller for a brand new document with one background raster layer (the bitmap is copied).</summary>
        public static LayerDocumentController FromBitmap(Bitmap bitmap, string backgroundName = "Background",
            int undoLimit = 50, IShapeRasterizer? rasterizer = null)
        {
            return new LayerDocumentController(Document.FromBitmap(bitmap, backgroundName),
                rasterizer ?? new ShapeRasterizer(), undoLimit);
        }

        /// <summary>Creates a controller around an existing document (for example one returned by <see cref="Load" />).</summary>
        public static LayerDocumentController FromDocument(Document document, int undoLimit = 50,
            IShapeRasterizer? rasterizer = null)
        {
            ArgumentNullException.ThrowIfNull(document);
            return new LayerDocumentController(document, rasterizer ?? new ShapeRasterizer(), undoLimit);
        }

        /// <summary>Loads a .slimdoc file into a new controller.</summary>
        public static LayerDocumentController Load(string path, int undoLimit = 50, IShapeRasterizer? rasterizer = null)
        {
            return FromDocument(new DocumentSerializer().Load(path), undoLimit, rasterizer);
        }

        /// <summary>Saves the document as a .slimdoc file (see <see cref="DocumentSerializer" />) and marks history clean.</summary>
        public void Save(string path)
        {
            new DocumentSerializer().Save(Document, path);
            History.MarkClean();
        }

        /// <summary>Adds a raster layer on top, makes it active, and returns its id.</summary>
        public Guid AddRasterLayer(string name = "Layer")
        {
            var layer = new RasterLayer(Document.Width, Document.Height, name);
            History.Execute(new AddLayerCommand(layer));
            ActiveLayerId = layer.Id;
            return layer.Id;
        }

        /// <summary>Adds a shape layer on top, makes it active, and returns its id.</summary>
        public Guid AddShapeLayer(string name = "Shapes")
        {
            var layer = new ShapeLayer(name);
            History.Execute(new AddLayerCommand(layer));
            ActiveLayerId = layer.Id;
            return layer.Id;
        }

        /// <summary>Duplicates a layer, placing the copy directly above the original and making it active.</summary>
        public Guid DuplicateLayer(Guid layerId)
        {
            var index = Document.IndexOf(layerId);
            if (index < 0) throw new KeyNotFoundException($"Layer {layerId} not found.");

            var copy = Document.Layers[index].Duplicate();
            History.Execute(new AddLayerCommand(copy, index + 1));
            ActiveLayerId = copy.Id;
            return copy.Id;
        }

        /// <summary>Removes a layer. If it was active, the layer that ends up in its place becomes active (if any).</summary>
        public void RemoveLayer(Guid layerId)
        {
            var index = Document.IndexOf(layerId);
            History.Execute(new RemoveLayerCommand(layerId));

            if (ActiveLayerId != layerId) return;

            var count = Document.Layers.Count;
            ActiveLayerId = count == 0 ? null : Document.Layers[Math.Min(index, count - 1)].Id;
        }

        /// <summary>Moves a layer to a new stacking position.</summary>
        public void MoveLayer(Guid layerId, int newIndex)
        {
            History.Execute(new MoveLayerCommand(layerId, newIndex));
        }

        /// <summary>Changes name, visibility, opacity and/or blend mode of a layer.</summary>
        public void SetLayerProperties(Guid layerId, LayerProperties properties)
        {
            History.Execute(new SetLayerPropertiesCommand(layerId, properties));
        }

        /// <summary>Makes an existing layer the active one (the target of new strokes and shapes).</summary>
        public void SetActiveLayer(Guid layerId)
        {
            if (Document.FindLayer(layerId) is null) throw new KeyNotFoundException($"Layer {layerId} not found.");
            ActiveLayerId = layerId;
        }

        /// <summary>
        ///     Draws one flushed batch of a pencil/eraser drag onto the active raster layer, connecting it to the
        ///     previous batch. Does nothing if the active layer is missing or is a shape layer (a caller that
        ///     wants that to be an error can check <see cref="ActiveLayerId" /> and its layer's type itself).
        ///     Undo/redo must not be called while a stroke is in progress (between a call with
        ///     <paramref name="isFirstBatch" /> true and one with <paramref name="isLastBatch" /> true).
        /// </summary>
        public async Task DrawStrokeAsync(IReadOnlyList<Point> points, Point? previous, Color color, int radius,
            bool isFirstBatch, bool isLastBatch)
        {
            ArgumentNullException.ThrowIfNull(points);
            if (points.Count == 0) return;

            await _gate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (ActiveLayerId is not { } layerId || Document.FindLayer(layerId) is not RasterLayer layer) return;

                if (isFirstBatch || _strokeRecorder is null || _strokeLayerId != layerId)
                {
                    _strokeRecorder?.Rollback();
                    _strokeRecorder = new RasterEditRecorder(layer);
                    _strokeLayerId = layerId;
                }

                var dirty = StrokeBounds(points, previous, radius)
                    .Intersect(new PixelRect(0, 0, layer.Width, layer.Height));
                if (!dirty.IsEmpty) _strokeRecorder.Touch(dirty);

                using (var view = ViewOf(layer))
                {
                    ImageProcessor.DrawStroke(view, points, previous, color, radius);
                }

                Document.Raise(DocumentChangeKind.PixelsChanged, layerId, dirty);

                if (isLastBatch)
                {
                    var command = _strokeRecorder.Commit(color == Color.Transparent ? "Erase" : "Brush stroke");
                    _strokeRecorder = null;
                    if (command is not null) History.Record(command);
                }
            }
            finally
            {
                _gate.Release();
            }
        }

        /// <summary>
        ///     Runs an in-place pixel edit (fill, texture, filter, frame-based erase - anything that mutates a
        ///     whole Bitmap and hands the same reference back, the way <c>ImageProcessor.FillArea</c> and
        ///     friends do) against the active raster layer instead of a whole flattened image. The edit is one
        ///     undo step covering every pixel it touched.
        /// </summary>
        /// <exception cref="InvalidOperationException">The active layer is not a raster layer.</exception>
        public void ApplyToActiveRasterLayer(Action<Bitmap> edit, string description = "Edit layer")
        {
            ArgumentNullException.ThrowIfNull(edit);

            if (ActiveLayerId is not { } layerId || Document.FindLayer(layerId) is not RasterLayer layer)
            {
                throw new InvalidOperationException("The active layer is not a raster layer.");
            }

            var recorder = new RasterEditRecorder(layer);
            recorder.TouchAll();

            using (var view = ViewOf(layer))
            {
                edit(view);
            }

            var command = recorder.Commit(description);
            if (command is not null) History.Record(command);

            Document.Raise(DocumentChangeKind.PixelsChanged, layerId, new PixelRect(0, 0, layer.Width, layer.Height));
        }

        /// <summary>Adds a shape to the active shape layer.</summary>
        public void AddShape(Shape shape)
        {
            History.Execute(new AddShapeCommand(ActiveShapeLayerId(), shape));
        }

        /// <summary>Replaces a shape on the active shape layer by a changed copy with the same id.</summary>
        public void ReplaceShape(Shape shape)
        {
            History.Execute(new ReplaceShapeCommand(ActiveShapeLayerId(), shape));
        }

        /// <summary>Removes a shape from the active shape layer.</summary>
        public void RemoveShape(Guid shapeId)
        {
            History.Execute(new RemoveShapeCommand(ActiveShapeLayerId(), shapeId));
        }

        /// <summary>
        ///     Composites every visible layer into a new bitmap (the caller owns and must dispose it).
        ///     This is what should be pushed to the display and used for "flatten and save as PNG/JPEG".
        ///     <see cref="Common.Images.ImageZoom" />'s <c>LayeredDocument</c> property flattens the same way,
        ///     directly from <see cref="Document" />, and does not need this method itself.
        /// </summary>
        public Bitmap Flatten()
        {
            using var buffer = DocumentRenderer.Flatten(Document, _rasterizer);
            return buffer.ToBitmap();
        }

        /// <summary>Gets a value indicating whether there is something to undo.</summary>
        public bool CanUndo => History.CanUndo;

        /// <summary>Gets a value indicating whether there is something to redo.</summary>
        public bool CanRedo => History.CanRedo;

        /// <summary>Gets a value indicating whether the document differs from its last saved state.</summary>
        public bool IsDirty => History.IsDirty;

        /// <summary>Undoes the last step. Throws if a stroke is currently in progress.</summary>
        public void Undo()
        {
            EnsureNoStrokeInProgress();
            History.Undo();
        }

        /// <summary>Redoes the last undone step. Throws if a stroke is currently in progress.</summary>
        public void Redo()
        {
            EnsureNoStrokeInProgress();
            History.Redo();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _strokeRecorder?.Rollback();
            History.Dispose();
            Document.Dispose();
            _gate.Dispose();
        }

        private Guid ActiveShapeLayerId()
        {
            if (ActiveLayerId is not { } id || Document.FindLayer(id) is not ShapeLayer)
            {
                throw new InvalidOperationException("The active layer is not a shape layer.");
            }

            return id;
        }

        private void EnsureNoStrokeInProgress()
        {
            if (_strokeRecorder is not null)
            {
                throw new InvalidOperationException("Cannot undo/redo while a stroke is being drawn.");
            }
        }

        /// <summary>Builds a Bitmap that is a live view over a raster layer's own memory - drawing on it writes straight
        /// into the layer, with no copy in either direction. Disposing it only releases the GDI+ handle.</summary>
        private static Bitmap ViewOf(RasterLayer layer)
        {
            return new Bitmap(layer.Width, layer.Height, layer.Width * UnmanagedImageBuffer.BytesPerPixel,
                PixelFormat.Format32bppArgb, layer.Pixels.Buffer);
        }

        private static PixelRect StrokeBounds(IReadOnlyList<Point> points, Point? previous, int radius)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            void Include(Point p)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }

            if (previous is { } prev) Include(prev);
            foreach (var point in points) Include(point);

            return PixelRect.FromLtrb(minX, minY, maxX + 1, maxY + 1).Inflate(radius + 1);
        }
    }
}