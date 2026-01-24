using Avalonia.Controls;
using Shotora.App.Models;
using SkiaSharp;

namespace Shotora.App.Interfaces.DrawingServices;

public interface ISaveDrawingService
{
	Task<bool> SaveSelectionAsync(Window          owner, AppSettings settings, Func<SKBitmap?> buildCropped, bool prompt = true);
	Task<bool> CopySelectionAsync(Func<SKBitmap?> buildCropped);
}
