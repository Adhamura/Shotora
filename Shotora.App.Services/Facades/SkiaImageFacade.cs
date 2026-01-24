using System.Diagnostics.CodeAnalysis;
using Shotora.App.Interfaces.Facades;
using SkiaSharp;

namespace Shotora.App.Services.Facades;

[ExcludeFromCodeCoverage]
public class SkiaImageFacade : ISkiaImageFacade
{
	public void EncodeAndSaveToStream(SKBitmap bitmap, SKEncodedImageFormat format, int quality, Stream stream)
	{
		using var image = SKImage.FromBitmap(bitmap);
		using var data  = image.Encode(format, quality);
		data.SaveTo(stream);
	}
}
