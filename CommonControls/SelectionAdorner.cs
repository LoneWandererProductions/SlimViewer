using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;

namespace CommonControls
{
    internal sealed class SelectionAdorner : Adorner
    {
        private Point? startPoint;
        private Point? endPoint;
        private SelectionTools currentTool;
        private List<Point> freeformPoints = new List<Point>();

        public SelectionAdorner(UIElement adornedElement, SelectionTools tool)
            : base(adornedElement)
        {
            currentTool = tool;
        }

        /// <summary>
        /// Updates the selection for rectangle and ellipse tools.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public void UpdateSelection(Point start, Point end)
        {
            // Apply transformation to the start and end points if necessary
            MatrixTransform transform = AdornedElement.RenderTransform as MatrixTransform;
            if (transform != null)
            {
                Matrix matrix = transform.Matrix;
                startPoint = matrix.Transform(start);
                endPoint = matrix.Transform(end);
            }
            else
            {
                startPoint = start;
                endPoint = end;
            }

            InvalidateVisual();
        }

        /// <summary>
        /// Adds a point for the freeform tool.
        /// </summary>
        /// <param name="point">The freeform point.</param>
        public void AddFreeformPoint(Point point)
        {
            MatrixTransform transform = AdornedElement.RenderTransform as MatrixTransform;
            if (transform != null)
            {
                Matrix matrix = transform.Matrix;
                freeformPoints.Add(matrix.Transform(point));
            }
            else
            {
                freeformPoints.Add(point);
            }

            InvalidateVisual();
        }

        public void ClearFreeformPoints()
        {
            freeformPoints.Clear();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pen = new Pen(Brushes.Red, 2);

            // Only render if at least one of the start or end points is available
            if (startPoint.HasValue && endPoint.HasValue)
            {
                Rect selectionRect = new Rect(startPoint.Value, endPoint.Value);

                switch (currentTool)
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
            if (currentTool == SelectionTools.Freeform && freeformPoints.Count > 1)
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
