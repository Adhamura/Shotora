using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class RectangleDrawingService : IRectangleDrawingService
{
	private Panel?     _annotationCanvas;
	private Rectangle? _annotationHighlight;

	public Rect NormalizeRect(Point start, Point end)
	{
		var x      = Math.Min(start.X, end.X);
		var y      = Math.Min(start.Y, end.Y);
		var width  = Math.Abs(start.X - end.X);
		var height = Math.Abs(start.Y - end.Y);
		return new Rect(x, y, width, height);
	}

	public Rect GetBoundsFromPoints(Point start, Point end)
	{
		return NormalizeRect(start, end);
	}

	public Rect ClampToBounds(Rect rect, Size? displaySize)
	{
		if (displaySize == null)
		{
			return rect;
		}

		var maxW = displaySize.Value.Width;
		var maxH = displaySize.Value.Height;

		var x = Math.Clamp(rect.X,      0, Math.Max(0, maxW - 1));
		var y = Math.Clamp(rect.Y,      0, Math.Max(0, maxH - 1));
		var w = Math.Clamp(rect.Width,  0, Math.Max(0, maxW - x));
		var h = Math.Clamp(rect.Height, 0, Math.Max(0, maxH - y));
		return new Rect(x, y, w, h);
	}

	public void InitializeAnnotationCanvas(Panel annotationCanvas)
	{
		_annotationCanvas = annotationCanvas;
	}

	public void UpdateAnnotationHighlight(Control? visual, AnnotationItem? annotation)
	{
		var highlight = EnsureAnnotationHighlight();
		if (visual == null || annotation == null)
		{
			highlight.IsVisible = false;
			return;
		}

		var rect        = GetAnnotationRect(annotation);
		var translation = GetTranslation(visual);
		rect = new Rect(rect.X + translation.X, rect.Y + translation.Y, rect.Width, rect.Height);

		if (rect.Width <= 0 || rect.Height <= 0)
		{
			highlight.IsVisible = false;
			return;
		}

		PositionRectangle(highlight, rect);
		highlight.IsVisible = true;
	}

	public void UpdateAnnotationHighlight(OverlayInteractionState state)
	{
		UpdateAnnotationHighlight(state.SelectedVisual, state.SelectedAnnotation);
	}

	public SKRectI IntersectRect(SKRectI a, SKRectI b)
	{
		var left   = Math.Max(a.Left, b.Left);
		var top    = Math.Max(a.Top,  b.Top);
		var right  = Math.Min(a.Right,  b.Right);
		var bottom = Math.Min(a.Bottom, b.Bottom);
		if (right <= left || bottom <= top)
		{
			return new SKRectI(0, 0, 0, 0);
		}
		return new SKRectI(left, top, right, bottom);
	}

	public void DrawSkRect(SKCanvas canvas, AnnotationItem annotation, SKColor skColor)
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
			canvas.DrawRect(rect, fill);
		}
		canvas.DrawRect(rect, stroke);
	}

	public Rectangle CreateRectangle(AnnotationItem annotation, IBrush strokeBrush)
	{
		var rect = CreateRectangleCommon(strokeBrush, annotation.Thickness, annotation.Fill, 1, true);
		PositionRectangle(rect, ToRect(annotation));
		return rect;
	}

	public Rectangle CreatePreviewRectangle(Point position, IBrush strokeBrush, double thickness)
	{
		var rect = CreateRectangleCommon(strokeBrush, thickness, false, 1, true);
		PositionRectangle(rect, new Rect(position.X, position.Y, rect.Width, rect.Height));
		return rect;
	}

	public void UpdatePreviewBounds(Rectangle preview, Point start, Point current)
	{
		var bounds = NormalizeRect(start, current);
		PositionRectangle(preview, bounds);
	}

	public Rectangle CreateProcessingRectangle(AnnotationItem annotation, IBrush strokeBrush, double opacity)
	{
		var rect = CreateRectangleCommon(strokeBrush, annotation.Thickness, false, opacity, true);
		PositionRectangle(rect, ToRect(annotation));
		return rect;
	}

	public void ApplyTranslation(Rectangle visual, AnnotationItem annotation, double dx, double dy)
	{
		Canvas.SetLeft(visual, double.IsNaN(Canvas.GetLeft(visual)) ? 0 : Canvas.GetLeft(visual) + dx);
		Canvas.SetTop(visual, double.IsNaN(Canvas.GetTop(visual)) ? 0 : Canvas.GetTop(visual)    + dy);
		annotation.Bounds = annotation.Bounds with
		{
			X = annotation.Bounds.X + (float)dx,
			Y = annotation.Bounds.Y + (float)dy
		};
	}

	public PixelRect GetVirtualBounds(Screen? primary, IReadOnlyList<Screen> allScreens, double fallbackWidth, double fallbackHeight)
	{
		if (allScreens is
			{
				Count: > 0
			})
		{
			var left   = int.MaxValue;
			var top    = int.MaxValue;
			var right  = int.MinValue;
			var bottom = int.MinValue;
			foreach (var s in allScreens)
			{
				left   = Math.Min(left, s.Bounds.X);
				top    = Math.Min(top,  s.Bounds.Y);
				right  = Math.Max(right,  s.Bounds.X + s.Bounds.Width);
				bottom = Math.Max(bottom, s.Bounds.Y + s.Bounds.Height);
			}

			return new PixelRect(left, top, right - left, bottom - top);
		}

		var fallback = new PixelRect(0, 0, (int)fallbackWidth, (int)fallbackHeight);
		return primary?.Bounds ?? fallback;
	}

	public Rect GetAnnotationRect(AnnotationItem annotation)
	{
		if (annotation.Points.Count > 0)
		{
			var xs   = annotation.Points.Select(p => p.X).ToArray();
			var ys   = annotation.Points.Select(p => p.Y).ToArray();
			var minX = xs.Min();
			var maxX = xs.Max();
			var minY = ys.Min();
			var maxY = ys.Max();
			return new Rect(minX, minY, Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));
		}

		var b = annotation.Bounds;
		return NormalizeRect(new Point(b.Left, b.Top), new Point(b.Left + b.Width, b.Top + b.Height));
	}

	private static Vector GetTranslation(Control control)
	{
		if (control.RenderTransform is TranslateTransform tt)
		{
			return new Vector(tt.X, tt.Y);
		}

		if (control.RenderTransform is TransformGroup group)
		{
			foreach (var transform in group.Children)
			{
				if (transform is TranslateTransform childTr)
				{
					return new Vector(childTr.X, childTr.Y);
				}
			}
		}

		return new Vector(0, 0);
	}

	private Rectangle EnsureAnnotationHighlight()
	{
		if (_annotationCanvas == null)
		{
			throw new InvalidOperationException("Annotation canvas is not initialized.");
		}

		if (_annotationHighlight != null && _annotationCanvas.Children.Contains(_annotationHighlight))
		{
			return _annotationHighlight;
		}

		_annotationHighlight = new Rectangle
		{
			Stroke           = new SolidColorBrush(Color.FromArgb(230, 255, 255, 255)),
			StrokeDashArray  = [6, 4],
			StrokeThickness  = 2,
			Fill             = Brushes.Transparent,
			IsHitTestVisible = false,
			IsVisible        = false
		};
		_annotationCanvas.Children.Add(_annotationHighlight);
		return _annotationHighlight;
	}

	private static Rectangle CreateRectangleCommon(IBrush stroke, double thickness, bool fill, double opacity, bool isHitTestVisible)
	{
		return new Rectangle
		{
			Stroke           = stroke,
			StrokeThickness  = thickness,
			Fill             = fill ? stroke : Brushes.Transparent,
			Opacity          = opacity,
			IsHitTestVisible = isHitTestVisible
		};
	}

	private static Rect ToRect(AnnotationItem annotation)
	{
		return new Rect(annotation.Bounds.X, annotation.Bounds.Y, annotation.Bounds.Width, annotation.Bounds.Height);
	}

	private static void PositionRectangle(Rectangle rectangle, Rect bounds)
	{
		Canvas.SetLeft(rectangle, bounds.X);
		Canvas.SetTop(rectangle, bounds.Y);
		rectangle.Width  = bounds.Width;
		rectangle.Height = bounds.Height;
	}
}
