using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;

namespace Shotora.App.Services.DrawingServices;

public class TranslationDrawingService(IPolylineDrawingService  polylines,
									   ILineDrawingService      lines,
									   IRectangleDrawingService rectangles,
									   IEllipseDrawingService   ellipses,
									   IGridDrawingService      grids,
									   ITextDrawingService      textDrawingService)
	: ITranslationDrawingService
{
	public Vector GetTranslation(Control control)
	{
		switch (control.RenderTransform)
		{
			case TranslateTransform tt:
				return new Vector(tt.X, tt.Y);
			case TransformGroup group:
			{
				foreach (var transform in group.Children)
				{
					if (transform is TranslateTransform childTt)
					{
						return new Vector(childTt.X, childTt.Y);
					}
				}
				break;
			}
		}

		return new Vector(0, 0);
	}

	public bool ApplyShapeTranslation(Control visual, AnnotationItem annotation, IReadOnlyDictionary<Control, List<Control>> arrowGroups)
	{
		var offset = GetTranslation(visual);
		if (Math.Abs(offset.X) < 0.01 && Math.Abs(offset.Y) < 0.01)
		{
			visual.RenderTransform = null;
			return false;
		}

		visual.RenderTransform = null;
		var dx = offset.X;
		var dy = offset.Y;

		switch (visual)
		{
			case Polyline polyline:
				polylines.ApplyTranslation(polyline, annotation, dx, dy);
				break;
			case Line line:
				lines.ApplyTranslation(line, annotation, dx, dy);
				break;
			case Rectangle rect:
				rectangles.ApplyTranslation(rect, annotation, dx, dy);
				break;
			case Ellipse ellipse:
				ellipses.ApplyTranslation(ellipse, annotation, dx, dy);
				break;
			case Grid grid:
			{
				List<Control>? parts = null;
				arrowGroups?.TryGetValue(grid, out parts);
				grids.ApplyTranslation(grid, annotation, dx, dy, parts);
				break;
			}
			case TextBlock text:
				textDrawingService.ApplyTranslation(text, annotation, dx, dy);
				break;
		}

		return true;
	}
}
