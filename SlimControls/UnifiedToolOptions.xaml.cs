using System;
using System.Windows;
using System.Windows.Input;
using CommonControls;

namespace SlimControls
{
    public sealed partial class UnifiedToolOptions
    {
        /// <summary>
        /// DependencyProperty for ToolChangedCommand.
        /// </summary>
        public static readonly DependencyProperty ToolChangedCommandProperty =
            DependencyProperty.Register(
                nameof(ToolChangedCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the ToolChangedCommand.
        /// </summary>
        public ICommand? ToolChangedCommand
        {
            get => (ICommand?)GetValue(ToolChangedCommandProperty);
            set => SetValue(ToolChangedCommandProperty, value);
        }


        /// <summary>
        /// DependencyProperty for SelectedTool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(
                nameof(SelectedTool),
                typeof(ImageTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint, OnSelectedToolChanged));

        /// <summary>
        /// DependencyProperty for BrushSize
        /// </summary>
        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(
                nameof(BrushSize),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// DependencyProperty for EraseRadius
        /// </summary>
        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(
                nameof(EraseRadius),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// DependencyProperty for ColorTolerance
        /// </summary>
        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(
                nameof(ColorTolerance),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// Routed event for external subscribers to know when a tool becomes visible
        /// The selected tool changed event
        /// </summary>
        public static readonly RoutedEvent SelectedToolChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectedToolChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<ImageTools>),
                typeof(UnifiedToolOptions));

        /// <summary>
        /// Occurs when [selected tool changed].
        /// Event to notify external subscribers when a tool is selected
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ImageTools> SelectedToolChanged
        {
            add => AddHandler(SelectedToolChangedEvent, value);
            remove => RemoveHandler(SelectedToolChangedEvent, value);
        }

        /// <summary>
        /// Called when the SelectedTool property changes.
        /// </summary>
        private static void OnSelectedToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UnifiedToolOptions control || e.NewValue is not ImageTools newTool) return;

            var args = new RoutedPropertyChangedEventArgs<ImageTools>(
                (ImageTools)e.OldValue,
                newTool,
                SelectedToolChangedEvent);

            control.RaiseEvent(args);

            // Notify via the legacy event as well for compatibility
            control.NotifyToolSelection(Translator.ConvertToImageZoomTools(newTool));
        }

        /// <summary>
        /// Handles the ToolChangedRouted event of the AreaControl control.
        /// </summary>
        private void AreaControl_ToolChangedRouted(object sender, RoutedEventArgs e)
        {
            if (e is not RoutedPropertyChangedEventArgs<ImageZoomTools> toolArgs) return;

            var oldTool = toolArgs.OldValue;
            var newTool = toolArgs.NewValue;

            Console.WriteLine($"Tool changed from {oldTool} to {newTool}");

            // Notify via the legacy event for compatibility
            NotifyToolSelection(newTool);
        }

        /// <summary>
        /// Notifies the tool selection using ImageZoomTools.
        /// </summary>
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            ToolChanged?.Invoke(this, selectedTool);

            // Execute the bound command if available
            if (ToolChangedCommand?.CanExecute(selectedTool) == true)
            {
                ToolChangedCommand.Execute(selectedTool);
            }
        }

        /// <inheritdoc />
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the selected tool.
        /// </summary>
        public ImageTools SelectedTool
        {
            get => (ImageTools)GetValue(SelectedToolProperty);
            set => SetValue(SelectedToolProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the brush.
        /// </summary>
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the erase radius.
        /// </summary>
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the color tolerance.
        /// </summary>
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        /// <summary>
        /// Legacy event for backwards compatibility (optional).
        /// </summary>
        public event EventHandler<ImageZoomTools>? ToolChanged;
    }
}
