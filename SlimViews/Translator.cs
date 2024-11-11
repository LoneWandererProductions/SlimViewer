using CommonControls;

namespace SlimViews
{
    internal static class Translator
    {
        internal static SelectionTools GetToolsFromString(string command)
        {
            switch (command)
            {
                case "Move":
                    return SelectionTools.Move;
                case "Rectangle":
                    return SelectionTools.Rectangle;
                case "Ellipse":
                    return SelectionTools.Ellipse;
                case "Free Form":
                    return SelectionTools.FreeForm;
                default:
                    return SelectionTools.Move;
            }
        }
    }
}