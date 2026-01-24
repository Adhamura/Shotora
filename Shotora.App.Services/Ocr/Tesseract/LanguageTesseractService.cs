using Shared.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr.Tesseract;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using TesseractLanguageOption=Shotora.App.Models.Utilities.TesseractLanguageOption;

namespace Shotora.App.Services.Ocr.Tesseract;

public class LanguageTesseractService(ITessdataTesseractService tessdataTesseractService, ISettingsProvider settingsProvider, IFileFacade fileFacade, ILocalizationProvider localizationProvider)
	: ILanguageTesseractService
{
	public void PopulateLanguages(ICollection<TesseractLanguageOption> target, Action<TesseractLanguageOption>? onCreated = null)
	{
		target.Clear();
		foreach (var option in CreateLanguageOptions())
		{
			InitializePresentation(option);
			onCreated?.Invoke(option);
			target.Add(option);
		}
		UpdateInstallationStates(target);
	}

	public async Task DownloadAsync(TesseractLanguageOption option, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		var failedTemplate = localizationProvider.GetString(LocalizationKeys.TessStatusFailedFmt, LocalizationFallbacks.Tesseract.FailedFormat);
		option.CanDownload = false;

		if (tessdataTesseractService.IsInstalled(option.Code))
		{
			option.IsInstalled = true;
			option.Progress    = 100;
			option.StatusText  = localizationProvider.GetString(LocalizationKeys.TessStatusAlreadyInstalled, LocalizationFallbacks.Tesseract.AlreadyInstalled);
			option.CanDownload = false;
			return;
		}

		option.IsDownloading = true;
		option.Progress      = 0;
		option.StatusText    = localizationProvider.GetString(LocalizationKeys.TessStatusStarting, LocalizationFallbacks.Tesseract.Starting);

		var progressWrapper = new Progress<double>(p =>
		{
			option.Progress = p;
			progress?.Report(p);
		});

		try
		{
			var downloadResult = await tessdataTesseractService.DownloadAsync(option.Code, progressWrapper, cancellationToken);
			if (downloadResult.Success && tessdataTesseractService.IsInstalled(option.Code))
			{
				option.IsInstalled = true;
				option.Progress    = 100;
				option.StatusText  = localizationProvider.GetString(LocalizationKeys.TessStatusDownloaded, LocalizationFallbacks.Tesseract.Downloaded);
				option.CanDownload = false;
				return;
			}

			option.IsInstalled = false;
			option.Progress    = 0;
			option.StatusText  = string.Format(failedTemplate, downloadResult.Error ?? "Unknown failure");
			option.CanDownload = true;
		}
		catch (Exception ex)
		{
			option.StatusText  = string.Format(failedTemplate, ex.Message);
			option.Progress    = 0;
			option.CanDownload = true;
		}
		finally
		{
			option.IsDownloading = false;
			option.CanDownload   = !option.IsInstalled;
		}
	}

	public void RemoveLanguage(TesseractLanguageOption option)
	{
		var removed = fileFacade.TryDelete(OcrConstants.GetLanguagePath(option.Code));
		if (!removed || tessdataTesseractService.IsInstalled(option.Code))
		{
			return;
		}

		option.IsInstalled = false;
		option.Progress    = 0;
		option.StatusText  = localizationProvider.GetString(LocalizationKeys.TessStatusNotInstalled, LocalizationFallbacks.Tesseract.NotInstalled);
		option.CanDownload = true;
	}

	private IReadOnlyList<TesseractLanguageOption> CreateLanguageOptions()
	{
		return settingsProvider.BuildTesseractLanguages();
	}

	private void UpdateInstallationStates(IEnumerable<TesseractLanguageOption> options)
	{
		var ready = localizationProvider.GetString(LocalizationKeys.TessStatusReady, LocalizationFallbacks.Tesseract.Ready);
		foreach (var option in options)
		{
			var installed = tessdataTesseractService.IsInstalled(option.Code);
			option.IsInstalled = installed;
			option.Progress    = installed ? 100 : 0;
			if (installed)
			{
				option.StatusText = ready;
			}
			option.CanDownload = !installed;
		}
	}

	private void InitializePresentation(TesseractLanguageOption option)
	{
		var localized = string.IsNullOrWhiteSpace(option.LocalizationKey)
			? option.EnglishName
			: localizationProvider.GetString(option.LocalizationKey, option.EnglishName);

		var display = string.Equals(localized, option.EnglishName, StringComparison.OrdinalIgnoreCase)
			? localized
			: $"{localized} ({option.EnglishName})";

		option.DisplayName   = display;
		option.StatusText    = localizationProvider.GetString(LocalizationKeys.TessStatusNotInstalled, LocalizationFallbacks.Tesseract.NotInstalled);
		option.IsInstalled   = false;
		option.IsDownloading = false;
		option.Progress      = 0;
		option.CanDownload   = true;
	}
}
