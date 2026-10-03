using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;

namespace Shotora.App.Services.Updates;

public class UpdateCoordinator(
	IUpdateService         updateService,
	ISettingsSystemService settingsSystemService,
	TimeProvider           timeProvider) : IUpdateCoordinator
{
	/// <summary>How often the background loop re-evaluates the policy; the policy itself enforces the 24h interval.</summary>
	private static readonly TimeSpan PollInterval = TimeSpan.FromHours(1);

	private readonly CancellationTokenSource _cts = new();
	private          Task?                   _loop;

	public UpdateCoordinator(IUpdateService updateService, ISettingsSystemService settingsSystemService)
		: this(updateService, settingsSystemService, TimeProvider.System)
	{
	}

	public event EventHandler<UpdateCheckResult>? UpdateAvailable;

	public void Start()
	{
		_loop ??= Task.Run(() => RunLoopAsync(_cts.Token));
	}

	public async Task<UpdateCheckResult?> RunAutomaticCheckAsync(CancellationToken cancellationToken)
	{
		var settings = await settingsSystemService.LoadAsync().ConfigureAwait(false);
		var now      = timeProvider.GetUtcNow();
		if (!UpdatePolicy.ShouldRunAutomaticCheck(settings, now))
		{
			return null;
		}

		var result = await updateService.CheckForUpdatesAsync(cancellationToken).ConfigureAwait(false);
		if (result.Status == UpdateStatus.Failed)
		{
			// Offline or rate limited: try again on the next poll instead of waiting a full day.
			return result;
		}

		settings                    = await settingsSystemService.LoadAsync().ConfigureAwait(false);
		settings.LastUpdateCheckUtc = now;
		await settingsSystemService.SaveAsync(settings).ConfigureAwait(false);

		if (UpdatePolicy.ShouldPromptUser(result, settings, false))
		{
			UpdateAvailable?.Invoke(this, result);
		}

		return result;
	}

	public async Task SkipVersionAsync(string version)
	{
		var settings = await settingsSystemService.LoadAsync().ConfigureAwait(false);
		settings.SkippedUpdateVersion = VersionComparer.Normalize(version);
		await settingsSystemService.SaveAsync(settings).ConfigureAwait(false);
	}

	public void Dispose()
	{
		_cts.Cancel();
		_cts.Dispose();
		GC.SuppressFinalize(this);
	}

	private async Task RunLoopAsync(CancellationToken cancellationToken)
	{
		try
		{
			await Task.Delay(UpdateConstants.StartupCheckDelay, timeProvider, cancellationToken).ConfigureAwait(false);
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					await RunAutomaticCheckAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (Exception) when (!cancellationToken.IsCancellationRequested)
				{
					// Never let a background update check take the app down.
				}

				await Task.Delay(PollInterval, timeProvider, cancellationToken).ConfigureAwait(false);
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
