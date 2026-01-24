using Avalonia.Controls;
using Avalonia.Media;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IArrowDrawingService
{
	void    DrawArrow(SKCanvas                  canvas,     AnnotationItem annotation, SKColor                        color);
	Control CreateArrowContainer(AnnotationItem annotation, IBrush         stroke,     Action<Control, List<Control>> registerGroup);
	Control ReplaceArrowVisual(
		Panel                                canvas,
		Control                              visual,
		AnnotationItem                       annotation,
		IDictionary<Control, AnnotationItem> visualToAnnotation,
		IDictionary<Control, List<Control>>  arrowGroups,
		Control?                             selectedVisual,
		out Control?                         updatedSelectedVisual);
}
