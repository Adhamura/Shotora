using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Models.Drawings;
using Shotora.App.Models.Enums;
using SkiaSharp;

namespace Shotora.App.Services.DrawingServices;

public class AnnotationDrawingService(ISkiaDrawingService      skiaDrawingService,
									  ITextDrawingService      textDrawingService,
									  IPolylineDrawingService  polylines,
									  ILineDrawingService      lines,
									  IArrowDrawingService     arrows,
									  IRectangleDrawingService rectangles,
									  IEllipseDrawingService   ellipses)
	: IAnnotationDrawingService
{
	public SKBitmap BuildAnnotatedBitmap(SKBitmap captureRaw, IReadOnlyList<AnnotationItem> annotations)
	{
		if (captureRaw == null)
		{
			throw new InvalidOperationException("Capture bitmap is required.");
		}

		var       annotated = captureRaw.Copy();
		using var canvas    = new SKCanvas(annotated);
		canvas.DrawBitmap(captureRaw, 0, 0);

		foreach (var annotation in annotations)
		{
			DrawAnnotation(canvas, annotation, captureRaw);
		}

		return annotated;
	}

	public Control? CreateVisualForAnnotation(AnnotationItem annotation, Action<Control, List<Control>>? registerArrowGroup = null)
	{
		var avaloniaColor = ToAvaloniaColor(annotation);
		var strokeBrush   = CreateStrokeBrush(annotation, avaloniaColor);
		var register = registerArrowGroup ??
			((_, _) =>
			{
			});
		return CreateVisual(annotation, avaloniaColor, strokeBrush, register);
	}

	public Control? GetTopMostAnnotationAt(Panel canvas, Point position, IReadOnlyDictionary<Control, List<Control>> arrowGroups)
	{
		const int hitRadius = 8;
		var       allHits   = new HashSet<Control>();
		for (var dx = -hitRadius; dx <= hitRadius; dx += 4)
		{
			for (var dy = -hitRadius; dy <= hitRadius; dy += 4)
			{
				var testPoint = new Point(position.X + dx, position.Y + dy);
				var hits = canvas
					.GetVisualsAt(testPoint)
					.OfType<Control>();
				foreach (var hit in hits)
				{
					allHits.Add(hit);
				}
			}
		}

		if (allHits.Count == 0)
		{
			return null;
		}

		foreach (var hit in allHits)
		{
			foreach (var (container, components) in arrowGroups)
			{
				if (components.Contains(hit))
				{
					return container;
				}
			}
		}

		return allHits.FirstOrDefault();
	}

	public DrawingPreview BeginDrawing(OverlayTool tool, Point start, Color color, double thickness)
	{
		switch (tool)
		{
			case OverlayTool.Pen:
			case OverlayTool.Highlight:
			{
				var polyline = polylines.CreatePreviewPolyline(start, color, thickness, tool == OverlayTool.Highlight);
				return new DrawingPreview(polyline, null, null, null, null);
			}
			case OverlayTool.Line:
			case OverlayTool.Arrow:
			{
				var line = lines.CreateLine(start, start, new SolidColorBrush(color), thickness, false);
				return new DrawingPreview(null, line, null, null, null);
			}
			case OverlayTool.Rectangle:
			{
				var rect = rectangles.CreatePreviewRectangle(start, new SolidColorBrush(color), thickness);
				return new DrawingPreview(null, rect, null, null, null);
			}
			case OverlayTool.Ellipse:
			{
				var ellipse = ellipses.CreatePreviewEllipse(start, new SolidColorBrush(color), thickness);
				return new DrawingPreview(null, ellipse, null, null, null);
			}
			case OverlayTool.Blur:
			case OverlayTool.Pixelate:
			{
				var preview = skiaDrawingService.CreateEffectPreview(start, new SolidColorBrush(color), thickness);
				return new DrawingPreview(null, null, preview.Grid, preview.Image, preview.Border);
			}
			default:
				return default;
		}
	}

	public void UpdateDrawingPreview(OverlayInteractionState state, Point current)
	{
		switch (state.CurrentTool)
		{
			case OverlayTool.Pen:
			case OverlayTool.Highlight:
				if (state.CurrentPolyline != null)
				{
					polylines.AppendPreviewPoint(state.CurrentPolyline, current);
				}
				break;

			case OverlayTool.Line:
			case OverlayTool.Arrow:
			{
				if (state.CurrentPreviewShape is Line line)
				{
					line.EndPoint = current;
				}
				break;
			}

			case OverlayTool.Rectangle:
				if (state.CurrentPreviewShape is Rectangle rect)
				{
					rectangles.UpdatePreviewBounds(rect, state.DrawStart, current);
				}
				break;

			case OverlayTool.Ellipse:
				if (state.CurrentPreviewShape is Ellipse ellipse)
				{
					ellipses.UpdatePreviewBounds(ellipse, state.DrawStart, current);
				}
				break;

			case OverlayTool.Blur:
			case OverlayTool.Pixelate:
			{
				var previewGrid = state.EffectPreviewGrid;
				if (previewGrid == null)
				{
					break;
				}

				var bounds = rectangles.NormalizeRect(state.DrawStart, current);
				if (skiaDrawingService.UpdateEffectPreviewBounds(previewGrid, bounds))
				{
					skiaDrawingService.UpdateEffectPreview(state.EffectPreviewImage, state.Capture?.Raw, bounds, state.CurrentTool, state.Thickness);
				}
				break;
			}
		}
	}

	public void RefreshSelectedAnnotationVisual(OverlayInteractionState state)
	{
		ApplySelectionBrush(state.SelectedVisual, state.SelectedAnnotation);
	}

	private void ApplySelectionBrush(Control? selectedVisual, AnnotationItem? selectedAnnotation)
	{
		if (selectedVisual == null || selectedAnnotation == null)
		{
			rectangles.UpdateAnnotationHighlight(null, null);
			return;
		}

		var brush = new SolidColorBrush(Color.FromArgb(
			selectedAnnotation.Color.A,
			selectedAnnotation.Color.R,
			selectedAnnotation.Color.G,
			selectedAnnotation.Color.B));

		switch (selectedVisual)
		{
			case Polyline polyline:
				polyline.Stroke = brush;
				break;
			case Line line:
				line.Stroke = brush;
				break;
			case Rectangle rect:
				rect.Stroke = brush;
				if (selectedAnnotation.Fill)
				{
					rect.Fill = brush;
				}
				break;
			case Ellipse ellipse:
				ellipse.Stroke = brush;
				if (selectedAnnotation.Fill)
				{
					ellipse.Fill = brush;
				}
				break;
			case Grid grid:
				foreach (var child in grid.Children)
				{
					switch (child)
					{
						case Line arrowLine:
							arrowLine.Stroke = brush;
							break;
						case Polygon arrowHead:
							arrowHead.Fill   = brush;
							arrowHead.Stroke = brush;
							break;
					}
				}
				break;
			case TextBlock text:
				text.Foreground = brush;
				break;
		}

		rectangles.UpdateAnnotationHighlight(selectedVisual, selectedAnnotation);
	}

	private Control? CreateVisual(AnnotationItem annotation, Color avaloniaColor, SolidColorBrush strokeBrush, Action<Control, List<Control>> registerArrowGroup)
	{
		switch (annotation.Tool)
		{
			case AnnotationToolType.Pen:
			case AnnotationToolType.Highlight:
				return polylines.CreatePolyline(annotation, strokeBrush);
			case AnnotationToolType.Line:
				return lines.CreateLine(
					new Point(annotation.Bounds.X,                           annotation.Bounds.Y),
					new Point(annotation.Bounds.X + annotation.Bounds.Width, annotation.Bounds.Y + annotation.Bounds.Height),
					strokeBrush,
					annotation.Thickness,
					true);
			case AnnotationToolType.Arrow:
				return arrows.CreateArrowContainer(annotation, strokeBrush, registerArrowGroup);
			case AnnotationToolType.Rectangle:
				return rectangles.CreateRectangle(annotation, strokeBrush);
			case AnnotationToolType.Ellipse:
				return ellipses.CreateEllipse(annotation, strokeBrush);
			case AnnotationToolType.Text:
				return textDrawingService.CreateTextVisual(annotation, avaloniaColor);
			case AnnotationToolType.Blur:
			case AnnotationToolType.Pixelate:
				return rectangles.CreateProcessingRectangle(annotation, strokeBrush, 0.9);
			default:
				return null;
		}
	}

	private static Color ToAvaloniaColor(AnnotationItem annotation)
	{
		return Color.FromArgb(annotation.Color.A, annotation.Color.R, annotation.Color.G, annotation.Color.B);
	}

	private static SolidColorBrush CreateStrokeBrush(AnnotationItem annotation, Color avaloniaColor)
	{
		return new SolidColorBrush(avaloniaColor)
		{
			Opacity = annotation.Opacity
		};
	}

	private void DrawAnnotation(SKCanvas canvas, AnnotationItem annotation, SKBitmap baseBitmap)
	{
		var skColor = skiaDrawingService.ToSkColor(annotation.Color, annotation.Opacity);

		switch (annotation.Tool)
		{
			case AnnotationToolType.Text:
				textDrawingService.DrawText(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Pen:
			case AnnotationToolType.Highlight:
				polylines.DrawSkPolyline(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Line:
				lines.DrawSkLine(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Arrow:
				arrows.DrawArrow(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Rectangle:
				rectangles.DrawSkRect(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Ellipse:
				ellipses.DrawSkEllipse(canvas, annotation, skColor);
				break;
			case AnnotationToolType.Blur:
			case AnnotationToolType.Pixelate:
				skiaDrawingService.DrawProcessedRegion(canvas, baseBitmap, annotation);
				break;
		}
	}
}
