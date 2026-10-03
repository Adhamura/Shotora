namespace Shotora.App.Models.Updates;

/// <summary>
///     Outcome of a single update check against the GitHub releases feed.
/// </summary>
/// <param name="Status">UpToDate, UpdateAvailable or Failed.</param>
/// <param name="CurrentVersion">Version of the running application.</param>
/// <param name="LatestVersion">Newest published version, when known.</param>
/// <param name="ReleaseNotes">Markdown release notes of the newest version, when available.</param>
/// <param name="ReleaseUrl">Web page of the newest release.</param>
/// <param name="CanInstallInPlace">True when the update can be downloaded and applied by the app itself (Velopack install).</param>
/// <param name="Error">Failure description when <paramref name="Status" /> is Failed.</param>
public sealed record UpdateCheckResult(
	UpdateStatus Status,
	string       CurrentVersion,
	string?      LatestVersion     = null,
	string?      ReleaseNotes      = null,
	string?      ReleaseUrl        = null,
	bool         CanInstallInPlace = false,
	string?      Error             = null)
{
	public bool IsUpdateAvailable => Status == UpdateStatus.UpdateAvailable;

	public static UpdateCheckResult UpToDate(string currentVersion, string? latestVersion, string? releaseUrl)
	{
		return new UpdateCheckResult(UpdateStatus.UpToDate, currentVersion, latestVersion, ReleaseUrl: releaseUrl);
	}

	public static UpdateCheckResult Failed(string currentVersion, string error)
	{
		return new UpdateCheckResult(UpdateStatus.Failed, currentVersion, Error: error);
	}
}
