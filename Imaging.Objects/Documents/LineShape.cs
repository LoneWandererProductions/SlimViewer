/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents;
 * FILE:        LineShape.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Imaging.Objects.Documents;

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