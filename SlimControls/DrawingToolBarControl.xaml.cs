/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        DrawingToolBarControl.xaml.cs
 * PURPOSE:     Toolbar for drawing tools
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SlimControls
{
    /// <summary>
    /// Menu for selecting drawing tools and modes, bound to a DrawingState
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class DrawingToolBarControl
    {
        /// <summary>
        /// Occurs when [tool changed].
        /// </summary>
        public event EventHandler ToolChanged; // external event for host

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingToolBarControl"/> class.
        /// </summary>
        public DrawingToolBarControl()
        {
            InitializeComponent();
        }

        // Dependency property for binding to the drawing state

        /// <summary>
        /// Gets or sets the state of the tool.
        /// </summary>
        /// <value>
        /// The state of the tool.
        /// </value>
        public DrawingState ToolState
        {
            get => (DrawingState)GetValue(ToolStateProperty);
            set => SetValue(ToolStateProperty, value);
        }

        /// <summary>
        /// The tool state property
        /// </summary>
        public static readonly DependencyProperty ToolStateProperty =
            DependencyProperty.Register(
                nameof(ToolState),
                typeof(DrawingState),
                typeof(DrawingToolBarControl),
                new PropertyMetadata(null, OnToolStateChanged));

        /// <summary>
        /// The tool changed command property
        /// </summary>
        public static readonly DependencyProperty ToolChangedCommandProperty =
            DependencyProperty.Register(
                nameof(ToolChangedCommand),
                typeof(ICommand),
                typeof(DrawingToolBarControl),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tool changed command.
        /// </summary>
        /// <value>
        /// The tool changed command.
        /// </value>
        public ICommand? ToolChangedCommand
        {
            get => (ICommand?)GetValue(ToolChangedCommandProperty);
            set => SetValue(ToolChangedCommandProperty, value);
        }

        /// <summary>
        /// Called when [tool state changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the ToolOrModeChanged event of the ToolState control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ToolState_ToolOrModeChanged(object sender, EventArgs e)
        {
            // Fire the control's event and command
            HandleToolChanged();
        }

        /// <summary>
        /// Handles the tool changed.
        /// </summary>
        private void HandleToolChanged()
        {
            ToolChanged?.Invoke(this, EventArgs.Empty);

            if (ToolChangedCommand?.CanExecute(ToolState) ?? false)
                ToolChangedCommand.Execute(ToolState);
        }
    }
}