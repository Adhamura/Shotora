using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using Shotora.App.Models.Utilities;
using TesseractLanguageOption=Shotora.App.Models.Utilities.TesseractLanguageOption;

namespace Shotora.App.Services.Providers;

public class SettingsProvider(ILocalizationProvider localizationProvider) : ISettingsProvider
{
	public List<LanguageOption> BuildDisplayLanguages()
	{
		var list = new List<LanguageOption>();
		foreach (var lang in LanguageCatalog.All)
		{
			var localizedName = localizationProvider.GetString(lang.LocalizationKey, lang.EnglishName);
			var englishName   = localizationProvider.GetStringForLanguage(LanguageCollection.DefaultOcrLanguageCode, lang.LocalizationKey, lang.EnglishName);
			list.Add(new LanguageOption(localizedName, englishName, lang.Code));
		}
		return list;
	}

	public List<TesseractLanguageOption> BuildTesseractLanguages()
	{
		var list = new List<TesseractLanguageOption>();
		foreach (var preset in LanguageCollection.TesseractLanguages)
		{
			var info   = LanguageCatalog.Find(preset.BaseCode);
			var locKey = info?.LocalizationKey;
			var name   = info?.EnglishName ?? preset.FallbackEnglish ?? preset.Code;
			list.Add(new TesseractLanguageOption(preset.Code, name, locKey));
		}
		return list;
	}

	public string HotkeyToString(HotkeySetting hotkey)
	{
		var parts = new List<string>();
		if (hotkey.Modifiers.HasFlag(KeyModifiers.Control))
		{
			parts.Add("Ctrl");
		}
		if (hotkey.Modifiers.HasFlag(KeyModifiers.Alt))
		{
			parts.Add("Alt");
		}
		if (hotkey.Modifiers.HasFlag(KeyModifiers.Shift))
		{
			parts.Add("Shift");
		}
		if (hotkey.Modifiers.HasFlag(KeyModifiers.Win))
		{
			parts.Add("Win");
		}

		var keyName = HotkeyUtilities.GetKeyName(hotkey.Key);
		parts.Add(keyName);
		return string.Join("+", parts);
	}

	public HotkeySetting ParseHotkey(string text, HotkeySetting fallback)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return fallback;
		}

		var parts     = text.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var modifiers = KeyModifiers.None;
		var keyCode   = fallback.Key;

		foreach (var part in parts)
		{
			var normalized = part.Trim().Replace(" ", string.Empty).ToLowerInvariant();
			switch (normalized)
			{
				case "ctrl":
				case "control":
					modifiers |= KeyModifiers.Control;
					break;
				case "alt":
					modifiers |= KeyModifiers.Alt;
					break;
				case "shift":
					modifiers |= KeyModifiers.Shift;
					break;
				case "win":
				case "windows":
					modifiers |= KeyModifiers.Win;
					break;
				default:
					var parsed = HotkeyUtilities.KeyFromName(part);
					if (parsed.HasValue)
					{
						keyCode = parsed.Value;
					}
					break;
			}
		}

		return new HotkeySetting
		{
			Modifiers = modifiers,
			Key       = keyCode
		};
	}
}
