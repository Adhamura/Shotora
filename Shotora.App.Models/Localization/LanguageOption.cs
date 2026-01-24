using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.Localization;

[ExcludeFromCodeCoverage]
public readonly record struct LanguageOption(string localizedName, string englishName, string code)
{
	public string Code { get; } = code;

	public string LocalizedName { get; } = localizedName;

	public string EnglishName { get; } = englishName;

	public string Name => LocalizedName;

	public string DisplayName => string.Equals(LocalizedName, EnglishName, StringComparison.OrdinalIgnoreCase)
		? LocalizedName
		: $"{LocalizedName} ({EnglishName})";
}
