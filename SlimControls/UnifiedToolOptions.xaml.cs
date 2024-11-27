using System.Windows;
using System.Windows.Controls;


//Implementation:
//<local:UnifiedToolOptions SelectedTool="{Binding CurrentTool}" />
//in View:
//public UnifiedToolOptions.ToolType CurrentTool { get; set; } = UnifiedToolOptions.ToolType.Paint;

namespace SlimControls
{
    public partial class UnifiedToolOptions : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedToolOptions" /> class.
        /// </summary>
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency Property for SelectedTool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(
                nameof(SelectedTool),
                typeof(ImageTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Paint));

        /// <summary>
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
    }
}