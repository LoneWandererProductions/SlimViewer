/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Exp
 * FILE:        DrawingToolBarControl.xaml.cs
 * PURPOSE:     Toolbar for drawing tools
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

#nullable enable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Exp
{
    public partial class DrawingToolBarControl : UserControl
    {
        public event EventHandler ToolChanged; // external event for host

        public DrawingToolBarControl()
        {
            InitializeComponent();
        }

        // Dependency property for binding to the drawing state
        public DrawingState ToolState
        {
            get => (DrawingState)GetValue(ToolStateProperty);
            set => SetValue(ToolStateProperty, value);
        }

        public static readonly DependencyProperty ToolStateProperty =
            DependencyProperty.Register(
                nameof(ToolState),
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
            if (d is not DrawingToolBarControl ctrl) return;

            // Unsubscribe from previous state event
            if (e.OldValue is DrawingState oldState)
                oldState.ToolOrModeChanged -= ctrl.ToolState_ToolOrModeChanged;

            // Update DataContext
            ctrl.DataContext = e.NewValue;

            // Subscribe to new state event
            if (e.NewValue is DrawingState newState)
                newState.ToolOrModeChanged += ctrl.ToolState_ToolOrModeChanged;
        }

        private void ToolState_ToolOrModeChanged(object sender, EventArgs e)
        {
            // Fire the control's event and command
            HandleToolChanged();
        }

        private void HandleToolChanged()
        {
            ToolChanged?.Invoke(this, EventArgs.Empty);

            if (ToolChangedCommand?.CanExecute(ToolState) ?? false)
                ToolChangedCommand.Execute(ToolState);
        }
    }
}