using Avalonia;
using Avalonia.Media;
using Shotora.App.Models;
using Shotora.App.Models.Enums;

namespace Shotora.App.Interfaces.Builders;

public interface IAnnotationItemBuilder
{
	AnnotationItem BuildFreehand(AnnotationToolType    tool,  Color              color, float thickness, float opacity,      IEnumerable<Point> points);
	AnnotationItem BuildBounds(OverlayInteractionState state, AnnotationToolType tool,  Rect  bounds,    bool  fill = false, float              arrowHeadSize = 0);
	AnnotationItem CloneAnnotation(AnnotationItem      source);
}
