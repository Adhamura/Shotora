using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Updates;

/// <summary>
///     Coordinates update checks against GitHub releases.
///     Velopack installations update in place (download, apply, restart); portable and dev builds
///     fall back to the GitHub releases API and point the user to the release page.
/// </summary>
public class UpdateService(
	IVelopackUpdateAdapter velopackUpdateAdapter,
	IGitHubReleaseClient   gitHubReleaseClient,
	IAppVersionProvider    appVersionProvider) : IUpdateService
{
	private readonly SemaphoreSlim _gate = new(1, 1);

	public string CurrentVersion => appVersionProvider.Version;

	public UpdateStatus Status { get; private set; } = UpdateStatus.Idle;

	public int DownloadProgress { get; private set; }

	public UpdateCheckResult? LastResult { get; private set; }

	public event EventHandler? StateChanged;

	public async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
	{
		if (!await _gate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
		{
			// A check or download is already running; report the state we know about.
			return LastResult ?? new UpdateCheckResult(Status, CurrentVersion);
		}

		try
		{
			if (Status is UpdateStatus.ReadyToInstall && LastResult != null)
			{
				return LastResult;
			}

			SetState(UpdateStatus.Checking);
			var result = await CheckCoreAsync(cancellationToken).ConfigureAwait(false);
			LastResult = result;
			SetState(result.Status);
			return result;
		}
		catch (OperationCanceledException)
		{
			SetState(LastResult?.Status ?? UpdateStatus.Idle);
			throw;
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task<bool> DownloadUpdateAsync(CancellationToken cancellationToken = default)
	{
		if (LastResult is not { IsUpdateAvailable: true, CanInstallInPlace: true })
		{
			return false;
		}

		if (Status == UpdateStatus.ReadyToInstall)
		{
			return true;
		}

		if (!await _gate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
		{
			return false;
		}

		try
		{
			DownloadProgress = 0;
			SetState(UpdateStatus.Downloading);

			await velopackUpdateAdapter.DownloadUpdatesAsync(ReportProgress, cancellationToken).ConfigureAwait(false);

			DownloadProgress = 100;
			SetState(UpdateStatus.ReadyToInstall);
			return true;
		}
		catch (OperationCanceledException)
		{
			DownloadProgress = 0;
			SetState(UpdateStatus.UpdateAvailable);
			throw;
		}
		catch (Exception ex)
		{
			DownloadProgress = 0;
			LastResult       = LastResult with { Error = ex.Message };
			SetState(UpdateStatus.Failed);
			return false;
		}
		finally
		{
			_gate.Release();
		}
	}

	public void ApplyUpdateAndRestart()
	{
		if (Status != UpdateStatus.ReadyToInstall)
		{
			throw new InvalidOperationException("No downloaded update is ready to install.");
		}

		SetState(UpdateStatus.Installing);
		try
		{
			velopackUpdateAdapter.ApplyUpdatesAndRestart();
		}
		catch (Exception ex)
		{
			LastResult = (LastResult ?? new UpdateCheckResult(UpdateStatus.Failed, CurrentVersion)) with { Error = ex.Message };
			SetState(UpdateStatus.Failed);
			throw;
		}
	}

	private async Task<UpdateCheckResult> CheckCoreAsync(CancellationToken cancellationToken)
	{
		var current = CurrentVersion;

		if (velopackUpdateAdapter.IsInstalled)
		{
			try
			{
				var update = await velopackUpdateAdapter.CheckForUpdatesAsync(cancellationToken).ConfigureAwait(false);
				if (update == null)
				{
					return UpdateCheckResult.UpToDate(current, current, UpdateConstants.LatestReleaseUrl);
				}

				return new UpdateCheckResult(
					UpdateStatus.UpdateAvailable,
					current,
					VersionComparer.Normalize(update.Version),
					update.ReleaseNotes,
					UpdateConstants.LatestReleaseUrl,
					CanInstallInPlace: true);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception)
			{
				// The Velopack feed may be missing for this platform/channel; fall back to the GitHub API.
			}
		}

		try
		{
			var release = await gitHubReleaseClient.GetLatestReleaseAsync(cancellationToken).ConfigureAwait(false);
			if (release == null)
			{
				return UpdateCheckResult.UpToDate(current, null, UpdateConstants.ReleasesUrl);
			}

			var latest = VersionComparer.Normalize(release.TagName);
			if (!VersionComparer.IsNewer(release.TagName, current))
			{
				return UpdateCheckResult.UpToDate(current, latest, release.HtmlUrl);
			}

			return new UpdateCheckResult(
				UpdateStatus.UpdateAvailable,
				current,
				latest,
				release.Body,
				release.HtmlUrl);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex)
		{
			return UpdateCheckResult.Failed(current, ex.Message);
		}
	}

	private void ReportProgress(int percent)
	{
		var clamped = Math.Clamp(percent, 0, 100);
		if (clamped == DownloadProgress)
		{
			return;
		}

		DownloadProgress = clamped;
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void SetState(UpdateStatus status)
	{
		Status = status;
		StateChanged?.Invoke(this, EventArgs.Empty);
	}
}
