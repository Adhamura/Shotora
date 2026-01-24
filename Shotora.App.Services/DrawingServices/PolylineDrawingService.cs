using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using SkiaSharp;
using DrawingPointF=System.Drawing.PointF;

namespace Shotora.App.Services.DrawingServices;

public class PolylineDrawingService : IPolylineDrawingService
{
	public Polyline CreatePolyline(AnnotationItem annotation, IBrush strokeBrush)
	{
		var polyline = CreateBasePolyline(strokeBrush, annotation.Thickness, false);
		polyline.IsHitTestVisible = true;
		polyline.Points           = new Points(annotation.Points.Select(pt => new Point(pt.X, pt.Y)));
		return polyline;
	}

	public Polyline CreatePreviewPolyline(Point start, Color color, double thickness, bool highlight)
	{
		var stroke = highlight
			? Color.FromArgb(128, color.R, color.G, color.B)
			: color;

		var points = new List<Point>
		{
			start
		};
		var polyline = CreateBasePolyline(new SolidColorBrush(stroke), highlight ? thickness * 3 : thickness, true);
		polyline.Tag    = points;
		polyline.Points = new Points(points);
		return polyline;
	}

	public void AppendPreviewPoint(Polyline polyline, Point point)
	{
		if (polyline.Tag is not List<Point> points)
		{
			points       = [];
			polyline.Tag = points;
		}
		points.Add(point);
		polyline.Points = new Points(points);
	}

	public void ApplyTranslation(Polyline visual, AnnotationItem annotation, double dx, double dy)
	{
		var shiftedPoints = visual.Points
			.Select(p => new Point(p.X + dx, p.Y + dy))
			.ToList();

		visual.Points = new Points(shiftedPoints);
		annotation.Points = shiftedPoints
			.Select(p => new DrawingPointF((float)p.X, (float)p.Y))
			.ToList();
	}

	public void DrawSkPolyline(SKCanvas canvas, AnnotationItem annotation, SKColor skColor)
	{
		if (annotation.Points.Count < 2)
		{
			return;
		}

		using var paint = new SKPaint
		{
			IsAntialias = true,
			Color       = skColor,
			Style       = SKPaintStyle.Stroke,
			StrokeWidth = annotation.Thickness,
			StrokeCap   = SKStrokeCap.Round,
			StrokeJoin  = SKStrokeJoin.Round
		};

		var points = annotation.Points.Select(p => new SKPoint(p.X, p.Y)).ToArray();
		canvas.DrawPoints(SKPointMode.Polygon, points, paint);
	}

	private static Polyline CreateBasePolyline(IBrush stroke, double thickness, bool isPreview)
	{
		return new Polyline
		{
			Stroke           = stroke,
			StrokeThickness  = thickness,
			StrokeLineCap    = PenLineCap.Round,
			StrokeJoin       = PenLineJoin.Round,
			IsHitTestVisible = !isPreview
		};
	}
}
