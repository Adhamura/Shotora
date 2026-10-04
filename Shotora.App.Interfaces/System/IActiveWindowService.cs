using Avalonia;

namespace Shotora.App.Interfaces.System;

public interface IActiveWindowService
{
	PixelRect? GetForegroundWindowBounds();
}
