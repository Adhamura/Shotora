using Shotora.App.Models;
using Shotora.App.Models.Localization;
using TesseractLanguageOption=Shotora.App.Models.Utilities.TesseractLanguageOption;

namespace Shotora.App.Interfaces.Providers;

public interface ISettingsProvider
{
	List<LanguageOption> BuildDisplayLanguages();

	List<TesseractLanguageOption> BuildTesseractLanguages();

	string HotkeyToString(HotkeySetting hotkey);

	HotkeySetting ParseHotkey(string text, HotkeySetting fallback);
}
