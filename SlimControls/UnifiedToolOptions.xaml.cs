using CommonControls;
using System;
using System.Windows;

namespace SlimControls
{
    public partial class UnifiedToolOptions
    {
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
        /// Called when [selected tool changed].
        /// Called when the SelectedTool property changes
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the ToolChangedRouted event of the AreaControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AreaControl_ToolChangedRouted(object sender, RoutedEventArgs e)
        {
            //TODO 
        }


        /// <summary>
        /// Notifies the tool selection.
        /// Raise the legacy ToolChanged event
        /// </summary>
        /// <param name="selectedTool">The selected tool.</param>
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

        /// <summary>
        /// Gets or sets the size of the brush.
        /// BrushSize Property
        /// </summary>
        /// <value>
        /// The size of the brush.
        /// </value>
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        /// <summary>
        /// EraseRadius Property
        /// Gets or sets the erase radius.
        /// </summary>
        /// <value>
        /// The erase radius.
        /// </value>
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }


        /// <summary>
        /// ColorTolerance Property
        /// Gets or sets the color tolerance.
        /// </summary>
        /// <value>
        /// The color tolerance.
        /// </value>
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        /// <summary>
        /// Legacy event for backwards compatibility (optional)
        /// Occurs when [tool changed].
        /// </summary>
        public event EventHandler<ImageZoomTools>? ToolChanged;
    }
}
