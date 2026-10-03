namespace Shotora.App.Interfaces.Providers;

public interface IAppVersionProvider
{
	/// <summary>Human-readable version of the running application, e.g. "1.2.0".</summary>
	string Version { get; }
}
