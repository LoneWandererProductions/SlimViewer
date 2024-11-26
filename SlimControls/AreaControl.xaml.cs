using System.Windows;
using System.Windows.Controls;

namespace SlimControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AreaControl : UserControl
    {
        public static readonly DependencyProperty SelectedToolTypeProperty =
            DependencyProperty.Register("SelectedToolType", typeof(string), typeof(AreaControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SelectedFillTypeProperty =
            DependencyProperty.Register("SelectedFillType", typeof(string), typeof(AreaControl), new PropertyMetadata(default(string)));

        public string SelectedToolType
        {
            get => (string)GetValue(SelectedToolTypeProperty);
            set => SetValue(SelectedToolTypeProperty, value);
        }

        public string SelectedFillType
        {
            get => (string)GetValue(SelectedFillTypeProperty);
            set => SetValue(SelectedFillTypeProperty, value);
        }

        public AreaControl()
        {
            InitializeComponent();
        }
    }
}