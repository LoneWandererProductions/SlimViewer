/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingToolBarControl.xaml.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public static readonly DependencyProperty ToolChangedCommandProperty =
            DependencyProperty.Register(
                nameof(ToolChangedCommand),
                typeof(ICommand),
                typeof(DrawingToolBarControl),
                new PropertyMetadata(null));

        public ICommand? ToolChangedCommand
        {
            get => (ICommand?)GetValue(ToolChangedCommandProperty);
            set => SetValue(ToolChangedCommandProperty, value);
        }

        private static void OnToolStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawingToolBarControl ctrl)
            {
                ctrl.DataContext = e.NewValue;
            }
        }

        public event EventHandler ToolChanged; // external event for host

        private void HandleToolChanged(object sender, EventArgs e)
        {
            ToolChanged?.Invoke(this, EventArgs.Empty);
            ToolChangedCommand?.Execute(ToolState);
        }
    }
}