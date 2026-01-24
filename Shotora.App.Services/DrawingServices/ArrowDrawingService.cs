using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Models.AtomModels;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class ArrowDrawingService(ILineDrawingService lines) : IArrowDrawingService
{
	public Control ReplaceArrowVisual(
		Panel                                canvas,
		Control                              visual,
		AnnotationItem                       annotation,
		IDictionary<Control, AnnotationItem> visualToAnnotation,
		IDictionary<Control, List<Control>>  arrowGroups,
		Control?                             selectedVisual,
		out Control?                         updatedSelectedVisual)
	{
		if (canvas == null)
		{
			updatedSelectedVisual = selectedVisual;
			return visual;
		}

		var index = canvas.Children.IndexOf(visual);

		canvas.Children.Remove(visual);
		visualToAnnotation.Remove(visual);
		arrowGroups.Remove(visual);

		var stroke      = CreateStrokeBrush(annotation);
		var replacement = CreateArrowContainer(annotation, stroke, (container, parts) => arrowGroups[container] = parts);

		if (index >= 0 && index <= canvas.Children.Count)
		{
			canvas.Children.Insert(index, replacement);
		}
		else
		{
			canvas.Children.Add(replacement);
		}

		visualToAnnotation[replacement] = annotation;
		if (selectedVisual == visual)
		{
			selectedVisual = replacement;
		}

		updatedSelectedVisual = selectedVisual;
		return replacement;
	}

	public Control CreateArrowContainer(AnnotationItem annotation, IBrush stroke, Action<Control, List<Control>> registerGroup)
	{
		var (start, end) = GetEndpoints(annotation);
		var geometry  = ComputeGeometry(start, end, annotation.ArrowHeadSize);
		var lineEnd   = geometry.IsValid ? geometry.LineEnd : end;
		var arrowLine = lines.CreateLine(start, lineEnd, stroke, annotation.Thickness, true);
		arrowLine.StrokeLineCap = PenLineCap.Flat;
		return BuildArrowContainer(arrowLine, geometry, stroke, registerGroup);
	}

	public void DrawArrow(SKCanvas canvas, AnnotationItem annotation, SKColor color)
	{
		var (start, end) = GetEndpoints(annotation);
		var geometry = ComputeGeometry(start, end, annotation.ArrowHeadSize);
		if (!geometry.IsValid)
		{
			using var fallbackPaint = new SKPaint
			{
				IsAntialias = true,
				Color       = color,
				Style       = SKPaintStyle.Stroke,
				StrokeWidth = annotation.Thickness,
				StrokeCap   = SKStrokeCap.Round
			};
			canvas.DrawLine(new SKPoint((float)start.X, (float)start.Y), new SKPoint((float)end.X, (float)end.Y), fallbackPaint);
			return;
		}

		using var paint = new SKPaint
		{
			IsAntialias = true,
			Color       = color,
			Style       = SKPaintStyle.Stroke,
			StrokeWidth = annotation.Thickness,
			StrokeCap   = SKStrokeCap.Butt
		};
		canvas.DrawLine(ToSkPoint(geometry.Start), ToSkPoint(geometry.LineEnd), paint);

		using var fill = new SKPaint
		{
			IsAntialias = true,
			Color       = color,
			Style       = SKPaintStyle.Fill
		};
		using var path = new SKPath();
		path.MoveTo(ToSkPoint(geometry.Tip));
		path.LineTo(ToSkPoint(geometry.Left));
		path.LineTo(ToSkPoint(geometry.Right));
		path.Close();
		canvas.DrawPath(path, fill);
	}

	private static Control BuildArrowContainer(Line arrowLine, ArrowGeometry geometry, IBrush stroke, Action<Control, List<Control>> registerGroup)
	{
		var container = new Grid
		{
			IsHitTestVisible = true
		};
		arrowLine.IsHitTestVisible = true;
		arrowLine.Stroke           = stroke;
		container.Children.Add(arrowLine);

		var parts = new List<Control>
		{
			arrowLine
		};

		var head = CreateArrowHead(geometry, stroke);
		if (head != null)
		{
			head.IsHitTestVisible = true;
			container.Children.Add(head);
			parts.Add(head);
		}

		registerGroup(container, parts);
		return container;
	}

	private static Polygon? CreateArrowHead(ArrowGeometry geometry, IBrush stroke)
	{
		if (!geometry.IsValid)
		{
			return null;
		}

		return new Polygon
		{
			Points =
			{
				geometry.Tip,
				geometry.Left,
				geometry.Right
			},
			Fill            = stroke,
			Stroke          = stroke,
			StrokeThickness = 1
		};
	}

	private static ArrowGeometry ComputeGeometry(Point start, Point end, double headSize)
	{
		var dx     = end.X - start.X;
		var dy     = end.Y - start.Y;
		var length = Math.Sqrt(dx * dx + dy * dy);

		if (length < 1)
		{
			return default;
		}

		var unitX = dx / length;
		var unitY = dy / length;

		var arrowLength = Math.Min(headSize > 0 ? headSize : 20, length * 0.75);
		var arrowWidth  = arrowLength * 0.5;

		var arrowBase = new Point(end.X - unitX * arrowLength, end.Y - unitY * arrowLength);
		var perpX     = -unitY * arrowWidth;
		var perpY     = unitX  * arrowWidth;

		return new ArrowGeometry(start, end, arrowBase, new Point(arrowBase.X + perpX, arrowBase.Y + perpY), new Point(arrowBase.X - perpX, arrowBase.Y - perpY), true);
	}

	private static SKPoint ToSkPoint(Point p)
	{
		return new SKPoint((float)p.X, (float)p.Y);
	}

	private static SolidColorBrush CreateStrokeBrush(AnnotationItem annotation)
	{
		var color = Color.FromArgb(annotation.Color.A, annotation.Color.R, annotation.Color.G, annotation.Color.B);
		return new SolidColorBrush(color)
		{
			Opacity = annotation.Opacity
		};
	}

	private static (Point Start, Point End) GetEndpoints(AnnotationItem annotation)
	{
		if (annotation.Points is
			{
				Count: >= 2
			})
		{
			return (new Point(annotation.Points[0].X, annotation.Points[0].Y), new Point(annotation.Points[1].X, annotation.Points[1].Y));
		}

		var start = new Point(annotation.Bounds.X,                           annotation.Bounds.Y);
		var end   = new Point(annotation.Bounds.X + annotation.Bounds.Width, annotation.Bounds.Y + annotation.Bounds.Height);
		return (start, end);
	}
}
