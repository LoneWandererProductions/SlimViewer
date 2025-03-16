using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using CommonControls;

// ReSharper disable MissingSpace

namespace SlimControls
{
    public sealed partial class UnifiedToolOptions
    {
        /// <summary>
        /// DependencyProperty for ToolChangedCommand.
        /// </summary>
        public static readonly DependencyProperty ToolChangedCommandProperty =
            DependencyProperty.Register(
                nameof(ToolChangedCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for SelectedTool.
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(
                nameof(SelectedTool),
                typeof(ImageTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Move, OnSelectedToolChanged)); // Add callback

        /// <summary>
        /// DependencyProperty for BrushSize.
        /// </summary>
        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(
                nameof(BrushSize),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// DependencyProperty for EraseRadius.
        /// </summary>
        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(
                nameof(EraseRadius),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// DependencyProperty for ColorTolerance.
        /// </summary>
        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(
                nameof(ColorTolerance),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        /// Routed event for external subscribers to know when a tool becomes visible.
        /// </summary>
        public static readonly RoutedEvent SelectedToolChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectedToolChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<ImageTools>),
                typeof(UnifiedToolOptions));

        // DependencyProperty for FilterCommand.
        public static readonly DependencyProperty FilterCommandProperty =
            DependencyProperty.Register(
                nameof(FilterCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        // DependencyProperty for TextureCommand.
        public static readonly DependencyProperty TextureCommandProperty =
            DependencyProperty.Register(
                nameof(TextureCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for FillTypeChangedCommand.
        /// </summary>
        public static readonly DependencyProperty FillTypeChangedCommandProperty =
            DependencyProperty.Register(
                nameof(FillTypeChangedCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for SelectedToolCode.
        /// </summary>
        public static readonly DependencyProperty SelectedToolCodeProperty =
            DependencyProperty.Register(
                nameof(SelectedToolCode),
                typeof(EnumTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(EnumTools.Move, OnSelectedToolCodeChanged));

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedToolOptions" /> class.
        /// </summary>
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        public ICommand ToolChangedCommand
        {
            get => (ICommand)GetValue(ToolChangedCommandProperty);
            set => SetValue(ToolChangedCommandProperty, value);
        }

        public ICommand FilterCommand
        {
            get => (ICommand)GetValue(FilterCommandProperty);
            set => SetValue(FilterCommandProperty, value);
        }

        public ICommand TextureCommand
        {
            get => (ICommand)GetValue(TextureCommandProperty);
            set => SetValue(TextureCommandProperty, value);
        }

        public ICommand FillTypeChangedCommand
        {
            get => (ICommand)GetValue(FillTypeChangedCommandProperty);
            set => SetValue(FillTypeChangedCommandProperty, value);
        }

        public ImageTools SelectedTool
        {
            get => (ImageTools)GetValue(SelectedToolProperty);
            set => SetValue(SelectedToolProperty, value);
        }

        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        public EnumTools SelectedToolCode
        {
            get => (EnumTools)GetValue(SelectedToolCodeProperty);
            set => SetValue(SelectedToolCodeProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<ImageTools> SelectedToolChanged
        {
            add => AddHandler(SelectedToolChangedEvent, value);
            remove => RemoveHandler(SelectedToolChangedEvent, value);
        }

        private void AreaControl_ToolChangedRouted(object sender, RoutedEventArgs e)
        {
            if (e is not RoutedPropertyChangedEventArgs<ImageZoomTools> toolArgs) return;

            Trace.WriteLine($"Tool changed from {toolArgs.OldValue} to {toolArgs.NewValue}");

            NotifyToolSelection(toolArgs.NewValue);
        }

        private void AreaControl_FillTypeChangedRouted(object sender, RoutedEventArgs e)
        {
            if (e is not RoutedPropertyChangedEventArgs<string> fillArgs) return;

            Trace.WriteLine($"Fill type changed from '{fillArgs.OldValue}' to '{fillArgs.NewValue}'");


            SelectedToolCode = Translator.GetFillTool(fillArgs.NewValue);

            FillTypeChangedCommand?.Execute(fillArgs.NewValue);
        }

        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            ToolChangedCommand?.Execute(selectedTool);
        }

        private void AreaControl_FilterConfigExecuted(object sender, string e)
        {
            Trace.WriteLine($"Filter configuration executed: {e}");

            SelectedToolCode = EnumTools.Filter;
            FilterCommand?.Execute(Translator.GetFilterFromString(e));
        }

        private void AreaControl_TextureConfigExecuted(object sender, string e)
        {
            Trace.WriteLine($"Texture configuration executed: {e}");

            SelectedToolCode = EnumTools.Texture;
            TextureCommand?.Execute(Translator.GetTextureFromString(e));
        }

        private static void OnSelectedToolCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UnifiedToolOptions control) return;

            if (e.NewValue is not EnumTools newCode) return;

            // ✅ Ensure SelectedTool updates when SelectedToolCode changes
            control.SetCurrentValue(SelectedToolProperty, Translator.MapEnumToolsToTool(newCode));
        }

        private static void OnSelectedToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UnifiedToolOptions control)
            {
                Trace.WriteLine($"Error: Expected d to be UnifiedToolOptions, but got {d?.GetType().Name ?? "null"}");
                return;
            }

            if (e.NewValue is not ImageTools newTool)
            {
                Trace.WriteLine($"Error: Expected e.NewValue to be ImageTools, but got {e.NewValue?.GetType().Name ?? "null"}");
                return;
            }

            var args = new RoutedPropertyChangedEventArgs<ImageTools>(
                (ImageTools)e.OldValue,
                newTool,
                SelectedToolChangedEvent);

            control.RaiseEvent(args);

            // SetCurrentValue ensures WPF detects the update and updates the binding
            control.SetCurrentValue(SelectedToolCodeProperty, Translator.MapToolToEnumTools(newTool));
        }
    }
}
