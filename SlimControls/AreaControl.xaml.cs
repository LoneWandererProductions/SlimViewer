using System;
using System.Windows;
using CommonControls;

namespace SlimControls
{
    public sealed partial class AreaControl
    {
        /// <summary>
        /// CLR Event for tool selection (not RoutedEvent-related).
        /// </summary>
        public event EventHandler<ImageZoomTools>? ToolChangedClr;

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
            // Get the current tool (old value) from the SelectedToolType property
            var oldTool = Translator.GetToolsFromString(SelectedToolType);

            // Raise CLR event
            ToolChangedClr?.Invoke(this, selectedTool);

            // Raise RoutedEvent with property changed args
            var args = new RoutedPropertyChangedEventArgs<ImageZoomTools>(
                oldTool, // Old tool value
                selectedTool, // New tool value
                ToolChangedEvent); // The routed event

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