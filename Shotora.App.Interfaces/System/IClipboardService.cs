using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace Shotora.App.Interfaces.System;

public interface IClipboardService
{
	void AttachTopLevel(TopLevel topLevel);

	Task SetImageAsync(Bitmap bitmap);
}
