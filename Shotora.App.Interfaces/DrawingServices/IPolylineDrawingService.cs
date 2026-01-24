using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IPolylineDrawingService
{
	void     ApplyTranslation(Polyline     visual,     AnnotationItem annotation, double  dx, double dy);
	void     DrawSkPolyline(SKCanvas       canvas,     AnnotationItem annotation, SKColor color);
	Polyline CreatePolyline(AnnotationItem annotation, IBrush         strokeBrush);
	Polyline CreatePreviewPolyline(Point   start,      Color          color, double thickness, bool highlight);
	void     AppendPreviewPoint(Polyline   polyline,   Point          point);
}
