using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Shotora.App.Models;
using Shotora.App.Models.Drawings;
using Shotora.App.Models.Enums;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IAnnotationDrawingService
{
	SKBitmap       BuildAnnotatedBitmap(SKBitmap                           captureRaw, IReadOnlyList<AnnotationItem>   annotations);
	Control?       CreateVisualForAnnotation(AnnotationItem                annotation, Action<Control, List<Control>>? registerArrowGroup = null);
	DrawingPreview BeginDrawing(OverlayTool                                tool,       Point                           start, Color color, double thickness);
	void           UpdateDrawingPreview(OverlayInteractionState            state,      Point                           current);
	void           RefreshSelectedAnnotationVisual(OverlayInteractionState state);
	Control?       GetTopMostAnnotationAt(Panel                            canvas, Point position, IReadOnlyDictionary<Control, List<Control>> arrowGroups);
}
