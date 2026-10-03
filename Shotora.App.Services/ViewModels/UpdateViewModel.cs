using CommunityToolkit.Mvvm.Input;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using Shotora.App.Models.Updates;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Services.ViewModels;

/// <summary>
///     Presentation state for the update panel (About window) and the update dialog.
///     All state lives in <see cref="IUpdateService" />; this view model only projects it.
/// </summary>
public sealed class UpdateViewModel : ViewModelBase, IDisposable
{
	private readonly ILocalizationProvider _localization;
	private readonly IUpdateCoordinator    _coordinator;
	private readonly IUpdateService        _updateService;
	private readonly IUrlLauncherService   _urlLauncher;
	private readonly Action<Action>        _uiDispatch;

	public UpdateViewModel(
		IUpdateService        updateService,
		IUpdateCoordinator    coordinator,
		ILocalizationProvider localization,
		IUrlLauncherService   urlLauncher)
		: this(updateService, coordinator, localization, urlLauncher, PostToUiThread)
	{
	}

	public UpdateViewModel(
		IUpdateService        updateService,
		IUpdateCoordinator    coordinator,
		ILocalizationProvider localization,
		IUrlLauncherService   urlLauncher,
		Action<Action>        uiDispatch)
	{
		_updateService = updateService;
		_coordinator   = coordinator;
		_localization  = localization;
		_urlLauncher   = urlLauncher;
		_uiDispatch    = uiDispatch;

		CheckCommand           = new AsyncRelayCommand(CheckAsync,    () => !IsBusy);
		InstallCommand         = new AsyncRelayCommand(DownloadAsync, () => CanDownload);
		RestartCommand         = new RelayCommand(Restart, () => IsReadyToInstall);
		OpenReleasePageCommand = new RelayCommand(OpenReleasePage);
		SkipVersionCommand     = new AsyncRelayCommand(SkipVersionAsync, () => IsUpdateAvailable);
		RemindLaterCommand     = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));

		_updateService.StateChanged    += OnServiceStateChanged;
		_localization.LanguageChanged += OnLanguageChanged;
	}

	/// <summary>Raised when the hosting dialog should close (skip / later).</summary>
	public event EventHandler? CloseRequested;

	public IAsyncRelayCommand CheckCommand { get; }

	public IAsyncRelayCommand InstallCommand { get; }

	public IRelayCommand RestartCommand { get; }

	public IRelayCommand OpenReleasePageCommand { get; }

	public IAsyncRelayCommand SkipVersionCommand { get; }

	public IRelayCommand RemindLaterCommand { get; }

	public UpdateStatus Status => _updateService.Status;

	private UpdateCheckResult? Result => _updateService.LastResult;

	public string CurrentVersionText => Format(LocalizationKeys.UpdateCurrentVersionFmt, LocalizationFallbacks.Update.CurrentVersionFormat, _updateService.CurrentVersion);

	public bool IsBusy => Status is UpdateStatus.Checking or UpdateStatus.Downloading or UpdateStatus.Installing;

	public bool IsIndeterminate => Status is UpdateStatus.Checking or UpdateStatus.Installing;

	public bool IsDownloading => Status == UpdateStatus.Downloading;

	public int DownloadProgress => _updateService.DownloadProgress;

	public bool IsUpdateAvailable => Result?.IsUpdateAvailable == true && Status is UpdateStatus.UpdateAvailable or UpdateStatus.Failed or UpdateStatus.Downloading or UpdateStatus.ReadyToInstall;

	public bool IsReadyToInstall => Status == UpdateStatus.ReadyToInstall;

	public bool HasError => Status == UpdateStatus.Failed;

	public bool IsUpToDate => Status == UpdateStatus.UpToDate;

	/// <summary>Download &amp; install in place (Velopack installs only).</summary>
	public bool CanDownload => Result is { IsUpdateAvailable: true, CanInstallInPlace: true } && Status is UpdateStatus.UpdateAvailable or UpdateStatus.Failed;

	/// <summary>Portable/dev builds: the user downloads the release manually.</summary>
	public bool ShowOpenReleasePage => Result is { IsUpdateAvailable: true, CanInstallInPlace: false } && !IsBusy;

	public bool ShowCheckButton => !IsUpdateAvailable && !IsBusy;

	public string? LatestVersion => Result?.LatestVersion;

	public string? ReleaseNotes => string.IsNullOrWhiteSpace(Result?.ReleaseNotes) ? null : Result!.ReleaseNotes!.Trim();

	/// <summary>Release notes converted from Markdown to display-ready plain text.</summary>
	public string? ReleaseNotesText => ReleaseNotesFormatter.ToPlainText(ReleaseNotes);

	public bool HasReleaseNotes => IsUpdateAvailable && ReleaseNotesText != null;

	/// <summary>Primary footer action of the update dialog: exactly one of install / restart / open release page.</summary>
	public bool ShowInstallButton => CanDownload;

	/// <summary>Dialog dismiss label: "Remind me later" while an update is pending, otherwise "Close".</summary>
	public string DismissText => IsUpdateAvailable && !IsReadyToInstall
		? Text(LocalizationKeys.UpdateLaterButton, LocalizationFallbacks.Update.LaterButton)
		: Text(LocalizationKeys.CommonClose,       LocalizationFallbacks.Common.Close);

	public bool CanSkipVersion => IsUpdateAvailable && !IsBusy && !IsReadyToInstall;

	public string StatusTitle => Status switch
	{
		UpdateStatus.Checking       => Text(LocalizationKeys.UpdateChecking, LocalizationFallbacks.Update.Checking),
		UpdateStatus.UpToDate       => Text(LocalizationKeys.UpdateUpToDate, LocalizationFallbacks.Update.UpToDate),
		UpdateStatus.UpdateAvailable => Format(LocalizationKeys.UpdateAvailableFmt, LocalizationFallbacks.Update.AvailableFormat, LatestVersion ?? string.Empty),
		UpdateStatus.Downloading    => Format(LocalizationKeys.UpdateDownloadingFmt, LocalizationFallbacks.Update.DownloadingFormat, DownloadProgress),
		UpdateStatus.ReadyToInstall => Text(LocalizationKeys.UpdateReady,      LocalizationFallbacks.Update.Ready),
		UpdateStatus.Installing     => Text(LocalizationKeys.UpdateInstalling, LocalizationFallbacks.Update.Installing),
		UpdateStatus.Failed         => Text(LocalizationKeys.UpdateFailed,     LocalizationFallbacks.Update.Failed),
		_                           => CurrentVersionText
	};

	public string StatusDetail => Status switch
	{
		UpdateStatus.UpToDate => Format(LocalizationKeys.UpdateUpToDateDetailFmt, LocalizationFallbacks.Update.UpToDateDetailFormat, _updateService.CurrentVersion),
		UpdateStatus.UpdateAvailable when Result?.CanInstallInPlace == true
			=> Format(LocalizationKeys.UpdateAvailableDetailFmt, LocalizationFallbacks.Update.AvailableDetailFormat, _updateService.CurrentVersion),
		UpdateStatus.UpdateAvailable => Text(LocalizationKeys.UpdateManualDetail, LocalizationFallbacks.Update.ManualDetail),
		UpdateStatus.ReadyToInstall  => Text(LocalizationKeys.UpdateReadyDetail,  LocalizationFallbacks.Update.ReadyDetail),
		UpdateStatus.Failed          => Text(LocalizationKeys.UpdateFailedDetail, LocalizationFallbacks.Update.FailedDetail),
		UpdateStatus.Idle            => Text(LocalizationKeys.UpdateIdle,         LocalizationFallbacks.Update.Idle),
		_                            => string.Empty
	};

	public bool HasStatusDetail => !string.IsNullOrEmpty(StatusDetail);

	/// <summary>Technical error message, shown in a tooltip for diagnostics.</summary>
	public string? ErrorDetail => HasError ? Result?.Error : null;

	public void Dispose()
	{
		_updateService.StateChanged    -= OnServiceStateChanged;
		_localization.LanguageChanged -= OnLanguageChanged;
	}

	public void Refresh()
	{
		OnPropertyChanged(string.Empty);
		CheckCommand.NotifyCanExecuteChanged();
		InstallCommand.NotifyCanExecuteChanged();
		RestartCommand.NotifyCanExecuteChanged();
		SkipVersionCommand.NotifyCanExecuteChanged();
	}

	private async Task CheckAsync()
	{
		try
		{
			await _updateService.CheckForUpdatesAsync();
		}
		catch (OperationCanceledException)
		{
		}
	}

	private async Task DownloadAsync()
	{
		try
		{
			await _updateService.DownloadUpdateAsync();
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void Restart()
	{
		try
		{
			_updateService.ApplyUpdateAndRestart();
		}
		catch (Exception)
		{
			// The service switches to Failed and the panel shows the error state.
		}
	}

	private void OpenReleasePage()
	{
		_urlLauncher.Open(Result?.ReleaseUrl ?? UpdateConstants.LatestReleaseUrl);
	}

	private async Task SkipVersionAsync()
	{
		if (LatestVersion is { } version)
		{
			await _coordinator.SkipVersionAsync(version);
		}

		CloseRequested?.Invoke(this, EventArgs.Empty);
	}

	private void OnServiceStateChanged(object? sender, EventArgs e)
	{
		_uiDispatch(Refresh);
	}

	private void OnLanguageChanged(object? sender, EventArgs e)
	{
		_uiDispatch(Refresh);
	}

	private string Text(string key, string fallback)
	{
		return _localization.GetString(key, fallback);
	}

	private string Format(string key, string fallback, object argument)
	{
		var format = Text(key, fallback);
		try
		{
			return string.Format(format, argument);
		}
		catch (FormatException)
		{
			return string.Format(fallback, argument);
		}
	}

	private static void PostToUiThread(Action action)
	{
		if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
		{
			action();
		}
		else
		{
			Avalonia.Threading.Dispatcher.UIThread.Post(action);
		}
	}
}
