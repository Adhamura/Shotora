using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Shotora.App.Interfaces.Ocr;
using Shotora.App.Views;

namespace Shotora.App.Services;

[ExcludeFromCodeCoverage]
public class OcrResultPresenter : IOcrResultPresenter
{
	private OcrResultWindow? _window;

	public void Present(string text, Rect anchor)
	{
		if (_window == null)
		{
			_window        =  new OcrResultWindow(text, anchor);
			_window.Closed += (_, _) => _window = null;
			_window.Show();
			_window.Activate();
			return;
		}

		_window.UpdateResult(text, anchor);
		if (!_window.IsVisible)
		{
			_window.Show();
		}

		_window.Activate();
	}

	public void Close()
	{
		_window?.Close();
		_window = null;
	}
}
