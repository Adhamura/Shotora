using Avalonia;
using Avalonia.Controls;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Interfaces.Providers;

public interface ITrayMenuWindowService
{
	TrayMenuWindowModel CreateWindow(
		Action onCaptureRegion,
		Action onCaptureFull,
		Action onSettings,
		Action onAbout,
		Action onCheckForUpdates,
		Action onExit,
		string themeName);
	void     UpdateLabels(TrayMenuWindowModel model);
	void     ApplyTheme(TrayMenuWindowModel   model, string     themeName);
	void     ShowAt(TrayMenuWindowModel       model, PixelPoint anchor);
	void     Hide(TrayMenuWindowModel         model);
	void     Close(TrayMenuWindowModel        model);
	bool     IsVisible(TrayMenuWindowModel    model);
	Screens? GetScreens(TrayMenuWindowModel   model);
	void     ExecuteOnUiThread(Action?        action);
}
