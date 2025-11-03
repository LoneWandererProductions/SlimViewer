using System.Drawing;

namespace Exp
{
    public record BrushToolConfig : ToolBase
    {
        public BrushTool Brush { get; init; }
        public double Size { get; init; }
        public double Opacity { get; init; }
        public Color Color { get; init; }
    }

}
