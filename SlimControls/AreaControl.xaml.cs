/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     SlimControls
 * FILE:        AreaControl.xaml.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommonControls;
using ViewModel;

namespace SlimControls
{
    public sealed partial class AreaControl
    {
        /// <summary>
        ///     DependencyProperty for the selected tool type.
        /// </summary>
        public static readonly DependencyProperty SelectedToolTypeProperty =
            DependencyProperty.Register(
                nameof(SelectedToolType),
                typeof(string),
                typeof(AreaControl),
                new PropertyMetadata(default(string), OnSelectedToolTypeChanged));

        /// <summary>
        ///     RoutedEvent for ToolChanged (for XAML and WPF event routing support).
        /// </summary>
        public static readonly RoutedEvent ToolChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ToolChangedRouted),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AreaControl));

        /// <summary>
        ///     DependencyProperty for the fill type.
        /// </summary>
        public static readonly DependencyProperty FillTypeProperty =
            DependencyProperty.Register(
                nameof(FillType),
                typeof(string),
                typeof(AreaControl),
                new PropertyMetadata(string.Empty, OnFillTypeChanged));

        /// <summary>
        ///     RoutedEvent for FillType changes.
        /// </summary>
        public static readonly RoutedEvent FillTypeChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(FillTypeChangedRouted),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(AreaControl));

        /// <summary>
        ///     Initializes a new instance of the <see cref="AreaControl" /> class.
        /// </summary>
        public AreaControl()
        {
            InitializeComponent();
            InitializeCommands();
        }

        /// <summary>
        ///     Gets the texture configuration command.
        /// </summary>
        public ICommand TextureConfigCommand { get; set; }

        /// <summary>
        ///     Gets the filter configuration command.
        /// </summary>
        public ICommand FilterConfigCommand { get; set; }

        /// <summary>
        ///     Gets or sets the type of the fill.
        /// </summary>
        public string FillType
        {
            get => (string)GetValue(FillTypeProperty);
            set => SetValue(FillTypeProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected tool type.
        /// </summary>
        public string SelectedToolType
        {
            get => (string)GetValue(SelectedToolTypeProperty);
            set => SetValue(SelectedToolTypeProperty, value);
        }

        /// <summary>
        ///     CLR Event for tool selection (not RoutedEvent-related).
        /// </summary>
        public event EventHandler<ImageZoomTools> ToolChangedClr;

        /// <summary>
        ///     Occurs when the texture configuration command is executed.
        /// </summary>
        public event EventHandler<string> TextureConfigExecuted;

        /// <summary>
        ///     Occurs when the filter configuration command is executed.
        /// </summary>
        public event EventHandler<string> FilterConfigExecuted;

        /// <summary>
        ///     CLR Wrapper for the RoutedEvent.
        /// </summary>
        public event RoutedEventHandler ToolChangedRouted
        {
            add => AddHandler(ToolChangedEvent, value);
            remove => RemoveHandler(ToolChangedEvent, value);
        }

        /// <summary>
        ///     CLR Wrapper for the FillTypeChanged RoutedEvent.
        /// </summary>
        public event RoutedEventHandler FillTypeChangedRouted
        {
            add => AddHandler(FillTypeChangedEvent, value);
            remove => RemoveHandler(FillTypeChangedEvent, value);
        }

        /// <summary>
        ///     Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            TextureConfigCommand = new DelegateCommand<string>(ExecuteTextureConfigCommand);
            FilterConfigCommand = new DelegateCommand<string>(ExecuteFilterConfigCommand);
        }

        /// <summary>
        ///     Raises the RoutedEvent with the specified old and new values.
        /// </summary>
        private void RaiseRoutedEvent<T>(RoutedEvent routedEvent, T oldValue, T newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<T>(oldValue, newValue, routedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        ///     Raise both CLR and Routed events when the tool changes.
        /// </summary>
        private void NotifyToolSelection(ImageZoomTools selectedTool)
        {
            var oldTool = Translator.GetToolsFromString(SelectedToolType);
            ToolChangedClr?.Invoke(this, selectedTool);

            RaiseRoutedEvent(ToolChangedEvent, oldTool, selectedTool);
        }

        /// <summary>
        ///     Callback invoked when the SelectedToolType changes.
        /// </summary>
        private static void OnSelectedToolTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AreaControl control || e.NewValue is not string newToolString)
            {
                Debug.WriteLine("Unexpected object type or value in OnSelectedToolTypeChanged.");
                return;
            }

            var transformedTool = Translator.GetToolsFromString(newToolString);
            control.NotifyToolSelection(transformedTool);
        }

        /// <summary>
        ///     Callback invoked when the FillType changes.
        /// </summary>
        /// <param name="d">The source object where the property change occurred.</param>
        /// <param name="e">Details about the property change event.</param>
        private static void OnFillTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AreaControl control)
                return;

            // Avoid redundant processing when the old and new values are the same.
            if (Equals(e.OldValue, e.NewValue))
                return;

            // Raise a routed event with the old and new values.
            var oldFillType = e.OldValue as string;
            var newFillType = e.NewValue as string;

            Trace.WriteLine($"FillType changed from '{oldFillType}' to '{newFillType}'");

            // Raise the FillTypeChangedEvent with old and new values as part of the event args.
            control.RaiseRoutedEvent(FillTypeChangedEvent, oldFillType, newFillType);
        }

        /// <summary>
        ///     Executes the texture configuration command.
        /// </summary>
        private void ExecuteTextureConfigCommand(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                Trace.WriteLine("TextureConfigCommand executed with null or empty parameter.");
                return;
            }

            TextureConfigExecuted?.Invoke(this, parameter);
        }

        /// <summary>
        ///     Executes the filter configuration command.
        /// </summary>
        private void ExecuteFilterConfigCommand(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                Trace.WriteLine("FilterConfigCommand executed with null or empty parameter.");
                return;
            }

            FilterConfigExecuted?.Invoke(this, parameter);
        }
    }
}