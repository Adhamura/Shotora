using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class EllipseDrawingService : IEllipseDrawingService
{
	public void DrawSkEllipse(SKCanvas canvas, AnnotationItem annotation, SKColor skColor)
	{
		var rect = new SKRect(
			annotation.Bounds.X,
			annotation.Bounds.Y,
			annotation.Bounds.X + annotation.Bounds.Width,
			annotation.Bounds.Y + annotation.Bounds.Height);

		using var stroke = new SKPaint
		{
			IsAntialias = true,
			Color       = skColor,
			Style       = SKPaintStyle.Stroke,
			StrokeWidth = annotation.Thickness
		};
		if (annotation.Fill)
		{
			using var fill = new SKPaint
			{
				IsAntialias = true,
				Color       = skColor,
				Style       = SKPaintStyle.Fill
			};
			canvas.DrawOval(rect, fill);
		}
		canvas.DrawOval(rect, stroke);
	}

	public Ellipse CreateEllipse(AnnotationItem annotation, IBrush strokeBrush)
	{
		var ellipse = CreateEllipseCommon(strokeBrush, annotation.Thickness, annotation.Fill, true);
		PositionEllipse(ellipse, ToRect(annotation));
		return ellipse;
	}

	public Ellipse CreatePreviewEllipse(Point position, IBrush strokeBrush, double thickness)
	{
		var ellipse = CreateEllipseCommon(strokeBrush, thickness, false, true);
		PositionEllipse(ellipse, new Rect(position.X, position.Y, ellipse.Width, ellipse.Height));
		return ellipse;
	}

	public void UpdatePreviewBounds(Ellipse preview, Point start, Point current)
	{
		var bounds = Normalize(start, current);
		PositionEllipse(preview, bounds);
	}

	public void ApplyTranslation(Ellipse visual, AnnotationItem annotation, double dx, double dy)
	{
		Canvas.SetLeft(visual, double.IsNaN(Canvas.GetLeft(visual)) ? 0 : Canvas.GetLeft(visual) + dx);
		Canvas.SetTop(visual, double.IsNaN(Canvas.GetTop(visual)) ? 0 : Canvas.GetTop(visual)    + dy);
		annotation.Bounds = annotation.Bounds with
		{
			X = annotation.Bounds.X + (float)dx,
			Y = annotation.Bounds.Y + (float)dy
		};
	}

	private static Rect Normalize(Point start, Point current)
	{
		return new Rect(
			Math.Min(start.X, current.X),
			Math.Min(start.Y, current.Y),
			Math.Abs(start.X - current.X),
			Math.Abs(start.Y - current.Y));
	}

	private static Rect ToRect(AnnotationItem annotation)
	{
		return new Rect(annotation.Bounds.X, annotation.Bounds.Y, annotation.Bounds.Width, annotation.Bounds.Height);
	}

	private static void PositionEllipse(Ellipse ellipse, Rect bounds)
	{
		Canvas.SetLeft(ellipse, bounds.X);
		Canvas.SetTop(ellipse, bounds.Y);
		ellipse.Width  = bounds.Width;
		ellipse.Height = bounds.Height;
	}

	private static Ellipse CreateEllipseCommon(IBrush stroke, double thickness, bool fill, bool isHitTestVisible)
	{
		return new Ellipse
		{
			Stroke           = stroke,
			StrokeThickness  = thickness,
			Fill             = fill ? stroke : Brushes.Transparent,
			IsHitTestVisible = isHitTestVisible
		};
	}
}
