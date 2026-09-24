/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Imaging.Objects.Documents;
 * FILE:        PolygonShape.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Collections.Immutable;

namespace Imaging.Objects.Documents;

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