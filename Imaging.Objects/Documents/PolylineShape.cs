/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents;
 * FILE:        PolylineShape.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Immutable;

namespace Imaging.Objects.Documents;

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