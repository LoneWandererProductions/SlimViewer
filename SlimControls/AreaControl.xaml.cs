using System;
using System.Windows;
using System.Windows.Controls;
using CommonControls;

namespace SlimControls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public sealed partial class AreaControl
    {
        /// <summary>
        ///     The selected tool type property
        /// </summary>
        public static readonly DependencyProperty SelectedToolTypeProperty =
            DependencyProperty.Register(
                nameof(SelectedToolType),
                typeof(ImageZoomTools),
                typeof(AreaControl),
                new PropertyMetadata(default(ImageZoomTools), OnSelectedToolTypeChanged));

        /// <summary>
        ///     Callback invoked when the SelectedToolType changes.
        /// </summary>
        private static void OnSelectedToolTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AreaControl control && e.NewValue is ImageZoomTools newTool)
            {
                control.NotifyToolSelection(newTool);
            }
        }

        /// <summary>
        ///     The selected fill type property
        /// </summary>
        public static readonly DependencyProperty SelectedFillTypeProperty =
            DependencyProperty.Register(nameof(SelectedFillType), typeof(string), typeof(AreaControl),
                new PropertyMetadata(default(string)));

        // Event to notify when a tool is selected
        public event EventHandler<ImageZoomTools>? ToolChanged;


        // Raise ToolChanged when a tool is selected
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            ToolChanged?.Invoke(this, selectedTool);
        }

        // Example: Hook this to a button click or selection change
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NotifyToolSelection(ImageZoomTools.Rectangle); // Example for Rectangle tool
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AreaControl" /> class.
        /// </summary>
        public AreaControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the type of the selected tool.
        /// </summary>
        /// <value>
        ///     The type of the selected tool.
        /// </value>
        public ImageZoomTools SelectedToolType
        {
            get => (ImageZoomTools)GetValue(SelectedToolTypeProperty);
            set => SetValue(SelectedToolTypeProperty, value);
        }

        /// <summary>
        ///     Gets or sets the type of the selected fill.
        /// </summary>
        /// <value>
        ///     The type of the selected fill.
        /// </value>
        public string SelectedFillType
        {
            get => (string)GetValue(SelectedFillTypeProperty);
            set => SetValue(SelectedFillTypeProperty, value);
        }
    }
}