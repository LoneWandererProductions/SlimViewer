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
        /// Updates the selection.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void UpdateSelection(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;
            InvalidateVisual();
        }

        public void AddFreeformPoint(Point point)
        {
            freeformPoints.Add(point);
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
            if (currentTool == SelectionTools.SelectRectangle || currentTool == SelectionTools.SelectEllipse)
            {
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
                    }
                }
            }
            else if (currentTool == SelectionTools.Freeform)
            {
                if (freeformPoints.Count > 1)
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

}