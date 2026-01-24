namespace Shotora.App.Interfaces.Abstractions;

public interface ITrayService : IDisposable
{
	void Initialize(
		Action captureRegion,
		Action captureFull,
		Action showSettings,
		Action showAbout,
		Action exitApp);

	void RefreshLocalization();
	void RefreshTheme(string themeName);
}
