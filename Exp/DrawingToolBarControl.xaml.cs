/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingToolBarControl.xaml.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System.Windows;
using System.Windows.Controls;

namespace Exp
{
    public partial class DrawingToolBarControl : UserControl
    {
        public DrawingToolBarControl()
        {
            InitializeComponent();
        }

        // Dependency property for binding
        public DrawingState ToolState
        {
            get => (DrawingState)GetValue(ToolStateProperty);
            set => SetValue(ToolStateProperty, value);
        }

        public static readonly DependencyProperty ToolStateProperty =
            DependencyProperty.Register(nameof(ToolState),
                                        typeof(DrawingState),
                                        typeof(DrawingToolBarControl),
                                        new PropertyMetadata(null, OnToolStateChanged));

        private static void OnToolStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawingToolBarControl ctrl)
            {
                ctrl.DataContext = e.NewValue;
            }
        }
    }
}
