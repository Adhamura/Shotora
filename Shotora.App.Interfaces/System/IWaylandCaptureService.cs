using SkiaSharp;

namespace Shotora.App.Interfaces.System;

public interface IWaylandCaptureService
{
	bool IsWaylandSession();

	Task<SKBitmap> CaptureDesktopAsync(CancellationToken cancellationToken = default);
}
