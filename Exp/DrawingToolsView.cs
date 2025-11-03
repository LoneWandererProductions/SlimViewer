using System.Windows.Input;

namespace Exp
{
     public class DrawingToolsView
    {
        public ICommand SelectTool { get; }
        public ICommand SelectShape { get; }
        public ICommand ToggleFill { get; }
        public ICommand ToggleTexture { get; }
    }
}
