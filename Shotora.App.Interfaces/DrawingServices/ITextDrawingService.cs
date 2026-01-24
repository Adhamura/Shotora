using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface ITextDrawingService
{
	void                                               DrawText(SKCanvas                 canvas,     AnnotationItem annotation, SKColor        color);
	Task<(Control Visual, AnnotationItem Annotation)?> CreateTextAnnotationAsync(Window  owner,      double         thickness,  Color          color,      Point  position);
	Task<bool>                                         ApplyTextEditAsync(Window         owner,      Control        visual,     AnnotationItem annotation, double thickness);
	TextBlock                                          CreateTextVisual(AnnotationItem   annotation, Color          color);
	void                                               ApplyTranslation(Control          visual,     AnnotationItem annotation, double dx,     double dy);
	Task                                               ShowOcrForSelectionAsync(SKBitmap capture,    Rect           selection,  Rect   anchor, string languages, OcrEngineType engine);
	void                                               CloseOcrWindow();
}
