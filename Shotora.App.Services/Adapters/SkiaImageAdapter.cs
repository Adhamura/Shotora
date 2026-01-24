using Avalonia;
using Shotora.App.Interfaces.Adapters;
using SkiaSharp;

namespace Shotora.App.Services.Adapters;

public class SkiaImageAdapter : ISkiaImageAdapter
{
	public byte[]? ToPngBytes(SKBitmap source, Rect rect)
	{
		var x      = Math.Max(0, (int)Math.Floor(rect.X));
		var y      = Math.Max(0, (int)Math.Floor(rect.Y));
		var width  = Math.Min(source.Width  - x, (int)Math.Ceiling(rect.Width));
		var height = Math.Min(source.Height - y, (int)Math.Ceiling(rect.Height));
		if (width <= 0 || height <= 0)
		{
			return null;
		}

		var       subsetRect = new SKRectI(x, y, x + width, y + height);
		using var subset     = new SKBitmap();
		if (!source.ExtractSubset(subset, subsetRect))
		{
			return null;
		}

		using var image = SKImage.FromBitmap(subset);
		using var data  = image.Encode(SKEncodedImageFormat.Png, 100);
		return data.ToArray();
	}
}