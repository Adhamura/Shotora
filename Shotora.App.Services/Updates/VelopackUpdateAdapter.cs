using System.Diagnostics.CodeAnalysis;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;
using Velopack;
using Velopack.Sources;

namespace Shotora.App.Services.Updates;

[ExcludeFromCodeCoverage]
public class VelopackUpdateAdapter : IVelopackUpdateAdapter
{
	private readonly Lazy<UpdateManager?> _manager = new(CreateManager);
	private          UpdateInfo?          _pendingUpdate;
	private          bool                 _downloaded;

	public bool IsInstalled => _manager.Value?.IsInstalled == true;

	public string? InstalledVersion => IsInstalled ? _manager.Value?.CurrentVersion?.ToString() : null;

	public async Task<VelopackUpdateModel?> CheckForUpdatesAsync(CancellationToken cancellationToken)
	{
		var manager = RequireManager();
		cancellationToken.ThrowIfCancellationRequested();

		var update = await manager.CheckForUpdatesAsync().ConfigureAwait(false);
		_pendingUpdate = update;
		_downloaded    = false;

		if (update == null)
		{
			return null;
		}

		var target = update.TargetFullRelease;
		return new VelopackUpdateModel(target.Version.ToString(), target.NotesMarkdown);
	}

	public async Task DownloadUpdatesAsync(Action<int> progress, CancellationToken cancellationToken)
	{
		var manager = RequireManager();
		var update  = _pendingUpdate ?? throw new InvalidOperationException("No update has been found yet.");

		await manager.DownloadUpdatesAsync(update, progress, cancellationToken).ConfigureAwait(false);
		_downloaded = true;
	}

	public void ApplyUpdatesOnExitAndRestart()
	{
		var manager = RequireManager();
		if (_pendingUpdate == null || !_downloaded)
		{
			throw new InvalidOperationException("The update has not been downloaded.");
		}

		manager.WaitExitThenApplyUpdates(_pendingUpdate.TargetFullRelease, false, true, []);
	}

	private UpdateManager RequireManager()
	{
		var manager = _manager.Value;
		if (manager is not { IsInstalled: true })
		{
			throw new InvalidOperationException("Shotora is not installed via the Velopack installer.");
		}

		return manager;
	}

	private static UpdateManager? CreateManager()
	{
		try
		{
			var source = new GithubSource(UpdateConstants.RepositoryUrl, null, false);
			return new UpdateManager(source);
		}
		catch (Exception)
		{
			// Running from a dev build / unsupported layout: in-place updates are unavailable.
			return null;
		}
	}
}
