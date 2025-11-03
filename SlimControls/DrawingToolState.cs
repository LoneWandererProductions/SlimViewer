namespace SlimControls
{
    public enum DrawTool
    {
        None,
        Pencil,
        Eraser,
        Shape
    }

    public enum ShapeType
    {
        None,
        Rectangle,
        Ellipse,
        Freeform
    }

    public class DrawingToolState
    {
        public DrawTool ActiveTool { get; set; }
        public ShapeType ActiveShape { get; set; }

        // Brush settings
        public double BrushSize { get; set; } = 5;
        public double BrushOpacity { get; set; } = 1.0;
        public string BrushColor { get; set; } = "#000000";

        // Fill options
        public bool UseFill { get; set; }
        public bool UseTexture { get; set; }
        public string TextureName { get; set; }

        // Filters
        public string Filter { get; set; }
    }
}
