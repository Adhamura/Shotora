using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Updates;

[ExcludeFromCodeCoverage]
public static class UpdateConstants
{
	public const string RepositoryOwner = "Adhamura";
	public const string RepositoryName  = "Shotora";
	public const string RepositoryUrl   = "https://github.com/" + RepositoryOwner + "/" + RepositoryName;
	public const string ReleasesUrl     = RepositoryUrl + "/releases";
	public const string LatestReleaseUrl = ReleasesUrl + "/latest";
	public const string LatestReleaseApiUrl = "https://api.github.com/repos/" + RepositoryOwner + "/" + RepositoryName + "/releases/latest";

	/// <summary>Minimum time between two automatic background checks.</summary>
	public static readonly TimeSpan AutomaticCheckInterval = TimeSpan.FromHours(24);

	/// <summary>Delay after startup before the first automatic check, so it never competes with app start.</summary>
	public static readonly TimeSpan StartupCheckDelay = TimeSpan.FromSeconds(20);
}
