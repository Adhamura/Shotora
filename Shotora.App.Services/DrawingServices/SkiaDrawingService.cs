using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Models.Drawings;
using Shotora.App.Models.Enums;
using SkiaSharp;
using Bitmap=Avalonia.Media.Imaging.Bitmap;
using DrawingColor=System.Drawing.Color;
using AvaloniaImage=Avalonia.Controls.Image;
using AvaloniaPoint=Avalonia.Point;

namespace Shotora.App.Services.DrawingServices;

public class SkiaDrawingService(IRectangleDrawingService _rectangles) : ISkiaDrawingService
{
	public SKColor ToSkColor(DrawingColor color, float opacity = 1f)
	{
		var alpha = (byte)Math.Clamp(color.A * opacity, 0, 255);
		return new SKColor(color.R, color.G, color.B, alpha);
	}

	public void DrawProcessedRegion(SKCanvas canvas, SKBitmap baseBitmap, AnnotationItem annotation)
	{
		var bounds = new Rect(annotation.Bounds.X, annotation.Bounds.Y, annotation.Bounds.Width, annotation.Bounds.Height);
		if (!TryProcessRegion(
				baseBitmap,
				bounds,
				subset => annotation.Tool == AnnotationToolType.Blur
					? CreateBlurredBitmap(subset)
					: CreatePixelatedBitmap(subset, Math.Max(4, (int)Math.Round(annotation.Thickness * 2))),
				out var processed,
				out var region))
		{
			return;
		}

		using (processed)
		{
			var dest = new SKRect(region.Left, region.Top, region.Right, region.Bottom);
			canvas.DrawBitmap(processed, dest);
		}
	}

	public EffectPreview CreateEffectPreview(AvaloniaPoint position, IBrush strokeBrush, double thickness)
	{
		var grid = new Grid
		{
			IsHitTestVisible = false
		};
		var image = new AvaloniaImage
		{
			Stretch = Stretch.Fill
		};
		var border = new Rectangle
		{
			Stroke          = strokeBrush,
			StrokeThickness = thickness,
			Fill            = Brushes.Transparent,
			Opacity         = 0.9
		};
		grid.Children.Add(image);
		grid.Children.Add(border);
		Canvas.SetLeft(grid, position.X);
		Canvas.SetTop(grid, position.Y);
		return new EffectPreview(grid, image, border);
	}

	public bool UpdateEffectPreviewBounds(Grid grid, Rect bounds)
	{
		if (bounds is
			{
				Width: > 0, Height: > 0
			})
		{
			grid.Width  = bounds.Width;
			grid.Height = bounds.Height;
			Canvas.SetLeft(grid, bounds.X);
			Canvas.SetTop(grid, bounds.Y);
			grid.IsVisible = true;
			return true;
		}

		grid.IsVisible = false;
		return false;
	}

	public void UpdateEffectPreview(AvaloniaImage? image, SKBitmap? captureRaw, Rect bounds, OverlayTool tool, double thickness)
	{
		if (image == null || captureRaw == null)
		{
			return;
		}

		try
		{
			if (!TryProcessRegion(
					captureRaw,
					bounds,
					subset => tool == OverlayTool.Blur
						? CreateBlurredBitmap(subset)
						: CreatePixelatedBitmap(subset, Math.Max(4, (int)Math.Round(thickness * 2))),
					out var processed,
					out _))
			{
				return;
			}

			using (processed)
			{
				image.Source = ConvertToAvaloniaBitmap(processed);
			}
		}
		catch
		{
		}
	}

	private static SKBitmap CreateBlurredBitmap(SKBitmap source)
	{
		var       output = new SKBitmap(source.Info);
		using var canvas = new SKCanvas(output);
		using var image  = SKImage.FromBitmap(source);
		using var paint = new SKPaint
		{
			IsAntialias   = true,
			FilterQuality = SKFilterQuality.High,
			ImageFilter   = SKImageFilter.CreateBlur(4f, 4f)
		};
		canvas.DrawImage(image, 0, 0, paint);
		return output;
	}

	private static SKBitmap CreatePixelatedBitmap(SKBitmap source, int pixelSize)
	{
		pixelSize = Math.Max(1, pixelSize);
		var       reducedInfo = new SKImageInfo(Math.Max(1, source.Width / pixelSize), Math.Max(1, source.Height / pixelSize), source.ColorType, source.AlphaType);
		using var reduced     = source.Resize(reducedInfo, SKFilterQuality.None);
		if (reduced == null)
		{
			return source.Copy();
		}

		var       output   = new SKBitmap(source.Info);
		using var canvas   = new SKCanvas(output);
		var       destRect = new SKRect(0, 0, source.Width, source.Height);
		using var paint = new SKPaint
		{
			FilterQuality = SKFilterQuality.None,
			IsAntialias   = false
		};
		canvas.DrawBitmap(reduced, destRect, paint);
		return output;
	}

	private static Bitmap ConvertToAvaloniaBitmap(SKBitmap bitmap)
	{
		using var image = SKImage.FromBitmap(bitmap);
		using var data  = image.Encode(SKEncodedImageFormat.Png, 100);
		using var ms    = new MemoryStream();
		data.SaveTo(ms);
		ms.Position = 0;
		return new Bitmap(ms);
	}

	private static SKRectI ToRegion(Rect bounds)
	{
		return new SKRectI(
			(int)Math.Round(bounds.X),
			(int)Math.Round(bounds.Y),
			(int)Math.Round(bounds.Right),
			(int)Math.Round(bounds.Bottom));
	}

	private bool TryProcessRegion(
		SKBitmap                 source,
		Rect                     bounds,
		Func<SKBitmap, SKBitmap> processor,
		out SKBitmap             processed,
		out SKRectI              region)
	{
		region = ToRegion(bounds);
		var full = new SKRectI(0, 0, source.Width, source.Height);
		region = _rectangles.IntersectRect(region, full);
		if (region.Width <= 0 || region.Height <= 0)
		{
			processed = null!;
			return false;
		}

		using var subset = new SKBitmap();
		if (!source.ExtractSubset(subset, region))
		{
			processed = null!;
			return false;
		}

		processed = processor(subset);
		return true;
	}
}
