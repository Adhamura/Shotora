namespace Shotora.App.Interfaces.System;

public interface IApplicationShutdownService
{
	/// <summary>Shuts the application down through the desktop lifetime (tray icon, windows and services are disposed).</summary>
	void Shutdown();
}
