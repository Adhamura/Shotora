using System.Diagnostics.CodeAnalysis;
using Avalonia;
using SkiaSharp;
using AvaloniaBitmap=Avalonia.Media.Imaging.Bitmap;

namespace Shotora.App.Models.ItemModels;

[ExcludeFromCodeCoverage]
public sealed class CaptureResultItemModel : IDisposable
{
	public SKBitmap       Raw     { get; init; }
	public AvaloniaBitmap Display { get; init; }
	public PixelRect      Bounds  { get; set; }

	public void Dispose()
	{
		Raw.Dispose();
		Display.Dispose();
	}
}
