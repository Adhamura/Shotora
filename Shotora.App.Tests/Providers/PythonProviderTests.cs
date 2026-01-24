using System.Diagnostics;
using Moq;
using NativeSupport.Enums;
using Shared.Interfaces.Adapters;
using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.System;
using Shotora.App.Services.Providers;
using IEnvironmentFacade=Shared.Interfaces.Facades.IEnvironmentFacade;

namespace Shotora.App.Tests.Providers;

public class PythonProviderTests
{
	private readonly Mock<IEasyOcrRuntime>                _easyOcrRuntime       = new(MockBehavior.Strict);
	private readonly Mock<IEnvironmentFacade>             _environmentFacade    = new(MockBehavior.Strict);
	private readonly Mock<IFileFacade>                    _fileFacade           = new(MockBehavior.Strict);
	private readonly Mock<IHttpClientAdapter>             _httpClientAdapter    = new(MockBehavior.Strict);
	private readonly Mock<IPackageDetectorEasyOcrService> _packageDetector      = new(MockBehavior.Strict);
	private readonly Mock<IProcessSystemService>          _processSystemService = new(MockBehavior.Strict);
	private readonly Mock<ISevenZipFacade>                _sevenZipFacade       = new(MockBehavior.Strict);
	private PythonProvider CreateSut()
	{
		return new PythonProvider(
			_processSystemService.Object,
			_fileFacade.Object,
			_httpClientAdapter.Object,
			_packageDetector.Object,
			_easyOcrRuntime.Object,
			_environmentFacade.Object,
			_sevenZipFacade.Object);
	}

	private void SetupGetEmbeddedZipPath(string? zipPath)
	{
		_fileFacade.Setup(f => f.FileExists(It.Is<string>(s => s.Contains(OcrConstants.PythonArchive))))
			.Returns(zipPath != null);
	}

	#region GetEmbeddedZipPath Tests

	[Fact]
	public void Given_FileExists_When_GetEmbeddedZipPath_Then_ReturnsPath()
	{
		var sut = CreateSut();
		SetupGetEmbeddedZipPath("/path/to/python.7z");

		var result = sut.GetEmbeddedZipPath();

		_fileFacade.Verify(f => f.FileExists(It.Is<string>(s => s.Contains(OcrConstants.PythonArchive))), Times.Once);
	}

	[Fact]
	public void Given_FileNotExists_When_GetEmbeddedZipPath_Then_ReturnsNull()
	{
		var sut = CreateSut();
		SetupGetEmbeddedZipPath(null);

		var result = sut.GetEmbeddedZipPath();

		_fileFacade.Verify(f => f.FileExists(It.Is<string>(s => s.Contains(OcrConstants.PythonArchive))), Times.Once);
	}

	#endregion

	#region EnsureExtractedAsync Tests

	[Fact]
	public async Task Given_HealthyInstallation_When_EnsureExtractedAsync_Then_ReturnsSuccess()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "Lib");
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureExtractedAsync();

		Assert.True(result.Success);
		Assert.Equal(pythonPath, result.PythonPath);
		Assert.Null(result.Error);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task Given_HealthyInstallationUnix_When_EnsureExtractedAsync_Then_ReturnsSuccess()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "lib");
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Linux);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureExtractedAsync();

		Assert.True(result.Success);
		Assert.Equal(pythonPath, result.PythonPath);
		_fileFacade.Verify(f => f.CreateDirectory(It.IsAny<string>()), Times.Once);
	}

	[Fact]
	public async Task Given_MissingZipPath_When_EnsureExtractedAsync_Then_ReturnsFailure()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "Lib");

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(false);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		SetupGetEmbeddedZipPath(null);

		var result = await sut.EnsureExtractedAsync();

		Assert.False(result.Success);
		Assert.NotNull(result.Error);
		Assert.Contains("Embedded Python runtime missing", result.Error);
	}

	[Fact]
	public async Task Given_CachedValidResult_When_EnsureExtractedAsync_Then_ReturnsCached()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "Lib");
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.SetupSequence(f => f.FileExists(It.IsAny<string>()))
			.Returns(true)
			.Returns(true)
			.Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		SetupGetEmbeddedZipPath(zipPath);

		var result1 = await sut.EnsureExtractedAsync();
		Assert.True(result1.Success);

		var result2 = await sut.EnsureExtractedAsync();
		Assert.True(result2.Success);
		Assert.Equal(result1.PythonPath, result2.PythonPath);
	}

	[Fact]
	public async Task Given_PythonMissingAfterExtract_When_EnsureExtractedAsync_Then_ReturnsFailure()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.SetupSequence(f => f.FileExists(It.IsAny<string>()))
			.Returns(false)
			.Returns(true)
			.Returns(false);
		_fileFacade.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(false);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>())).Verifiable();
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureExtractedAsync();

		Assert.False(result.Success);
		Assert.NotNull(result.Error);
	}

	#endregion

	#region EnsureEasyOcrAsync Tests

	[Fact]
	public async Task Given_RuntimeNotSuccessful_When_EnsureEasyOcrAsync_Then_ReturnsEarly()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(false);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_packageDetector.Setup(p => p.HasEasyOcrPackage()).Returns(false);
		SetupGetEmbeddedZipPath(null);

		var result = await sut.EnsureEasyOcrAsync();

		Assert.False(result.Success);
		_packageDetector.Verify(p => p.HasEasyOcrPackage(), Times.Once);
	}

	[Theory]
	[InlineData(RuntimeOs.Windows)]
	[InlineData(RuntimeOs.Linux)]
	[InlineData(RuntimeOs.Mac)]
	public async Task Given_SevenZipFacadeThrows_When_FallbackFinds7zOnPath_Then_RunSevenZipProcessAsyncBuildsCorrectArgs(RuntimeOs os)
	{
		var sut = CreateSut();

		var pythonPath  = "/path/to/python";
		var installRoot = @"D:\path\to\install";
		var rootFull    = Path.GetFullPath(installRoot);

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));

		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(os);

		var pythonExistsAfterExtraction = false;
		_fileFacade
			.Setup(f => f.FileExists(It.IsAny<string>()))
			.Returns((string p) =>
			{
				if (p.Contains(OcrConstants.PythonArchive, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}

				if (string.Equals(p, pythonPath, StringComparison.OrdinalIgnoreCase))
				{
					return pythonExistsAfterExtraction;
				}

				if (p.Contains("fakebin", StringComparison.OrdinalIgnoreCase))
				{
					var fileName = Path.GetFileName(p);
					if (fileName == "7zz"     ||
						fileName == "7z"      ||
						fileName == "7za"     ||
						fileName == "7zr"     ||
						fileName == "7zz.exe" ||
						fileName == "7z.exe"  ||
						fileName == "7za.exe" ||
						fileName == "7zr.exe")
					{
						return true;
					}
				}

				return false;
			});

		_fileFacade.Setup(f => f.DirectoryExists(It.IsAny<string>())).Returns(false);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>()));
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));

		_sevenZipFacade
			.Setup(z => z.ExtractToDir(It.IsAny<string>(), It.IsAny<string>(), true, true, string.Empty))
			.Throws(new Exception("force fallback"));

		var fakeBinDir = os == RuntimeOs.Windows ? @"C:\fakebin" : "/fakebin";
		_environmentFacade.Setup(e => e.GetEnvironmentVariable("PATH")).Returns(fakeBinDir);

		string?   capturedExe  = null;
		string[]? capturedArgs = null;
		string?   capturedDest = null;

		_processSystemService
			.Setup(p => p.RunAsync(
				It.Is<string>(exe => (exe.Contains("7z") || exe.Contains("fakebin")) && exe != "chmod"),
				It.IsAny<IEnumerable<string>>(),
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()))
			.Callback<string, IEnumerable<string>?, string, CancellationToken>((exe, args, dest, ct) =>
			{
				if (args != null && args.FirstOrDefault() == "x")
				{
					capturedExe  = exe;
					capturedArgs = args.ToArray();
					capturedDest = dest;

					pythonExistsAfterExtraction = true;
				}
			})
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));

		_processSystemService
			.Setup(p => p.RunAsync(
				It.Is<string>(exe => exe == "chmod"),
				It.IsAny<IEnumerable<string>>(),
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));

		var result = await sut.EnsureExtractedAsync();

		Assert.NotNull(capturedExe);
		Assert.NotNull(capturedArgs);
		Assert.NotNull(capturedDest);

		Assert.Equal(rootFull, capturedDest);

		if (os == RuntimeOs.Windows)
		{
			Assert.Equal(new[]
			{
				"x", "-y", "-o" + rootFull, capturedArgs![3]
			}, capturedArgs!);
			Assert.DoesNotContain("-snl", capturedArgs!);
		}
		else
		{
			Assert.Equal("x",             capturedArgs![0]);
			Assert.Equal("-y",            capturedArgs![1]);
			Assert.Equal("-snl",          capturedArgs![2]);
			Assert.Equal("-o" + rootFull, capturedArgs![3]);

			Assert.True(capturedArgs![4].Contains(OcrConstants.PythonArchive, StringComparison.OrdinalIgnoreCase));
		}

		_processSystemService.Verify(p => p.RunAsync(
			It.IsAny<string>(),
			It.IsAny<IEnumerable<string>>(),
			It.IsAny<string>(),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Given_EmptyPythonPath_When_EnsureEasyOcrAsync_Then_ReturnsEarly()
	{
		var sut         = CreateSut();
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "Lib");
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(string.Empty);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(string.Empty)).Returns(false);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_packageDetector.Setup(p => p.HasEasyOcrPackage()).Returns(false);
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureEasyOcrAsync();

		Assert.False(result.Success);
		_packageDetector.Verify(p => p.HasEasyOcrPackage(), Times.Once);
	}

	[Fact]
	public async Task Given_EasyOcrAlreadyInstalled_When_EnsureEasyOcrAsync_Then_ReturnsSuccess()
	{
		var sut         = CreateSut();
		var pythonPath  = "/path/to/python";
		var installRoot = "/path/to/install";
		var libDir      = Path.Combine(installRoot, "Lib");
		var zipPath     = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(Path.Combine(installRoot, "site-packages"));
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_packageDetector.Setup(p => p.HasEasyOcrPackage()).Returns(true);
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureEasyOcrAsync();

		Assert.True(result.Success);
		Assert.Equal(pythonPath, result.PythonPath);
		_packageDetector.Verify(p => p.HasEasyOcrPackage(), Times.Once);
		_packageDetector.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_PipAvailable_When_EnsureEasyOcrAsync_Then_InstallsPackages()
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = Path.Combine(installRoot, "Lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.SetupSequence(p => p.HasEasyOcrPackage())
			.Returns(false)
			.Returns(true);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());
		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));
		SetupGetEmbeddedZipPath(zipPath);

		var progress = new Mock<IProgress<string>>(MockBehavior.Loose);
		var result   = await sut.EnsureEasyOcrAsync(progress.Object);

		Assert.True(result.Success);
		Assert.Equal(pythonPath, result.PythonPath);
		_processSystemService.Verify(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
		progress.Verify(p => p.Report(It.IsAny<string>()), Times.AtLeastOnce);
	}

	[Fact]
	public async Task Given_PipBootstrapNeeded_When_EnsureEasyOcrAsync_Then_DownloadsAndRunsBootstrap()
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = Path.Combine(installRoot, "Lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.SetupSequence(p => p.HasEasyOcrPackage())
			.Returns(false)
			.Returns(true);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());

		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "pip not found"))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "ensurepip failed"))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));
		_httpClientAdapter.Setup(h => h.Get(It.Is<string>(s => s.Contains("bootstrap") || s.Contains("pip")), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
			.ReturnsAsync([1, 2, 3]);
		_fileFacade.Setup(f => f.WriteAll(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);
		_fileFacade.Setup(f => f.TryDelete(It.IsAny<string>())).Returns(true);
		SetupGetEmbeddedZipPath(zipPath);

		var progress = new Mock<IProgress<string>>(MockBehavior.Loose);
		var result   = await sut.EnsureEasyOcrAsync(progress.Object);

		_httpClientAdapter.Verify(h => h.Get(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
		_fileFacade.Verify(f => f.WriteAll(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
		Assert.True(result.Success);
	}

	[Fact]
	public async Task Given_PipBootstrapFails_When_EnsureEasyOcrAsync_Then_ReturnsFailure()
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = Path.Combine(installRoot, "Lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.Setup(p => p.HasEasyOcrPackage()).Returns(false);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());
		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "pip not found"))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "ensurepip failed"))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "bootstrap failed"))
			.ReturnsAsync(new ProcessExecutionResult(1, "", "pip still not found"));
		_httpClientAdapter.Setup(h => h.Get(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()))
			.ReturnsAsync([1, 2, 3]);
		_fileFacade.Setup(f => f.WriteAll(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);
		_fileFacade.Setup(f => f.TryDelete(It.IsAny<string>())).Returns(true);
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureEasyOcrAsync();

		Assert.False(result.Success);
		Assert.NotNull(result.Error);
		Assert.Contains("pip bootstrap failed", result.Error);
	}

	[Theory]
	[InlineData(RuntimeOs.Windows)]
	[InlineData(RuntimeOs.Mac)]
	[InlineData(RuntimeOs.Linux)]
	public async Task Given_DifferentOs_When_EnsureEasyOcrAsync_Then_HandlesOsCorrectly(RuntimeOs os)
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = os == RuntimeOs.Windows ? Path.Combine(installRoot, "Lib") : Path.Combine(installRoot, "lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(os);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.SetupSequence(p => p.HasEasyOcrPackage())
			.Returns(false)
			.Returns(true);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());
		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureEasyOcrAsync();

		_processSystemService.Verify(p => p.GetCurrentOs(), Times.AtLeastOnce);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task Given_PackageInstallationSucceeds_When_EnsureEasyOcrAsync_Then_ReturnsSuccess()
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = Path.Combine(installRoot, "Lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.SetupSequence(p => p.HasEasyOcrPackage())
			.Returns(false)
			.Returns(true);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());
		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));
		SetupGetEmbeddedZipPath(zipPath);

		var progress = new Mock<IProgress<string>>(MockBehavior.Loose);
		var result   = await sut.EnsureEasyOcrAsync(progress.Object);

		Assert.True(result.Success);
		Assert.Equal(pythonPath, result.PythonPath);
		Assert.Null(result.Error);
		_packageDetector.Verify(p => p.HasEasyOcrPackage(), Times.Exactly(2));
		progress.Verify(p => p.Report(It.IsAny<string>()), Times.AtLeastOnce);
	}

	[Fact]
	public async Task Given_PackageInstallationFails_When_EnsureEasyOcrAsync_Then_ReturnsFailure()
	{
		var sut          = CreateSut();
		var pythonPath   = "/path/to/python";
		var installRoot  = "/path/to/install";
		var libDir       = Path.Combine(installRoot, "Lib");
		var sitePackages = Path.Combine(installRoot, "site-packages");
		var zipPath      = "/path/to/python.7z";

		_easyOcrRuntime.Setup(r => r.PythonExePath).Returns(pythonPath);
		_easyOcrRuntime.Setup(r => r.InstallDirectory).Returns(installRoot);
		_easyOcrRuntime.Setup(r => r.SitePackagesPath).Returns(sitePackages);
		_processSystemService.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);
		_fileFacade.Setup(f => f.FileExists(pythonPath)).Returns(true);
		_fileFacade.Setup(f => f.DirectoryExists(libDir)).Returns(true);
		_fileFacade.Setup(f => f.CreateDirectory(It.IsAny<string>())).Verifiable();
		_fileFacade.Setup(f => f.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>()));
		_environmentFacade.Setup(e => e.GetEnvironmentVariable(It.IsAny<string>())).Returns("");
		_packageDetector.SetupSequence(p => p.HasEasyOcrPackage())
			.Returns(false)
			.Returns(false);
		_processSystemService.Setup(p => p.CreateProcessStartInfo(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
			.Returns(new ProcessStartInfo());
		_processSystemService.SetupSequence(p => p.RunAsync(It.IsAny<ProcessStartInfo>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""))
			.ReturnsAsync(new ProcessExecutionResult(0, "", ""));
		SetupGetEmbeddedZipPath(zipPath);

		var result = await sut.EnsureEasyOcrAsync();

		Assert.False(result.Success);
		Assert.NotNull(result.Error);
		Assert.Contains("EasyOCR package missing", result.Error);
	}

	#endregion
}
