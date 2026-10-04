using Shotora.App.Models.System;

namespace Shotora.App.Interfaces.System;

public interface IScreenshotPortal
{
	Task<ScreenshotPortalResult> CaptureToFileAsync(CancellationToken cancellationToken = default);
}
