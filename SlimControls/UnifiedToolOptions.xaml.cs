using CommonControls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    public partial class UnifiedToolOptions : UserControl
    {
        // DependencyProperty for SelectedTool
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(nameof(SelectedTool), typeof(ImageTools), typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint, OnSelectedToolChanged));

        // DependencyProperty for BrushSize
        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(nameof(BrushSize), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // DependencyProperty for EraseRadius
        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(nameof(EraseRadius), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // DependencyProperty for ColorTolerance
        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(nameof(ColorTolerance), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // Event to notify external subscribers when a tool is selected
        public event EventHandler<ImageZoomTools>? ToolChanged;

        // Called when the SelectedTool property changes
        private static void OnSelectedToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UnifiedToolOptions control && e.NewValue is ImageTools newTool)
            {
                // Notify external listeners
                control.NotifyToolSelection((ImageZoomTools)newTool);
            }
        }

        // Raise the ToolChanged event
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            ToolChanged?.Invoke(this, selectedTool);
        }

        /// <summary>
        /// Constructor
        /// Initializes a new instance of the <see cref="UnifiedToolOptions"/> class.
        /// </summary>
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// SelectedTool Property
        /// Gets or sets the selected tool.
        /// </summary>
        /// <value>
        /// The selected tool.
        /// </value>
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
    }
}
