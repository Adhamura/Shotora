using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface ILineDrawingService
{
	Line CreateLine(Point      start,  Point          end,        IBrush  stroke, double thickness, bool hitTestVisible);
	void ApplyTranslation(Line visual, AnnotationItem annotation, double  dx,     double dy);
	void DrawSkLine(SKCanvas   canvas, AnnotationItem annotation, SKColor color);
}
