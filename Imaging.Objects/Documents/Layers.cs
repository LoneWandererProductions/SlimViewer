/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents
 * FILE:        Layers.cs
 * PURPOSE:     The layer types of a document: pixel layers and shape layers.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Imaging.Objects.Documents
{
    /// <summary>
    ///     How a layer is combined with the ones below. Only <see cref="Normal" /> exists so far;
    ///     the value is stored in files already so more modes can be added without a format change.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>Straight-alpha "source over".</summary>
        Normal = 0
    }

    /// <summary>
    ///     Snapshot of the user-visible settings of a layer. Used by the undoable property command.
    /// </summary>
    public sealed record LayerProperties(string Name, bool Visible, double Opacity, BlendMode Blend);

    /// <summary>
    ///     Base class of all layers. Layers are plain data holders; changes that should be undoable go
    ///     through <see cref="IDocumentCommand" />s.
    /// </summary>
    public abstract class Layer
    {
        private string _name;

        private double _opacity = 1.0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Layer" /> class.
        /// </summary>
        protected Layer(Guid id, string? name)
        {
            Id = id;
            _name = name ?? string.Empty;
        }

        /// <summary>Gets the stable identity of the layer.</summary>
        public Guid Id { get; }

        /// <summary>Gets or sets the display name.</summary>
        public string Name
        {
            get => _name;
            set => _name = value ?? string.Empty;
        }

        /// <summary>Gets or sets a value indicating whether the layer takes part in rendering.</summary>
        public bool Visible { get; set; } = true;

        /// <summary>Gets or sets the opacity, clamped to 0..1.</summary>
        public double Opacity
        {
            get => _opacity;
            set => _opacity = double.IsFinite(value) ? Math.Clamp(value, 0d, 1d) : 1d;
        }

        /// <summary>Gets or sets the blend mode.</summary>
        public BlendMode Blend { get; set; } = BlendMode.Normal;

        /// <summary>Gets or sets all user-visible settings at once.</summary>
        public LayerProperties Properties
        {
            get => new(Name, Visible, Opacity, Blend);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                Name = value.Name;
                Visible = value.Visible;
                Opacity = value.Opacity;
                Blend = value.Blend;
            }
        }

        /// <summary>Creates an independent copy with a new id (name defaults to "&lt;name&gt; copy").</summary>
        public abstract Layer Duplicate(string? name = null);

        /// <summary>Copies visibility, opacity and blend mode to <paramref name="target" />.</summary>
        protected void CopySettingsTo(Layer target)
        {
            target.Visible = Visible;
            target.Opacity = Opacity;
            target.Blend = Blend;
        }
    }

    /// <summary>
    ///     A layer of pixels, stored in an <see cref="UnmanagedImageBuffer" /> (straight-alpha BGRA, same layout
    ///     as GDI+ Format32bppArgb). Always the size of the document.
    /// </summary>
    public sealed class RasterLayer : Layer, IDisposable
    {
        private UnmanagedImageBuffer? _pixels;

        /// <summary>Creates a fully transparent layer.</summary>
        public RasterLayer(int width, int height, string name = "Layer")
            : this(Guid.NewGuid(), name, CreateTransparent(width, height))
        {
        }

        /// <summary>Creates a layer that takes ownership of <paramref name="pixels" />.</summary>
        public RasterLayer(UnmanagedImageBuffer pixels, string name = "Layer")
            : this(Guid.NewGuid(), name, pixels)
        {
        }

        /// <summary>Creates a layer with a known id (used by the loader). Takes ownership of the pixels.</summary>
        public RasterLayer(Guid id, string? name, UnmanagedImageBuffer pixels)
            : base(id, name)
        {
            ArgumentNullException.ThrowIfNull(pixels);

            _pixels = pixels;
            Width = pixels.Width;
            Height = pixels.Height;
        }

        /// <summary>Gets the width in pixels.</summary>
        public int Width { get; }

        /// <summary>Gets the height in pixels.</summary>
        public int Height { get; }

        /// <summary>Gets a value indicating whether the pixel memory has been released.</summary>
        public bool IsDisposed => _pixels is null;

        /// <summary>Gets the pixel buffer. Writes bypass history; use <see cref="RasterEditRecorder" /> for that.</summary>
        public UnmanagedImageBuffer Pixels => _pixels ?? throw new ObjectDisposedException(nameof(RasterLayer));

        /// <inheritdoc />
        public override Layer Duplicate(string? name = null)
        {
            var copy = new RasterLayer(Guid.NewGuid(), name ?? $"{Name} copy", (UnmanagedImageBuffer)Pixels.Clone());
            CopySettingsTo(copy);
            return copy;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _pixels?.Dispose();
            _pixels = null;
        }

        private static UnmanagedImageBuffer CreateTransparent(int width, int height)
        {
            DocumentLimits.EnsureValidSize(width, height);

            // AllocHGlobal does not zero memory, so a new layer must be cleared explicitly.
            var buffer = new UnmanagedImageBuffer(width, height);
            buffer.Clear(0, 0, 0, 0);
            return buffer;
        }
    }

    /// <summary>
    ///     A layer of geometry. Nothing is rasterized until a renderer asks for it. Shapes are drawn in list
    ///     order, the last one on top.
    /// </summary>
    public sealed class ShapeLayer : Layer
    {
        private readonly List<Shape> _shapes = new();

        /// <summary>Creates an empty shape layer.</summary>
        public ShapeLayer(string name = "Shapes")
            : base(Guid.NewGuid(), name)
        {
        }

        /// <summary>Creates an empty shape layer with a known id (used by the loader).</summary>
        public ShapeLayer(Guid id, string? name)
            : base(id, name)
        {
        }

        /// <summary>Gets the shapes, bottom to top.</summary>
        public IReadOnlyList<Shape> Shapes => _shapes;

        /// <summary>Gets the index of the shape with the given id, or -1.</summary>
        public int IndexOf(Guid shapeId)
        {
            for (var i = 0; i < _shapes.Count; i++)
            {
                if (_shapes[i].Id == shapeId) return i;
            }

            return -1;
        }

        /// <summary>Gets the shape with the given id, or null.</summary>
        public Shape? Find(Guid shapeId)
        {
            var index = IndexOf(shapeId);
            return index < 0 ? null : _shapes[index];
        }

        /// <summary>Inserts a shape. Ids must be unique inside the layer.</summary>
        public void Insert(int index, Shape shape)
        {
            ArgumentNullException.ThrowIfNull(shape);
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _shapes.Count);

            if (IndexOf(shape.Id) >= 0) throw new ArgumentException(string.Format(ImageResource.ErrorShapeExists, shape.Id));

            _shapes.Insert(index, shape);
        }

        /// <summary>Removes and returns the shape at <paramref name="index" />.</summary>
        public Shape RemoveAt(int index)
        {
            var shape = _shapes[index];
            _shapes.RemoveAt(index);
            return shape;
        }

        /// <summary>Replaces the shape at <paramref name="index" /> (same id required) and returns the old one.</summary>
        public Shape Replace(int index, Shape shape)
        {
            ArgumentNullException.ThrowIfNull(shape);

            var old = _shapes[index];
            if (old.Id != shape.Id) throw new ArgumentException(ImageResource.ErrorShapeIdMismatch);

            _shapes[index] = shape;
            return old;
        }

        /// <inheritdoc />
        public override Layer Duplicate(string? name = null)
        {
            var copy = new ShapeLayer(Guid.NewGuid(), name ?? $"{Name} copy");
            CopySettingsTo(copy);

            // Shapes are immutable, so sharing the records is safe.
            copy._shapes.AddRange(_shapes);
            return copy;
        }
    }
}
