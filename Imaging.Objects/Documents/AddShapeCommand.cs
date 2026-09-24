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
}
