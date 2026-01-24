using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
using NativeSupport.Enums;
using Shared.Models.Enums;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.AtomModels;
using Shotora.App.Models.ItemModels;
using SkiaSharp;

namespace Shotora.App.Services.System;

public class ScreenshotService(IProcessSystemService processSystemService) : IScreenshotService
{
	public Task<CaptureResultItemModel?> CaptureAsync(PixelRect bounds)
	{
		return Task.Run(() =>
		{
			if (processSystemService.GetCurrentOs() == RuntimeOs.Windows)
			{
				return CaptureWindows(bounds);
			}

			if (processSystemService.GetCurrentOs() == RuntimeOs.Mac)
			{
				return CaptureMac(bounds);
			}

			return processSystemService.GetCurrentOs() == RuntimeOs.Linux ? CaptureLinux(bounds) : null;
		});
	}

	[SupportedOSPlatform("windows")]
	private static CaptureResultItemModel? CaptureWindows(PixelRect bounds)
	{
		try
		{
			using var raw = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
			using (var g = Graphics.FromImage(raw))
			{
				g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, raw.Size, CopyPixelOperation.SourceCopy);
			}

			using var ms = new MemoryStream();
			raw.Save(ms, ImageFormat.Png);
			var bytes = ms.ToArray();

			var skBitmap = SKBitmap.Decode(bytes);
			if (skBitmap == null)
			{
				return null;
			}

			using var displayStream = new MemoryStream(bytes);
			var       display       = new Avalonia.Media.Imaging.Bitmap(displayStream);

			return new CaptureResultItemModel
			{
				Raw     = skBitmap,
				Display = display,
				Bounds  = bounds
			};
		}
		catch
		{
			return null;
		}
	}

	[SupportedOSPlatform("macos")]
	private static CaptureResultItemModel? CaptureMac(PixelRect bounds)
	{
		var image   = IntPtr.Zero;
		var cfBytes = IntPtr.Zero;
		try
		{
			var rect = new ImageRectangleModel
			{
				Location = new LocationPointModel
				{
					X = bounds.X,
					Y = bounds.Y
				},
				SizeModel = new ImageSizeModel
				{
					width  = bounds.Width,
					height = bounds.Height
				}
			};

			image = CGWindowListCreateImage(rect, (uint)CGWindowListOption.OnScreenOnly, 0, (uint)CGWindowImageOption.BoundsIgnoreFraming);
			if (image == IntPtr.Zero)
			{
				return null;
			}

			var width       = CGImageGetWidth(image);
			var height      = CGImageGetHeight(image);
			var bitsPerPix  = CGImageGetBitsPerPixel(image);
			var bytesPerRow = CGImageGetBytesPerRow(image);

			if (width <= 0 || height <= 0 || bitsPerPix is not (32 or 24))
			{
				return null;
			}

			var provider = CGImageGetDataProvider(image);
			if (provider == IntPtr.Zero)
			{
				return null;
			}

			cfBytes = CGDataProviderCopyData(provider);
			if (cfBytes == IntPtr.Zero)
			{
				return null;
			}

			var length = (int)CFDataGetLength(cfBytes);
			if (length <= 0)
			{
				return null;
			}

			var srcPtr  = CFDataGetBytePtr(cfBytes);
			var srcData = new byte[length];
			Marshal.Copy(srcPtr, srcData, 0, length);

			var       info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
			using var bmp  = new SKBitmap(info);

			var srcStride = bytesPerRow;
			var dstStride = info.RowBytes;
			var rowSize   = Math.Min(srcStride, dstStride);

			var span = srcData.AsSpan();
			for (var y = 0; y < height; y++)
			{
				var srcIndex = y * srcStride;
				var dstPtr   = bmp.GetPixels();
				var rowPtr   = IntPtr.Add(dstPtr, dstStride * y);
				Marshal.Copy(srcData, srcIndex, rowPtr, rowSize);
			}

			using var encoded = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 100);
			using var ms      = new MemoryStream();
			encoded.SaveTo(ms);
			var bytes = ms.ToArray();

			var skCopy = SKBitmap.Decode(bytes);
			if (skCopy == null)
			{
				return null;
			}

			using var displayStream = new MemoryStream(bytes);
			var       displayBitmap = new Avalonia.Media.Imaging.Bitmap(displayStream);

			return new CaptureResultItemModel
			{
				Raw     = skCopy,
				Display = displayBitmap,
				Bounds  = bounds
			};
		}
		catch
		{
			return null;
		}
		finally
		{
			if (cfBytes != IntPtr.Zero)
			{
				CFRelease(cfBytes);
			}
			if (image != IntPtr.Zero)
			{
				CGImageRelease(image);
			}
		}
	}

	[SupportedOSPlatform("linux")]
	private static CaptureResultItemModel? CaptureLinux(PixelRect bounds)
	{
		var waylandDisplay = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
		var xdgSessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
		var isWayland      = !string.IsNullOrEmpty(waylandDisplay) || xdgSessionType?.Equals("wayland", StringComparison.OrdinalIgnoreCase) == true;

		if (isWayland)
		{
			throw new InvalidOperationException(
				"Screen capture is not supported under Wayland. Please run the application with X11 backend " +
				"by setting the environment variable: WAYLAND_DISPLAY= (empty) or XDG_SESSION_TYPE=x11, "     +
				"or use 'GDK_BACKEND=x11' before launching the application.");
		}

		var display  = IntPtr.Zero;
		var imagePtr = IntPtr.Zero;
		try
		{
			display = XOpenDisplay(IntPtr.Zero);
			if (display == IntPtr.Zero)
			{
				throw new InvalidOperationException(
					"Failed to open X11 display. Ensure X11 is running and DISPLAY environment variable is set. " +
					"If using Wayland, X11 screen capture is not supported.");
			}

			var root = XDefaultRootWindow(display);
			imagePtr = XGetImage(display, root, bounds.X, bounds.Y, (uint)bounds.Width, (uint)bounds.Height, ulong.MaxValue, ZPixmap);
			if (imagePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException(
					"Failed to capture screen image via X11. This may occur if running under Wayland " +
					"or if the application lacks permission to capture the screen.");
			}

			var image = Marshal.PtrToStructure<ImageModel>(imagePtr);
			if (image.bits_per_pixel is not (16 or 24 or 32))
			{
				throw new InvalidOperationException(
					$"Unsupported pixel format: {image.bits_per_pixel} bits per pixel. Only 16, 24, and 32 bpp are supported.");
			}

			var       info     = new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
			using var skBitmap = new SKBitmap(info);

			var sourceBuffer = new byte[image.bytes_per_line * image.height];
			Marshal.Copy(image.data, sourceBuffer, 0, sourceBuffer.Length);

			var destPtr       = skBitmap.GetPixels();
			var bytesPerPixel = Math.Max(1, (image.bits_per_pixel + 7) / 8);
			var byteOrderMsb  = image.byte_order != 0;
			var redShift      = GetMaskShift(image.red_mask);
			var greenShift    = GetMaskShift(image.green_mask);
			var blueShift     = GetMaskShift(image.blue_mask);
			var redBits       = GetMaskBitCount(image.red_mask);
			var greenBits     = GetMaskBitCount(image.green_mask);
			var blueBits      = GetMaskBitCount(image.blue_mask);
			for (var y = 0; y < bounds.Height; y++)
			{
				var destRowPtr  = IntPtr.Add(destPtr, info.RowBytes * y);
				var srcRowIndex = image.bytes_per_line * y;

				if (image.bits_per_pixel == 32 && image.bytes_per_line >= info.RowBytes)
				{
					Marshal.Copy(sourceBuffer, srcRowIndex, destRowPtr, info.RowBytes);
				}
				else
				{
					var row = new byte[info.RowBytes];
					for (var x = 0; x < bounds.Width; x++)
					{
						var srcIdx = srcRowIndex + x * bytesPerPixel;
						var dstIdx = x * 4;
						if (srcIdx + bytesPerPixel > sourceBuffer.Length)
						{
							break;
						}

						var pixel = ReadPixel(sourceBuffer, srcIdx, bytesPerPixel, byteOrderMsb);
						row[dstIdx + 2] = ScaleMasked(pixel, image.red_mask,   redShift,   redBits);
						row[dstIdx + 1] = ScaleMasked(pixel, image.green_mask, greenShift, greenBits);
						row[dstIdx + 0] = ScaleMasked(pixel, image.blue_mask,  blueShift,  blueBits);
						row[dstIdx + 3] = 255;
					}

					Marshal.Copy(row, 0, destRowPtr, info.RowBytes);
				}
			}

			using var imageEncoded  = SKImage.FromBitmap(skBitmap).Encode(SKEncodedImageFormat.Png, 100);
			using var managedStream = new MemoryStream();
			imageEncoded.SaveTo(managedStream);
			var bytes = managedStream.ToArray();

			var skCopy = SKBitmap.Decode(bytes);
			if (skCopy == null)
			{
				return null;
			}

			using var displayStream = new MemoryStream(bytes);
			var       displayBitmap = new Avalonia.Media.Imaging.Bitmap(displayStream);

			return new CaptureResultItemModel
			{
				Raw     = skCopy,
				Display = displayBitmap,
				Bounds  = bounds
			};
		}
		catch (InvalidOperationException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to capture screen on Linux: {ex.Message}", ex);
		}
		finally
		{
			if (imagePtr != IntPtr.Zero)
			{
				XDestroyImage(imagePtr);
			}

			if (display != IntPtr.Zero)
			{
				XCloseDisplay(display);
			}
		}
	}

	private static uint ReadPixel(byte[] buffer, int startIndex, int bytesPerPixel, bool msbFirst)
	{
		uint value = 0;
		if (msbFirst)
		{
			for (var i = 0; i < bytesPerPixel; i++)
			{
				value = value << 8 | buffer[startIndex + i];
			}
		}
		else
		{
			for (var i = 0; i < bytesPerPixel; i++)
			{
				value |= (uint)buffer[startIndex + i] << 8 * i;
			}
		}

		return value;
	}

	private static int GetMaskShift(uint mask)
	{
		if (mask == 0)
		{
			return 0;
		}

		var shift = 0;
		while ((mask & 1) == 0)
		{
			mask >>= 1;
			shift++;
		}

		return shift;
	}

	private static int GetMaskBitCount(uint mask)
	{
		var count = 0;
		while (mask != 0)
		{
			count +=  (int)(mask & 1);
			mask  >>= 1;
		}

		return count;
	}

	private static byte ScaleMasked(uint pixel, uint mask, int shift, int bits)
	{
		if (mask == 0 || bits <= 0)
		{
			return 0;
		}

		var value = (pixel & mask) >> shift;
		var max   = (1u << bits) - 1;
		if (max == 0)
		{
			return 0;
		}

		return (byte)Math.Min(255, Math.Round(value * 255.0 / max));
	}

	#region macOS interop

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGWindowListCreateImage(ImageRectangleModel screenRect, uint windowOption, uint windowID, uint imageOption);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetWidth(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetHeight(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetBitsPerPixel(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern int CGImageGetBytesPerRow(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGImageGetDataProvider(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern void CGImageRelease(IntPtr image);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGDataProviderCopyData(IntPtr provider);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern IntPtr CFDataGetBytePtr(IntPtr data);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern nint CFDataGetLength(IntPtr data);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern void CFRelease(IntPtr cf);

	#endregion

	#region X11 interop

	private const int ZPixmap = 2;

	[DllImport("libX11.so.6")] private static extern IntPtr XOpenDisplay(IntPtr display_name);

	[DllImport("libX11.so.6")] private static extern void XCloseDisplay(IntPtr display);

	[DllImport("libX11.so.6")] private static extern IntPtr XDefaultRootWindow(IntPtr display);

	[DllImport("libX11.so.6")] private static extern IntPtr XGetImage(IntPtr     display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format);
	[DllImport("libX11.so.6")] private static extern void   XDestroyImage(IntPtr image);

	#endregion
}
