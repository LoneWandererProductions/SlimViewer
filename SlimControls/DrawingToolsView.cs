/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        DrawingToolsView.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Windows.Input;

namespace SlimControls
{
     public class DrawingToolsView
    {
        public ICommand SelectTool { get; }
        public ICommand SelectShape { get; }
        public ICommand ToggleFill { get; }
        public ICommand ToggleTexture { get; }
    }
}
