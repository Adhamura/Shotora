using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Shared.Models.Constants;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class LanguageCatalog
{
	public static readonly LanguageInfo[] All = LanguageCollection.All.Select(t => new LanguageInfo(t.Code, t.Name, $"{LocalizationKeys.LanguagePrefix}{t.Code}")).ToArray();

	private static readonly FrozenDictionary<string, LanguageInfo> ByCode = All.ToFrozenDictionary<LanguageInfo, string>(l => l.Code, StringComparer.OrdinalIgnoreCase);
	private static bool TryGet(string code, out LanguageInfo info)
	{
		return ByCode.TryGetValue(code, out info);
	}

	public static LanguageInfo? Find(string code)
	{
		return TryGet(code, out var info) ? info : null;
	}
}
