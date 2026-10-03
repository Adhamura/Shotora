using Shotora.App.Models.Updates;

namespace Shotora.App.Interfaces.Updates;

public interface IUpdateService
{
	string CurrentVersion { get; }

	UpdateStatus Status { get; }

	/// <summary>Download progress in percent (0-100) while <see cref="Status" /> is Downloading.</summary>
	int DownloadProgress { get; }

	UpdateCheckResult? LastResult { get; }

	/// <summary>Raised (on an arbitrary thread) whenever <see cref="Status" />, progress or the last result changes.</summary>
	event EventHandler? StateChanged;

	Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default);

	/// <summary>Downloads the available update. Returns false when nothing can be installed in place or the download failed.</summary>
	Task<bool> DownloadUpdateAsync(CancellationToken cancellationToken = default);

	/// <summary>Installs the downloaded update and restarts the app.</summary>
	void ApplyUpdateAndRestart();
}
