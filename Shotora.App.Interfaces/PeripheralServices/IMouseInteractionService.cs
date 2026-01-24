using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Shotora.App.Interfaces.PeripheralServices;

public interface IMouseInteractionService
{
	MouseButton? ParseMouseButton(string? text,          MouseButton?     fallback);
	MouseButton? GetReleasedButton(Canvas OverlayCanvas, PointerEventArgs e);

	WindowEdge? GetResizeEdge(Window window, Point position, double borderThickness = 6);

	StandardCursorType GetResizeCursor(WindowEdge edge);
}
