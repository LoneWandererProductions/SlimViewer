using System.Drawing;

namespace SlimControls
{
    public record ShapeToolConfig : ToolBase
    {
        public ShapeTool Shape { get; init; }
        public AreaOperation Operation { get; init; }

        // Fill mode
        public Color? FillColor { get; init; }
        public double? FillOpacity { get; init; }

        // Texture config
        public string? TextureName { get; init; }
        public double? TextureScale { get; init; }

        // Filter config
        public string? FilterType { get; init; }
        public double? FilterStrength { get; init; }

        // Erase area config maybe threshold etc later
    }

}
