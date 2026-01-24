using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class LineDrawingService : ILineDrawingService
{
	public Line CreateLine(Point start, Point end, IBrush stroke, double thickness, bool hitTestVisible)
	{
		return new Line
		{
			Stroke           = stroke,
			StrokeThickness  = thickness,
			StrokeLineCap    = PenLineCap.Round,
			StartPoint       = start,
			EndPoint         = end,
			IsHitTestVisible = hitTestVisible
		};
	}

	public void ApplyTranslation(Line visual, AnnotationItem annotation, double dx, double dy)
	{
		visual.StartPoint = new Point(visual.StartPoint.X + dx, visual.StartPoint.Y + dy);
		visual.EndPoint   = new Point(visual.EndPoint.X   + dx, visual.EndPoint.Y   + dy);
		annotation.Bounds = annotation.Bounds with
		{
			X = annotation.Bounds.X + (float)dx,
			Y = annotation.Bounds.Y + (float)dy
		};
	}

	public void DrawSkLine(SKCanvas canvas, AnnotationItem annotation, SKColor skColor)
	{
		var start = new SKPoint(annotation.Bounds.X,                           annotation.Bounds.Y);
		var end   = new SKPoint(annotation.Bounds.X + annotation.Bounds.Width, annotation.Bounds.Y + annotation.Bounds.Height);

		using var paint = new SKPaint
		{
			IsAntialias = true,
			Color       = skColor,
			Style       = SKPaintStyle.Stroke,
			StrokeWidth = annotation.Thickness,
			StrokeCap   = SKStrokeCap.Round
		};
		canvas.DrawLine(start, end, paint);
	}
}
