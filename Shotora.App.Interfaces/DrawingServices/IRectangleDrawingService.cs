using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IRectangleDrawingService
{
	Rect      NormalizeRect(Point                               start, Point end);
	Rect      GetBoundsFromPoints(Point                         start, Point end);
	Rect      ClampToBounds(Rect                                rect,  Size? displaySize);
	void      InitializeAnnotationCanvas(Panel                  annotationCanvas);
	void      UpdateAnnotationHighlight(Control?                visual, AnnotationItem? annotation);
	void      UpdateAnnotationHighlight(OverlayInteractionState state);
	SKRectI   IntersectRect(SKRectI                             a,          SKRectI               b);
	void      DrawSkRect(SKCanvas                               canvas,     AnnotationItem        annotation, SKColor color);
	Rectangle CreateRectangle(AnnotationItem                    annotation, IBrush                strokeBrush);
	Rectangle CreatePreviewRectangle(Point                      position,   IBrush                strokeBrush, double thickness);
	void      UpdatePreviewBounds(Rectangle                     preview,    Point                 start,       Point  current);
	Rectangle CreateProcessingRectangle(AnnotationItem          annotation, IBrush                strokeBrush, double opacity);
	PixelRect GetVirtualBounds(Screen?                          primary,    IReadOnlyList<Screen> allScreens,  double fallbackWidth, double fallbackHeight);
	void      ApplyTranslation(Rectangle                        visual,     AnnotationItem        annotation,  double dx,            double dy);
}
