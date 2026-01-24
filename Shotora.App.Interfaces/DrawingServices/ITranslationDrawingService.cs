using Avalonia;
using Avalonia.Controls;
using Shotora.App.Models;

namespace Shotora.App.Interfaces.DrawingServices;

public interface ITranslationDrawingService
{
	Vector GetTranslation(Control        control);
	bool   ApplyShapeTranslation(Control visual, AnnotationItem annotation, IReadOnlyDictionary<Control, List<Control>> arrowGroups);
}
