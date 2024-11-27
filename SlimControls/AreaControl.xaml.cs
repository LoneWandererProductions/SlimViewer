using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public sealed partial class AreaControl
    {
        /// <summary>
        /// The selected tool type property
        /// </summary>
        public static readonly DependencyProperty SelectedToolTypeProperty =
            DependencyProperty.Register(nameof(SelectedToolType),typeof(string), typeof(AreaControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The selected fill type property
        /// </summary>
        public static readonly DependencyProperty SelectedFillTypeProperty =
            DependencyProperty.Register(nameof(SelectedFillType), typeof(string), typeof(AreaControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the type of the selected tool.
        /// </summary>
        /// <value>
        /// The type of the selected tool.
        /// </value>
        public string SelectedToolType
        {
            get => (string)GetValue(SelectedToolTypeProperty);
            set => SetValue(SelectedToolTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets the type of the selected fill.
        /// </summary>
        /// <value>
        /// The type of the selected fill.
        /// </value>
        public string SelectedFillType
        {
            get => (string)GetValue(SelectedFillTypeProperty);
            set => SetValue(SelectedFillTypeProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaControl"/> class.
        /// </summary>
        public AreaControl()
        {
            InitializeComponent();
        }
    }
}