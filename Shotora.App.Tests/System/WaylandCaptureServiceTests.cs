using Avalonia;
using Moq;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.System;
using Shotora.App.Services.System;
using SkiaSharp;

namespace Shotora.App.Tests.System;

public class WaylandCaptureServiceTests : IDisposable
{
	private readonly Mock<IEnvironmentFacade>    _environment          = new();
	private readonly Mock<IFileFacade>           _fileFacade           = new();
	private readonly Mock<IScreenshotPortal>     _portal               = new(MockBehavior.Strict);
	private readonly Mock<IProcessSystemService> _processSystemService = new(MockBehavior.Strict);
	private readonly WaylandCaptureService       _sut;
	private readonly List<string>                _tempFiles = [];

	public WaylandCaptureServiceTests()
	{
		_fileFacade.Setup(f => f.FileExists(It.IsAny<string>())).Returns<string>(File.Exists);
		_fileFacade.Setup(f => f.OpenRead(It.IsAny<string>())).Returns<string>(path => File.OpenRead(path));
		_fileFacade.Setup(f => f.TryDelete(It.IsAny<string>())).Returns<string>(path =>
		{
			File.Delete(path);
			return true;
		});
		_sut = new WaylandCaptureService(_portal.Object, _processSystemService.Object, _fileFacade.Object, _environment.Object);
	}

	public void Dispose()
	{
		foreach (var file in _tempFiles.Where(File.Exists))
		{
			File.Delete(file);
		}
	}

	[Theory]
	[InlineData("wayland-0", null,      true)]
	[InlineData(null,        "wayland", true)]
	[InlineData("",          "Wayland", true)]
	[InlineData(null,        "x11",     false)]
	[InlineData(null,        null,      false)]
	public void Given_SessionEnvironment_When_IsWaylandSession_Then_DetectsWayland(string? waylandDisplay, string? sessionType, bool expected)
	{
		_environment.Setup(e => e.GetEnvironmentVariable("WAYLAND_DISPLAY")).Returns(waylandDisplay!);
		_environment.Setup(e => e.GetEnvironmentVariable("XDG_SESSION_TYPE")).Returns(sessionType!);

		Assert.Equal(expected, _sut.IsWaylandSession());
	}

	[Fact]
	public async Task Given_PortalCaptures_When_CaptureDesktop_Then_ReturnsImageAndDeletesHandOffFile()
	{
		var file = WritePng(Path.Combine(Path.GetTempPath(), $"shotora-test-{Guid.NewGuid():N}.png"), 64, 32);
		_portal.Setup(p => p.CaptureToFileAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ScreenshotPortalResult(ScreenshotPortalStatus.Captured, file));

		using var bitmap = await _sut.CaptureDesktopAsync();

		Assert.Equal(64, bitmap.Width);
		Assert.Equal(32, bitmap.Height);
		Assert.False(File.Exists(file));
		_processSystemService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_PortalDenied_When_CaptureDesktop_Then_ThrowsWithoutTryingOtherTools()
	{
		_portal.Setup(p => p.CaptureToFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ScreenshotPortalResult.Denied);

		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CaptureDesktopAsync());

		Assert.Equal(WaylandCaptureService.DeniedMessage, exception.Message);
		_processSystemService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_PortalUnavailable_When_CaptureDesktop_Then_FallsBackToGrim()
	{
		_portal.Setup(p => p.CaptureToFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ScreenshotPortalResult.Unavailable);
		SetupTool("grim", path => WritePng(path, 40, 20));

		using var bitmap = await _sut.CaptureDesktopAsync();

		Assert.Equal(40, bitmap.Width);
		_processSystemService.Verify(p => p.RunAsync("grim", It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Given_GrimMissing_When_CaptureDesktop_Then_TriesSpectacleNext()
	{
		_portal.Setup(p => p.CaptureToFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ScreenshotPortalResult.Unavailable);
		_processSystemService.Setup(p => p.RunAsync("grim", It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new global::System.ComponentModel.Win32Exception("No such file or directory"));
		SetupTool("spectacle", path => WritePng(path, 30, 30));

		using var bitmap = await _sut.CaptureDesktopAsync();

		Assert.Equal(30, bitmap.Width);
		_processSystemService.Verify(p => p.RunAsync("gnome-screenshot", It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Given_NothingWorks_When_CaptureDesktop_Then_ThrowsHelpfulMessage()
	{
		_portal.Setup(p => p.CaptureToFileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ScreenshotPortalResult.Unavailable);
		_processSystemService.Setup(p => p.RunAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new InvalidOperationException("Failed (exit 1)"));

		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CaptureDesktopAsync());

		Assert.Equal(WaylandCaptureService.UnavailableMessage, exception.Message);
		_processSystemService.Verify(p => p.RunAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()), Times.Exactly(3));
	}

	[Fact]
	public void Given_HiDpiDesktop_When_FitDesktopToBounds_Then_ScalesToLogicalSize()
	{
		using var desktop = new SKBitmap(new SKImageInfo(400, 200, SKColorType.Rgba8888, SKAlphaType.Premul));
		desktop.Erase(SKColors.Red);

		using var fitted = ScreenshotService.FitDesktopToBounds(desktop, new PixelRect(0, 0, 200, 100));

		Assert.Equal(200, fitted.Width);
		Assert.Equal(100, fitted.Height);
		Assert.Equal(SKColors.Red, fitted.GetPixel(100, 50));
	}

	[Fact]
	public void Given_MatchingSize_When_FitDesktopToBounds_Then_ReturnsIndependentCopy()
	{
		using var desktop = new SKBitmap(new SKImageInfo(50, 40, SKColorType.Bgra8888, SKAlphaType.Premul));

		using var fitted = ScreenshotService.FitDesktopToBounds(desktop, new PixelRect(0, 0, 50, 40));

		Assert.NotSame(desktop, fitted);
		Assert.Equal(50, fitted.Width);
		Assert.Equal(40, fitted.Height);
	}

	private void SetupTool(string fileName, Action<string> writeOutput)
	{
		_processSystemService.Setup(p => p.RunAsync(fileName, It.IsAny<IEnumerable<string>?>(), null, It.IsAny<CancellationToken>()))
			.Callback<string, IEnumerable<string>?, string?, CancellationToken>((_, args, _, _) => writeOutput(args!.Last()))
			.ReturnsAsync(new ProcessExecutionResult(0, string.Empty, string.Empty));
	}

	private string WritePng(string path, int width, int height)
	{
		_tempFiles.Add(path);
		using var bitmap = new SKBitmap(width, height);
		bitmap.Erase(SKColors.CornflowerBlue);
		using var data   = bitmap.Encode(SKEncodedImageFormat.Png, 100);
		using var stream = File.Create(path);
		data.SaveTo(stream);
		return path;
	}
}
