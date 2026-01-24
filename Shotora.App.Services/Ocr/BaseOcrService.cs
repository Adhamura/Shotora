using System.Collections.Frozen;
using System.Diagnostics;
using NativeSupport.Constants;
using NativeSupport.Enums;
using Shared.Interfaces.Facades;
using Shared.Models.Constants;
using Shotora.App.Interfaces.Adapters;
using Shotora.App.Interfaces.Ocr;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Ocr.Tesseract;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ItemModels;
using SkiaSharp;
using Tesseract;
using AvaloniaRect=Avalonia.Rect;

namespace Shotora.App.Services.Ocr;

public class BaseOcrService(
	ITessdataTesseractService  tessdataTesseractService,
	IMaintenanceEasyOcrService maintenanceEasyOcrRuntime,
	ISkiaImageAdapter          skiaImageAdapter,
	IProcessSystemService      processSystemService,
	IFileFacade                fileFacade,
	IEasyOcrRuntime            easyOcrRuntime) : IBaseOcrService
{
	private static readonly FrozenDictionary<string, string> EasyOcrCodeMap = new Dictionary<string, string>(LanguageCollection.EasyOcrCodeMap, StringComparer.OrdinalIgnoreCase).ToFrozenDictionary();
	public async Task<OcrResultDataItemModel> RecognizeAsync(SKBitmap source, AvaloniaRect selection, string languages, OcrEngineType engine)
	{
		if (selection.Width < 2 || selection.Height < 2)
		{
			return new OcrResultDataItemModel(null, OcrMessages.SelectionTooSmall);
		}

		var cropBytes = skiaImageAdapter.ToPngBytes(source, selection);
		if (cropBytes == null)
		{
			return new OcrResultDataItemModel(null, OcrMessages.SelectionReadFailed);
		}

		var langCodes = NormalizeLanguageCodes(languages);

		var (text, error) = engine == OcrEngineType.EasyOcr
			? await TryRecognizeWithEasyOcrAsync(cropBytes, langCodes)
			: await TryRecognizeWithTesseractAsync(cropBytes, langCodes);

		return !string.IsNullOrWhiteSpace(text)
			? new OcrResultDataItemModel(text.Trim(), null)
			: new OcrResultDataItemModel(null,        error);
	}

	private async Task<(string? Text, string? Error)> TryRecognizeWithTesseractAsync(byte[] cropBytes, string[] langCodes)
	{
		var     langString    = string.Join("+", langCodes);
		string? effectivePath = null;
		var     macDirs       = ConfigureTesseractSearch();

		try
		{
			var ensureResult = await tessdataTesseractService.EnsureAsync(langCodes);
			if (ensureResult.Success && !string.IsNullOrWhiteSpace(ensureResult.TessdataPath))
			{
				effectivePath = ensureResult.TessdataPath;
				var resolvedLangs = ensureResult.ResolvedLanguages.Length > 0 ? ensureResult.ResolvedLanguages : langCodes;
				langString = string.Join("+", resolvedLangs);
			}

			if (!string.IsNullOrWhiteSpace(effectivePath))
			{
				var managedResult = await Task.Run(() =>
				{
					using var engineInstance = new TesseractEngine(effectivePath, langString);
					using var pix            = Pix.LoadFromMemory(cropBytes);
					using var page           = engineInstance.Process(pix);
					var       text           = page.GetText()?.Trim();
					return text;
				});

				if (!string.IsNullOrWhiteSpace(managedResult))
				{
					return (managedResult, null);
				}
			}
		}
		catch (Exception)
		{
		}

		var (cliResult, cliError) = await TryRecognizeWithTesseractCliAsync(cropBytes, langString, effectivePath ?? OcrConstants.TessDataDir, macDirs);
		return (cliResult, cliError);
	}

	private string[] ConfigureTesseractSearch()
	{
		var baseDir    = AppContext.BaseDirectory;
		var os         = processSystemService.GetCurrentOs();
		var arch       = NativeArchitecture.GetCurrentArchFolder;
		var candidates = GetMacTesseractCandidateDirs(baseDir, os, arch);
		return candidates.Length == 0 ? [baseDir] : candidates;
	}
	private string[] ResolveTesseractCliSearchDirs(IReadOnlyCollection<string>? macDirs)
	{
		var osFolder = NativeArchitecture.GetTesseractOsFolder;
		var candidates = osFolder == null
			? []
			: macDirs?.Where(d => !string.IsNullOrWhiteSpace(d)).ToArray() ?? GetMacTesseractCandidateDirs(os: processSystemService.GetCurrentOs(), type: NativeArchitecture.GetCurrentArchFolder);

		var existingDirs = candidates.Where(Directory.Exists).ToArray();
		return existingDirs;
	}

	private async Task<(string? Text, string? Error)> TryRecognizeWithTesseractCliAsync(
		byte[]                       imageBytes,
		string                       languages,
		string?                      tessdataPath,
		IReadOnlyCollection<string>? macDirs)
	{
		var macCandidates = ResolveTesseractCliSearchDirs(macDirs);
		var primaryMacDir = macCandidates.FirstOrDefault() ?? string.Empty;

		var executableName = NativeArchitecture.GetTesseractExecutableName();
		var tesseractPath  = FindExecutableInPath(executableName, macCandidates);

		if (string.IsNullOrEmpty(tesseractPath) && !string.IsNullOrWhiteSpace(primaryMacDir))
		{
			var candidate = Path.Combine(primaryMacDir, "tesseract");
			if (fileFacade.FileExists(candidate))
			{
				tesseractPath = candidate;
			}
		}

		if (!string.IsNullOrEmpty(tesseractPath) && !fileFacade.FileExists(tesseractPath))
		{
			tesseractPath = null;
		}
		if (string.IsNullOrWhiteSpace(tesseractPath))
		{
			return (null, null);
		}

		try
		{
			return await WithTempPngAsync(imageBytes, async tempImage =>
			{
				var psi = processSystemService.CreateUtf8ProcessStartInfo(
					tesseractPath,
					$"\"{tempImage}\" stdout -l {languages} --psm 6");

				if (!string.IsNullOrWhiteSpace(tessdataPath) && Directory.Exists(tessdataPath))
				{
					psi.Environment["TESSDATA_PREFIX"] = tessdataPath;
				}

				if (macCandidates.Length > 0)
				{
					var extraPath = string.Join(Path.PathSeparator, macCandidates);
					psi.Environment["PATH"] = string.Join(Path.PathSeparator, extraPath, Environment.GetEnvironmentVariable("PATH") ?? string.Empty);
					var libEnv = NativeArchitecture.GetUnixLibraryEnvVar();
					if (!string.IsNullOrWhiteSpace(libEnv))
					{
						var libPath = string.Join(Path.PathSeparator, extraPath, Environment.GetEnvironmentVariable(libEnv) ?? string.Empty);
						psi.Environment[libEnv] = libPath;
					}
				}

				var run = await RunProcessAsync(psi, TimeSpan.FromSeconds(30), OcrMessages.TesseractStartFailed, OcrMessages.TesseractTimeout);
				return MapProcessResult(run, OcrMessages.TesseractReturnedEmpty, OcrMessages.TesseractCliErrorFormat);
			});
		}
		catch (Exception ex)
		{
			return (null, $"tesseract CLI failed: {ex.Message}");
		}
	}

	private async Task<(string? Text, string? Error)> TryRecognizeWithEasyOcrAsync(byte[] cropBytes, string[] langCodes)
	{
		var mappedLangs = MapEasyOcrLanguages(langCodes);
		var langs       = mappedLangs.Length == 0 ? LanguageCollection.DefaultOcrLanguageCode : string.Join(",", mappedLangs);

		var runtime = await maintenanceEasyOcrRuntime.EnsureRuntimeAsync();
		if (!runtime.Success || string.IsNullOrWhiteSpace(runtime.PythonPath) || !fileFacade.FileExists(runtime.PythonPath))
		{
			var msg = runtime.Error ?? OcrMessages.EasyOcrRuntimeMissing;
			return (null, msg);
		}

		var pythonPath = runtime.PythonPath;
		var script     = await maintenanceEasyOcrRuntime.EnsureSupportScriptAsync();
		var sitePath   = easyOcrRuntime.SitePackagesPath;
		var installDir = easyOcrRuntime.InstallDirectory;

		fileFacade.CreateDirectory(sitePath);
		fileFacade.CreateDirectory(installDir);

		try
		{
			return await WithTempPngAsync(cropBytes, async tempImage =>
			{
				var psi = processSystemService.CreateUtf8ProcessStartInfo(pythonPath, $"\"{script}\" \"{langs}\" \"{tempImage}\"", installDir);

				if (!string.IsNullOrWhiteSpace(installDir))
				{
					psi.Environment["PYTHONHOME"] = installDir;
					var currentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
					psi.Environment["PATH"] = string.IsNullOrWhiteSpace(currentPath)
						? installDir
						: string.Join(Path.PathSeparator.ToString(), installDir, currentPath);
				}
				if (!string.IsNullOrWhiteSpace(sitePath))
				{
					psi.Environment["PYTHONPATH"]       = sitePath;
					psi.Environment["PYTHONNOUSERSITE"] = "1";
				}
				psi.Environment["PYTHONIOENCODING"] = "utf-8";
				psi.Environment["PYTHONUTF8"]       = "1";

				var run = await RunProcessAsync(psi, TimeSpan.FromSeconds(45), OcrMessages.EasyOcrStartFailed, OcrMessages.EasyOcrTimeout);
				if (run.ExitCode == 2 && run.StdErr.StartsWith("EASYOCR_IMPORT_ERROR:", StringComparison.OrdinalIgnoreCase))
				{
					var detail = run.StdErr.Length > "EASYOCR_IMPORT_ERROR:".Length
						? run.StdErr["EASYOCR_IMPORT_ERROR:".Length..].Trim()
						: string.Empty;
					var msg = string.IsNullOrWhiteSpace(detail)
						? OcrMessages.EasyOcrPackageMissing
						: $"{OcrMessages.EasyOcrPackageMissing} ({detail})";
					return (null, msg);
				}
				return MapProcessResult(run, OcrMessages.EasyOcrReturnedEmpty, OcrMessages.EasyOcrErrorFormat);
			});
		}
		catch (Exception ex)
		{
			return (null, $"EasyOCR failed: {ex.Message}");
		}
	}

	private string? FindExecutableInPath(string fileName, IEnumerable<string>? additionalDirectories = null)
	{
		var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
		var paths   = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
		if (additionalDirectories != null)
		{
			paths.AddRange(additionalDirectories.Where(d => !string.IsNullOrWhiteSpace(d)));
		}

		foreach (var dir in paths)
		{
			try
			{
				var candidate = Path.Combine(dir, fileName);
				if (fileFacade.FileExists(candidate))
				{
					return candidate;
				}
			}
			catch
			{
			}
		}

		return null;
	}

	private static string[] GetMacTesseractCandidateDirs(string? preferred = null, RuntimeOs os = RuntimeOs.Mac, string? type = null)
	{
		var osFolder = NativeArchitecture.GetTesseractOsFolder;
		if (string.IsNullOrWhiteSpace(osFolder))
		{
			return [];
		}

		var arch      = string.IsNullOrWhiteSpace(type) ? NativeArchitecture.GetCurrentArchFolder : type;
		var baseDir   = AppContext.BaseDirectory;
		var parentDir = Path.GetDirectoryName(baseDir) ?? baseDir;
		var resources = Path.Combine(parentDir, "Resources");

		var dirs = new List<string>();
		if (!string.IsNullOrWhiteSpace(preferred))
		{
			dirs.Add(preferred);
		}

		dirs.Add(baseDir);
		dirs.Add(Path.Combine(baseDir,   arch));
		dirs.Add(Path.Combine(baseDir,   NativeArchitecture.BinariesFolder, osFolder, arch));
		dirs.Add(Path.Combine(parentDir, NativeArchitecture.BinariesFolder, osFolder, arch));
		dirs.Add(Path.Combine(resources, NativeArchitecture.BinariesFolder, osFolder, arch));
		return dirs.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
	}

	private static string[] NormalizeLanguageCodes(string languages)
	{
		if (string.IsNullOrWhiteSpace(languages))
		{
			return [LanguageCollection.DefaultOcrTesseractCode];
		}

		var langCodes = languages.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return langCodes.Length == 0 ? [LanguageCollection.DefaultOcrTesseractCode] : langCodes;
	}

	private static string[] MapEasyOcrLanguages(string[] langCodes)
	{
		if (langCodes.Length == 0)
		{
			return [LanguageCollection.DefaultOcrLanguageCode];
		}

		var languages = langCodes
			.Select(code => EasyOcrCodeMap.GetValueOrDefault(code, code))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		return languages.Length == 0 ? [LanguageCollection.DefaultOcrLanguageCode] : languages;
	}

	private async Task<T> WithTempPngAsync<T>(byte[] imageBytes, Func<string, Task<T>> action)
	{
		var tempImage = Path.Combine(Path.GetTempPath(), $"shotora_ocr_{Guid.NewGuid():N}.png");
		try
		{
			await fileFacade.WriteAll(tempImage, imageBytes);
			return await action(tempImage);
		}
		finally
		{
			fileFacade.TryDelete(tempImage);
		}
	}

	private static (string? Text, string? Error) MapProcessResult(
		(int? ExitCode, string StdOut, string StdErr, string? Error) run,
		string                                                       emptyOutputError,
		string                                                       errorFormat)
	{
		if (run.Error != null)
		{
			return (null, run.Error);
		}

		if (run.ExitCode == 0)
		{
			return string.IsNullOrWhiteSpace(run.StdOut) ? (null, emptyOutputError) : (run.StdOut, null);
		}

		return (null, string.Format(errorFormat, run.ExitCode ?? -1, run.StdErr));
	}

	private async static Task<(int? ExitCode, string StdOut, string StdErr, string? Error)> RunProcessAsync(ProcessStartInfo psi, TimeSpan timeout, string startError, string timeoutError)
	{
		try
		{
			using var process = Process.Start(psi);
			if (process == null)
			{
				return (null, string.Empty, string.Empty, startError);
			}

			var stdOutTask = process.StandardOutput.ReadToEndAsync();
			var stdErrTask = process.StandardError.ReadToEndAsync();

			using var cts = new CancellationTokenSource(timeout);
			try
			{
				await process.WaitForExitAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
				TryKill(process);
				return (null, string.Empty, string.Empty, timeoutError);
			}

			var stdOut = (await stdOutTask).Trim();
			var stdErr = (await stdErrTask).Trim();
			return (process.ExitCode, stdOut, stdErr, null);
		}
		catch (Exception ex)
		{
			return (null, string.Empty, string.Empty, ex.GetBaseException().Message);
		}
	}

	private static void TryKill(Process process)
	{
		try
		{
			process.Kill(true);
		}
		catch
		{
		}
	}
}