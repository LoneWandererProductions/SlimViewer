using CommonControls;
using System;
using System.Windows;

namespace SlimControls
{
    public partial class UnifiedToolOptions
    {
        // DependencyProperty for SelectedTool
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(
                nameof(SelectedTool),
                typeof(ImageTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint, OnSelectedToolChanged));

        // DependencyProperty for BrushSize
        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(
                nameof(BrushSize),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // DependencyProperty for EraseRadius
        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(
                nameof(EraseRadius),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // DependencyProperty for ColorTolerance
        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(
                nameof(ColorTolerance),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // Routed event for external subscribers to know when a tool becomes visible
        public static readonly RoutedEvent SelectedToolChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectedToolChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<ImageTools>),
                typeof(UnifiedToolOptions));

        // Event to notify external subscribers when a tool is selected
        public event RoutedPropertyChangedEventHandler<ImageTools> SelectedToolChanged
        {
            add => AddHandler(SelectedToolChangedEvent, value);
            remove => RemoveHandler(SelectedToolChangedEvent, value);
        }

        // Called when the SelectedTool property changes
        private static void OnSelectedToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UnifiedToolOptions control && e.NewValue is ImageTools newTool)
            {
                var args = new RoutedPropertyChangedEventArgs<ImageTools>(
                    (ImageTools)e.OldValue,
                    newTool,
                    SelectedToolChangedEvent);

                control.RaiseEvent(args);

                // Notify via the legacy event as well for compatibility
                control.NotifyToolSelection(newTool);
            }
        }

        // Raise the legacy ToolChanged event
        private void NotifyToolSelection(ImageTools selectedTool)
        {
            ToolChanged?.Invoke(this, (ImageZoomTools)selectedTool);
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// Initializes a new instance of the <see cref="T:SlimControls.UnifiedToolOptions" /> class.
        /// </summary>
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// SelectedTool Property
        /// Gets or sets the selected tool.
        /// </summary>
        public ImageTools SelectedTool
        {
            get => (ImageTools)GetValue(SelectedToolProperty);
            set => SetValue(SelectedToolProperty, value);
        }

        // BrushSize Property
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        // EraseRadius Property
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        // ColorTolerance Property
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        // Legacy event for backwards compatibility (optional)
        public event EventHandler<ImageZoomTools>? ToolChanged;
    }
}
