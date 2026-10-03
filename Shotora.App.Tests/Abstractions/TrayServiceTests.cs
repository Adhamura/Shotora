using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Moq;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services.Abstractions;

namespace Shotora.App.Tests.Abstractions;

public class TrayServiceTests
{
	private readonly Mock<ILocalizationProvider>  _localization = new(MockBehavior.Strict);
	private readonly Mock<ITrayMenuWindowService> _menuService  = new(MockBehavior.Strict);
	private readonly TrayService                  _sut;
	static TrayServiceTests()
	{
		try
		{
			AppBuilder.Configure<Application>()
				.UseHeadless(new AvaloniaHeadlessPlatformOptions
				{
					UseHeadlessDrawing = true
				})
				.SetupWithoutStarting();
		}
		catch (InvalidOperationException)
		{
		}
		_ = Dispatcher.UIThread;
	}

	public TrayServiceTests()
	{
		_sut = new TrayService(_localization.Object, _menuService.Object);
	}

	[Fact]
	public void Given_MenuWindowAndMacItems_When_RefreshLocalization_Then_UpdatesHeadersAndLabels()
	{
		RunOnUi(() =>
		{
			var region   = new NativeMenuItem();
			var full     = new NativeMenuItem();
			var settings = new NativeMenuItem();
			var about    = new NativeMenuItem();
			var updates  = new NativeMenuItem();
			var exit     = new NativeMenuItem();
			SetPrivateField("_macCaptureRegionItem", region);
			SetPrivateField("_macCaptureFullItem",   full);
			SetPrivateField("_macSettingsItem",      settings);
			SetPrivateField("_macAboutItem",         about);
			SetPrivateField("_macUpdatesItem",       updates);
			SetPrivateField("_macExitItem",          exit);

			var menuWindow = (TrayMenuWindowModel)RuntimeHelpers.GetUninitializedObject(typeof(TrayMenuWindowModel));
			SetPrivateField("_menuWindow", menuWindow);

			_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion)).Returns("cap-region");
			_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull)).Returns("cap-full");
			_localization.Setup(l => l.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings)).Returns("settings");
			_localization.Setup(l => l.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About)).Returns("about");
			_localization.Setup(l => l.GetString(LocalizationKeys.TrayCheckForUpdates, LocalizationFallbacks.Tray.CheckForUpdates)).Returns("updates");
			_localization.Setup(l => l.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit)).Returns("exit");

			_menuService.Setup(s => s.UpdateLabels(menuWindow));

			_sut.RefreshLocalization();

			Assert.Equal("cap-region", region.Header);
			Assert.Equal("cap-full",   full.Header);
			Assert.Equal("settings",   settings.Header);
			Assert.Equal("about",      about.Header);
			Assert.Equal("updates",    updates.Header);
			Assert.Equal("exit",       exit.Header);
			_menuService.Verify(s => s.UpdateLabels(menuWindow), Times.Once);
			_menuService.VerifyNoOtherCalls();
			_localization.VerifyAll();
		});
	}

	[Fact]
	public void Given_NoMenuWindow_When_RefreshLocalization_Then_DoesNotCallMenuService()
	{
		_sut.RefreshLocalization();

		_localization.VerifyNoOtherCalls();
		_menuService.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_ExistingMenuWindow_When_CreateMenuWindow_Then_ReusesAndUpdates()
	{
		var menuWindow = (TrayMenuWindowModel)RuntimeHelpers.GetUninitializedObject(typeof(TrayMenuWindowModel));
		SetPrivateField("_menuWindow",   menuWindow);
		SetPrivateField("_currentTheme", "Dark");

		_menuService.Setup(s => s.UpdateLabels(menuWindow));
		_menuService.Setup(s => s.ApplyTheme(menuWindow, "Dark"));

		var result = InvokeCreateMenuWindow();

		Assert.Same(menuWindow, result);
		_menuService.Verify(s => s.UpdateLabels(menuWindow),       Times.Once);
		_menuService.Verify(s => s.ApplyTheme(menuWindow, "Dark"), Times.Once);
		_menuService.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_NoMenuWindow_When_CreateMenuWindow_Then_CreatesWithActions()
	{
		var captureRegion = () =>
		{
		};
		var captureFull = () =>
		{
		};
		var showSettings = () =>
		{
		};
		var showAbout = () =>
		{
		};
		var checkUpdates = () =>
		{
		};
		var exit = () =>
		{
		};

		SetPrivateField("_captureRegion", captureRegion);
		SetPrivateField("_captureFull",   captureFull);
		SetPrivateField("_showSettings",  showSettings);
		SetPrivateField("_showAbout",     showAbout);
		SetPrivateField("_checkForUpdates", checkUpdates);
		SetPrivateField("_exitApp",       exit);
		SetPrivateField("_currentTheme",  "Dark");

		var created = (TrayMenuWindowModel)RuntimeHelpers.GetUninitializedObject(typeof(TrayMenuWindowModel));
		_menuService.Setup(s => s.CreateWindow(captureRegion, captureFull, showSettings, showAbout, checkUpdates, exit, "Dark"))
			.Returns(created);

		var result = InvokeCreateMenuWindow();

		Assert.Same(created, result);
		Assert.Same(created, GetPrivateField("_menuWindow"));
		_menuService.Verify(s => s.CreateWindow(captureRegion, captureFull, showSettings, showAbout, checkUpdates, exit, "Dark"), Times.Once);
		_menuService.VerifyNoOtherCalls();
	}

	[Theory]
	[InlineData("Sunset", "Sunset")]
	[InlineData("  ",     "Dark")]
	[InlineData(null,     "Dark")]
	public void Given_ThemeName_When_RefreshTheme_Then_SetsThemeAndAppliesToMenu(string? input, string expected)
	{
		var menuWindow = (TrayMenuWindowModel)RuntimeHelpers.GetUninitializedObject(typeof(TrayMenuWindowModel));
		SetPrivateField("_menuWindow", menuWindow);

		_menuService.Setup(s => s.ApplyTheme(menuWindow, expected));

		_sut.RefreshTheme(input!);

		var currentTheme = $"{GetPrivateField("_currentTheme")}";
		Assert.Equal(expected, currentTheme);
		_menuService.Verify(s => s.ApplyTheme(menuWindow, expected), Times.Once);
		_menuService.VerifyNoOtherCalls();
		_localization.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_NoScreens_When_GetFallbackPosition_Then_ReturnsDefaultPoint()
	{
		var result = InvokePixelPoint("_menuWindow", null, "GetFallbackPosition");

		Assert.Equal(new PixelPoint(100, 100), result);
	}

	[Fact]
	public void Given_NoScreens_When_NormalizeAnchor_Then_ReturnsOriginal()
	{
		var anchor     = new PixelPoint(10, 20);
		var normalized = InvokePixelPoint("_menuWindow", null, "NormalizeAnchor", anchor);

		Assert.Equal(anchor, normalized);
	}

	[Fact]
	public void Given_MenuWindowExists_When_ShowCustomMenu_Then_ReusesWindowAndShowsAtNormalizedPoint()
	{
		var menuWindow = (TrayMenuWindowModel)RuntimeHelpers.GetUninitializedObject(typeof(TrayMenuWindowModel));
		SetPrivateField("_menuWindow",   menuWindow);
		SetPrivateField("_currentTheme", "Dark");

		_menuService.Setup(s => s.GetScreens(menuWindow)).Returns((Screens?)null);
		_menuService.Setup(s => s.UpdateLabels(menuWindow));
		_menuService.Setup(s => s.ApplyTheme(menuWindow, "Dark"));
		_menuService.Setup(s => s.ShowAt(menuWindow, new PixelPoint(400, 500)));
		_menuService.Setup(s => s.ExecuteOnUiThread(It.IsAny<Action>()))
			.Callback<Action>(action => action());

		InvokeVoid("ShowCustomMenu", new PixelPoint(400, 500));

		_menuService.VerifyAll();
	}

	[Fact]
	public void Given_InvalidAsset_When_LoadAvaloniaIconBitmap_Then_ReturnsNull()
	{
		var method = typeof(TrayService).GetMethod("LoadAvaloniaIconBitmap", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException("LoadAvaloniaIconBitmap not found.");

		var result = method.Invoke(_sut, []);

		Assert.Null(result);
	}

	[Fact]
	public void Given_WindowsPlatform_When_GetCursorPositionCrossPlatform_Then_UsesFallback()
	{
		var method = typeof(TrayService).GetMethod("GetCursorPositionCrossPlatform", BindingFlags.Instance | BindingFlags.NonPublic) ??
			throw new InvalidOperationException("GetCursorPositionCrossPlatform not found.");

		var result = (PixelPoint)method.Invoke(_sut, [])!;

		Assert.Equal(new PixelPoint(100, 100), result);
	}

	private void SetPrivateField(string name, object? value)
	{
		var field = typeof(TrayService).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"{name} field not found.");
		field.SetValue(_sut, value);
	}

	private object? GetPrivateField(string name)
	{
		var field = typeof(TrayService).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"{name} field not found.");
		return field.GetValue(_sut);
	}

	private static void RunOnUi(Action action)
	{
		Dispatcher.UIThread.Post(action);
		Dispatcher.UIThread.RunJobs();
	}

	private TrayMenuWindowModel InvokeCreateMenuWindow()
	{
		var method = typeof(TrayService).GetMethod("CreateMenuWindow", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException("CreateMenuWindow not found.");
		return (TrayMenuWindowModel)method.Invoke(_sut, [])!;
	}

	private PixelPoint InvokePixelPoint(string fieldNameForMenu, object? menuValue, string methodName, params object[] args)
	{
		if (fieldNameForMenu != null)
		{
			SetPrivateField(fieldNameForMenu, menuValue);
		}

		var method = typeof(TrayService).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"Method '{methodName}' not found.");
		return (PixelPoint)method.Invoke(_sut, args)!;
	}

	private void InvokeVoid(string methodName, params object[] args)
	{
		var method = typeof(TrayService).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"Method '{methodName}' not found.");
		method.Invoke(_sut, args);
	}
}
