using System.Diagnostics;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Updates;

/// <summary>
///     Coordinates update checks against GitHub releases.
///     Velopack installations update in place (download, apply, restart); portable and dev builds
///     fall back to the GitHub releases API and point the user to the release page.
/// </summary>
public class UpdateService(
	IVelopackUpdateAdapter      velopackUpdateAdapter,
	IGitHubReleaseClient        gitHubReleaseClient,
	IAppVersionProvider         appVersionProvider,
	IApplicationShutdownService applicationShutdownService) : IUpdateService
{
	private readonly SemaphoreSlim            _downloadGate = new(1, 1);
	private readonly Lock                     _sync         = new();
	private          Task<UpdateCheckResult>? _inflightCheck;

	public string CurrentVersion => appVersionProvider.Version;

	public UpdateStatus Status { get; private set; } = UpdateStatus.Idle;

	public int DownloadProgress { get; private set; }

	public UpdateCheckResult? LastResult { get; private set; }

	public string? LastError { get; private set; }

	public event EventHandler? StateChanged;

	public Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
	{
		TaskCompletionSource<UpdateCheckResult> completion;
		lock (_sync)
		{
			// While an update is downloading or ready, a new check would only throw that progress away.
			if (Status is UpdateStatus.Downloading or UpdateStatus.ReadyToInstall or UpdateStatus.Installing && LastResult != null)
			{
				return Task.FromResult(LastResult);
			}

			if (_inflightCheck != null)
			{
				return _inflightCheck.WaitAsync(cancellationToken);
			}

			// Publish the shared task before the check starts so its cleanup can never race the assignment.
			completion     = new TaskCompletionSource<UpdateCheckResult>();
			_inflightCheck = completion.Task;
		}

		_ = RunCheckAsync(completion);
		return completion.Task.WaitAsync(cancellationToken);
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

		if (!await _downloadGate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
		{
			return false;
		}

		try
		{
			lock (_sync)
			{
				if (_inflightCheck != null)
				{
					return false;
				}

				DownloadProgress = 0;
				LastError        = null;
				SetState(UpdateStatus.Downloading);
			}

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
			Trace.WriteLine($"[Shotora] Update download failed: {ex}");
			DownloadProgress = 0;
			LastError        = ex.Message;
			SetState(UpdateStatus.Failed);
			return false;
		}
		finally
		{
			_downloadGate.Release();
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
			velopackUpdateAdapter.ApplyUpdatesOnExitAndRestart();
		}
		catch (Exception ex)
		{
			Trace.WriteLine($"[Shotora] Applying update failed: {ex}");
			LastError = ex.Message;
			SetState(UpdateStatus.Failed);
			throw;
		}

		// The updater waits for this process to exit, so shut down cleanly (tray icon, settings, windows).
		applicationShutdownService.Shutdown();
	}

	private async Task RunCheckAsync(TaskCompletionSource<UpdateCheckResult> completion)
	{
		UpdateCheckResult result;
		try
		{
			SetState(UpdateStatus.Checking);
			result = await CheckCoreAsync().ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			// CheckCoreAsync maps failures itself; this only guards against a throwing StateChanged subscriber.
			result = UpdateCheckResult.Failed(CurrentVersion, ex.Message);
		}

		LastResult = result;
		LastError  = result.Error;
		lock (_sync)
		{
			_inflightCheck = null;
		}

		try
		{
			SetState(result.Status);
		}
		finally
		{
			completion.TrySetResult(result);
		}
	}

	private async Task<UpdateCheckResult> CheckCoreAsync()
	{
		var current = CurrentVersion;

		if (velopackUpdateAdapter.IsInstalled)
		{
			try
			{
				var update = await velopackUpdateAdapter.CheckForUpdatesAsync(CancellationToken.None).ConfigureAwait(false);
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
			catch (Exception ex)
			{
				// The Velopack feed may be missing for this platform/channel; fall back to the GitHub API.
				Trace.WriteLine($"[Shotora] Velopack update check failed, falling back to GitHub API: {ex.Message}");
			}
		}

		try
		{
			var release = await gitHubReleaseClient.GetLatestReleaseAsync(CancellationToken.None).ConfigureAwait(false);
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
		catch (Exception ex)
		{
			Trace.WriteLine($"[Shotora] Update check failed: {ex.Message}");
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
