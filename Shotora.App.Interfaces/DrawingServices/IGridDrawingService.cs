using Avalonia.Controls;
using Shotora.App.Models;

namespace Shotora.App.Interfaces.DrawingServices;

public interface IGridDrawingService
{
	void ApplyTranslation(Grid visual, AnnotationItem annotation, double dx, double dy, IReadOnlyList<Control>? arrowParts);
}
