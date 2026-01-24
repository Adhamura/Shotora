using Avalonia.Controls;
using Avalonia.Input.Platform;
using Shotora.App.Interfaces.System;
using Bitmap=Avalonia.Media.Imaging.Bitmap;

namespace Shotora.App.Services.System;

public class ClipboardService : IClipboardService
{
	private Bitmap?       _pinnedBitmap;
	private MemoryStream? _pinnedImageStream;
	private TopLevel?     _topLevel;

	public void AttachTopLevel(TopLevel topLevel)
	{
		_topLevel = topLevel;
	}

	public async Task SetImageAsync(Bitmap bitmap)
	{
		if (_topLevel?.Clipboard is
			{
			} cb)
		{
			_pinnedBitmap?.Dispose();
			_pinnedImageStream?.Dispose();

			var stream = new MemoryStream();
			bitmap.Save(stream);
			stream.Position = 0;

			var ownedBitmap = new Bitmap(stream);
			_pinnedImageStream = stream;
			_pinnedBitmap      = ownedBitmap;

			await cb.SetBitmapAsync(ownedBitmap);
		}
	}
}
