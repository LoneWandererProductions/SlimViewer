using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommonControls;

// ReSharper disable MissingSpace

namespace SlimControls
{
    public sealed partial class UnifiedToolOptions
    {
        /// <summary>
        ///     DependencyProperty for ToolChangedCommand.
        /// </summary>
        public static readonly DependencyProperty ToolChangedCommandProperty =
            DependencyProperty.Register(
                nameof(ToolChangedCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <summary>
        ///     DependencyProperty for SelectedTool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty =
            DependencyProperty.Register(
                nameof(SelectedTool),
                typeof(ImageTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(ImageTools.Area, OnSelectedToolChanged));


        /// <summary>
        ///     DependencyProperty for BrushSize
        /// </summary>
        public static readonly DependencyProperty BrushSizeProperty =
            DependencyProperty.Register(
                nameof(BrushSize),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        ///     DependencyProperty for EraseRadius
        /// </summary>
        public static readonly DependencyProperty EraseRadiusProperty =
            DependencyProperty.Register(
                nameof(EraseRadius),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        ///     DependencyProperty for ColorTolerance
        /// </summary>
        public static readonly DependencyProperty ColorToleranceProperty =
            DependencyProperty.Register(
                nameof(ColorTolerance),
                typeof(double),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(1.0));

        /// <summary>
        ///     Routed event for external subscribers to know when a tool becomes visible
        ///     The selected tool changed event
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
        ///     DependencyProperty for FillTypeChangedCommand.
        /// </summary>
        public static readonly DependencyProperty FillTypeChangedCommandProperty =
            DependencyProperty.Register(
                nameof(FillTypeChangedCommand),
                typeof(ICommand),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(null));

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnifiedToolOptions" /> class.
        /// </summary>
        public UnifiedToolOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the ToolChangedCommand.
        /// </summary>
        public ICommand ToolChangedCommand
        {
            get => (ICommand)GetValue(ToolChangedCommandProperty);
            set => SetValue(ToolChangedCommandProperty, value);
        }

        // ICommand property for FilterCommand
        public ICommand FilterCommand
        {
            get => (ICommand)GetValue(FilterCommandProperty);
            set => SetValue(FilterCommandProperty, value);
        }

        // ICommand property for TextureCommand
        public ICommand TextureCommand
        {
            get => (ICommand)GetValue(TextureCommandProperty);
            set => SetValue(TextureCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the FillTypeChangedCommand.
        /// </summary>
        public ICommand FillTypeChangedCommand
        {
            get => (ICommand)GetValue(FillTypeChangedCommandProperty);
            set => SetValue(FillTypeChangedCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected tool.
        /// </summary>
        public ImageTools SelectedTool
        {
            get => (ImageTools)GetValue(SelectedToolProperty);
            set => SetValue(SelectedToolProperty, value);
        }

        /// <summary>
        ///     Gets or sets the size of the brush.
        /// </summary>
        public double BrushSize
        {
            get => (double)GetValue(BrushSizeProperty);
            set => SetValue(BrushSizeProperty, value);
        }

        /// <summary>
        ///     Gets or sets the erase radius.
        /// </summary>
        public double EraseRadius
        {
            get => (double)GetValue(EraseRadiusProperty);
            set => SetValue(EraseRadiusProperty, value);
        }

        /// <summary>
        ///     Gets or sets the color tolerance.
        /// </summary>
        public double ColorTolerance
        {
            get => (double)GetValue(ColorToleranceProperty);
            set => SetValue(ColorToleranceProperty, value);
        }

        /// <summary>
        ///     Occurs when [selected tool changed].
        ///     Event to notify external subscribers when a tool is selected
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ImageTools> SelectedToolChanged
        {
            add => AddHandler(SelectedToolChangedEvent, value);
            remove => RemoveHandler(SelectedToolChangedEvent, value);
        }

        /// <summary>
        ///     Handles the ToolChangedRouted event of the AreaControl control.
        /// </summary>
        private void AreaControl_ToolChangedRouted(object sender, RoutedEventArgs e)
        {
            if (e is not RoutedPropertyChangedEventArgs<ImageZoomTools> toolArgs) return;

            var oldTool = toolArgs.OldValue;
            var newTool = toolArgs.NewValue;

            Trace.WriteLine($"Tool changed from {oldTool} to {newTool}");

            // Notify via the legacy event for compatibility
            NotifyToolSelection(newTool);
        }

        /// <summary>
        ///     Handles the FillTypeChangedRouted event of the AreaControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AreaControl_FillTypeChangedRouted(object sender, RoutedEventArgs e)
        {
            if (e is not RoutedPropertyChangedEventArgs<string> fillArgs) return;

            // Retrieve the old and new FillType values from the event args
            var oldFillType = fillArgs.OldValue;
            var newFillType = fillArgs.NewValue;

            Trace.WriteLine($"Fill type configuration changed from '{oldFillType}' to '{newFillType}'");

            //set code
            SelectedToolCode = EnumTools.SolidColor;

            // Execute the command if the FillTypeChangedCommand is available
            if (FillTypeChangedCommand?.CanExecute(newFillType) == true)
                FillTypeChangedCommand.Execute(newFillType); // Execute the command with the new fill type
        }

        /// <summary>
        ///     Notifies the tool selection using ImageZoomTools.
        /// </summary>
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            // Execute the bound command if available
            if (ToolChangedCommand?.CanExecute(selectedTool) == true) ToolChangedCommand.Execute(selectedTool);
        }

        /// <summary>
        ///     Areas the control filter configuration executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AreaControl_FilterConfigExecuted(object sender, string e)
        {
            Trace.WriteLine($"Filter configuration executed: {e}");

            var filter = Translator.GetFilterFromString(e);

            //set code
            SelectedToolCode = EnumTools.Filter;

            // Execute the bound command if available
            if (FilterCommand?.CanExecute(filter) == true) FilterCommand.Execute(filter);
        }

        /// <summary>
        ///     Areas the control texture configuration executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AreaControl_TextureConfigExecuted(object sender, string e)
        {
            Trace.WriteLine($"Texture configuration executed: {e}");

            var texture = Translator.GetTextureFromString(e);

            //set code
            SelectedToolCode = EnumTools.Texture;

            // Execute the bound command if available
            if (TextureCommand?.CanExecute(texture) == true) TextureCommand.Execute(texture);
        }

        /// <summary>
        /// DependencyProperty for SelectedToolCode
        /// </summary>
        public static readonly DependencyProperty SelectedToolCodeProperty =
            DependencyProperty.Register(
                nameof(SelectedToolCode),
                typeof(EnumTools),
                typeof(UnifiedToolOptions),
                new PropertyMetadata(EnumTools.Move, OnSelectedToolCodeChanged));

        /// <summary>
        /// Gets or sets the SelectedToolCode as an integer.
        /// </summary>
        public EnumTools SelectedToolCode
        {
            get => (EnumTools)GetValue(SelectedToolCodeProperty);
            set => SetValue(SelectedToolCodeProperty, value);
        }

        /// <summary>
        /// Called when the SelectedToolCode property changes.
        /// </summary>
        private static void OnSelectedToolCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UnifiedToolOptions control) return;

            EnumTools newCode = (EnumTools)e.NewValue;
            control.SelectedTool = Translator.MapCodeToTool(newCode);
        }

        /// <summary>
        /// Called when the SelectedTool property changes.
        /// </summary>
        private static void OnSelectedToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UnifiedToolOptions control || e.NewValue is not ImageTools newTool) return;

            var args = new RoutedPropertyChangedEventArgs<ImageTools>(
                (ImageTools)e.OldValue,
                newTool,
                SelectedToolChangedEvent);

            control.RaiseEvent(args);

            // Update SelectedToolCode
            control.SelectedToolCode = Translator.MapToolToEnumTools(newTool);
        }
    }
}
