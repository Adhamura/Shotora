using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IEllipseDrawingService
{
	void    DrawSkEllipse(SKCanvas       canvas,     AnnotationItem annotation, SKColor color);
	Ellipse CreateEllipse(AnnotationItem annotation, IBrush         strokeBrush);
	Ellipse CreatePreviewEllipse(Point   position,   IBrush         strokeBrush, double thickness);
	void    UpdatePreviewBounds(Ellipse  preview,    Point          start,       Point  current);
	void    ApplyTranslation(Ellipse     visual,     AnnotationItem annotation,  double dx, double dy);
}
