using Avalonia;

namespace Shotora.App.Interfaces.Ocr;

public interface IOcrResultPresenter
{
	void Present(string text, Rect anchor);
	void Close();
}
