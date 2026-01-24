using Avalonia;
using SkiaSharp;

namespace Shotora.App.Interfaces.Adapters;

public interface ISkiaImageAdapter
{
	byte[]? ToPngBytes(SKBitmap source, Rect rect);
}
