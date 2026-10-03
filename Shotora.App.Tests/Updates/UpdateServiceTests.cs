using Moq;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models.Updates;
using Shotora.App.Services.Updates;

namespace Shotora.App.Tests.Updates;

public class UpdateServiceTests
{
	private readonly Mock<IGitHubReleaseClient>   _github   = new(MockBehavior.Strict);
	private readonly Mock<IVelopackUpdateAdapter> _velopack = new(MockBehavior.Strict);
	private readonly Mock<IAppVersionProvider>    _version  = new(MockBehavior.Strict);
	private readonly Mock<IApplicationShutdownService> _shutdown = new();
	private readonly UpdateService                _sut;

	public UpdateServiceTests()
	{
		_version.Setup(v => v.Version).Returns("1.0.0");
		_sut = new UpdateService(_velopack.Object, _github.Object, _version.Object, _shutdown.Object);
	}

	private static GitHubReleaseModel Release(string tag)
	{
		return new GitHubReleaseModel(tag, tag, "notes", $"https://example/{tag}", false, false);
	}

	[Fact]
	public void Given_PortableBuildAndNewerRelease_When_Check_Then_ReportsUpdateWithoutInPlaceInstall()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Release("v1.1.0"));

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpdateAvailable, result.Status);
			Assert.Equal("1.1.0", result.LatestVersion);
			Assert.Equal("https://example/v1.1.0", result.ReleaseUrl);
			Assert.False(result.CanInstallInPlace);
			Assert.Equal(UpdateStatus.UpdateAvailable, _sut.Status);
			Assert.False(await _sut.DownloadUpdateAsync());
		});
	}

	[Fact]
	public void Given_PortableBuildAndSameRelease_When_Check_Then_ReportsUpToDate()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Release("1.0"));

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpToDate, result.Status);
			Assert.Equal("1.0.0", result.LatestVersion);
		});
	}

	[Fact]
	public void Given_NoPublishedRelease_When_Check_Then_ReportsUpToDate()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync((GitHubReleaseModel?)null);

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpToDate, result.Status);
		});
	}

	[Fact]
	public void Given_NetworkFailure_When_Check_Then_ReportsFailedWithError()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException("offline"));

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.Failed, result.Status);
			Assert.Equal("offline",           result.Error);
			Assert.Equal(UpdateStatus.Failed, _sut.Status);
			Assert.Equal("offline",           _sut.LastError);
		});
	}

	[Fact]
	public void Given_InstalledAndVelopackFindsUpdate_When_CheckDownloadApply_Then_RunsFullFlow()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(true);
			_velopack.Setup(v => v.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new VelopackUpdateModel("1.2.0", "## New"));
			_velopack.Setup(v => v.DownloadUpdatesAsync(It.IsAny<Action<int>>(), It.IsAny<CancellationToken>()))
				.Callback<Action<int>, CancellationToken>((progress, _) =>
				{
					progress(40);
					progress(150);
				})
				.Returns(Task.CompletedTask);
			_velopack.Setup(v => v.ApplyUpdatesOnExitAndRestart());

			var states = new List<UpdateStatus>();
			_sut.StateChanged += (_, _) => states.Add(_sut.Status);

			var result = await _sut.CheckForUpdatesAsync();
			Assert.True(result.CanInstallInPlace);
			Assert.Equal("1.2.0", result.LatestVersion);
			Assert.Equal("## New", result.ReleaseNotes);

			Assert.True(await _sut.DownloadUpdateAsync());
			Assert.Equal(UpdateStatus.ReadyToInstall, _sut.Status);
			Assert.Equal(100,                         _sut.DownloadProgress);

			_sut.ApplyUpdateAndRestart();

			Assert.Equal(UpdateStatus.Installing, _sut.Status);
			Assert.Contains(UpdateStatus.Checking,    states);
			Assert.Contains(UpdateStatus.Downloading, states);
			_velopack.Verify(v => v.ApplyUpdatesOnExitAndRestart(), Times.Once);
			_shutdown.Verify(s => s.Shutdown(), Times.Once);
			_github.VerifyNoOtherCalls();
		});
	}

	[Fact]
	public void Given_InstalledAndUpToDate_When_Check_Then_DoesNotQueryGitHub()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(true);
			_velopack.Setup(v => v.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync((VelopackUpdateModel?)null);

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpToDate, result.Status);
			_github.VerifyNoOtherCalls();
		});
	}

	[Fact]
	public void Given_InstalledButVelopackFeedFails_When_Check_Then_FallsBackToGitHubApi()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(true);
			_velopack.Setup(v => v.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("no feed"));
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Release("v3.0.0"));

			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpdateAvailable, result.Status);
			Assert.False(result.CanInstallInPlace);
		});
	}

	[Fact]
	public void Given_DownloadThrows_When_DownloadUpdate_Then_ReportsFailedAndAllowsRetry()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(true);
			_velopack.Setup(v => v.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new VelopackUpdateModel("1.2.0", null));
			_velopack.SetupSequence(v => v.DownloadUpdatesAsync(It.IsAny<Action<int>>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new IOException("disk full"))
				.Returns(Task.CompletedTask);

			await _sut.CheckForUpdatesAsync();

			Assert.False(await _sut.DownloadUpdateAsync());
			Assert.Equal(UpdateStatus.Failed, _sut.Status);
			Assert.Equal("disk full",         _sut.LastError);
			Assert.True(_sut.LastResult?.IsUpdateAvailable);

			Assert.True(await _sut.DownloadUpdateAsync());
			Assert.Equal(UpdateStatus.ReadyToInstall, _sut.Status);
			Assert.Null(_sut.LastError);
		});
	}

	[Fact]
	public void Given_NothingDownloaded_When_ApplyUpdateAndRestart_Then_Throws()
	{
		Assert.Throws<InvalidOperationException>(() => _sut.ApplyUpdateAndRestart());
		_velopack.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_CallerCancels_When_Check_Then_StopsWaitingButSharedCheckCompletes()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			var release = new TaskCompletionSource<GitHubReleaseModel?>();
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).Returns(release.Task);
			using var cts = new CancellationTokenSource();

			var cancelled = _sut.CheckForUpdatesAsync(cts.Token);
			cts.Cancel();
			await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cancelled);

			release.SetResult(Release("v1.1.0"));
			var result = await _sut.CheckForUpdatesAsync();

			Assert.Equal(UpdateStatus.UpdateAvailable, result.Status);
		});
	}

	[Fact]
	public void Given_CheckInFlight_When_CheckAgain_Then_SharesSingleRequest()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(false);
			var release = new TaskCompletionSource<GitHubReleaseModel?>();
			_github.Setup(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>())).Returns(release.Task);

			var first  = _sut.CheckForUpdatesAsync();
			var second = _sut.CheckForUpdatesAsync();
			release.SetResult(Release("v1.1.0"));

			Assert.Same(await first, await second);
			_github.Verify(g => g.GetLatestReleaseAsync(It.IsAny<CancellationToken>()), Times.Once);
		});
	}

	[Fact]
	public void Given_ApplyFails_When_ApplyUpdateAndRestart_Then_ReportsFailureAndDoesNotShutDown()
	{
		RunIsolated(async () =>
		{
			_velopack.Setup(v => v.IsInstalled).Returns(true);
			_velopack.Setup(v => v.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new VelopackUpdateModel("1.2.0", null));
			_velopack.Setup(v => v.DownloadUpdatesAsync(It.IsAny<Action<int>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
			_velopack.Setup(v => v.ApplyUpdatesOnExitAndRestart()).Throws(new IOException("locked"));
			await _sut.CheckForUpdatesAsync();
			await _sut.DownloadUpdateAsync();

			Assert.Throws<IOException>(() => _sut.ApplyUpdateAndRestart());

			Assert.Equal(UpdateStatus.Failed, _sut.Status);
			Assert.Equal("locked",            _sut.LastError);
			_shutdown.Verify(s => s.Shutdown(), Times.Never);
		});
	}

	/// <summary>
	///     Runs an async test body on a pool thread and blocks until it finishes, so its continuations can never
	///     move xUnit's runner thread (the Avalonia-based tests in this assembly require a stable thread).
	/// </summary>
	private static void RunIsolated(Func<Task> body)
	{
		Task.Run(body).GetAwaiter().GetResult();
	}
}
