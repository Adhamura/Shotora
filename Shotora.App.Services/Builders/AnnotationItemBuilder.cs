using Avalonia;
using Avalonia.Media;
using Shotora.App.Interfaces.Builders;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using DrawingPointF=System.Drawing.PointF;
using DrawingColor=System.Drawing.Color;
using DrawingRectangleF=System.Drawing.RectangleF;

namespace Shotora.App.Services.Builders;

public class AnnotationItemBuilder : IAnnotationItemBuilder
{
	public AnnotationItem BuildFreehand(AnnotationToolType tool, Color color, float thickness, float opacity, IEnumerable<Point> points)
	{
		var annotation = new AnnotationItem
		{
			Tool      = tool,
			Color     = ToDrawingColor(color),
			Thickness = thickness,
			Opacity   = opacity
		};

		foreach (var point in points)
		{
			annotation.Points.Add(new DrawingPointF((float)point.X, (float)point.Y));
		}

		return annotation;
	}

	public AnnotationItem BuildBounds(OverlayInteractionState state, AnnotationToolType tool, Rect bounds, bool fill = false, float arrowHeadSize = 0)
	{
		return new AnnotationItem
		{
			Tool          = tool,
			Color         = ToDrawingColor(state.CurrentColor),
			Thickness     = (float)state.Thickness,
			Bounds        = ToDrawingRect(bounds),
			Fill          = fill,
			ArrowHeadSize = arrowHeadSize
		};
	}

	public AnnotationItem CloneAnnotation(AnnotationItem source)
	{
		return new AnnotationItem
		{
			Tool          = source.Tool,
			Color         = source.Color,
			Thickness     = source.Thickness,
			Opacity       = source.Opacity,
			Bounds        = new DrawingRectangleF(source.Bounds.X, source.Bounds.Y, source.Bounds.Width, source.Bounds.Height),
			Fill          = source.Fill,
			Text          = source.Text,
			FontFamily    = source.FontFamily,
			FontSize      = source.FontSize,
			Bold          = source.Bold,
			Italic        = source.Italic,
			ArrowHeadSize = source.ArrowHeadSize,
			Points        = source.Points.Select(p => new DrawingPointF(p.X, p.Y)).ToList()
		};
	}

	private static DrawingColor ToDrawingColor(Color color)
	{
		return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
	}

	private static DrawingRectangleF ToDrawingRect(Rect bounds)
	{
		return new DrawingRectangleF((float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
	}
}
