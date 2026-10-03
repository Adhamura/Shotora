using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

public interface IUpdateService
{
	string CurrentVersion { get; }

	UpdateStatus Status { get; }

	/// <summary>Download progress in percent (0-100) while <see cref="Status" /> is Downloading.</summary>
	int DownloadProgress { get; }

	/// <summary>Outcome of the most recent completed check.</summary>
	UpdateCheckResult? LastResult { get; }

	/// <summary>Technical description of the last failure (check, download or install); null after a success.</summary>
	string? LastError { get; }

	/// <summary>Raised (on an arbitrary thread) whenever <see cref="Status" />, progress or the last result changes.</summary>
	event EventHandler? StateChanged;

	/// <summary>
	///     Checks GitHub for a newer release. Concurrent callers share the in-flight check;
	///     cancelling only stops waiting, the shared check keeps running.
	/// </summary>
	Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default);

	/// <summary>Downloads the available update. Returns false when nothing can be installed in place or the download failed.</summary>
	Task<bool> DownloadUpdateAsync(CancellationToken cancellationToken = default);

	/// <summary>Hands the downloaded update to the installer and shuts the app down; the installer restarts it.</summary>
	void ApplyUpdateAndRestart();
}
