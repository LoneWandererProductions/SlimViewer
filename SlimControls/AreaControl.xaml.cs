using System;
using System.Windows;
using System.Windows.Controls;
using CommonControls;

namespace SlimControls
{
    public sealed partial class AreaControl : UserControl
    {
        /// <summary>
        /// CLR Event for tool selection (not RoutedEvent-related).
        /// </summary>
        public event EventHandler<ImageZoomTools>? ToolChangedCLR;

        /// <summary>
        /// DependencyProperty for the selected tool type
        /// </summary>
        public static readonly DependencyProperty SelectedToolTypeProperty =
            DependencyProperty.Register(
                nameof(SelectedToolType),
                typeof(string),
                typeof(AreaControl),
                new PropertyMetadata(default(string), OnSelectedToolTypeChanged));

        /// <summary>
        /// RoutedEvent for ToolChanged (for XAML and WPF event routing support).
        /// </summary>
        public static readonly RoutedEvent ToolChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ToolChangedRouted),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AreaControl));

        /// <summary>
        /// CLR Wrapper for the RoutedEvent.
        /// </summary>
        public event RoutedEventHandler ToolChangedRouted
        {
            add => AddHandler(ToolChangedEvent, value);
            remove => RemoveHandler(ToolChangedEvent, value);
        }

        /// <summary>
        /// Raise both CLR and Routed events when the tool changes.
        /// </summary>
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            // Raise CLR event
            ToolChangedCLR?.Invoke(this, selectedTool);

            // Raise RoutedEvent
            var args = new RoutedEventArgs(ToolChangedEvent)
            {
                Source = this
            };
            RaiseEvent(args);
        }

        /// <summary>
        /// Callback invoked when the SelectedToolType changes.
        /// </summary>
        private static void OnSelectedToolTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AreaControl control && e.NewValue is string newToolString)
            {
                // Transform the string to the ImageZoomTools enum
                var transformedTool = Translator.GetToolsFromString(newToolString);

                // Notify listeners about the new tool
                control.NotifyToolSelection(transformedTool);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaControl"/> class.
        /// </summary>
        public AreaControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the selected tool type.
        /// </summary>
        public string SelectedToolType
        {
            get => (string)GetValue(SelectedToolTypeProperty);
            set => SetValue(SelectedToolTypeProperty, value);
        }
    }
}
