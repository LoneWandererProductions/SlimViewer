using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    public partial class UnifiedToolOptions : UserControl
    {
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(nameof(SelectedTool), typeof(ImageTools), typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint));

        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(nameof(BrushSize), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(nameof(EraseRadius), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(nameof(ColorTolerance), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        // SelectedTool DependencyProperty
        public ImageTools SelectedTool
        {
            get => (ImageTools)GetValue(SelectedToolProperty);
            set => SetValue(SelectedToolProperty, value);
        }

        // BrushSize DependencyProperty
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        // EraseRadius DependencyProperty
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        // ColorTolerance DependencyProperty
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }
    }
}