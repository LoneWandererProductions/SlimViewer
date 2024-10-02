/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     CommonControls
 * FILE:        CommonControls/SelectionAdorner.cs
 * PURPOSE:     Extensions for ImageZoom, handle all the selection Tools.
 * PROGRAMER:   Peter Geinitz (Wayfarer)
 */

using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;

namespace CommonControls
{
    /// <summary>
    /// Adorner for ImageZoom
    /// </summary>
    /// <seealso cref="System.Windows.Documents.Adorner" />
    internal sealed class SelectionAdorner : Adorner
    {
        /// <summary>
        /// The start point
        /// </summary>
        private Point? startPoint;

        /// <summary>
        /// The end point
        /// </summary>
        private Point? endPoint;

        /// <summary>
        /// The freeform points
        /// </summary>
        private List<Point> freeformPoints = new List<Point>();

        /// <summary>
        /// The image transform
        /// Store the transform applied to the image
        /// </summary>
        private Transform imageTransform;

        /// <summary>
        /// Gets or sets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        public SelectionTools Tool { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="tool">The tool.</param>
        /// <param name="transform">The transform.</param>
        public SelectionAdorner(UIElement adornedElement, SelectionTools tool, Transform transform = null)
            : base(adornedElement)
        {
            Tool = tool;
            imageTransform = transform ?? Transform.Identity;  // Use the provided transform, or default to Identity if none provided
        }

        /// <summary>
        /// Updates the selection for rectangle and ellipse tools.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public void UpdateSelection(Point start, Point end)
        {
            // Apply transformation to the start and end points if necessary
            startPoint = imageTransform.Transform(start);
            endPoint = imageTransform.Transform(end);

            InvalidateVisual();
        }

        /// <summary>
        /// Adds a point for the freeform tool.
        /// </summary>
        /// <param name="point">The freeform point.</param>
        public void AddFreeformPoint(Point point)
        {
            freeformPoints.Add(imageTransform.Transform(point));
            InvalidateVisual();
        }

        /// <summary>
        /// Clears the freeform points.
        /// </summary>
        public void ClearFreeformPoints()
        {
            freeformPoints.Clear();
            InvalidateVisual();
        }

        /// <summary>
        /// Updates the image transform when the image is resized or cropped.
        /// </summary>
        /// <param name="transform">The new transform to apply.</param>
        public void UpdateImageTransform(Transform transform)
        {
            imageTransform = transform ?? Transform.Identity;  // Use the provided transform, or default to Identity if none provided
            InvalidateVisual();
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = new Pen(Brushes.Red, 2);

            // Only render if at least one of the start or end points is available
            if (startPoint.HasValue && endPoint.HasValue)
            {
                Rect selectionRect = new(startPoint.Value, endPoint.Value);

                switch (Tool)
                {
                    case SelectionTools.SelectRectangle:
                        drawingContext.DrawRectangle(null, pen, selectionRect);
                        break;

                    case SelectionTools.SelectEllipse:
                        drawingContext.DrawEllipse(null, pen, selectionRect.TopLeft, selectionRect.Width / 2, selectionRect.Height / 2);
                        break;

                    case SelectionTools.Erase:
                        // Draw the erase area (which can behave similarly to a rectangle tool, but with different logic)
                        drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)), pen, selectionRect);
                        break;

                    case SelectionTools.SelectPixel:
                        // Select a single pixel (this can be visualized as a very small rectangle)
                        drawingContext.DrawRectangle(Brushes.Red, pen, new Rect(startPoint.Value, new Size(1, 1)));
                        break;
                }
            }

            // Freeform selection tool rendering
            if (Tool == SelectionTools.Freeform && freeformPoints.Count > 1)
            {
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(freeformPoints[0], false, false);
                    ctx.PolyLineTo(freeformPoints.Skip(1).ToArray(), true, false);
                }
                drawingContext.DrawGeometry(null, pen, geometry);
            }
        }
    }
}
