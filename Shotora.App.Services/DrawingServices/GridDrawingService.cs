using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using DrawingPointF=System.Drawing.PointF;

namespace Shotora.App.Services.DrawingServices;

public class GridDrawingService : IGridDrawingService
{
	public void ApplyTranslation(Grid visual, AnnotationItem annotation, double dx, double dy, IReadOnlyList<Control>? arrowParts)
	{
		var      parts          = arrowParts ?? visual.Children.OfType<Control>().ToList();
		Line?    translatedLine = null;
		Polygon? arrowHead      = null;

		if (parts is
			{
				Count: > 0
			})
		{
			foreach (var part in parts)
			{
				switch (part)
				{
					case Line arrowLine:
						arrowLine.StartPoint = new Point(arrowLine.StartPoint.X + dx, arrowLine.StartPoint.Y + dy);
						arrowLine.EndPoint   = new Point(arrowLine.EndPoint.X   + dx, arrowLine.EndPoint.Y   + dy);
						translatedLine       = arrowLine;
						break;
					case Polygon head:
						arrowHead = head;
						break;
				}
			}

			if (arrowHead != null && translatedLine != null)
			{
				arrowHead.Points.Clear();
				var start  = translatedLine.StartPoint;
				var end    = translatedLine.EndPoint;
				var dxLine = end.X - start.X;
				var dyLine = end.Y - start.Y;
				var length = Math.Sqrt(dxLine * dxLine + dyLine * dyLine);
				if (length > 0.1)
				{
					var unitX       = dxLine / length;
					var unitY       = dyLine / length;
					var arrowLength = Math.Min(annotation.ArrowHeadSize > 0 ? annotation.ArrowHeadSize : 20, length * 0.75);
					var arrowWidth  = arrowLength * 0.5;
					var baseX       = end.X - unitX * arrowLength;
					var baseY       = end.Y - unitY * arrowLength;
					var perpX       = -unitY * arrowWidth;
					var perpY       = unitX  * arrowWidth;
					arrowHead.Points.Add(end);
					arrowHead.Points.Add(new Point(baseX + perpX, baseY + perpY));
					arrowHead.Points.Add(new Point(baseX - perpX, baseY - perpY));
				}
			}
		}
		else
		{
			var gridLeft = double.IsNaN(Canvas.GetLeft(visual)) ? 0 : Canvas.GetLeft(visual);
			var gridTop  = double.IsNaN(Canvas.GetTop(visual)) ? 0 : Canvas.GetTop(visual);
			Canvas.SetLeft(visual, gridLeft + dx);
			Canvas.SetTop(visual, gridTop   + dy);
		}

		if (annotation.Points is
			{
				Count: >= 2
			})
		{
			annotation.Points[0] = new DrawingPointF(annotation.Points[0].X + (float)dx, annotation.Points[0].Y + (float)dy);
			annotation.Points[1] = new DrawingPointF(annotation.Points[1].X + (float)dx, annotation.Points[1].Y + (float)dy);
		}
		else if (translatedLine != null)
		{
			annotation.Points = new List<DrawingPointF>
			{
				new((float)translatedLine.StartPoint.X, (float)translatedLine.StartPoint.Y),
				new((float)translatedLine.EndPoint.X, (float)translatedLine.EndPoint.Y)
			};
		}

		annotation.Bounds = annotation.Bounds with
		{
			X = annotation.Bounds.X + (float)dx,
			Y = annotation.Bounds.Y + (float)dy
		};
	}
}
