using NativeSupport.Enums;
using Shared.Interfaces.Adapters;
using Shared.Interfaces.Facades;
using Shared.Models.Constants;
using Shotora.App.Interfaces.Ocr.Tesseract;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Utilities;

namespace Shotora.App.Services.Ocr.Tesseract;

public class TessdataTesseractService(
	IProcessSystemService processSystemService,
	IFileFacade           fileFacade,
	IHttpClientAdapter    httpClientAdapter) : ITessdataTesseractService
{
	private readonly RuntimeOs _currentOs = processSystemService.GetCurrentOs();

	public async Task<TessdataEnsureResult> EnsureAsync(IEnumerable<string> languageCodes, CancellationToken cancellationToken = default)
	{
		var requested = NormalizeLanguages(languageCodes);

		if (TryGetExistingPath(requested, out var existingPath))
		{
			return new TessdataEnsureResult(true, existingPath, requested, null);
		}

		foreach (var lang in requested)
		{
			var downloadResult = await DownloadInternalAsync(lang, null, cancellationToken);
			if (!downloadResult.Success)
			{
				return await TryFallbackToEnglishAsync(requested, downloadResult.Error, cancellationToken);
			}
		}

		if (TryGetExistingPath(requested, out var afterDownload))
		{
			return new TessdataEnsureResult(true, afterDownload, requested, null);
		}

		return await TryFallbackToEnglishAsync(requested, "Unable to resolve tessdata path after download.", cancellationToken);
	}

	public async Task<TessdataDownloadResult> DownloadAsync(string languageCode, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		return await DownloadInternalAsync(languageCode, progress, cancellationToken);
	}

	public bool IsInstalled(string languageCode)
	{
		var path = OcrConstants.GetLanguagePath(languageCode);
		return fileFacade.FileExists(path);
	}

	private async Task<TessdataEnsureResult> TryFallbackToEnglishAsync(string[] requested, string? error, CancellationToken cancellationToken)
	{
		if (requested.Contains(LanguageCollection.DefaultOcrTesseractCode, StringComparer.OrdinalIgnoreCase))
		{
			return new TessdataEnsureResult(false, null, requested, error ?? "Tessdata not found.");
		}

		var fallback = new[]
		{
			LanguageCollection.DefaultOcrTesseractCode
		};
		var downloadResult = await DownloadInternalAsync(LanguageCollection.DefaultOcrTesseractCode, null, cancellationToken);
		if (downloadResult.Success && TryGetExistingPath(fallback, out var engPath))
		{
			return new TessdataEnsureResult(true, engPath, fallback, error);
		}

		return new TessdataEnsureResult(false, null, requested, error ?? "Tessdata not found.");
	}

	private async Task<TessdataDownloadResult> DownloadInternalAsync(string languageCode, IProgress<double>? progress, CancellationToken cancellationToken)
	{
		try
		{
			if (fileFacade.FileExists(OcrConstants.GetLanguagePath(languageCode)))
			{
				progress?.Report(100);
				return new TessdataDownloadResult(true, null);
			}

			fileFacade.CreateDirectory(OcrConstants.TessDataDir);
			var tempPath = OcrConstants.TessDataDir + ".tmp";

			try
			{
				foreach (var urlBase in new[]
						 {
							 OcrConstants.TessdataBestBase, OcrConstants.TessdataFastBase
						 })
				{
					var url        = $"{urlBase}/{languageCode}{OcrConstants.TessdataExtension}";
					var downloaded = await httpClientAdapter.DownloadFileAsync(url, tempPath, progress, cancellationToken);
					if (!downloaded)
					{
						continue;
					}

					var filePath = Path.Combine(OcrConstants.TessDataDir, $"{languageCode}{OcrConstants.TessdataExtension}");

					fileFacade.Move(tempPath, filePath);

					return fileFacade.FileExists(filePath)
						? new TessdataDownloadResult(true,  null)
						: new TessdataDownloadResult(false, $"Downloaded tessdata for '{languageCode}' but file was empty.");
				}

				return new TessdataDownloadResult(false, $"Failed to download tessdata for '{languageCode}'.");
			}
			finally
			{
				fileFacade.TryDelete(tempPath);
			}
		}
		catch (Exception ex)
		{
			return new TessdataDownloadResult(false, ex.Message);
		}
	}

	private bool TryGetExistingPath(IEnumerable<string> languageCodes, out string path)
	{
		var candidates = GetCandidateDirectories();
		foreach (var candidate in candidates)
		{
			if (!Directory.Exists(candidate))
			{
				continue;
			}
			var hasAll = languageCodes.All(lang => fileFacade.FileExists(OcrConstants.GetLanguagePath(lang, candidate)));
			if (!hasAll)
			{
				continue;
			}

			path = candidate;
			return true;
		}

		path = string.Empty;
		return false;
	}

	private IReadOnlyList<string> GetCandidateDirectories()
	{
		var directories = new List<string>
		{
			OcrConstants.TessDataDir,
			Path.Combine(AppContext.BaseDirectory, "tessdata")
		};

		if (_currentOs != RuntimeOs.Windows)
		{
			var tessDataPrefix = Environment.GetEnvironmentVariable("TESSDATA_PREFIX");
			if (!string.IsNullOrWhiteSpace(tessDataPrefix))
			{
				directories.Insert(0, tessDataPrefix);
			}
		}

		return directories;
	}

	private static string[] NormalizeLanguages(IEnumerable<string> languageCodes)
	{
		return languageCodes?
			.Where(l => !string.IsNullOrWhiteSpace(l))
			.Select(l => l.Trim())
			.Where(l => l.Length > 0)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray() ?? [LanguageCollection.DefaultOcrTesseractCode];
	}
}
