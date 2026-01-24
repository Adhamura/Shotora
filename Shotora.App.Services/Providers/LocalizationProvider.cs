using System.Collections.Concurrent;
using System.Globalization;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Shared.Models.Constants;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using AvaloniaResourceDictionary=Avalonia.Controls.ResourceDictionary;
using AvaloniaApp=Avalonia.Application;

namespace Shotora.App.Services.Providers;

public class LocalizationProvider : ILocalizationProvider
{
	private readonly ConcurrentDictionary<string, AvaloniaResourceDictionary> _avaloniaLanguageCache = new(StringComparer.OrdinalIgnoreCase);
	private          string                                                   _currentLanguage       = LanguageCollection.DefaultOcrLanguageCode;

	public event EventHandler? LanguageChanged;

	public void SetLanguage(string languageCode)
	{
		if (string.IsNullOrWhiteSpace(languageCode))
		{
			languageCode = LanguageCollection.DefaultOcrLanguageCode;
		}

		if (string.Equals(_currentLanguage, languageCode, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		_currentLanguage = languageCode;
		var culture = new CultureInfo(languageCode);
		Thread.CurrentThread.CurrentUICulture = culture;
		Thread.CurrentThread.CurrentCulture   = culture;
		LanguageChanged?.Invoke(this, EventArgs.Empty);
	}

	public string GetString(string key, string fallback = "")
	{
		return TryGetAvaloniaString(key) ?? fallback;
	}

	public string GetStringForLanguage(string languageCode, string key, string fallback = "")
	{
		languageCode = string.IsNullOrWhiteSpace(languageCode) ? LanguageCollection.DefaultOcrLanguageCode : languageCode;

		return TryGetFromAvaloniaDictionary(languageCode, key) ?? fallback;
	}

	public EasyOcrStatusTexts BuildEasyOcrStatusTexts()
	{
		return new EasyOcrStatusTexts
		{
			Ready                   = GetString(LocalizationKeys.EasyOcrStatusReady,              LocalizationFallbacks.EasyOcr.Ready),
			NotInstalled            = GetString(LocalizationKeys.EasyOcrStatusNotInstalled,       LocalizationFallbacks.EasyOcr.NotInstalled),
			Broken                  = GetString(LocalizationKeys.EasyOcrStatusBroken,             LocalizationFallbacks.EasyOcr.Broken),
			PythonMissing           = GetString(LocalizationKeys.EasyOcrStatusPythonMissing,      LocalizationFallbacks.EasyOcr.PythonMissing),
			DownloadingPython       = GetString(LocalizationKeys.EasyOcrStatusDownloadingPython,  LocalizationFallbacks.EasyOcr.DownloadingPython),
			InstallingPython        = GetString(LocalizationKeys.EasyOcrStatusInstallingPython,   LocalizationFallbacks.EasyOcr.InstallingPython),
			InstallingEasyOcr       = GetString(LocalizationKeys.EasyOcrStatusInstallingEasyOcr,  LocalizationFallbacks.EasyOcr.InstallingEasyOcr),
			PythonNotInstalled      = GetString(LocalizationKeys.EasyOcrStatusPythonNotInstalled, LocalizationFallbacks.EasyOcr.PythonNotInstalled),
			EasyOcrNotFound         = GetString(LocalizationKeys.EasyOcrStatusNotFound,           LocalizationFallbacks.EasyOcr.EasyOcrNotFound),
			InstallFailedFormat     = GetString(LocalizationKeys.EasyOcrStatusInstallFailedFmt,   LocalizationFallbacks.EasyOcr.InstallFailedFormat),
			Removing                = GetString(LocalizationKeys.EasyOcrStatusRemove,             LocalizationFallbacks.EasyOcr.Remove),
			Removed                 = GetString(LocalizationKeys.EasyOcrStatusRemoved,            LocalizationFallbacks.EasyOcr.Removed),
			DeleteFailedFormat      = GetString(LocalizationKeys.EasyOcrStatusDeleteFailedFmt,    LocalizationFallbacks.EasyOcr.DeleteFailedFormat),
			Repairing               = GetString(LocalizationKeys.EasyOcrStatusRepairing,          LocalizationFallbacks.EasyOcr.Repairing),
			RepairFailed            = GetString(LocalizationKeys.EasyOcrStatusRepairFailed,       LocalizationFallbacks.EasyOcr.RepairFailed),
			Repaired                = GetString(LocalizationKeys.EasyOcrStatusRepaired,           LocalizationFallbacks.EasyOcr.Repaired),
			CleaningOldPythonFormat = GetString(LocalizationKeys.EasyOcrStatusCleaningOldFmt,     LocalizationFallbacks.EasyOcr.CleaningOldFormat)
		};
	}

	private static string? TryGetAvaloniaString(string key)
	{
		var resources = AvaloniaApp.Current?.Resources;
		if (resources == null)
		{
			return null;
		}

		if (resources.TryGetResource(key, ThemeVariant.Default, out var value) &&
			value is string s                                                  &&
			!string.IsNullOrWhiteSpace(s))
		{
			return s;
		}

		return null;
	}

	private string? TryGetFromAvaloniaDictionary(string languageCode, string key)
	{
		var dict = _avaloniaLanguageCache.GetOrAdd(languageCode, LoadAvaloniaDictionary);
		if (dict == null)
		{
			return null;
		}

		if (dict.TryGetResource(key, ThemeVariant.Default, out var value) &&
			value is string s                                             &&
			!string.IsNullOrWhiteSpace(s))
		{
			return s;
		}

		return null;
	}

	private static AvaloniaResourceDictionary LoadAvaloniaDictionary(string languageCode)
	{
		try
		{
			var uri  = new Uri($"avares://Shotora.App/Resources/Languages/Strings.{languageCode}.axaml");
			var dict = AvaloniaXamlLoader.Load(uri) as AvaloniaResourceDictionary;
			return dict ?? new AvaloniaResourceDictionary();
		}
		catch
		{
			return new AvaloniaResourceDictionary();
		}
	}
}
