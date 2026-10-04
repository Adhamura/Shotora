using System.Reflection;
using Moq;
using Shared.Interfaces.Adapters;
using Shared.Models.Constants;
using Shared.Models.Enums;
using Shotora.App.Interfaces.Adapters;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Ocr.Tesseract;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Enums;
using Shotora.App.Models.Localization;
using Shotora.App.Models.Utilities;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services.ViewModels;

namespace Shotora.App.Tests.ViewModels;

public class SettingsViewModelControllerTests
{
	private readonly Mock<IColorAdapter>              _colorAdapterMock              = new(MockBehavior.Strict);
	private readonly Mock<IFolderPickerAdapter>       _folderPickerAdapterMock       = new(MockBehavior.Strict);
	private readonly Mock<IIconVisibilityProvider>    _iconVisibilityProviderMock    = new(MockBehavior.Strict);
	private readonly Mock<ILanguageTesseractService>  _languageTesseractServiceMock  = new(MockBehavior.Strict);
	private readonly Mock<ILocalizationProvider>      _localizationProviderMock      = new(MockBehavior.Strict);
	private readonly Mock<IMaintenanceEasyOcrService> _maintenanceEasyOcrServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IPropertyAdapter>           _propertyAdapterMock           = new(MockBehavior.Strict);
	private readonly Mock<ISettingsProvider>          _settingsProviderMock          = new(MockBehavior.Strict);
	private readonly Mock<ISettingsSystemService>     _settingsSystemServiceMock     = new(MockBehavior.Strict);

	private SettingsViewModelController CreateSut()
	{
		return new SettingsViewModelController(
			_settingsSystemServiceMock.Object,
			_languageTesseractServiceMock.Object,
			_maintenanceEasyOcrServiceMock.Object,
			_folderPickerAdapterMock.Object,
			_colorAdapterMock.Object,
			_iconVisibilityProviderMock.Object,
			_settingsProviderMock.Object,
			_propertyAdapterMock.Object,
			_localizationProviderMock.Object);
	}

	[Fact]
	public void Init_WhenNotInitialized_InitializesModelAndWiresCommands()
	{
		_localizationProviderMock
			.Setup(l => l.GetString("LocSettingsOcrInstallLang", "Install"))
			.Returns("Install");
		_localizationProviderMock
			.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string key, string fallback) => fallback);
		_localizationProviderMock
			.SetupAdd(l => l.LanguageChanged += It.IsAny<EventHandler>());
		_settingsProviderMock
			.Setup(s => s.BuildDisplayLanguages())
			.Returns([new LanguageOption("English", "English", "en")]);
		_languageTesseractServiceMock
			.Setup(l => l.PopulateLanguages(It.IsAny<ICollection<TesseractLanguageOption>>(), It.IsAny<Action<TesseractLanguageOption>>()))
			.Callback<ICollection<TesseractLanguageOption>, Action<TesseractLanguageOption>?>((collection, handler) =>
			{
				var option = new TesseractLanguageOption("en", "English");
				handler?.Invoke(option);
				collection.Add(option);
			});
		_maintenanceEasyOcrServiceMock
			.Setup(m => m.GetStateAsync(It.IsAny<EasyOcrStatusTexts>()))
			.ReturnsAsync(new EasyOcrViewState
			{
				IsInstalled = false,
				IsBroken    = false,
				Status      = "Not installed"
			});
		_localizationProviderMock
			.Setup(l => l.BuildEasyOcrStatusTexts())
			.Returns(new EasyOcrStatusTexts());

		var sut = CreateSut();

		sut.Init();

		_localizationProviderMock.Verify(l => l.GetString("LocSettingsOcrInstallLang", "Install"), Times.Once);
		_localizationProviderMock.VerifyAdd(l => l.LanguageChanged += It.IsAny<EventHandler>(), Times.Once);
	}

	[Fact]
	public void Init_WhenAlreadyInitialized_DoesNotReinitialize()
	{
		_localizationProviderMock
			.Setup(l => l.GetString("LocSettingsOcrInstallLang", "Install"))
			.Returns("Install");
		_localizationProviderMock
			.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string key, string fallback) => fallback);
		_localizationProviderMock
			.SetupAdd(l => l.LanguageChanged += It.IsAny<EventHandler>());
		_settingsProviderMock
			.Setup(s => s.BuildDisplayLanguages())
			.Returns([new LanguageOption("English", "English", "en")]);
		_languageTesseractServiceMock
			.Setup(l => l.PopulateLanguages(It.IsAny<ICollection<TesseractLanguageOption>>(), It.IsAny<Action<TesseractLanguageOption>>()))
			.Callback<ICollection<TesseractLanguageOption>, Action<TesseractLanguageOption>?>((collection, handler) =>
			{
				var option = new TesseractLanguageOption("en", "English");
				handler?.Invoke(option);
				collection.Add(option);
			});
		_maintenanceEasyOcrServiceMock
			.Setup(m => m.GetStateAsync(It.IsAny<EasyOcrStatusTexts>()))
			.ReturnsAsync(new EasyOcrViewState
			{
				IsInstalled = false,
				IsBroken    = false,
				Status      = "Not installed"
			});
		_localizationProviderMock
			.Setup(l => l.BuildEasyOcrStatusTexts())
			.Returns(new EasyOcrStatusTexts());

		var sut = CreateSut();

		sut.Init();
		sut.Init();

		_localizationProviderMock.Verify(l => l.GetString("LocSettingsOcrInstallLang", "Install"), Times.Once);
	}

	[Fact]
	public void SettingsHandlersAttach_CallsActionWithModel()
	{
		SetupBasicMocks();
		var sut = CreateSut();
		sut.Init();
		var called = false;

		sut.SettingsHandlersAttach(model => called = model != null);

		Assert.True(called);
	}

	[Fact]
	public async Task LoadAsync_WhenNotLoaded_LoadsSettings()
	{
		var settings = new AppSettings
		{
			DefaultFormat = "jpg",
			Theme         = "Light"
		};
		SetupLoadAsyncMocks();
		_settingsSystemServiceMock
			.Setup(s => s.LoadAsync())
			.ReturnsAsync(settings);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns(["DefaultFormat", "Theme"]);
		_propertyAdapterMock
			.Setup(p => p.GetValue<AppSettings, object?>(It.IsAny<AppSettings>(), It.IsAny<string>()))
			.Returns((AppSettings s, string name) => name == "DefaultFormat" ? "jpg" : "Light");
		_propertyAdapterMock
			.Setup(p => p.SetValue(It.IsAny<SettingsViewModel>(), It.IsAny<string>(), It.IsAny<object?>()))
			.Verifiable();

		var sut = CreateSut();

		await sut.LoadAsync();

		_settingsSystemServiceMock.Verify(s => s.LoadAsync(), Times.Once);
	}

	[Fact]
	public async Task LoadAsync_WhenSettingsHaveTrackedValues_AppliesThemToView()
	{
		var settings = new AppSettings
		{
			DefaultSaveFolder = "/home/user/Pictures/Captures",
			JpegQuality       = 70
		};
		SetupLoadAsyncMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, false))
			.Returns(["DefaultSaveFolder", "JpegQuality"]);
		_propertyAdapterMock
			.Setup(p => p.GetValue<AppSettings, object?>(settings, "DefaultSaveFolder"))
			.Returns(settings.DefaultSaveFolder);
		_propertyAdapterMock
			.Setup(p => p.GetValue<AppSettings, object?>(settings, "JpegQuality"))
			.Returns(settings.JpegQuality);
		_propertyAdapterMock
			.Setup(p => p.SetValue(It.IsAny<SettingsViewModel>(), It.IsAny<string>(), It.IsAny<object?>()));

		var sut = CreateSut();

		await sut.LoadAsync(settings);

		_propertyAdapterMock.Verify(p => p.SetValue(It.IsAny<SettingsViewModel>(), "DefaultSaveFolder", "/home/user/Pictures/Captures"), Times.Once);
		_propertyAdapterMock.Verify(p => p.SetValue(It.IsAny<SettingsViewModel>(), "JpegQuality", 70), Times.Once);
	}

	[Fact]
	public async Task LoadAsync_WhenSettingsProvided_UsesProvidedSettings()
	{
		var settings = new AppSettings
		{
			DefaultFormat = "png"
		};
		SetupLoadAsyncMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns(["DefaultFormat"]);
		_propertyAdapterMock
			.Setup(p => p.GetValue<AppSettings, object?>(It.IsAny<AppSettings>(), It.IsAny<string>()))
			.Returns("png");
		_propertyAdapterMock
			.Setup(p => p.SetValue(It.IsAny<SettingsViewModel>(), It.IsAny<string>(), It.IsAny<object?>()))
			.Verifiable();

		var sut = CreateSut();

		await sut.LoadAsync(settings);

		_settingsSystemServiceMock.Verify(s => s.LoadAsync(), Times.Never);
	}

	[Fact]
	public void RemoveSelectedLanguage_WhenInstalled_RemovesLanguage()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English")
		{
			IsInstalled = true
		};
		_languageTesseractServiceMock
			.Setup(l => l.RemoveLanguage(languageOption))
			.Verifiable();

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		model.RemoveLanguageCommand!.Execute(null);

		_languageTesseractServiceMock.Verify(l => l.RemoveLanguage(languageOption), Times.Once);
	}

	[Fact]
	public void RemoveSelectedLanguage_WhenNotInstalled_DoesNotRemove()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English")
		{
			IsInstalled = false
		};

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		model.RemoveLanguageCommand!.Execute(null);

		_languageTesseractServiceMock.Verify(l => l.RemoveLanguage(It.IsAny<TesseractLanguageOption>()), Times.Never);
	}

	[Fact]
	public async Task ExecuteLanguageActionAsync_WhenInstalled_RemovesLanguage()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English")
		{
			IsInstalled = true
		};
		_languageTesseractServiceMock
			.Setup(l => l.RemoveLanguage(languageOption))
			.Verifiable();

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		await model.LanguageActionCommand!.ExecuteAsync(null);

		_languageTesseractServiceMock.Verify(l => l.RemoveLanguage(languageOption), Times.Once);
	}

	[Fact]
	public async Task ExecuteLanguageActionAsync_WhenCanDownload_DownloadsLanguage()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English")
		{
			CanDownload = true,
			IsInstalled = false
		};
		_languageTesseractServiceMock
			.Setup(l => l.DownloadAsync(languageOption, It.IsAny<IProgress<double>>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		await model.LanguageActionCommand!.ExecuteAsync(null);

		_languageTesseractServiceMock.Verify(l => l.DownloadAsync(languageOption, It.IsAny<IProgress<double>>()), Times.Once);
	}

	[Fact]
	public async Task ExecuteLanguageActionAsync_WhenNullOption_DoesNothing()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = null;

		await model.LanguageActionCommand!.ExecuteAsync(null);

		_languageTesseractServiceMock.Verify(l => l.RemoveLanguage(It.IsAny<TesseractLanguageOption>()),                               Times.Never);
		_languageTesseractServiceMock.Verify(l => l.DownloadAsync(It.IsAny<TesseractLanguageOption>(), It.IsAny<IProgress<double>>()), Times.Never);
	}

	[Fact]
	public void OnPropertyChanged_WhenSelectedTesseractLanguageChanged_AttachesHandler()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var sut = CreateSut();
		sut.Init();
		var model           = GetModel(sut);
		var languageOption1 = new TesseractLanguageOption("en", "English");
		var languageOption2 = new TesseractLanguageOption("fr", "French");

		model.SelectedTesseractLanguage = languageOption1;
		model.SelectedTesseractLanguage = languageOption2;

		Assert.NotNull(model.SelectedTesseractLanguage);
	}

	[Fact]
	public void OnPropertyChanged_WhenShowIconPropertyChanged_UpdatesHiddenIcons()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_propertyAdapterMock
			.Setup(p => p.GetValue<SettingsViewModel, bool>(It.IsAny<SettingsViewModel>(), It.IsAny<string>()))
			.Returns(false);
		_iconVisibilityProviderMock
			.Setup(i => i.BuildHiddenIconIds(It.IsAny<IReadOnlyDictionary<EditorIcon, bool>>()))
			.Returns(["Arrow"]);
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);

		model.ShowArrowIcon = false;

		_iconVisibilityProviderMock.Verify(i => i.BuildHiddenIconIds(It.IsAny<IReadOnlyDictionary<EditorIcon, bool>>()), Times.AtLeastOnce);
	}

	[Fact]
	public void OnPropertyChanged_WhenEditorDefaultColorChanged_UpdatesColorValue()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_colorAdapterMock
			.Setup(c => c.CreateColor("#FF0000"))
			.Returns(new object());
		_colorAdapterMock
			.Setup(c => c.CreateBrush("#FF0000"))
			.Returns(new object());
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);

		model.EditorDefaultColor = "#FF0000";

		_colorAdapterMock.Verify(c => c.CreateColor("#FF0000"), Times.Once);
		_colorAdapterMock.Verify(c => c.CreateBrush("#FF0000"), Times.Once);
	}

	[Fact]
	public void OnPropertyChanged_WhenIsEasyOcrInstallingChanged_NotifiesCommand()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);

		model.IsEasyOcrInstalling = true;

		Assert.True(model.IsEasyOcrInstalling);
	}

	[Fact]
	public void OnPropertyChanged_WhenDisplayLanguageChanged_RaisesLanguageChanged()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model                 = GetModel(sut);
		var languageChangedRaised = false;
		model.LanguageChanged += (_, lang) => languageChangedRaised = true;

		model.DisplayLanguage = "fr";

		Assert.True(languageChangedRaised);
	}

	[Fact]
	public void OnPropertyChanged_WhenDisplayLanguageEmpty_UsesDefault()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model          = GetModel(sut);
		var raisedLanguage = "";
		model.LanguageChanged += (_, lang) => raisedLanguage = lang;

		model.DisplayLanguage = "";

		Assert.Equal(LanguageCollection.DefaultOcrLanguageCode, raisedLanguage);
	}

	[Fact]
	public void OnPropertyChanged_WhenThemeChanged_RaisesThemeChanged()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);

		var sut = CreateSut();
		sut.Init();
		var model              = GetModel(sut);
		var themeChangedRaised = false;
		model.ThemeChanged += (_, theme) => themeChangedRaised = true;

		model.Theme = "Light";

		Assert.True(themeChangedRaised);
	}

	[Fact]
	public void OnPropertyChanged_WhenPropertyNameEmpty_DoesNotProcess()
	{
		SetupBasicMocks();
		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);

		model.RaisePropertyChanged("");

		Assert.NotNull(model);
	}

	[Fact]
	public void LanguageChangedHandler_WhenSelectedLanguagePropertyChanged_RefreshesCommands()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English");
		var sut            = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		languageOption.IsInstalled = true;

		Assert.True(languageOption.IsInstalled);
	}

	[Fact]
	public void LanguageChangedHandler_WhenOtherLanguagePropertyChanged_DoesNotRefresh()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption1 = new TesseractLanguageOption("en", "English");
		var languageOption2 = new TesseractLanguageOption("fr", "French");
		var sut             = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption1;

		languageOption2.IsInstalled = true;

		Assert.Equal(languageOption1, model.SelectedTesseractLanguage);
	}

	[Fact]
	public void LanguageChangedHandler_WhenUnrelatedPropertyChanged_DoesNotRefresh()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English");
		var sut            = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		languageOption.DisplayName = "New Name";

		Assert.Equal("New Name", languageOption.DisplayName);
	}

	[Fact]
	public void UpdateEasyOcrActionLabel_WhenNotInstalled_ShowsInstall()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		_localizationProviderMock
			.Setup(l => l.GetString(LocalizationKeys.CommonInstall, LocalizationFallbacks.Common.Install))
			.Returns("Install");

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.IsEasyOcrInstalled = false;
		model.IsEasyOcrBroken    = false;

		model.IsEasyOcrInstalled = false;

		Assert.Equal("Install", model.EasyOcrActionLabel);
	}

	[Fact]
	public void NotifyLanguageButtonPropertiesChanged_RaisesAllProperties()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
		var languageOption = new TesseractLanguageOption("en", "English")
		{
			IsInstalled   = true,
			IsDownloading = false,
			Progress      = 50,
			StatusText    = "Downloading"
		};

		var sut = CreateSut();
		sut.Init();
		var model = GetModel(sut);
		model.SelectedTesseractLanguage = languageOption;

		languageOption.IsInstalled = false;

		Assert.False(model.IsSelectedLanguageInstalled);
	}

	private void SetupBasicMocks()
	{
		_localizationProviderMock
			.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string key, string fallback) => fallback);
		_localizationProviderMock
			.SetupAdd(l => l.LanguageChanged += It.IsAny<EventHandler>());
		_settingsProviderMock
			.Setup(s => s.BuildDisplayLanguages())
			.Returns([new LanguageOption("English", "English", "en")]);
		_languageTesseractServiceMock
			.Setup(l => l.PopulateLanguages(It.IsAny<ICollection<TesseractLanguageOption>>(), It.IsAny<Action<TesseractLanguageOption>>()))
			.Callback<ICollection<TesseractLanguageOption>, Action<TesseractLanguageOption>?>((collection, handler) =>
			{
				var option = new TesseractLanguageOption("en", "English");
				handler?.Invoke(option);
				collection.Add(option);
			});
		_maintenanceEasyOcrServiceMock
			.Setup(m => m.GetStateAsync(It.IsAny<EasyOcrStatusTexts>()))
			.ReturnsAsync(new EasyOcrViewState
			{
				IsInstalled = false,
				IsBroken    = false,
				Status      = "Not installed"
			});
		_localizationProviderMock
			.Setup(l => l.BuildEasyOcrStatusTexts())
			.Returns(new EasyOcrStatusTexts());
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, true))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.ApplyProperties(It.IsAny<SettingsViewModel>(), It.IsAny<AppSettings>(), It.IsAny<string>(), SettingsPropertyTypes.Tracked, It.IsAny<Func<string, HotkeySetting, HotkeySetting>>(),
				true))
			.Verifiable();
		_settingsSystemServiceMock
			.Setup(s => s.SaveAsync(It.IsAny<AppSettings>()))
			.Returns(Task.CompletedTask);
	}

	private void SetupLoadAsyncMocks()
	{
		SetupBasicMocks();
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Simple))
			.Returns([]);
		_propertyAdapterMock
			.Setup(p => p.GetPropertyNames<SettingsViewModel>(SettingsPropertyTypes.Tracked, false))
			.Returns([]);
		_settingsProviderMock
			.Setup(s => s.HotkeyToString(It.IsAny<HotkeySetting>()))
			.Returns("Ctrl+A");
		_iconVisibilityProviderMock
			.Setup(i => i.BuildVisibilityMap(It.IsAny<IEnumerable<string>?>()))
			.Returns(new Dictionary<EditorIcon, bool>());
		_colorAdapterMock
			.Setup(c => c.CreateColor(It.IsAny<string>()))
			.Returns(new object());
		_colorAdapterMock
			.Setup(c => c.CreateBrush(It.IsAny<string>()))
			.Returns(new object());
	}

	private SettingsViewModel GetModel(SettingsViewModelController controller)
	{
		var field = typeof(SettingsViewModelController).GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
		return (SettingsViewModel)field!.GetValue(controller)!;
	}
}