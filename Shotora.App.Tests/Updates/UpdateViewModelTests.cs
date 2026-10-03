using Moq;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;
using Shotora.App.Services.ViewModels;

namespace Shotora.App.Tests.Updates;

public class UpdateViewModelTests
{
	private readonly Mock<IUpdateCoordinator>    _coordinator  = new();
	private readonly Mock<ILocalizationProvider> _localization = new();
	private readonly Mock<IUpdateService>        _service      = new();
	private readonly Mock<IUrlLauncherService>   _urls         = new();
	private readonly UpdateViewModel             _sut;

	public UpdateViewModelTests()
	{
		_localization.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()))
			.Returns<string, string>((_, fallback) => fallback);
		_service.Setup(s => s.CurrentVersion).Returns("1.0.0");
		_sut = new UpdateViewModel(_service.Object, _coordinator.Object, _localization.Object, _urls.Object, action => action());
	}

	private void Arrange(UpdateStatus status, UpdateCheckResult? result, int progress = 0)
	{
		_service.Setup(s => s.Status).Returns(status);
		_service.Setup(s => s.LastResult).Returns(result);
		_service.Setup(s => s.DownloadProgress).Returns(progress);
		_service.Raise(s => s.StateChanged += null, EventArgs.Empty);
	}

	[Fact]
	public void Given_Idle_When_Created_Then_OffersCheck()
	{
		Arrange(UpdateStatus.Idle, null);

		Assert.True(_sut.ShowCheckButton);
		Assert.True(_sut.CheckCommand.CanExecute(null));
		Assert.False(_sut.IsUpdateAvailable);
		Assert.Equal("Version 1.0.0", _sut.StatusTitle);
	}

	[Fact]
	public void Given_InPlaceUpdate_When_StateChanges_Then_OffersDownloadAndNotes()
	{
		Arrange(UpdateStatus.UpdateAvailable, new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0", " notes ", "https://r", CanInstallInPlace: true));

		Assert.Equal("Shotora 1.2.0 is available", _sut.StatusTitle);
		Assert.True(_sut.CanDownload);
		Assert.True(_sut.InstallCommand.CanExecute(null));
		Assert.False(_sut.ShowOpenReleasePage);
		Assert.False(_sut.ShowCheckButton);
		Assert.True(_sut.HasReleaseNotes);
		Assert.Equal("notes", _sut.ReleaseNotes);
	}

	[Fact]
	public void Given_PortableUpdate_When_OpenReleasePage_Then_LaunchesReleaseUrl()
	{
		Arrange(UpdateStatus.UpdateAvailable, new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0", ReleaseUrl: "https://github.com/x/releases/tag/v1.2.0"));

		Assert.True(_sut.ShowOpenReleasePage);
		Assert.False(_sut.CanDownload);

		_sut.OpenReleasePageCommand.Execute(null);

		_urls.Verify(u => u.Open("https://github.com/x/releases/tag/v1.2.0"), Times.Once);
	}

	[Fact]
	public void Given_Downloading_When_StateChanges_Then_ShowsProgress()
	{
		Arrange(UpdateStatus.Downloading, new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0", CanInstallInPlace: true), 42);

		Assert.True(_sut.IsBusy);
		Assert.True(_sut.IsDownloading);
		Assert.False(_sut.IsIndeterminate);
		Assert.Equal(42, _sut.DownloadProgress);
		Assert.Equal("Downloading update… 42%", _sut.StatusTitle);
		Assert.False(_sut.CheckCommand.CanExecute(null));
	}

	[Fact]
	public void Given_ReadyToInstall_When_Restart_Then_AppliesUpdate()
	{
		Arrange(UpdateStatus.ReadyToInstall, new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0", CanInstallInPlace: true));

		Assert.True(_sut.RestartCommand.CanExecute(null));
		_sut.RestartCommand.Execute(null);

		_service.Verify(s => s.ApplyUpdateAndRestart(), Times.Once);
	}

	[Fact]
	public void Given_Failed_When_StateChanges_Then_ExposesErrorDetail()
	{
		Arrange(UpdateStatus.Failed, UpdateCheckResult.Failed("1.0.0", "timeout"));

		Assert.True(_sut.HasError);
		Assert.Equal("timeout",                   _sut.ErrorDetail);
		Assert.Equal("Couldn't check for updates", _sut.StatusTitle);
		Assert.True(_sut.ShowCheckButton);
	}

	[Fact]
	public async Task Given_Update_When_SkipVersion_Then_PersistsAndRequestsClose()
	{
		Arrange(UpdateStatus.UpdateAvailable, new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.2.0"));
		var closed = false;
		_sut.CloseRequested += (_, _) => closed = true;

		await _sut.SkipVersionCommand.ExecuteAsync(null);

		_coordinator.Verify(c => c.SkipVersionAsync("1.2.0"), Times.Once);
		Assert.True(closed);
	}

	[Fact]
	public async Task Given_Check_When_Executed_Then_DelegatesToService()
	{
		Arrange(UpdateStatus.Idle, null);
		_service.Setup(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(UpdateCheckResult.UpToDate("1.0.0", "1.0.0", null));

		await _sut.CheckCommand.ExecuteAsync(null);

		_service.Verify(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}
}
