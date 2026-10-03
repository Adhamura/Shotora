using System.Reflection;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Providers;

public class AppVersionProvider(IVelopackUpdateAdapter velopackUpdateAdapter) : IAppVersionProvider
{
	private string? _version;

	public string Version => _version ??= Resolve(velopackUpdateAdapter.InstalledVersion, Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

	public static string Resolve(string? installedVersion, Assembly assembly)
	{
		if (!string.IsNullOrWhiteSpace(installedVersion))
		{
			return VersionComparer.Normalize(installedVersion);
		}

		var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		if (VersionComparer.TryParse(informational, out _, out _))
		{
			return VersionComparer.Normalize(informational);
		}

		var version = assembly.GetName().Version;
		return version == null ? "1.0.0" : $"{version.Major}.{version.Minor}.{Math.Max(version.Build, 0)}";
	}
}
