/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents
 * FILE:        Commands.cs
 * PURPOSE:     Undoable edits of a document. A command stores just enough to apply and revert itself:
 *              ids and small records for structure/shape edits, tile patches for pixel edits.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Imaging.Objects.Documents
{
    /// <summary>
    ///     One undoable step. <see cref="Apply" /> is used for the first execution and for redo.
    ///     Commands that own layers implement <see cref="IDisposable" />; <see cref="DocumentHistory" /> disposes
    ///     them when they can no longer be reached (trimmed, redo branch dropped, history disposed).
    /// </summary>
    public interface IDocumentCommand
    {
        /// <summary>Gets a short human-readable name for menus ("Undo Add layer").</summary>
        string Description { get; }

        /// <summary>Executes (or re-executes) the change.</summary>
        void Apply(Document document);

        /// <summary>Undoes the change.</summary>
        void Revert(Document document);
    }

    /// <summary>Inserts a layer (at the top unless an index is given).</summary>
    public sealed class AddLayerCommand : IDocumentCommand, IDisposable
    {
        private readonly int? _index;

        private readonly Layer _layer;

        private bool _applied;

        /// <summary>Initializes a new instance of the <see cref="AddLayerCommand" /> class.</summary>
        public AddLayerCommand(Layer layer, int? index = null)
        {
            ArgumentNullException.ThrowIfNull(layer);
            _layer = layer;
            _index = index;
        }

        /// <inheritdoc />
        public string Description => "Add layer";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            document.InsertLayer(_index ?? document.Layers.Count, _layer);
            _applied = true;
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            document.RemoveLayer(_layer.Id, out _);
            _applied = false;
        }

        /// <summary>Releases the layer if it is not part of a document (undone and discarded).</summary>
        public void Dispose()
        {
            if (!_applied) (_layer as IDisposable)?.Dispose();
        }
    }

    /// <summary>Removes a layer. The command keeps it alive so undo can put it back.</summary>
    public sealed class RemoveLayerCommand : IDocumentCommand, IDisposable
    {
        private readonly Guid _layerId;

        private int _index;

        private Layer? _removed;

        /// <summary>Initializes a new instance of the <see cref="RemoveLayerCommand" /> class.</summary>
        public RemoveLayerCommand(Guid layerId)
        {
            _layerId = layerId;
        }

        /// <inheritdoc />
        public string Description => "Delete layer";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            _removed = document.RemoveLayer(_layerId, out _index);
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            if (_removed is null) return;

            document.InsertLayer(_index, _removed);
            _removed = null;
        }

        /// <summary>Releases the removed layer once it can no longer be restored.</summary>
        public void Dispose()
        {
            (_removed as IDisposable)?.Dispose();
            _removed = null;
        }
    }

    /// <summary>Moves a layer in the stack.</summary>
    public sealed class MoveLayerCommand : IDocumentCommand
    {
        private readonly Guid _layerId;

        private readonly int _newIndex;

        private int _oldIndex;

        /// <summary>Initializes a new instance of the <see cref="MoveLayerCommand" /> class.</summary>
        public MoveLayerCommand(Guid layerId, int newIndex)
        {
            _layerId = layerId;
            _newIndex = newIndex;
        }

        /// <inheritdoc />
        public string Description => "Move layer";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            _oldIndex = document.IndexOf(_layerId);
            document.MoveLayer(_layerId, _newIndex);
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            document.MoveLayer(_layerId, _oldIndex);
        }
    }

    /// <summary>
    ///     Changes name, visibility, opacity and/or blend mode. For a slider, execute one command when the drag ends,
    ///     not one per mouse move.
    /// </summary>
    public sealed class SetLayerPropertiesCommand : IDocumentCommand
    {
        private readonly LayerProperties _after;

        private readonly Guid _layerId;

        private LayerProperties? _before;

        /// <summary>Initializes a new instance of the <see cref="SetLayerPropertiesCommand" /> class.</summary>
        public SetLayerPropertiesCommand(Guid layerId, LayerProperties after)
        {
            ArgumentNullException.ThrowIfNull(after);
            _layerId = layerId;
            _after = after;
        }

        /// <inheritdoc />
        public string Description => "Change layer";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            var layer = document.GetLayer(_layerId);
            _before ??= layer.Properties;
            layer.Properties = _after;
            document.Raise(DocumentChangeKind.LayerPropertiesChanged, _layerId, document.Bounds);
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            if (_before is null) return;

            document.GetLayer(_layerId).Properties = _before;
            document.Raise(DocumentChangeKind.LayerPropertiesChanged, _layerId, document.Bounds);
        }
    }

    /// <summary>Adds a shape to a shape layer (on top unless an index is given).</summary>
    public sealed class AddShapeCommand : IDocumentCommand
    {
        private readonly int? _index;

        private readonly Guid _layerId;

        private readonly Shape _shape;

        /// <summary>Initializes a new instance of the <see cref="AddShapeCommand" /> class.</summary>
        public AddShapeCommand(Guid layerId, Shape shape, int? index = null)
        {
            ArgumentNullException.ThrowIfNull(shape);
            _layerId = layerId;
            _shape = shape;
            _index = index;
        }

        /// <inheritdoc />
        public string Description => "Add shape";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            var layer = document.GetLayer<ShapeLayer>(_layerId);
            layer.Insert(_index ?? layer.Shapes.Count, _shape);
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _shape.GetBounds());
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            var layer = document.GetLayer<ShapeLayer>(_layerId);
            layer.RemoveAt(layer.IndexOf(_shape.Id));
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _shape.GetBounds());
        }
    }

    /// <summary>Removes a shape from a shape layer.</summary>
    public sealed class RemoveShapeCommand : IDocumentCommand
    {
        private readonly Guid _layerId;

        private readonly Guid _shapeId;

        private int _index;

        private Shape? _removed;

        /// <summary>Initializes a new instance of the <see cref="RemoveShapeCommand" /> class.</summary>
        public RemoveShapeCommand(Guid layerId, Guid shapeId)
        {
            _layerId = layerId;
            _shapeId = shapeId;
        }

        /// <inheritdoc />
        public string Description => "Delete shape";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            var layer = document.GetLayer<ShapeLayer>(_layerId);
            _index = layer.IndexOf(_shapeId);
            if (_index < 0) throw new KeyNotFoundException(string.Format(ImageResource.ErrorShapeNotFound, _shapeId));

            _removed = layer.RemoveAt(_index);
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _removed.GetBounds());
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            if (_removed is null) return;

            document.GetLayer<ShapeLayer>(_layerId).Insert(_index, _removed);
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _removed.GetBounds());
        }
    }

    /// <summary>
    ///     Replaces a shape by a changed copy with the same id: move, resize, restyle. The dirty region covers
    ///     the old and the new position.
    /// </summary>
    public sealed class ReplaceShapeCommand : IDocumentCommand
    {
        private readonly Guid _layerId;

        private readonly Shape _after;

        private Shape? _before;

        private int _index;

        /// <summary>Initializes a new instance of the <see cref="ReplaceShapeCommand" /> class.</summary>
        public ReplaceShapeCommand(Guid layerId, Shape after)
        {
            ArgumentNullException.ThrowIfNull(after);
            _layerId = layerId;
            _after = after;
        }

        /// <inheritdoc />
        public string Description => "Edit shape";

        /// <inheritdoc />
        public void Apply(Document document)
        {
            var layer = document.GetLayer<ShapeLayer>(_layerId);
            _index = layer.IndexOf(_after.Id);
            if (_index < 0) throw new KeyNotFoundException(string.Format(ImageResource.ErrorShapeNotFound, _after.Id));

            _before = layer.Replace(_index, _after);
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _before.GetBounds().Union(_after.GetBounds()));
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            if (_before is null) return;

            document.GetLayer<ShapeLayer>(_layerId).Replace(_index, _before);
            document.Raise(DocumentChangeKind.ShapesChanged, _layerId, _before.GetBounds().Union(_after.GetBounds()));
        }
    }

    /// <summary>
    ///     Pixel edit stored as tile patches. Created by <see cref="RasterEditRecorder.Commit" />; the pixels are
    ///     already changed at that point, so hand it to <see cref="DocumentHistory.Record" />, not
    ///     <see cref="DocumentHistory.Execute" />.
    /// </summary>
    public sealed class RasterPatchCommand : IDocumentCommand
    {
        private readonly List<TilePatch> _tiles;

        private readonly Guid _layerId;

        internal RasterPatchCommand(Guid layerId, string description, List<TilePatch> tiles)
        {
            _layerId = layerId;
            Description = description;
            _tiles = tiles;
        }

        /// <inheritdoc />
        public string Description { get; }

        /// <summary>Gets the number of stored tiles.</summary>
        public int TileCount => _tiles.Count;

        /// <summary>Gets the memory held by this step in bytes (before + after pixels).</summary>
        public long ByteSize
        {
            get
            {
                long total = 0;
                foreach (var tile in _tiles)
                {
                    total += tile.Before.Length + tile.After.Length;
                }

                return total;
            }
        }

        /// <inheritdoc />
        public void Apply(Document document)
        {
            Write(document, useAfter: true);
        }

        /// <inheritdoc />
        public void Revert(Document document)
        {
            Write(document, useAfter: false);
        }

        internal static void WriteTile(RasterLayer layer, int tx, int ty, int width, int height, byte[] pixels)
        {
            const int bpp = UnmanagedImageBuffer.BytesPerPixel;
            var target = layer.Pixels.BufferSpan;

            for (var y = 0; y < height; y++)
            {
                var offset = ((((ty * RasterEditRecorder.TileSize) + y) * layer.Width) +
                              (tx * RasterEditRecorder.TileSize)) * bpp;

                pixels.AsSpan(y * width * bpp, width * bpp).CopyTo(target.Slice(offset, width * bpp));
            }
        }

        private void Write(Document document, bool useAfter)
        {
            var layer = document.GetLayer<RasterLayer>(_layerId);
            var region = PixelRect.Empty;

            foreach (var tile in _tiles)
            {
                WriteTile(layer, tile.Tx, tile.Ty, tile.Width, tile.Height, useAfter ? tile.After : tile.Before);

                region = region.Union(new PixelRect(tile.Tx * RasterEditRecorder.TileSize,
                    tile.Ty * RasterEditRecorder.TileSize, tile.Width, tile.Height));
            }

            document.Raise(DocumentChangeKind.PixelsChanged, _layerId, region);
        }
    }
}
