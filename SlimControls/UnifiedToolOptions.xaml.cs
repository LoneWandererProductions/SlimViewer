using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    public partial class UnifiedToolOptions : UserControl
    {
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

        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(nameof(SelectedTool), typeof(ImageTools), typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint));

        // BrushSize DependencyProperty
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(nameof(BrushSize), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // EraseRadius DependencyProperty
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(nameof(EraseRadius), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        // ColorTolerance DependencyProperty
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(nameof(ColorTolerance), typeof(double), typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));
    }
}
