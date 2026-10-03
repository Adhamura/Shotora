namespace Shotora.App.Interfaces.System;

public interface IUrlLauncherService
{
	/// <summary>Opens an http(s) or mailto URL with the system handler. Returns false when it could not be opened.</summary>
	bool Open(string url);
}
