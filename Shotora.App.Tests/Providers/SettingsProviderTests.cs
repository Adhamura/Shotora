using Moq;
using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Services.Providers;

namespace Shotora.App.Tests.Providers;

public class SettingsProviderTests
{
	[Fact]
	public void Given_LocalizationProvider_When_BuildDisplayLanguages_Then_MapsLocalizedAndEnglishNames()
	{
		var localization = new Mock<ILocalizationProvider>(MockBehavior.Strict);
		localization
			.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string key, string fallback) =>
			{
				Assert.False(string.IsNullOrWhiteSpace(key));
				Assert.False(string.IsNullOrWhiteSpace(fallback));
				return $"loc-{key}-{fallback}";
			});
		localization
			.Setup(l => l.GetStringForLanguage(LanguageCollection.DefaultOcrLanguageCode, It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string languageCode, string key, string fallback) =>
			{
				Assert.Equal(LanguageCollection.DefaultOcrLanguageCode, languageCode);
				Assert.False(string.IsNullOrWhiteSpace(key));
				Assert.False(string.IsNullOrWhiteSpace(fallback));
				return $"en-{key}-{fallback}";
			});

		var sut = new SettingsProvider(localization.Object);

		var result = sut.BuildDisplayLanguages();

		Assert.Equal(LanguageCatalog.All.Length, result.Count);

		foreach (var lang in LanguageCatalog.All)
		{
			var option = result.Single(o => o.Code == lang.Code);
			Assert.Equal($"loc-{lang.LocalizationKey}-{lang.EnglishName}", option.LocalizedName);
			Assert.Equal($"en-{lang.LocalizationKey}-{lang.EnglishName}",  option.EnglishName);
			Assert.Equal(option.LocalizedName,                             option.Name);
		}

		localization.Verify(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()),                                                       Times.Exactly(LanguageCatalog.All.Length));
		localization.Verify(l => l.GetStringForLanguage(LanguageCollection.DefaultOcrLanguageCode, It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(LanguageCatalog.All.Length));
		localization.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_TesseractLanguages_When_BuildTesseractLanguages_Then_ReturnsOrderedOptions()
	{
		var localization = new Mock<ILocalizationProvider>(MockBehavior.Strict);
		var sut          = new SettingsProvider(localization.Object);

		var result = sut.BuildTesseractLanguages();

		Assert.Equal(LanguageCollection.TesseractLanguages.Length, result.Count);

		for (var i = 0; i < LanguageCollection.TesseractLanguages.Length; i++)
		{
			var preset = LanguageCollection.TesseractLanguages[i];
			var option = result[i];

			var baseInfo     = LanguageCatalog.Find(preset.BaseCode);
			var expectedName = baseInfo?.EnglishName ?? preset.FallbackEnglish ?? preset.Code;

			Assert.Equal(preset.Code,               option.Code);
			Assert.Equal(expectedName,              option.EnglishName);
			Assert.Equal(baseInfo?.LocalizationKey, option.LocalizationKey);
		}

		localization.VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData(KeyModifiers.Control | KeyModifiers.Alt, 0x41, "Ctrl+Alt+A")]
	[InlineData(KeyModifiers.Shift   | KeyModifiers.Win, 0x2C, "Shift+Win+PrintScreen")]
	[InlineData(KeyModifiers.None,                       0x7B, "F12")]
	public void Given_HotkeySetting_When_HotkeyToString_Then_JoinsModifiersAndKey(KeyModifiers modifiers, int keyCode, string expected)
	{
		var sut = new SettingsProvider(Mock.Of<ILocalizationProvider>());
		var hotkey = new HotkeySetting
		{
			Modifiers = modifiers,
			Key       = keyCode
		};

		var result = sut.HotkeyToString(hotkey);

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Given_NullOrWhitespace_When_ParseHotkey_Then_ReturnsFallback(string? text)
	{
		var sut      = new SettingsProvider(Mock.Of<ILocalizationProvider>());
		var fallback = HotkeySetting.RegionDefault();

		var result = sut.ParseHotkey(text!, fallback);

		Assert.Same(fallback, result);
	}

	[Fact]
	public void Given_ModifierAndFunctionKeyText_When_ParseHotkey_Then_ParsesAllParts()
	{
		var sut      = new SettingsProvider(Mock.Of<ILocalizationProvider>());
		var fallback = HotkeySetting.FullscreenDefault();

		var result = sut.ParseHotkey("Ctrl+Alt+Shift+Win+F5", fallback);

		Assert.NotSame(fallback, result);
		Assert.Equal(KeyModifiers.Control | KeyModifiers.Alt | KeyModifiers.Shift | KeyModifiers.Win, result.Modifiers);
		Assert.Equal(0x74,                                                                            result.Key);
		Assert.Equal(fallback.Modifiers,                                                              fallback.Modifiers);
		Assert.Equal(fallback.Key,                                                                    fallback.Key);
	}

	[Fact]
	public void Given_TextWithSpacesAndAlias_When_ParseHotkey_Then_TrimsAndParsesKeyName()
	{
		var sut      = new SettingsProvider(Mock.Of<ILocalizationProvider>());
		var fallback = HotkeySetting.RegionDefault();

		var result = sut.ParseHotkey("  shift +  win +  PrtSc  ", fallback);

		Assert.NotSame(fallback, result);
		Assert.Equal(KeyModifiers.Shift | KeyModifiers.Win, result.Modifiers);
		Assert.Equal(0x2C,                                  result.Key);
	}

	[Fact]
	public void Given_UnknownKey_When_ParseHotkey_Then_KeepsFallbackKey()
	{
		var sut = new SettingsProvider(Mock.Of<ILocalizationProvider>());
		var fallback = new HotkeySetting
		{
			Modifiers = KeyModifiers.None,
			Key       = 0x30
		};

		var result = sut.ParseHotkey("Ctrl+Unknown", fallback);

		Assert.NotSame(fallback, result);
		Assert.Equal(KeyModifiers.Control, result.Modifiers);
		Assert.Equal(fallback.Key,         result.Key);
	}
}
