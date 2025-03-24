﻿/*
* COPYRIGHT:   See COPYING in the top level directory
* PROJECT:     CommonControls
* FILE:        CommonControls/ImageZoom.xaml.cs
* PURPOSE:     Image View Control, that can handle some tools
* PROGRAMER:   Peter Geinitz (Wayfarer)
*/

// ReSharper disable EventNeverSubscribedTo.Global, only used outside of the dll
// ReSharper disable MemberCanBeInternal, must be visible, if we want to use it outside of the dll
// ReSharper disable UnusedType.Global
// ReSharper disable MissingSpace

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonControls
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     ImageZoom Image
    /// </summary>
    public sealed partial class ImageZoom : IDisposable
    {
        /// <summary>
        ///     Delegate for Image Frame
        /// </summary>
        /// <param name="frame">The frame.</param>
        public delegate void DelegateFrame(SelectionFrame frame);

        /// <summary>
        ///     Delegate for Image Point
        /// </summary>
        /// <param name="point">The point.</param>
        public delegate void DelegatePoint(Point point);

        /// <summary>
        ///     The image source property
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
            typeof(BitmapImage),
            typeof(ImageZoom), new PropertyMetadata(OnImageSourcePropertyChanged));

        /// <summary>
        ///     The gif image source property
        /// </summary>
        public static readonly DependencyProperty ImageGifSourceProperty = DependencyProperty.Register(
            nameof(ImageGifPath),
            typeof(string),
            typeof(ImageZoom), new PropertyMetadata(OnImageGifSourcePropertyChanged));

        /// <summary>
        ///     The tools
        /// </summary>
        public static readonly DependencyProperty SelectionToolProperty =
            DependencyProperty.Register(
                nameof(SelectionTool),
                typeof(ImageZoomTools),
                typeof(ImageZoom),
                new PropertyMetadata(OnSelectionToolChanged));

        /// <summary>
        ///     The autoplay gif Property
        /// </summary>
        public static readonly DependencyProperty AutoplayGif = DependencyProperty.Register(nameof(AutoplayGifImage),
            typeof(bool),
            typeof(ImageZoom), null);

        /// <summary>
        ///     The zoom scale property
        /// </summary>
        public static readonly DependencyProperty ZoomScaleProperty = DependencyProperty.Register(
            nameof(ZoomScale),
            typeof(double),
            typeof(ImageZoom),
            new PropertyMetadata(1.0, OnZoomScaleChanged));

        /// <summary>
        ///     The image clicked command property
        /// </summary>
        public static readonly DependencyProperty SelectedPointCommandProperty = DependencyProperty.Register(
            nameof(SelectedPointCommand), typeof(ICommand), typeof(ImageZoom), new PropertyMetadata(null));


        /// <summary>
        ///     The selected frame property
        /// </summary>
        public static readonly DependencyProperty SelectedFrameCommandProperty =
            DependencyProperty.Register(nameof(SelectedFrameCommand), typeof(ICommand), typeof(ImageZoom),
                new PropertyMetadata(null));

        /// <summary>
        ///     The selected free form points command property
        /// </summary>
        public static readonly DependencyProperty SelectedFreeFormPointsCommandProperty =
            DependencyProperty.Register(nameof(SelectedFreeFormPointsCommand), typeof(ICommand), typeof(ImageZoom),
                new PropertyMetadata(null));


        /// <summary>
        ///     The lock
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        ///     The disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     The mouse down
        ///     Set to 'true' when mouse is held down.
        /// </summary>
        private bool _mouseDown;

        /// <summary>
        ///     The origin Point.
        /// </summary>
        private Point _originPoint;

        /// <summary>
        ///     The mouse down position
        /// </summary>
        private Point _startPoint;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window" /> class.
        /// </summary>
        public ImageZoom()
        {
            InitializeComponent();
            if (BtmImage.Source == null) return;

            MainCanvas.Height = BtmImage.Source.Height;
            MainCanvas.Width = BtmImage.Source.Width;
        }

        /// <summary>
        ///     The selection adorner
        /// </summary>
        private SelectionAdorner SelectionAdorner { get; set; }

        /// <summary>
        ///     Gets or sets the image clicked command.
        /// </summary>
        /// <value>
        ///     The image clicked command.
        /// </value>
        public ICommand SelectedPointCommand
        {
            get => (ICommand)GetValue(SelectedPointCommandProperty);
            set => SetValue(SelectedPointCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected frame.
        /// </summary>
        /// <value>
        ///     The selected frame.
        /// </value>
        public ICommand SelectedFrameCommand
        {
            get => (ICommand)GetValue(SelectedFrameCommandProperty);
            set => SetValue(SelectedFrameCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected free form points command.
        /// </summary>
        /// <value>
        ///     The selected free form points command.
        /// </value>
        public ICommand SelectedFreeFormPointsCommand
        {
            get => (ICommand)GetValue(SelectedFreeFormPointsCommandProperty);
            set => SetValue(SelectedFreeFormPointsCommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets the zoom scale.
        /// </summary>
        public double ZoomScale
        {
            get => (double)GetValue(ZoomScaleProperty);
            set => SetValue(ZoomScaleProperty, value);
        }

        /// <summary>
        ///     Gets or sets the items source.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public BitmapImage ItemsSource
        {
            get => (BitmapImage)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        ///     Gets or sets the gif image  path.
        /// </summary>
        /// <value>
        ///     The gif image path.
        /// </value>
        public string ImageGifPath
        {
            get => (string)GetValue(ImageGifSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        ///     Gets or sets the zoom.
        /// </summary>
        /// <value>
        ///     The zoom.
        /// </value>
        public ImageZoomTools SelectionTool
        {
            get => (ImageZoomTools)GetValue(SelectionToolProperty);
            set => SetValue(SelectionToolProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [autoplay GIF image].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [autoplay GIF image]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoplayGifImage
        {
            get => (bool)GetValue(AutoplayGif);
            set => SetValue(AutoplayGif, value);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Implementation of IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent finalizer from running.
        }

        /// <summary>
        ///     Called when the ZoomScale property changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnZoomScaleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ImageZoom;
            control?.UpdateZoomScale((double)e.NewValue);
        }

        /// <summary>
        ///     Occurs when [selected frame] was changed
        /// </summary>
        public event DelegateFrame SelectedFrame;

        /// <summary>
        ///     Occurs when [selected point].
        /// </summary>
        public event DelegatePoint SelectedPoint;

        /// <summary>
        ///     Called when [image source property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnImageSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ImageZoom;
            control?.OnImageSourceChanged();
        }

        /// <summary>
        ///     Called when [image GIF source property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnImageGifSourcePropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ImageZoom;
            control?.OnImageSourceGifChanged();
        }

        /// <summary>
        ///     Called when [selection tool changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void OnSelectionToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageZoom;
            if (control == null) return; // Ensure that we are working with an ImageZoom instance

            var newTool = (ImageZoomTools)e.NewValue;

            // Detach the previous adorner if needed
            if (control.SelectionAdorner == null) return;

            control.SelectionAdorner.Tool = newTool; // Update the tool in the adorner
            control.SelectionAdorner.ClearFreeFormPoints(); // Reset any existing free-form points if applicable
        }

        /// <summary>
        ///     Called when [image source GIF changed].
        /// </summary>
        private void OnImageSourceGifChanged()
        {
            if (!File.Exists(ImageGifPath))
            {
                BtmImage.GifSource = null;
                BtmImage.Source = null;
                return;
            }

            //reset position
            var matrix = BtmImage.RenderTransform.Value;
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            BtmImage.RenderTransform = new MatrixTransform(matrix);

            //reset Scrollbar
            ScrollView.ScrollToTop();
            ScrollView.UpdateLayout();

            // Set GifSource and subscribe to the ImageLoaded event
            BtmImage.ImageLoaded += BtmImage_ImageLoaded;
            BtmImage.GifSource = ImageGifPath;

            // Ensure the adorner updates with the new zoom scale
            SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);
        }

        /// <summary>
        ///     Handles the ImageLoaded event of the BtmImage control.
        ///     Event handler for when the GIF has finished loading
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void BtmImage_ImageLoaded(object sender, EventArgs e)
        {
            // Unsubscribe to prevent memory leaks
            BtmImage.ImageLoaded -= BtmImage_ImageLoaded;

            // Now the source is fully loaded, you can safely access it
            MainCanvas.Height = BtmImage.Source.Height;
            MainCanvas.Width = BtmImage.Source.Width;

            // Update the adorner with the new image transform
            SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);

            // Reattach adorner for the new image (ensures correct behavior)
            AttachAdorner(SelectionTool);

            // Ensure the adorner updates with the new zoom scale
            SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);
        }

        /// <summary>
        ///     Called when [image source changed].
        ///     TODO add an option to not reset anything
        /// </summary>
        private void OnImageSourceChanged()
        {
            BtmImage.StopAnimation();
            BtmImage.Source = ItemsSource;

            if (BtmImage.Source == null) return;

            //reset Scaling
            Scale.ScaleX = 1;
            Scale.ScaleY = 1;

            //reset position
            var matrix = BtmImage.RenderTransform.Value;
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            BtmImage.RenderTransform = new MatrixTransform(matrix);

            //reset Scrollbar
            ScrollView.ScrollToTop();
            ScrollView.UpdateLayout();

            MainCanvas.Height = BtmImage.Source.Height;
            MainCanvas.Width = BtmImage.Source.Width;

            // Update the adorner with the new image transform
            SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);

            // Reattach adorner for new image (this ensures correct behavior for the new image)
            AttachAdorner(SelectionTool);
        }

        /// <summary>
        ///     Attaches the adorner.
        /// </summary>
        /// <param name="tool">The tool.</param>
        private void AttachAdorner(ImageZoomTools tool)
        {
            if (SelectionAdorner == null)
            {
                // Get the adorner layer for the BtmImage instead of the MainCanvas
                var adornerLayer = AdornerLayer.GetAdornerLayer(BtmImage);
                SelectionAdorner = new SelectionAdorner(BtmImage, tool);
                adornerLayer?.Add(SelectionAdorner);
            }
            else
            {
                // Clear points and reset for the new selection tool
                SelectionAdorner?.ClearFreeFormPoints();
                SelectionAdorner.Tool = tool; // Update the tool if necessary
            }
        }

        /// <summary>
        ///     Handles the MouseDown event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Capture and track the mouse.
            _mouseDown = true;

            // Get the mouse position relative to the canvas
            var rawPoint = e.GetPosition(MainCanvas);

            //TODO problem with our DPI and multiple Monitor Setup
            _startPoint = rawPoint;

            // Capture the mouse
            _ = MainCanvas.CaptureMouse();

            AttachAdorner(SelectionTool);

            switch (SelectionTool)
            {
                case ImageZoomTools.Move:
                    break;
                case ImageZoomTools.Trace:
                    SelectionAdorner.IsTracing = true;
                    break;
                case ImageZoomTools.Rectangle:
                case ImageZoomTools.Ellipse:
                case ImageZoomTools.FreeForm:
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        ///     Handles the MouseUp event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Release the mouse capture and stop tracking it.
            _mouseDown = false;
            MainCanvas.ReleaseMouseCapture();

            if (SelectionAdorner == null)
            {
                Trace.Write(ComCtlResources.InformationArdonerNull);
                return;
            }

            switch (SelectionTool)
            {
                case ImageZoomTools.Move:
                    // No specific action required for Move
                    break;

                case ImageZoomTools.Rectangle:
                case ImageZoomTools.Ellipse:

                    var frame = SelectionAdorner.CurrentSelectionFrame;
                    SelectedFrame?.Invoke(frame);
                    SelectedFrameCommand.Execute(frame);
                    break;
                case ImageZoomTools.FreeForm:
                    // Get the mouse position relative to the canvas
                    var rawPoint = e.GetPosition(MainCanvas);

                    //TODO problem with our DPI and multiple Monitor Setup

                    SelectionAdorner.FreeFormPoints.Add(rawPoint);
                    break;

                case ImageZoomTools.Trace:
                    SelectionAdorner.IsTracing = false;

                    // Implement logic for FreeFormPoints
                    var points = SelectionAdorner.FreeFormPoints;

                    if (points is { Count: > 0 })
                    {
                        // Process the collected freeform points
                        if (SelectedFreeFormPointsCommand?.CanExecute(points) == true)
                            SelectedFreeFormPointsCommand.Execute(points);

                        // Optionally, log or display the points
                        Trace.WriteLine($"Trace tool completed with {points.Count} points.");
                    }

                    break;

                case ImageZoomTools.Dot:
                    SetClickedPoint(e);

                    var endpoint = e.GetPosition(BtmImage);
                    SelectedPoint?.Invoke(endpoint);
                    break;
                default:
                    // Do nothing for unsupported tools
                    return;
            }

            if (SelectionAdorner == null) return;

            // Get the AdornerLayer for the image
            var adornerLayer = AdornerLayer.GetAdornerLayer(BtmImage);

            if (adornerLayer != null)
            {
                // Remove the SelectionAdorner
                adornerLayer.Remove(SelectionAdorner);
                SelectionAdorner = null; // Clear the reference
            }
        }

        /// <summary>
        ///     Handles the MouseMove event of the Canvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;

            // Get the mouse position relative to the image instead of the canvas
            var mousePos = e.GetPosition(BtmImage);

            switch (SelectionTool)
            {
                case ImageZoomTools.Move:
                {
                    var position = e.GetPosition(MainCanvas);
                    var matrix = BtmImage.RenderTransform.Value;
                    matrix.OffsetX = _originPoint.X + (position.X - _startPoint.X);
                    matrix.OffsetY = _originPoint.Y + (position.Y - _startPoint.Y);
                    BtmImage.RenderTransform = new MatrixTransform(matrix);

                    SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);
                    break;
                }

                case ImageZoomTools.Rectangle:
                case ImageZoomTools.Ellipse:
                {
                    // Update the adorner for rectangle or ellipse selection
                    SelectionAdorner?.UpdateSelection(_startPoint, mousePos);

                    break;
                }

                case ImageZoomTools.FreeForm:
                {
                    // Update the adorner for free form selection by adding points
                    SelectionAdorner?.AddFreeFormPoint(mousePos);

                    break;
                }

                case ImageZoomTools.Trace:
                    // Handle pixel selection if needed
                    break;
                case ImageZoomTools.Dot:
                    break;
                default:
                    // Nothing
                    return;
            }
        }

        /// <summary>
        ///     Handles the MouseWheel event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs" /> instance containing the event data.</param>
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            lock (_lock)
            {
                var zoomFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;
                var newZoomScale = Scale.ScaleX * zoomFactor; // Assume uniform scaling, so use ScaleX

                UpdateZoomScale(newZoomScale); // Centralize logic for updating the zoom scale

                // Ensure the adorner updates with the new zoom scale
                SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);
            }
        }

        /// <summary>
        /// Handles the MouseRightButtonUp event of the Canvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectionTool == ImageZoomTools.FreeForm)
            {
                CompleteFreeFormSelection();
            }
        }

        /// <summary>
        /// Completes the free form selection.
        /// </summary>
        private void CompleteFreeFormSelection()
        {
            var frame = SelectionAdorner.CurrentSelectionFrame;
            SelectedFrame?.Invoke(frame); // Notify listeners that selection is done

            if (SelectedFrameCommand?.CanExecute(frame) == true)
            {
                SelectedFrameCommand.Execute(frame); // Execute any bound command
            }

            SelectionAdorner.FreeFormPoints.Clear(); // Reset collected points for the next freeform drawing
        }

        /// <summary>
        ///     Updates the zoom scale for both ScaleX and ScaleY.
        /// </summary>
        /// <param name="zoomScale">The new zoom scale.</param>
        private void UpdateZoomScale(double zoomScale)
        {
            Scale.ScaleX = zoomScale;
            Scale.ScaleY = zoomScale;

            // Ensure the adorner updates with the new zoom scale
            SelectionAdorner?.UpdateImageTransform(BtmImage.RenderTransform);
        }

        /// <summary>
        ///     Sets the clicked point.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void SetClickedPoint(MouseEventArgs e)
        {
            var endpoint = e.GetPosition(BtmImage);
            SelectedPoint?.Invoke(endpoint);
            SelectedPointCommand.Execute(endpoint);
        }


        /// <summary>
        ///     Clean up managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether the method was called by Dispose or the finalizer.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return; // Early exit if already disposed

            lock (_lock) // Ensure thread-safety
            {
                if (_disposed) return; // Double-check in case Dispose was called by another thread

                if (disposing)
                {
                    // Managed resource cleanup

                    // Unsubscribe event handlers
                    if (SelectedFrame != null)
                        foreach (var d in SelectedFrame.GetInvocationList())
                            SelectedFrame -= (DelegateFrame)d;

                    if (SelectedPoint != null)
                        foreach (var d in SelectedPoint.GetInvocationList())
                            SelectedPoint -= (DelegatePoint)d;

                    // Dispose image resources
                    BtmImage?.StopAnimation();
                    if (BtmImage != null)
                    {
                        BtmImage.Source = null;
                        BtmImage.GifSource = null;

                        // Remove adorner
                        var adornerLayer = AdornerLayer.GetAdornerLayer(BtmImage);
                        adornerLayer?.Remove(SelectionAdorner);
                    }

                    SelectionAdorner = null;

                    if (BtmImage != null) BtmImage.ImageLoaded -= BtmImage_ImageLoaded;

                    // Release UI interaction resources
                    MainCanvas.ReleaseMouseCapture();
                }

                // If there are unmanaged resources, clean them here

                _disposed = true; // Mark as disposed
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="ImageZoom" /> class.
        /// </summary>
        ~ImageZoom()
        {
            Dispose(false); // Finalizer calls Dispose(false)
        }
    }
}