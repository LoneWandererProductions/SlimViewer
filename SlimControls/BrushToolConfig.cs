/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        BrushToolConfig.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Drawing;

namespace SlimControls
{
    public record BrushToolConfig : ToolBase
    {
        public BrushTool Brush { get; init; }
        public double Size { get; init; }
        public double Opacity { get; init; }
        public Color Color { get; init; }
    }
}