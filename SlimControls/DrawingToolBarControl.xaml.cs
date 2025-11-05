/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        DrawingToolBarControl.xaml.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    /// <summary>
    /// Interaktionslogik für Toolbar.xaml
    /// </summary>
    public partial class DrawingToolBarControl : UserControl
    {

        public DrawingToolState ToolState
        {
            get => (DrawingToolState)GetValue(ToolStateProperty);
            set => SetValue(ToolStateProperty, value);
        }

        public static readonly DependencyProperty ToolStateProperty =
            DependencyProperty.Register(nameof(ToolState),
                typeof(DrawingToolState),
                typeof(DrawingToolBarControl),
                new FrameworkPropertyMetadata(new DrawingToolState()));

        public DrawingToolBarControl()
        {
            InitializeComponent();
        }
    }
}
