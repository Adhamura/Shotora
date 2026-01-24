using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public class LanguageInfo(string code, string englishName, string key = "")
{
	public string Code            { get; } = code;
	public string EnglishName     { get; } = englishName;
	public string LocalizationKey { get; } = key;
}
