using System.Collections.Frozen;
using NativeSupport.Constants;
using NativeSupport.Enums;
using Shared.Interfaces.Adapters;
using Shared.Interfaces.Facades;
using Shared.Models.Constants;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Services.Providers;

public class PythonProvider(
	IProcessSystemService          processSystemService,
	IFileFacade                    fileFacade,
	IHttpClientAdapter             httpClientAdapter,
	IPackageDetectorEasyOcrService packageDetectorEasyOcrService,
	IEasyOcrRuntime                easyOcrRuntime,
	IEnvironmentFacade             environmentFacade,
	ISevenZipFacade                sevenZipFacade
)
	: IPythonProvider
{
	private readonly SemaphoreSlim               _gate = new(1, 1);
	private          Task<EasyOcrRuntimeResult>? _warmupTask;

	public async Task<EasyOcrRuntimeResult> EnsureExtractedAsync(CancellationToken cancellationToken = default)
	{
		var cached = _warmupTask;
		if (cached is
			{
				IsFaulted: false, IsCanceled: false
			})
		{
			var result = await cached.ConfigureAwait(false);
			if (result.Success && fileFacade.FileExists(result.PythonPath ?? string.Empty))
			{
				return result;
			}
		}

		_warmupTask = ExtractAsync(cancellationToken);
		return await _warmupTask.ConfigureAwait(false);
	}

	public string? GetEmbeddedZipPath()
	{
		var osFolder = NativeArchitecture.GetTesseractOsFolder;
		if (string.IsNullOrWhiteSpace(osFolder))
		{
			return null;
		}

		var arch      = NativeArchitecture.GetCurrentArchFolder;
		var candidate = Path.Combine(AppContext.BaseDirectory, OcrConstants.PythonFolder, osFolder, arch, OcrConstants.PythonArchive);
		return fileFacade.FileExists(candidate) ? candidate : null;
	}

	public async Task<EasyOcrRuntimeResult> EnsureEasyOcrAsync(IProgress<string>? status = null, CancellationToken cancellationToken = default)
	{
		var runtime = await EnsureExtractedAsync(cancellationToken).ConfigureAwait(false);

		var hasEasyOcr = packageDetectorEasyOcrService.HasEasyOcrPackage();

		if (!runtime.Success || string.IsNullOrWhiteSpace(runtime.PythonPath) || hasEasyOcr)
		{
			return runtime;
		}

		status?.Report("Preparing embedded Python...");

		fileFacade.CreateDirectory(easyOcrRuntime.SitePackagesPath);

		status?.Report("Bootstrapping pip...");
		var pipReady = await EnsurePipAsync(runtime.PythonPath, cancellationToken).ConfigureAwait(false);
		if (!pipReady.Success)
		{
			return pipReady;
		}

		status?.Report("Installing Step 1...");
		var installResult = await InstallPackagesAsync(runtime.PythonPath, cancellationToken, status).ConfigureAwait(false);
		if (!installResult.Success)
		{
			return installResult;
		}

		var finalHasEasyOcr = packageDetectorEasyOcrService.HasEasyOcrPackage();
		return finalHasEasyOcr
			? new EasyOcrRuntimeResult(true,  runtime.PythonPath, null)
			: new EasyOcrRuntimeResult(false, runtime.PythonPath, OcrMessages.EasyOcrPackageMissing);
	}

	private async Task<EasyOcrRuntimeResult> ExtractAsync(CancellationToken cancellationToken)
	{
		await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			var pythonPath  = easyOcrRuntime.PythonExePath;
			var installRoot = easyOcrRuntime.InstallDirectory;
			var zipPath     = GetEmbeddedZipPath();
			var isHealthy = processSystemService.GetCurrentOs() == RuntimeOs.Windows
				? fileFacade.FileExists(pythonPath) && fileFacade.DirectoryExists(Path.Combine(installRoot, "Lib"))
				: fileFacade.FileExists(pythonPath) && fileFacade.DirectoryExists(Path.Combine(installRoot, "lib"));
			if (isHealthy)
			{
				fileFacade.CreateDirectory(easyOcrRuntime.SitePackagesPath);
				RewritePthFile(zipPath, installRoot);
				return new EasyOcrRuntimeResult(true, pythonPath, null);
			}

			if (string.IsNullOrWhiteSpace(zipPath))
			{
				return new EasyOcrRuntimeResult(false, null, string.Format(OcrMessages.EasyOcrEmbeddedPythonMissingFormat, BuildPlatformStamp()));
			}

			var rootFull = Path.GetFullPath(installRoot);
			fileFacade.CreateDirectory(rootFull);

			await TryExtractWithSevenZip(zipPath, rootFull).ConfigureAwait(false);
			await EnsureUnixExecutablePermissionsAsync(rootFull).ConfigureAwait(false);
			fileFacade.CreateDirectory(easyOcrRuntime.SitePackagesPath);
			RewritePthFile(zipPath, installRoot);

			return !fileFacade.FileExists(pythonPath) ? new EasyOcrRuntimeResult(false, null, string.Format(OcrMessages.EasyOcrPythonMissingAfterExtract)) : new EasyOcrRuntimeResult(true, pythonPath, null);
		}
		catch (Exception ex)
		{
			var root   = ex.GetBaseException();
			var detail = $"{root.GetType().Name}: {root.Message}";
			return new EasyOcrRuntimeResult(false, null, string.Format(OcrMessages.EasyOcrEmbeddedPythonFailedFormat, detail));
		}
		finally
		{
			_gate.Release();
		}
	}
	private async Task TryExtractWithSevenZip(string zipPath, string destination)
	{
		var baseDir = AppContext.BaseDirectory;
		var arch    = NativeArchitecture.GetCurrentArchFolder;
		try
		{
			sevenZipFacade.ExtractToDir(zipPath, destination, true, true, string.Empty);
			return;
		}
		catch (Exception)
		{
		}

		var pathBase = Path.Combine(baseDir, OcrConstants.PythonFolder, BuildPlatformStamp());
		var unixExe2 = EnumerateSevenZipExecutables(pathBase, OcrConstants.UnixSevenZipExecutableCandidates).FirstOrDefault();
		if (await GetBy(unixExe2, baseDir, zipPath, destination))
		{
			return;
		}

		var pathExe = FindSevenZipOnPath(OcrConstants.UnixSevenZipExecutableCandidates);
		if (await GetBy(pathExe, baseDir, zipPath, destination))
		{
			return;
		}

		throw new InvalidOperationException(
			$"No suitable 7z executable found for platform {processSystemService.GetCurrentOs()} (arch {arch}). Tried names: {string.Join(", ", OcrConstants.UnixSevenZipExecutableCandidates)} (searched app directory, runtimes, and PATH).");
	}

	private async Task<bool> GetBy(string? unixExe, string baseDir, string zipPath, string destination)
	{
		if (!string.IsNullOrWhiteSpace(unixExe))
		{
			if (ShouldChmod(unixExe, baseDir))
			{
				await EnsureUnixExecutableFlagAsync(unixExe).ConfigureAwait(false);
			}
			await RunSevenZipProcessAsync(unixExe, zipPath, destination).ConfigureAwait(false);
			return true;
		}
		return false;
	}
	private IEnumerable<string> EnumerateSevenZipExecutables(string directory, IEnumerable<string> names)
	{
		if (!fileFacade.DirectoryExists(directory))
		{
			yield break;
		}

		foreach (var name in names)
		{
			foreach (var candidate in BuildExecutableCandidates(directory, name))
			{
				if (fileFacade.FileExists(candidate))
				{
					yield return candidate;
				}
			}
		}
	}

	private async Task EnsureUnixExecutableFlagAsync(string path)
	{
		if (processSystemService.GetCurrentOs() == RuntimeOs.Windows)
		{
			return;
		}
		await processSystemService.RunAsync("chmod", ["+x", path]).ConfigureAwait(false);
	}

	private async Task EnsureUnixExecutablePermissionsAsync(string installRoot)
	{
		if (processSystemService.GetCurrentOs() == RuntimeOs.Windows)
		{
			return;
		}

		try
		{
			var toChmod = new List<string>();
			var binDir  = Path.Combine(installRoot, "bin");
			if (fileFacade.DirectoryExists(binDir))
			{
				toChmod.AddRange(Directory.EnumerateFiles(binDir, "*", SearchOption.AllDirectories));
			}

			toChmod.AddRange(OcrConstants.sourceArray.Select(name => Path.Combine(installRoot, name))
				.Where(fileFacade.FileExists));

			toChmod = toChmod.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			foreach (var file in toChmod)
			{
				await EnsureUnixExecutableFlagAsync(file).ConfigureAwait(false);
			}
		}
		catch (Exception)
		{
		}
	}

	private static bool ShouldChmod(string path, string baseDir)
	{
		try
		{
			var full = Path.GetFullPath(path);
			var root = Path.GetFullPath(baseDir);
			return full.StartsWith(root, StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			return false;
		}
	}

	private async Task RunSevenZipProcessAsync(string exePath, string zipPath, string destination)
	{
		var args = processSystemService.GetCurrentOs() == RuntimeOs.Windows
			? new[]
			{
				"x", "-y", "-o" + destination, zipPath
			}
			: new[]
			{
				"x", "-y", "-snl", "-o" + destination, zipPath
			};
		await processSystemService.RunAsync(exePath, args, destination).ConfigureAwait(false);
	}

	private IEnumerable<string> BuildExecutableCandidates(string directory, string name)
	{
		yield return Path.Combine(directory, name);
		if (processSystemService.GetCurrentOs() == RuntimeOs.Windows)
		{
			yield return Path.Combine(directory, $"{name}.exe");
		}
	}

	private string? FindSevenZipOnPath(IEnumerable<string> candidates)
	{
		var pathValue = environmentFacade.GetEnvironmentVariable("PATH");
		if (string.IsNullOrWhiteSpace(pathValue))
		{
			return null;
		}

		foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
		{
			foreach (var name in candidates)
			{
				foreach (var candidate in BuildExecutableCandidates(directory, name))
				{
					if (fileFacade.FileExists(candidate))
					{
						return candidate;
					}
				}
			}
		}

		return null;
	}

	private string BuildPlatformStamp()
	{
		var arch = NativeArchitecture.GetCurrentArchFolder;
		var os   = NativeArchitecture.GetTesseractOsFolder;
		return $"{os}/{arch}";
	}

	private void RewritePthFile(string? zipPath, string installRoot)
	{
		if (string.IsNullOrWhiteSpace(zipPath))
		{
			return;
		}

		try
		{
			var name    = Path.GetFileNameWithoutExtension(zipPath) + "._pth";
			var pthFile = Path.Combine(installRoot, name);
			fileFacade.WriteAllLines(pthFile, [Path.GetFileName(zipPath) ?? "python.7z", ".", "Lib", @"Lib\site-packages", "import site"]);
		}
		catch (Exception)
		{
		}
	}

	private async Task<EasyOcrRuntimeResult> EnsurePipAsync(string pythonPath, CancellationToken cancellationToken)
	{
		var env = BuildPythonEnv();
		if (await IsPipAvailableAsync(pythonPath, env, cancellationToken).ConfigureAwait(false))
		{
			return new EasyOcrRuntimeResult(true, pythonPath, null);
		}

		using var ensureCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		ensureCts.CancelAfter(TimeSpan.FromMinutes(2));
		var ensure = await RunPythonAsync(pythonPath, ["-m", "ensurepip", "--default-pip"], env, ensureCts.Token).ConfigureAwait(false);
		if (!ensure.Success)
		{
			using var bootstrapCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			bootstrapCts.CancelAfter(TimeSpan.FromMinutes(3));
			var bootstrap = await BootstrapPipAsync(pythonPath, env, bootstrapCts.Token).ConfigureAwait(false);
			if (!bootstrap.Success)
			{
				return bootstrap;
			}
		}

		return await IsPipAvailableAsync(pythonPath, env, cancellationToken).ConfigureAwait(false)
			? new EasyOcrRuntimeResult(true,  pythonPath, null)
			: new EasyOcrRuntimeResult(false, pythonPath, "pip bootstrap failed: pip still unavailable after installation attempt.");
	}

	private async Task<EasyOcrRuntimeResult> BootstrapPipAsync(string pythonPath, FrozenDictionary<string, string> env, CancellationToken cancellationToken)
	{
		try
		{
			var workingDir = GetWorkingDirectory();
			var body       = await httpClientAdapter.Get(Links.Pip, cancellationToken);
			var tempFile   = Path.Combine(workingDir, $"shotora_getpip_{Guid.NewGuid():N}.py");
			await fileFacade.WriteAll(tempFile, body, cancellationToken);

			using var runCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			runCts.CancelAfter(TimeSpan.FromMinutes(3));
			var run = await RunPythonAsync(pythonPath, [tempFile], env, runCts.Token).ConfigureAwait(false);
			fileFacade.TryDelete(tempFile);
			return run.Success ? new EasyOcrRuntimeResult(true, pythonPath, null) : new EasyOcrRuntimeResult(false, pythonPath, $"pip bootstrap failed: {run.ErrorMessage}");
		}
		catch (Exception ex)
		{
			return new EasyOcrRuntimeResult(false, pythonPath, $"pip bootstrap failed: {ex.GetBaseException().Message}");
		}
	}

	private async Task<EasyOcrRuntimeResult> InstallPackagesAsync(string pythonPath, CancellationToken cancellationToken, IProgress<string>? status)
	{
		try
		{
			var numpyConstraint = processSystemService.GetCurrentOs() == RuntimeOs.Mac ? "numpy<2" : "numpy>=1.26.4";
			return await EasyOcrRuntimeResult(numpyConstraint, pythonPath, cancellationToken, status);
		}
		catch (Exception)
		{
			return await EasyOcrRuntimeResult("numpy<2", pythonPath, cancellationToken, status);
		}
	}

	private async Task<EasyOcrRuntimeResult> EasyOcrRuntimeResult(string numpyConstraint, string pythonPath, CancellationToken cancellationToken, IProgress<string>? status)
	{
		var env = BuildPythonEnv();
		var buildDepsArgs = BuildPipArgs(
			easyOcrRuntime.SitePackagesPath,
			OcrConstants.BuildDependencyPackages.Prepend(numpyConstraint),
			false,
			false);

		using var buildCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		buildCts.CancelAfter(TimeSpan.FromMinutes(5));
		await RunPythonAsync(pythonPath, buildDepsArgs, env, buildCts.Token).ConfigureAwait(false);

		var args = BuildPipArgs(
			easyOcrRuntime.SitePackagesPath,
			OcrConstants.EasyOcrPackages.Prepend(numpyConstraint),
			true,
			true);

		status?.Report("Installing Step 2 EasyOCR...");
		using var installCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		installCts.CancelAfter(TimeSpan.FromMinutes(6));
		await RunPythonAsync(pythonPath, args, env, installCts.Token).ConfigureAwait(false);
		return new EasyOcrRuntimeResult(true, pythonPath, null);
	}
	private FrozenDictionary<string, string> BuildPythonEnv()
	{
		var env = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		var installDir   = easyOcrRuntime.InstallDirectory;
		var sitePackages = easyOcrRuntime.SitePackagesPath;
		var currentPath  = environmentFacade.GetEnvironmentVariable("PATH");

		var tempDir = Path.Combine(installDir, "tmp");
		fileFacade.CreateDirectory(installDir);
		fileFacade.CreateDirectory(sitePackages);
		fileFacade.CreateDirectory(tempDir);
		Set("PYTHONHOME", installDir);
		Set("PYTHONPATH", sitePackages);
		Set("PATH", string.Join(Path.PathSeparator.ToString(), new[]
		{
			installDir, Path.Combine(installDir, "Scripts"), currentPath
		}.Where(s => !string.IsNullOrWhiteSpace(s))));
		Set("PYTHONNOUSERSITE",       "1");
		Set("TEMP",                   tempDir);
		Set("TMP",                    tempDir);
		Set("TMPDIR",                 tempDir);
		Set("PIP_NO_BUILD_ISOLATION", "0");

		var libEnvVar = NativeArchitecture.GetUnixLibraryEnvVar();
		if (!string.IsNullOrWhiteSpace(libEnvVar))
		{
			var libDir         = Path.Combine(installDir, "lib");
			var currentLibPath = environmentFacade.GetEnvironmentVariable(libEnvVar);
			var newLibPath     = string.IsNullOrWhiteSpace(currentLibPath) ? libDir : $"{libDir}{Path.PathSeparator}{currentLibPath}";
			Set(libEnvVar, newLibPath);
		}

		return env.ToFrozenDictionary();

		void Set(string key, string value)
		{
			env[key] = value;
		}
	}

	private async Task<(bool Success, string? ErrorMessage)> RunPythonAsync(
		string                           pythonPath,
		IEnumerable<string>              args,
		FrozenDictionary<string, string> env,
		CancellationToken                cancellationToken,
		bool                             retryOnPermissionDenied = true)
	{
		var argsList = args.ToList();
		try
		{
			var workingDir = GetWorkingDirectory();
			if (processSystemService.GetCurrentOs() != RuntimeOs.Windows)
			{
				await EnsureUnixExecutableFlagAsync(pythonPath).ConfigureAwait(false);
			}
			var psi = processSystemService.CreateProcessStartInfo(pythonPath, argsList, workingDir);

			foreach (var kv in env)
			{
				psi.Environment[kv.Key] = kv.Value;
			}

			NativeArchitecture.GetUnixLibraryEnvVar();

			var run = await processSystemService.RunAsync(psi, cancellationToken).ConfigureAwait(false);
			return run.ExitCode != 0 ? (false, run.StdErr) : (true, null);
		}
		catch (Exception ex)
		{
			var message = ex.GetBaseException().Message;
			if (retryOnPermissionDenied && processSystemService.GetCurrentOs() != RuntimeOs.Windows && message.Contains("Permission denied", StringComparison.OrdinalIgnoreCase))
			{
				await EnsureUnixExecutablePermissionsAsync(easyOcrRuntime.InstallDirectory).ConfigureAwait(false);
				await EnsureUnixExecutableFlagAsync(pythonPath).ConfigureAwait(false);
				return await RunPythonAsync(pythonPath, argsList, env, cancellationToken, false).ConfigureAwait(false);
			}

			return (false, message);
		}
	}

	private string GetWorkingDirectory()
	{
		var installDir = easyOcrRuntime.InstallDirectory;
		var tempDir    = Path.Combine(installDir, "tmp");
		fileFacade.CreateDirectory(tempDir);
		return tempDir;
	}

	private static List<string> BuildPipArgs(
		string              targetPath,
		IEnumerable<string> packages,
		bool                preferBinary,
		bool                noBuildIsolation,
		bool                noDeps = false)
	{
		var args = new List<string>
		{
			"-m",
			"pip",
			"install",
			"--no-warn-script-location",
			"--no-cache-dir",
			"--upgrade",
			"--no-input",
			"--target",
			targetPath
		};

		if (noDeps)
		{
			args.Add("--no-deps");
		}

		if (noBuildIsolation)
		{
			args.Add("--no-build-isolation");
		}

		if (preferBinary)
		{
			args.Add("--prefer-binary");
		}

		args.AddRange(packages);
		return args;
	}

	private async Task<bool> IsPipAvailableAsync(string pythonPath, FrozenDictionary<string, string> env, CancellationToken cancellationToken)
	{
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(TimeSpan.FromSeconds(10));
		var result = await RunPythonAsync(pythonPath, ["-m", "pip", "--version"], env, cts.Token).ConfigureAwait(false);
		return result.Success;
	}
}
