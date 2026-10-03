using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

/// <summary>
///     Thin wrapper over Velopack's UpdateManager so the update flow can be unit tested.
/// </summary>
public interface IVelopackUpdateAdapter
{
	/// <summary>True when the app runs from a Velopack installation and can update itself in place.</summary>
	bool IsInstalled { get; }

	/// <summary>Installed package version, or null when not installed via Velopack.</summary>
	string? InstalledVersion { get; }

	/// <summary>Checks the GitHub releases feed. Returns null when already up to date.</summary>
	Task<VelopackUpdateModel?> CheckForUpdatesAsync(CancellationToken cancellationToken);

	/// <summary>Downloads the update found by the last successful check.</summary>
	Task DownloadUpdatesAsync(Action<int> progress, CancellationToken cancellationToken);

	/// <summary>
	///     Launches the Velopack updater, which waits for this process to exit, applies the downloaded update
	///     and restarts Shotora. The caller must then shut the app down.
	/// </summary>
	void ApplyUpdatesOnExitAndRestart();
}
