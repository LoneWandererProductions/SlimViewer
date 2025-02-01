using System.Windows;

namespace SlimControls
{
    public sealed partial class ToolOptions
    {
        /// <summary>
        ///     Dependency Property for Header Text
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(ToolOptions),
                new PropertyMetadata("Tool Options")); // Default header text


        /// <summary>
        ///     Dependency Property for Slider Caption
        /// </summary>
        public static readonly DependencyProperty SliderCaptionProperty =
            DependencyProperty.Register(
                nameof(SliderCaption),
                typeof(string),
                typeof(ToolOptions),
                new PropertyMetadata("Option Value:")); // Default slider label text

        /// <summary>
        ///     Dependency Property for Slider Value
        /// </summary>
        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register(
                nameof(SliderValue),
                typeof(int),
                typeof(ToolOptions),
                new PropertyMetadata(1)); // Default slider value

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolOptions" /> class.
        /// </summary>
        public ToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the header text.
        /// </summary>
        /// <value>
        ///     The header text.
        /// </value>
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        /// <summary>
        ///     Gets or sets the slider caption.
        /// </summary>
        /// <value>
        ///     The slider caption.
        /// </value>
        public string SliderCaption
        {
            get => (string)GetValue(SliderCaptionProperty);
            set => SetValue(SliderCaptionProperty, value);
        }

        /// <summary>
        ///     Gets or sets the slider value.
        /// </summary>
        /// <value>
        ///     The slider value.
        /// </value>
        public double SliderValue
        {
            get => (int)GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }
    }
}