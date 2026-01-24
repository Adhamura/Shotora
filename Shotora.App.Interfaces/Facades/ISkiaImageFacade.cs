using SkiaSharp;

namespace Shotora.App.Interfaces.Facades;

public interface ISkiaImageFacade
{
	void EncodeAndSaveToStream(SKBitmap bitmap, SKEncodedImageFormat format, int quality, Stream stream);
}
