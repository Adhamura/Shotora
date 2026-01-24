using Shotora.App.Models;

namespace Shotora.App.Interfaces.Providers;

public interface ILocalizationProvider
{
	event EventHandler? LanguageChanged;

	void   SetLanguage(string languageCode);
	string GetString(string   key, string fallback = "");

	string             GetStringForLanguage(string languageCode, string key, string fallback = "");
	EasyOcrStatusTexts BuildEasyOcrStatusTexts();
}
