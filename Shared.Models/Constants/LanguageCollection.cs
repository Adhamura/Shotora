using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Models.Constants;

[ExcludeFromCodeCoverage]
public static class LanguageCollection
{
	public const string DefaultOcrLanguageCode  = "en";
	public const string DefaultOcrLanguageName  = "English";
	public const string DefaultOcrTesseractCode = "eng";

	private static readonly LanguageDefinition[] Definitions =
	[
		new(DefaultOcrLanguageCode, DefaultOcrLanguageName, TesseractCode: DefaultOcrTesseractCode),
		new("es", "Spanish", TesseractCode: "spa"),
		new("fr", "French", TesseractCode: "fra"),
		new("pt", "Portuguese", TesseractCode: "por"),
		new("de", "German", TesseractCode: "deu"),
		new("ru", "Russian", TesseractCode: "rus"),
		new("it", "Italian", TesseractCode: "ita"),
		new("pl", "Polish", TesseractCode: "pol"),
		new("uk", "Ukrainian", TesseractCode: "ukr"),
		new("nl", "Dutch", TesseractCode: "nld"),
		new("sv", "Swedish", TesseractCode: "swe"),
		new("da", "Danish", TesseractCode: "dan"),
		new("no", "Norwegian", TesseractCode: "nor"),
		new("is", "Icelandic", TesseractCode: "isl"),
		new("fi", "Finnish", TesseractCode: "fin"),
		new("et", "Estonian", TesseractCode: "est"),
		new("lv", "Latvian", TesseractCode: "lav"),
		new("ltg", "Latgalian", "lv", "ltg", TesseractCode: "ltg"),
		new("lt", "Lithuanian", TesseractCode: "lit"),
		new("ga", "Irish", TesseractCode: "gle"),
		new("gd", "Scottish Gaelic", TesseractCode: "gla"),
		new("cy", "Welsh", TesseractCode: "cym"),
		new("br", "Breton", TesseractCode: "bre"),
		new("ca", "Catalan", TesseractCode: "cat"),
		new("gl", "Galician", TesseractCode: "glg"),
		new("eu", "Basque", TesseractCode: "eus"),
		new("oc", "Occitan", TesseractCode: "oci"),
		new("ro", "Romanian", TesseractCode: "ron"),
		new("mo", "Moldovan", null, "ro"),
		new("bg", "Bulgarian", TesseractCode: "bul"),
		new("mk", "Macedonian", TesseractCode: "mkd"),
		new("sr", "Serbian", TesseractCode: "srp"),
		new("hr", "Croatian", TesseractCode: "hrv"),
		new("bs", "Bosnian", TesseractCode: "bos"),
		new("cnr", "Montenegrin", null, "bs"),
		new("sl", "Slovenian", TesseractCode: "slv"),
		new("cs", "Czech", TesseractCode: "ces"),
		new("sk", "Slovak", TesseractCode: "slk"),
		new("hu", "Hungarian", TesseractCode: "hun"),
		new("el", "Greek", TesseractCode: "ell"),
		new("sq", "Albanian", TesseractCode: "sqi"),
		new("hy", "Armenian", TesseractCode: "hye"),
		new("ka", "Georgian", TesseractCode: "kat"),
		new("tr", "Turkish", TesseractCode: "tur"),
		new("mt", "Maltese", TesseractCode: "mlt"),
		new("lb", "Luxembourgish", TesseractCode: "ltz"),
		new("fur", "Friulian", TesseractCode: "fur"),
		new("sc", "Sardinian", TesseractCode: "srd"),
		new("scn", "Sicilian"),
		new("nap", "Neapolitan"),
		new("vec", "Venetian"),
		new("rm", "Romansh", TesseractCode: "roh"),
		new("fo", "Faroese", TesseractCode: "fao"),
		new("gv", "Manx", TesseractCode: "glv"),
		new("kw", "Cornish", TesseractCode: "cor"),
		new("rup", "Aromanian", null, "ro"),
		new("ruq", "Megleno-Romanian", null, "ro"),
		new("ruo", "Istro-Romanian", null, "ro"),
		new("hsb", "Upper Sorbian", TesseractCode: "hsb"),
		new("dsb", "Lower Sorbian", TesseractCode: "dsb"),
		new("csb", "Kashubian", TesseractCode: "csb"),
		new("szl", "Silesian"),
		new("rue", "Rusyn"),
		new("gag", "Gagauz"),
		new("crh", "Crimean Tatar"),
		new("kdr", "Karaim"),
		new("vep", "Veps"),
		new("krl", "Karelian"),
		new("liv", "Livonian"),
		new("zh-Hans", "Chinese (Simplified)", "ch_sim", "zh-Hans", TesseractCode: "chi_sim"),
		new("zh-Hant", "Chinese (Traditional)", "ch_tra", "zh-Hant", TesseractCode: "chi_tra"),
		new("zh-TW", "Chinese (Taiwan)", "ch_tra", "zh-Hant", "Chinese (Traditional)"),
		new("ja", "Japanese", TesseractCode: "jpn"),
		new("ko", "Korean", TesseractCode: "kor")
	];

	private static readonly FrozenDictionary<string, string> NameByCode =
		Definitions.ToFrozenDictionary(d => d.Code, d => d.Name, StringComparer.OrdinalIgnoreCase);

	private static readonly string[] TesseractOrder =
	[
		"eng",
		"spa",
		"fra",
		"deu",
		"ita",
		"por",
		"rus",
		"ukr",
		"jpn",
		"kor",
		"chi_sim",
		"chi_tra",
		"pol",
		"nld",
		"swe",
		"dan",
		"nor",
		"isl",
		"fin",
		"est",
		"lav",
		"ltg",
		"lit",
		"gle",
		"gla",
		"cym",
		"bre",
		"cat",
		"glg",
		"eus",
		"oci",
		"ron",
		"bul",
		"mkd",
		"srp",
		"hrv",
		"bos",
		"slv",
		"ces",
		"slk",
		"hun",
		"ell",
		"sqi",
		"hye",
		"kat",
		"tur",
		"mlt",
		"ltz",
		"fur",
		"srd",
		"roh",
		"fao",
		"glv",
		"cor",
		"hsb",
		"dsb",
		"csb"
	];

	private static readonly FrozenDictionary<string, LanguageDefinition> TesseractDefinitionsByCode =
		Definitions
			.Where(d => !string.IsNullOrWhiteSpace(d.TesseractCode))
			.ToFrozenDictionary(d => d.TesseractCode!, StringComparer.OrdinalIgnoreCase);

	public static readonly (string Code, string Name)[] All =
		Definitions.Select(d => (d.Code, d.Name)).ToArray();

	public static readonly FrozenDictionary<string, string> EasyOcrCodeMap =
		Definitions
			.SelectMany(d => new[]
				{
					new
					{
						Key   = d.Code,
						Value = d.EasyOcrCode ?? d.Code
					},
					new
					{
						Key   = d.EasyOcrCode ?? d.Code,
						Value = d.EasyOcrCode ?? d.Code
					},
					new
					{
						Key   = $"{d.TesseractCode}",
						Value = d.EasyOcrCode ?? d.Code
					},
					new
					{
						Key   = $"{d.TesseractBaseCode}",
						Value = d.EasyOcrCode ?? d.Code
					}
				}
				.Where(x => !string.IsNullOrWhiteSpace(x.Key)))
			.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(g => g.Key, g => g.First().Value, StringComparer.OrdinalIgnoreCase)
			.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

	public static readonly (string Code, string BaseCode, string FallbackEnglish)[] TesseractLanguages =
		TesseractOrder
			.Select(code =>
			{
				if (!TesseractDefinitionsByCode.TryGetValue(code, out var definition))
				{
					throw new InvalidOperationException($"Missing Tesseract definition for '{code}'.");
				}

				var baseCode = definition.TesseractBaseCode        ?? definition.Code;
				var fallback = definition.TesseractFallbackEnglish ?? definition.Name;
				return (code, baseCode, fallback);
			})
			.ToArray();

	private static string NameFor(string code)
	{
		return string.IsNullOrWhiteSpace(code) ? string.Empty : NameByCode.GetValueOrDefault(code, code);
	}

	private record LanguageDefinition(string Code, string Name, string? EasyOcrCode = null, string? TesseractBaseCode = null, string? TesseractFallbackEnglish = null, string? TesseractCode = null);
}
