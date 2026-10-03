using Moq;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Models;
using Shotora.App.Models.Updates;
using Shotora.App.Services.Updates;

namespace Shotora.App.Tests.Updates;

public class UpdateCoordinatorTests
{
	private static readonly DateTimeOffset Now = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

	private readonly Mock<ISettingsSystemService> _settings = new(MockBehavior.Strict);
	private readonly Mock<IUpdateService>         _updates  = new(MockBehavior.Strict);
	private readonly AppSettings                  _stored   = new();
	private readonly UpdateCoordinator            _sut;

	public UpdateCoordinatorTests()
	{
		_settings.Setup(s => s.LoadAsync()).ReturnsAsync(() => _stored);
		_settings.Setup(s => s.SaveAsync(It.IsAny<AppSettings>())).Returns(Task.CompletedTask);
		_sut = new UpdateCoordinator(_updates.Object, _settings.Object, new FixedTimeProvider(Now));
	}

	[Fact]
	public async Task Given_AutoCheckDisabled_When_RunAutomaticCheck_Then_SkipsWithoutNetwork()
	{
		_stored.AutoCheckForUpdates = false;

		var result = await _sut.RunAutomaticCheckAsync(CancellationToken.None);

		Assert.Null(result);
		_updates.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_RecentCheck_When_RunAutomaticCheck_Then_Skips()
	{
		_stored.LastUpdateCheckUtc = Now.AddHours(-2);

		Assert.Null(await _sut.RunAutomaticCheckAsync(CancellationToken.None));
		_updates.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_UpdateFound_When_RunAutomaticCheck_Then_RecordsCheckAndRaisesEvent()
	{
		var found = new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.1.0");
		_updates.Setup(u => u.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(found);
		UpdateCheckResult? raised = null;
		_sut.UpdateAvailable += (_, r) => raised = r;

		var result = await _sut.RunAutomaticCheckAsync(CancellationToken.None);

		Assert.Same(found, result);
		Assert.Same(found, raised);
		Assert.Equal(Now, _stored.LastUpdateCheckUtc);
		_settings.Verify(s => s.SaveAsync(_stored), Times.Once);
	}

	[Fact]
	public async Task Given_SkippedVersionFound_When_RunAutomaticCheck_Then_DoesNotRaiseEvent()
	{
		_stored.SkippedUpdateVersion = "1.1.0";
		_updates.Setup(u => u.CheckForUpdatesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new UpdateCheckResult(UpdateStatus.UpdateAvailable, "1.0.0", "1.1.0"));
		var raised = false;
		_sut.UpdateAvailable += (_, _) => raised = true;

		await _sut.RunAutomaticCheckAsync(CancellationToken.None);

		Assert.False(raised);
		Assert.Equal(Now, _stored.LastUpdateCheckUtc);
	}

	[Fact]
	public async Task Given_CheckFailed_When_RunAutomaticCheck_Then_DoesNotRecordCheckSoItRetries()
	{
		_updates.Setup(u => u.CheckForUpdatesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(UpdateCheckResult.Failed("1.0.0", "offline"));

		var result = await _sut.RunAutomaticCheckAsync(CancellationToken.None);

		Assert.Equal(UpdateStatus.Failed, result?.Status);
		Assert.Null(_stored.LastUpdateCheckUtc);
		_settings.Verify(s => s.SaveAsync(It.IsAny<AppSettings>()), Times.Never);
	}

	[Fact]
	public async Task Given_Version_When_SkipVersion_Then_PersistsNormalizedVersion()
	{
		await _sut.SkipVersionAsync("v2.0");

		Assert.Equal("2.0.0", _stored.SkippedUpdateVersion);
		_settings.Verify(s => s.SaveAsync(_stored), Times.Once);
	}

	private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
	{
		public override DateTimeOffset GetUtcNow()
		{
			return now;
		}
	}
}
