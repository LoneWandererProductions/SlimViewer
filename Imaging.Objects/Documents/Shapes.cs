/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents
 * FILE:        Shapes.cs
 * PURPOSE:     Geometry as pure, immutable data. A shape layer stores these records; renderers turn them
 *              into pixels (or WPF visuals) only when needed.
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Imaging.Objects.Documents
{
    /// <summary>
    ///     Outline of a shape. <see cref="Argb" /> is straight (non-premultiplied) 0xAARRGGBB.
    /// </summary>
    public sealed record StrokeSpec(uint Argb, double Width)
    {
        /// <summary>Gets the default stroke: opaque black, one pixel.</summary>
        public static StrokeSpec Default { get; } = new(0xFF000000, 1.0);
    }

    /// <summary>
    ///     How the inside of a closed shape is filled. Texture and filter fills are data too: they name an
    ///     effect of the imaging pipeline instead of baking its result into pixels, so the region stays editable.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(NoFill), "none")]
    [JsonDerivedType(typeof(SolidFill), "solid")]
    [JsonDerivedType(typeof(TextureFill), "texture")]
    [JsonDerivedType(typeof(FilterFill), "filter")]
    public abstract record FillSpec
    {
        /// <summary>Gets the shared "no fill" instance.</summary>
        public static FillSpec None { get; } = new NoFill();
    }

    /// <summary>No fill.</summary>
    public sealed record NoFill : FillSpec;

    /// <summary>A solid colour, straight 0xAARRGGBB.</summary>
    public sealed record SolidFill(uint Argb) : FillSpec;

    /// <summary>A named texture of the imaging pipeline.</summary>
    public sealed record TextureFill(string Name) : FillSpec;

    /// <summary>A named filter of the imaging pipeline, applied to what is below the shape.</summary>
    public sealed record FilterFill(string Name) : FillSpec;

    /// <summary>
    ///     Base of all shapes. Shapes are immutable: "moving" one produces a new record with the same
    ///     <see cref="Id" />, which is exactly what an undoable replace command needs.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(LineShape), "line")]
    [JsonDerivedType(typeof(RectShape), "rect")]
    [JsonDerivedType(typeof(EllipseShape), "ellipse")]
    [JsonDerivedType(typeof(PolygonShape), "polygon")]
    [JsonDerivedType(typeof(PolylineShape), "polyline")]
    public abstract record Shape
    {
        /// <summary>Gets the identity of the shape inside its layer.</summary>
        public Guid Id { get; init; } = Guid.NewGuid();

        /// <summary>Gets the outline.</summary>
        public StrokeSpec Stroke { get; init; } = StrokeSpec.Default;

        /// <summary>Gets the fill. Ignored by open shapes (line, polyline).</summary>
        public FillSpec Fill { get; init; } = FillSpec.None;

        /// <summary>
        ///     Gets the pixels this shape can touch, including half the stroke width and one pixel of
        ///     anti-aliasing. This is the dirty region when the shape is added, moved or removed.
        /// </summary>
        public abstract PixelRect GetBounds();

        /// <summary>Returns a copy moved by the given offset.</summary>
        public abstract Shape Translate(double dx, double dy);

        /// <summary>
        ///     Checks that all numbers are finite and the shape has enough points to be drawn.
        ///     The loader rejects files that contain invalid shapes.
        /// </summary>
        public abstract bool IsValid();

        /// <summary>Validation shared by all shapes.</summary>
        protected bool IsStyleValid()
        {
            return Stroke is not null && Fill is not null && double.IsFinite(Stroke.Width) && Stroke.Width >= 0;
        }

        /// <summary>Turns a floating point bounding box into stroke-padded pixel bounds.</summary>
        protected PixelRect BoundsOf(double minX, double minY, double maxX, double maxY)
        {
            var width = Stroke is null ? 0d : Math.Max(0d, Stroke.Width);
            var pad = (int)Math.Ceiling(width / 2d) + 1;

            return PixelRect.FromLtrb(
                ToInt(Math.Floor(minX)) - pad,
                ToInt(Math.Floor(minY)) - pad,
                ToInt(Math.Ceiling(maxX)) + pad,
                ToInt(Math.Ceiling(maxY)) + pad);
        }

        private static int ToInt(double value)
        {
            return (int)Math.Clamp(value, -1_000_000_000d, 1_000_000_000d);
        }

        /// <summary>Stroke-padded bounds of a point list.</summary>
        protected PixelRect PointBounds(ImmutableArray<PointD> points)
        {
            if (points.IsDefaultOrEmpty) return PixelRect.Empty;

            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
            foreach (var point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            return BoundsOf(minX, minY, maxX, maxY);
        }

        /// <summary>Moves every point by the given offset.</summary>
        protected static ImmutableArray<PointD> MovePoints(ImmutableArray<PointD> points, double dx, double dy)
        {
            if (points.IsDefaultOrEmpty) return points;

            var builder = ImmutableArray.CreateBuilder<PointD>(points.Length);
            foreach (var point in points)
            {
                builder.Add(new PointD(point.X + dx, point.Y + dy));
            }

            return builder.MoveToImmutable();
        }

        /// <summary>True if there are at least <paramref name="minimum" /> points and all are finite.</summary>
        protected static bool ArePointsValid(ImmutableArray<PointD> points, int minimum)
        {
            if (points.IsDefault || points.Length < minimum) return false;

            foreach (var point in points)
            {
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y)) return false;
            }

            return true;
        }
    }

    /// <summary>A straight line. Open, so <see cref="Shape.Fill" /> is ignored.</summary>
    public sealed record LineShape(PointD From, PointD To) : Shape
    {
        /// <inheritdoc />
        public override PixelRect GetBounds()
        {
            return BoundsOf(Math.Min(From.X, To.X), Math.Min(From.Y, To.Y), Math.Max(From.X, To.X),
                Math.Max(From.Y, To.Y));
        }

        /// <inheritdoc />
        public override Shape Translate(double dx, double dy)
        {
            return this with { From = new PointD(From.X + dx, From.Y + dy), To = new PointD(To.X + dx, To.Y + dy) };
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            return IsStyleValid() && double.IsFinite(From.X) && double.IsFinite(From.Y) && double.IsFinite(To.X) &&
                   double.IsFinite(To.Y);
        }
    }

    /// <summary>An axis-aligned rectangle.</summary>
    public sealed record RectShape(double X, double Y, double Width, double Height) : Shape
    {
        /// <inheritdoc />
        public override PixelRect GetBounds()
        {
            return BoundsOf(Math.Min(X, X + Width), Math.Min(Y, Y + Height), Math.Max(X, X + Width),
                Math.Max(Y, Y + Height));
        }

        /// <inheritdoc />
        public override Shape Translate(double dx, double dy)
        {
            return this with { X = X + dx, Y = Y + dy };
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            return IsStyleValid() && double.IsFinite(X) && double.IsFinite(Y) && double.IsFinite(Width) &&
                   double.IsFinite(Height) && Width >= 0 && Height >= 0;
        }
    }

    /// <summary>An axis-aligned ellipse, described by its bounding box (like a selection frame).</summary>
    public sealed record EllipseShape(double X, double Y, double Width, double Height) : Shape
    {
        /// <inheritdoc />
        public override PixelRect GetBounds()
        {
            return BoundsOf(Math.Min(X, X + Width), Math.Min(Y, Y + Height), Math.Max(X, X + Width),
                Math.Max(Y, Y + Height));
        }

        /// <inheritdoc />
        public override Shape Translate(double dx, double dy)
        {
            return this with { X = X + dx, Y = Y + dy };
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            return IsStyleValid() && double.IsFinite(X) && double.IsFinite(Y) && double.IsFinite(Width) &&
                   double.IsFinite(Height) && Width >= 0 && Height >= 0;
        }
    }

    /// <summary>A closed polygon with at least three points.</summary>
    public sealed record PolygonShape(ImmutableArray<PointD> Points) : Shape
    {
        /// <inheritdoc />
        public override PixelRect GetBounds()
        {
            return PointBounds(Points);
        }

        /// <inheritdoc />
        public override Shape Translate(double dx, double dy)
        {
            return this with { Points = MovePoints(Points, dx, dy) };
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            return IsStyleValid() && ArePointsValid(Points, 3);
        }
    }

    /// <summary>An open polyline with at least two points. Stroke only.</summary>
    public sealed record PolylineShape(ImmutableArray<PointD> Points) : Shape
    {
        /// <inheritdoc />
        public override PixelRect GetBounds()
        {
            return PointBounds(Points);
        }

        /// <inheritdoc />
        public override Shape Translate(double dx, double dy)
        {
            return this with { Points = MovePoints(Points, dx, dy) };
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            return IsStyleValid() && ArePointsValid(Points, 2);
        }
    }
}
