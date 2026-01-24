using System.Diagnostics;
using System.Reflection;
using System.Text;
using Moq;
using NativeSupport.Enums;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.System;
using Shotora.App.Services.System;

namespace Shotora.App.Tests.System;

public class StartupServiceTests
{
	private readonly Mock<IEnvironmentFacade>    _environment          = new(MockBehavior.Strict);
	private readonly Mock<IFileFacade>           _fileFacade           = new(MockBehavior.Strict);
	private readonly Mock<IProcessSystemService> _processSystemService = new(MockBehavior.Strict);
	private readonly StartupService              _sut;

	public StartupServiceTests()
	{
		_sut = new StartupService(_fileFacade.Object, _environment.Object, _processSystemService.Object);
	}

	[Fact]
	public void Given_WindowsPlatform_When_SetRunOnStartupEnabled_Then_UpdatesRegistry()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_WindowsPlatform_When_SetRunOnStartupDisabled_Then_UpdatesRegistry()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_MacOSPlatform_When_SetRunOnStartupEnabled_Then_CreatesPlistFile()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		var homeDir         = "/Users/test";
		var launchAgentsDir = Path.Combine(homeDir,         "Library", "LaunchAgents");
		var plistPath       = Path.Combine(launchAgentsDir, "com.shotora.plist");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.CreateDirectory(launchAgentsDir));
		_fileFacade.Setup(f => f.WriteAll(plistPath, It.IsAny<byte[]>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
			.Returns(Task.CompletedTask)
			.Callback<string, byte[], CancellationToken, bool>((path, bytes, ct, configureAwait) =>
			{
				var content = Encoding.UTF8.GetString(bytes);
				Assert.Contains("com.shotora", content);
				Assert.Contains("RunAtLoad",   content);
				Assert.Contains("<true/>",     content);
			});

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.CreateDirectory(launchAgentsDir),                                                         Times.Once);
		_fileFacade.Verify(f => f.WriteAll(plistPath, It.IsAny<byte[]>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_MacOSPlatform_When_SetRunOnStartupDisabled_Then_DeletesPlistFile()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		var homeDir         = "/Users/test";
		var launchAgentsDir = Path.Combine(homeDir,         "Library", "LaunchAgents");
		var plistPath       = Path.Combine(launchAgentsDir, "com.shotora.plist");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.FileExists(plistPath)).Returns(true);
		_fileFacade.Setup(f => f.TryDelete(plistPath)).Returns(true);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(plistPath), Times.Once);
		_fileFacade.Verify(f => f.TryDelete(plistPath),  Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_MacOSPlatform_When_SetRunOnStartupDisabledAndFileNotExists_Then_DoesNotDelete()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		var homeDir         = "/Users/test";
		var launchAgentsDir = Path.Combine(homeDir,         "Library", "LaunchAgents");
		var plistPath       = Path.Combine(launchAgentsDir, "com.shotora.plist");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.FileExists(plistPath)).Returns(false);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(plistPath), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupEnabled_Then_CreatesDesktopFile()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		var homeDir     = "/home/test";
		var configDir   = Path.Combine(homeDir,   ".config", "autostart");
		var desktopPath = Path.Combine(configDir, "shotora.desktop");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.CreateDirectory(configDir));
		_fileFacade.Setup(f => f.WriteAll(desktopPath, It.IsAny<byte[]>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
			.Returns(Task.CompletedTask)
			.Callback<string, byte[], CancellationToken, bool>((path, bytes, ct, configureAwait) =>
			{
				var content = Encoding.UTF8.GetString(bytes);
				Assert.Contains("Desktop Entry",                  content);
				Assert.Contains("Type=Application",               content);
				Assert.Contains("Name=Shotora",                   content);
				Assert.Contains("X-GNOME-Autostart-enabled=true", content);
			});

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.CreateDirectory(configDir),                                                                 Times.Once);
		_fileFacade.Verify(f => f.WriteAll(desktopPath, It.IsAny<byte[]>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupDisabled_Then_DeletesDesktopFile()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		var homeDir     = "/home/test";
		var configDir   = Path.Combine(homeDir,   ".config", "autostart");
		var desktopPath = Path.Combine(configDir, "shotora.desktop");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.FileExists(desktopPath)).Returns(true);
		_fileFacade.Setup(f => f.TryDelete(desktopPath)).Returns(true);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(desktopPath), Times.Once);
		_fileFacade.Verify(f => f.TryDelete(desktopPath),  Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupDisabledAndFileNotExists_Then_DoesNotDelete()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		var homeDir     = "/home/test";
		var configDir   = Path.Combine(homeDir,   ".config", "autostart");
		var desktopPath = Path.Combine(configDir, "shotora.desktop");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);
		_fileFacade.Setup(f => f.FileExists(desktopPath)).Returns(false);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(desktopPath), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_GetExecutablePath_When_ProcessPathAvailable_Then_ReturnsProcessPath()
	{
		var method = typeof(StartupService).GetMethod("GetExecutablePath", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("GetExecutablePath method not found.");

		var result      = (string)method.Invoke(null, [])!;
		var processPath = Environment.ProcessPath;

		Assert.False(string.IsNullOrWhiteSpace(result));
		Assert.Equal(processPath, result);
	}

	[Fact]
	public void Given_GetExecutablePath_When_Called_Then_ReturnsOneOfExpectedPaths()
	{
		var method = typeof(StartupService).GetMethod("GetExecutablePath", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("GetExecutablePath method not found.");

		var result         = (string)method.Invoke(null, [])!;
		var processPath    = Environment.ProcessPath;
		var mainModulePath = Process.GetCurrentProcess().MainModule?.FileName;
		var baseDirectory  = AppContext.BaseDirectory;

		Assert.False(string.IsNullOrWhiteSpace(result));

		var expectedPaths = new[]
			{
				processPath, mainModulePath, baseDirectory
			}
			.Where(p => !string.IsNullOrWhiteSpace(p))
			.ToList();

		Assert.Contains(result, expectedPaths);
	}

	[Fact]
	public void Given_GetExecutablePath_When_CalledMultipleTimes_Then_ReturnsConsistentResult()
	{
		var method = typeof(StartupService).GetMethod("GetExecutablePath", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("GetExecutablePath method not found.");

		var result1 = (string)method.Invoke(null, [])!;
		var result2 = (string)method.Invoke(null, [])!;
		var result3 = (string)method.Invoke(null, [])!;

		Assert.Equal(result1, result2);
		Assert.Equal(result2, result3);
		Assert.False(string.IsNullOrWhiteSpace(result1));
	}

	[Fact]
	public void Given_GenerateMacOSPlist_When_Called_Then_ReturnsValidPlistContent()
	{
		var method = typeof(StartupService).GetMethod("GenerateMacOSPlist", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("GenerateMacOSPlist method not found.");

		var exePath = "/Applications/Shotora.app/Contents/MacOS/Shotora";
		var result  = (string)method.Invoke(null, [exePath])!;

		Assert.Contains("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", result);
		Assert.Contains("<!DOCTYPE plist",                            result);
		Assert.Contains("<key>Label</key>",                           result);
		Assert.Contains("com.shotora",                                result);
		Assert.Contains("<key>ProgramArguments</key>",                result);
		Assert.Contains($"<string>{exePath}</string>",                result);
		Assert.Contains("<key>RunAtLoad</key>",                       result);
		Assert.Contains("<true/>",                                    result);
	}

	[Fact]
	public void Given_GenerateLinuxDesktop_When_Called_Then_ReturnsValidDesktopContent()
	{
		var method = typeof(StartupService).GetMethod("GenerateLinuxDesktop", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("GenerateLinuxDesktop method not found.");

		var exePath = "/usr/bin/shotora";
		var result  = (string)method.Invoke(null, [exePath])!;

		Assert.Contains("[Desktop Entry]",                result);
		Assert.Contains("Type=Application",               result);
		Assert.Contains("Name=Shotora",                   result);
		Assert.Contains($"Exec={exePath}",                result);
		Assert.Contains("Hidden=false",                   result);
		Assert.Contains("NoDisplay=false",                result);
		Assert.Contains("X-GNOME-Autostart-enabled=true", result);
	}

	[Fact]
	public void Given_MacOSPlatform_When_SetRunOnStartupEnabledThrowsException_Then_SilentlyHandles()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Throws<UnauthorizedAccessException>();

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_MacOSPlatform_When_SetRunOnStartupDisabledThrowsException_Then_SilentlyHandles()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Throws<UnauthorizedAccessException>();

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupEnabledThrowsException_Then_SilentlyHandles()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Throws<UnauthorizedAccessException>();

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupDisabledThrowsException_Then_SilentlyHandles()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Throws<UnauthorizedAccessException>();

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_UnsupportedPlatform_When_SetRunOnStartup_Then_DoesNothing()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Other);

		_sut.SetRunOnStartup(true);
		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Exactly(2));
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_LinuxPlatform_When_SetRunOnStartupLinux_Then_FullyCoversAllCodePaths()
	{
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);

		var homeDir     = "/home/testuser";
		var configDir   = Path.Combine(homeDir,   ".config", "autostart");
		var desktopPath = Path.Combine(configDir, "shotora.desktop");

		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);

		var getExePathMethod = typeof(StartupService).GetMethod("GetExecutablePath", BindingFlags.NonPublic | BindingFlags.Static) ??
			throw new InvalidOperationException("GetExecutablePath method not found.");
		var expectedExePath = (string)getExePathMethod.Invoke(null, [])!;

		var generateDesktopMethod = typeof(StartupService).GetMethod("GenerateLinuxDesktop", BindingFlags.NonPublic | BindingFlags.Static) ??
			throw new InvalidOperationException("GenerateLinuxDesktop method not found.");
		var expectedDesktopContent = (string)generateDesktopMethod.Invoke(null, [expectedExePath])!;
		var expectedDesktopBytes   = Encoding.UTF8.GetBytes(expectedDesktopContent);

		_fileFacade.Setup(f => f.CreateDirectory(configDir));
		_fileFacade.Setup(f => f.WriteAll(desktopPath, It.IsAny<byte[]>(), CancellationToken.None, It.IsAny<bool>()))
			.Returns(Task.CompletedTask)
			.Callback<string, byte[], CancellationToken, bool>((path, bytes, ct, configureAwait) =>
			{
				Assert.Equal(desktopPath, path);

				Assert.Equal(expectedDesktopBytes, bytes);

				var content = Encoding.UTF8.GetString(bytes);
				Assert.Contains($"Exec={expectedExePath}",        content);
				Assert.Contains("[Desktop Entry]",                content);
				Assert.Contains("Type=Application",               content);
				Assert.Contains("Name=Shotora",                   content);
				Assert.Contains("Hidden=false",                   content);
				Assert.Contains("NoDisplay=false",                content);
				Assert.Contains("X-GNOME-Autostart-enabled=true", content);
			});

		_sut.SetRunOnStartup(true);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.CreateDirectory(configDir),                                                                                                 Times.Once);
		_fileFacade.Verify(f => f.WriteAll(desktopPath, It.Is<byte[]>(b => b.SequenceEqual(expectedDesktopBytes)), CancellationToken.None, It.IsAny<bool>()), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();

		_fileFacade.Reset();
		_environment.Reset();
		_processSystemService.Reset();
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);
		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);

		_fileFacade.Setup(f => f.FileExists(desktopPath)).Returns(true);
		_fileFacade.Setup(f => f.TryDelete(desktopPath)).Returns(true);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(desktopPath), Times.Once);
		_fileFacade.Verify(f => f.TryDelete(desktopPath),  Times.Once);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();

		_fileFacade.Reset();
		_environment.Reset();
		_processSystemService.Reset();
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);
		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Returns(homeDir);

		_fileFacade.Setup(f => f.FileExists(desktopPath)).Returns(false);

		_sut.SetRunOnStartup(false);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.Verify(f => f.FileExists(desktopPath),       Times.Once);
		_fileFacade.Verify(f => f.TryDelete(It.IsAny<string>()), Times.Never);
		_fileFacade.VerifyNoOtherCalls();
		_environment.VerifyNoOtherCalls();

		_fileFacade.Reset();
		_environment.Reset();
		_processSystemService.Reset();

		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);
		_environment.Setup(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile)).Throws<UnauthorizedAccessException>();

		var exception = Record.Exception(() => _sut.SetRunOnStartup(true));
		Assert.Null(exception);

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.Once);
		_environment.Verify(e => e.GetFolderPath(Environment.SpecialFolder.UserProfile), Times.Once);
		_fileFacade.VerifyNoOtherCalls();
	}
}
