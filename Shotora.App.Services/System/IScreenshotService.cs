using Avalonia;
using Shotora.App.Models.ItemModels;

namespace Shotora.App.Services.System;

public interface IScreenshotService
{
	Task<CaptureResultItemModel?> CaptureAsync(PixelRect bounds);
}
