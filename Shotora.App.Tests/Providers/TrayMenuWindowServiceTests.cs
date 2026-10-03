using System.Reflection;
using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Moq;
using NativeSupport.Enums;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.ItemModels;
using Shotora.App.Models.Localization;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services.Providers;

namespace Shotora.App.Tests.Providers;

public class TrayMenuWindowServiceTests : IDisposable
{
	private static   bool                        _headlessInitialized;
	private readonly Mock<IDispatcherFacade>     _dispatcher   = new(MockBehavior.Loose);
	private readonly Mock<ILocalizationProvider> _localization = new(MockBehavior.Strict);
	private readonly Mock<IProcessSystemService> _process      = new(MockBehavior.Strict);
	private readonly TrayMenuWindowService       _sut;

	public TrayMenuWindowServiceTests()
	{
		EnsureDispatcher();

		_dispatcher.Setup(d => d.Post(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
			.Callback<Action, DispatcherPriority>((action, _) => Dispatcher.UIThread.Post(action));
		_dispatcher.Setup(d => d.CheckAccess()).Returns(() => Dispatcher.UIThread.CheckAccess());
		_dispatcher.Setup(d => d.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
			.Callback<Action, DispatcherPriority>((action, _) => Dispatcher.UIThread.InvokeAsync(action));
		_sut = new TrayMenuWindowService(_localization.Object, _dispatcher.Object, _process.Object);
	}

	public void Dispose()
	{
		SetApplication(null);
	}

	[Fact]
	public void Given_Localization_When_CreateWindow_Then_BuildsButtonsAndLabels()
	{
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion)).Returns("Region");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull)).Returns("Full");
		_localization.Setup(l => l.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings)).Returns("Settings");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About)).Returns("About");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCheckForUpdates, LocalizationFallbacks.Tray.CheckForUpdates)).Returns("Updates");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit)).Returns("Exit");
		_process.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		var regionClicks  = 0;
		var fullClicks    = 0;
		var settingsCalls = 0;
		var aboutCalls    = 0;
		var updatesCalls  = 0;
		var exitCalls     = 0;

		var model = _sut.CreateWindow(
			() => regionClicks++,
			() => fullClicks++,
			() => settingsCalls++,
			() => aboutCalls++,
			() => updatesCalls++,
			() => exitCalls++,
			"Dark");

		Assert.Equal(RuntimeOs.Mac, model.CurrentOs);
		Assert.Equal("Region",      model.CaptureRegionButton.Content);
		Assert.Equal("Full",        model.CaptureFullButton.Content);
		Assert.Equal("Settings",    model.SettingsButton.Content);
		Assert.Equal("About",       model.AboutButton.Content);
		Assert.Equal("Updates",     model.UpdatesButton.Content);
		Assert.Equal("Exit",        model.ExitButton.Content);

		RaiseClick(model.CaptureRegionButton);
		RaiseClick(model.CaptureFullButton);
		RaiseClick(model.SettingsButton);
		RaiseClick(model.AboutButton);
		RaiseClick(model.UpdatesButton);
		RaiseClick(model.ExitButton);
		RunJobs();

		Assert.Equal(1, regionClicks);
		Assert.Equal(1, fullClicks);
		Assert.Equal(1, settingsCalls);
		Assert.Equal(1, aboutCalls);
		Assert.Equal(1, updatesCalls);
		Assert.Equal(1, exitCalls);

		_localization.VerifyAll();
		_process.VerifyAll();
	}

	[Fact]
	public void Given_AppResources_When_ApplyTheme_Then_UsesResourceBrushesAndRebindsHoverHandlers()
	{
		EnsureHeadless();
		var model = BuildModel();
		var app   = Application.Current ?? new Application();
		SetApplication(app);

		var bg     = Brushes.AliceBlue;
		var border = Brushes.BlanchedAlmond;
		var text   = Brushes.Brown;
		var hl     = Brushes.CadetBlue;
		app.Resources["PanelBackgroundBrush"] = bg;
		app.Resources["PanelBorderBrush"]     = border;
		app.Resources["TextPrimaryBrush"]     = text;
		app.Resources["HighlightBrush"]       = hl;
		app.Resources["PopupBackgroundBrush"] = Brushes.Chartreuse;
		app.Resources["ButtonHoverBrush"]     = Brushes.Coral;

		var enterHandler = new EventHandler<PointerEventArgs>((_, _) =>
		{
		});
		var exitHandler = new EventHandler<PointerEventArgs>((_, _) =>
		{
		});
		model.CaptureRegionButton.Tag = new HoverHandlers
		{
			Enter = enterHandler,
			Exit  = exitHandler
		};

		_sut.ApplyTheme(model, "Light");

		Assert.Same(bg,     model.Root.Background);
		Assert.Same(border, model.Root.BorderBrush);
		Assert.Same(hl,     model.Separator.Background);
		Assert.Same(text,   model.Window.Foreground);

		AssertHoverHandlersReplaced(model.CaptureRegionButton);
		AssertHoverHandlersReplaced(model.CaptureFullButton);
		AssertHoverHandlersReplaced(model.SettingsButton);
		AssertHoverHandlersReplaced(model.AboutButton);
		AssertHoverHandlersReplaced(model.UpdatesButton);
		AssertHoverHandlersReplaced(model.ExitButton);

		app.Resources.Remove("PanelBackgroundBrush");
		app.Resources.Remove("PanelBorderBrush");
		app.Resources.Remove("TextPrimaryBrush");
		app.Resources.Remove("HighlightBrush");
		app.Resources.Remove("PopupBackgroundBrush");
		app.Resources.Remove("ButtonHoverBrush");
	}

	[Theory]
	[InlineData("Light",  255, 250, 250, 252)]
	[InlineData("Sunset", 255, 248, 220, 194)]
	[InlineData("Dark",   255, 25,  26,  32)]
	public void Given_NoApplication_When_ApplyTheme_Then_FallbackBrushesMatchTheme(string theme, byte a, byte r, byte g, byte b)
	{
		EnsureHeadless();
		SetApplication(null);
		var model = BuildModel();
		model.Root.Background = null;

		_sut.ApplyTheme(model, theme);

		var solid = Assert.IsAssignableFrom<ISolidColorBrush>(model.Root.Background);
		Assert.Equal(Color.FromArgb(a, r, g, b), solid.Color);
	}

	[Fact]
	public void Given_NullAction_When_ExecuteOnUiThread_Then_DoesNothing()
	{
		_sut.ExecuteOnUiThread(null);
	}

	[Fact]
	public void Given_NonUiThread_When_ExecuteOnUiThread_Then_Dispatches()
	{
		EnsureHeadless();
		var executed = false;

		_sut.ExecuteOnUiThread(() => executed = true);
		RunJobs();

		Assert.True(executed);
	}

	[Fact]
	public void Given_UiThread_When_ExecuteOnUiThread_Then_RunsInline()
	{
		EnsureHeadless();
		var executed = false;

		Dispatcher.UIThread.Post(() => _sut.ExecuteOnUiThread(() => executed = true));
		RunJobs();

		Assert.True(executed);
	}

	[Fact]
	public void Given_WindowVisible_When_ShowAt_Then_Activates()
	{
		EnsureHeadless();
		var model = BuildModel();
		SetField(typeof(TopLevel), model.Window, "_isVisible", true);

		_sut.ShowAt(model, new PixelPoint(400, 300));
	}

	[Fact]
	public void Given_WindowHidden_When_ShowAt_Then_StoresAnchorAndPositionsWithinBounds()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 320, 320));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 0, 0));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(0, 0));

		var anchor = new PixelPoint(100, 100);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.Equal(anchor,                  model.LastAnchor);
		Assert.Equal(new PixelPoint(12, 112), model.Window.Position);
	}

	[Fact]
	public void Given_Model_When_HideCloseAndQueries_Then_DelegatesToWindow()
	{
		EnsureHeadless();
		var model = BuildModel();
		Dispatcher.UIThread.Post(() => model.Window.Show());
		RunJobs();
		Assert.True(model.Window.IsVisible);

		_sut.Hide(model);
		Assert.False(model.Window.IsVisible);

		_sut.Close(model);
		Assert.False(model.Window.IsVisible);
	}

	[Fact]
	public void Given_Model_When_IsVisible_Then_ReturnsWindowVisibility()
	{
		EnsureHeadless();
		var model = BuildModel();

		Assert.False(_sut.IsVisible(model));

		Dispatcher.UIThread.Post(() => model.Window.Show());
		RunJobs();

		Assert.True(_sut.IsVisible(model));
	}

	[Fact]
	public void Given_Model_When_GetScreens_Then_ReturnsWindowScreens()
	{
		EnsureHeadless();
		var model = BuildModel();

		var result = _sut.GetScreens(model);

		Assert.Same(model.Window.Screens, result);
	}

	[Fact]
	public void Given_Model_When_UpdateLabels_Then_SetsButtonContent()
	{
		EnsureHeadless();
		var model = BuildModel();
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion)).Returns("Capture Region");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull)).Returns("Capture Full");
		_localization.Setup(l => l.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings)).Returns("Settings");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About)).Returns("About");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayCheckForUpdates, LocalizationFallbacks.Tray.CheckForUpdates)).Returns("Updates");
		_localization.Setup(l => l.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit)).Returns("Exit");

		_sut.UpdateLabels(model);

		Assert.Equal("Capture Region", model.CaptureRegionButton.Content);
		Assert.Equal("Capture Full",   model.CaptureFullButton.Content);
		Assert.Equal("Settings",       model.SettingsButton.Content);
		Assert.Equal("About",          model.AboutButton.Content);
		Assert.Equal("Updates",        model.UpdatesButton.Content);
		Assert.Equal("Exit",           model.ExitButton.Content);
		_localization.VerifyAll();
	}

	[Fact]
	public void Given_WindowDeactivated_When_EventFires_Then_HidesWindow()
	{
		EnsureHeadless();
		_localization.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>())).Returns("Test");
		_process.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);

		var model = _sut.CreateWindow(() =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, "Dark");

		Assert.False(model.Window.ShowInTaskbar);
		Assert.True(model.Window.Topmost);
	}

	[Fact]
	public void Given_MacOs_When_WindowLosesFocus_Then_HidesWindow()
	{
		EnsureHeadless();
		_localization.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>())).Returns("Test");
		_process.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Mac);

		var model = _sut.CreateWindow(() =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, "Dark");

		Assert.Equal(RuntimeOs.Mac, model.CurrentOs);
	}

	[Fact]
	public void Given_NonMacOs_When_WindowLosesFocus_Then_DoesNotHideWindow()
	{
		EnsureHeadless();
		_localization.Setup(l => l.GetString(It.IsAny<string>(), It.IsAny<string>())).Returns("Test");
		_process.Setup(p => p.GetCurrentOs()).Returns(RuntimeOs.Windows);

		var model = _sut.CreateWindow(() =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, () =>
		{
		}, "Dark");

		Assert.Equal(RuntimeOs.Windows, model.CurrentOs);
	}

	[Fact]
	public void Given_NoScreens_When_ShowAt_Then_DoesNotThrow()
	{
		EnsureHeadless();
		var model = BuildModel();
		SetField(model.Window, "_screens", null);

		var exception = Record.Exception(() => _sut.ShowAt(model, new PixelPoint(100, 100)));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnchorNearLeftEdge_When_ShowAt_Then_ClampsToLeftMargin()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(50, 500);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.X >= 12);
	}

	[Fact]
	public void Given_AnchorNearRightEdge_When_ShowAt_Then_ClampsToRightMargin()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(1900, 500);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.X + 200 <= 1920 - 12);
	}

	[Fact]
	public void Given_AnchorNearTopEdge_When_ShowAt_Then_PositionsBelowAnchor()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(500, 50);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.Y >= anchor.Y + 12);
	}

	[Fact]
	public void Given_AnchorNearBottomEdge_When_ShowAt_Then_ClampsToBottomMargin()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(500, 1050);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.Y + 220 <= 1080 - 12);
	}

	[Fact]
	public void Given_AnchorCausesBottomOverflow_When_ShowAt_Then_ClampsToWorkingAreaBottom()
	{
		EnsureHeadless();
		var model = BuildModel();

		var screens = CreateScreens(new PixelRect(0, 0, 1920, 800));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(500, 100);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.Y >= 0);
	}

	[Fact]
	public void Given_WorkingAreaCausesBottomClamp_When_ShowAt_Then_PositionsAtBottomMargin()
	{
		EnsureHeadless();
		var model = BuildModel();

		var screens = CreateScreens(new PixelRect(0, 0, 500, 300));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(250, 250);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.Y + 220 <= 300);
	}

	[Fact]
	public void Given_VerySmallWorkingArea_When_ShowAt_Then_ClampsToBottomMinusMenuHeight()
	{
		EnsureHeadless();
		var model = BuildModel();

		var screens = CreateScreens(new PixelRect(0, 0, 500, 250));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(250, 125);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(model.Window.Position.Y >= 0);
		Assert.Equal(anchor, model.LastAnchor);
	}

	[Fact]
	public void Given_WindowVisible_When_ShowAt_Then_ActivatesInsteadOfShowing()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));
		SetField(typeof(TopLevel), model.Window, "_isVisible",  true);

		var anchor = new PixelPoint(500, 500);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.Equal(anchor, model.LastAnchor);
	}

	[Fact]
	public void Given_WindowHidden_When_ShowAt_Then_PostsUpdatePositionWithLoadedPriority()
	{
		EnsureHeadless();
		var postCalled = false;
		_dispatcher.Setup(d => d.Post(It.IsAny<Action>(), DispatcherPriority.Loaded))
			.Callback<Action, DispatcherPriority>((action, _) =>
			{
				postCalled = true;
				Dispatcher.UIThread.Post(action);
			});

		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 200, 220));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(200, 220));

		var anchor = new PixelPoint(500, 500);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.True(postCalled);
	}

	[Fact]
	public void Given_OnUiThread_When_ExecuteOnUiThread_Then_RunsActionDirectly()
	{
		EnsureHeadless();
		var executed          = false;
		var checkAccessCalled = false;
		var invokeAsyncCalled = false;

		_dispatcher.Setup(d => d.CheckAccess()).Returns(() =>
		{
			checkAccessCalled = true;
			return true;
		});
		_dispatcher.Setup(d => d.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
			.Callback<Action, DispatcherPriority>((_, _) => invokeAsyncCalled = true);

		_sut.ExecuteOnUiThread(() => executed = true);

		Assert.True(checkAccessCalled);
		Assert.True(executed);
		Assert.False(invokeAsyncCalled);
	}

	[Fact]
	public void Given_NotOnUiThread_When_ExecuteOnUiThread_Then_InvokesAsyncWithSendPriority()
	{
		EnsureHeadless();
		var                 executed          = false;
		var                 invokeAsyncCalled = false;
		DispatcherPriority? usedPriority      = null;

		_dispatcher.Setup(d => d.CheckAccess()).Returns(false);
		_dispatcher.Setup(d => d.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
			.Callback<Action, DispatcherPriority>((action, priority) =>
			{
				invokeAsyncCalled = true;
				usedPriority      = priority;
				action();
			});

		_sut.ExecuteOnUiThread(() => executed = true);

		Assert.True(invokeAsyncCalled);
		Assert.Equal(DispatcherPriority.Send, usedPriority);
		Assert.True(executed);
	}

	[Fact]
	public void Given_ZeroBoundsAndClientSize_When_ShowAt_Then_UsesFallbackDimensions()
	{
		EnsureHeadless();
		var model   = BuildModel();
		var screens = CreateScreens(new PixelRect(0, 0, 1920, 1080));
		SetField(model.Window,     "_screens",   screens);
		SetField(typeof(Visual),   model.Window, "_bounds",     new Rect(0, 0, 0, 0));
		SetField(typeof(TopLevel), model.Window, "_clientSize", new Size(0, 0));

		var anchor = new PixelPoint(500, 500);
		_sut.ShowAt(model, anchor);
		RunJobs();

		Assert.Equal(anchor, model.LastAnchor);
	}

	[Fact]
	public void Given_ExistingHoverHandlers_When_ApplyTheme_Then_RemovesOldHandlersAndAddsNew()
	{
		EnsureHeadless();
		SetApplication(null);
		var model = BuildModel();

		_sut.ApplyTheme(model, "Light");
		var firstTag = model.CaptureRegionButton.Tag;
		Assert.NotNull(firstTag);

		_sut.ApplyTheme(model, "Dark");
		var secondTag = model.CaptureRegionButton.Tag;

		Assert.NotSame(firstTag, secondTag);
	}

	[Fact]
	public void Given_AppWithPartialResources_When_ApplyTheme_Then_UsesFallbackForSeparator()
	{
		EnsureHeadless();
		var model = BuildModel();
		var app   = Application.Current ?? new Application();
		SetApplication(app);

		app.Resources["PanelBackgroundBrush"] = Brushes.White;
		app.Resources["PanelBorderBrush"]     = Brushes.Gray;
		app.Resources["TextPrimaryBrush"]     = Brushes.Black;

		_sut.ApplyTheme(model, "Light");

		Assert.Same(Brushes.Gray, model.Separator.Background);

		app.Resources.Remove("PanelBackgroundBrush");
		app.Resources.Remove("PanelBorderBrush");
		app.Resources.Remove("TextPrimaryBrush");
	}

	[Fact]
	public void Given_AppWithInputBackgroundBrush_When_ApplyTheme_Then_UsesAsFallback()
	{
		EnsureHeadless();
		var model = BuildModel();
		var app   = Application.Current ?? new Application();
		SetApplication(app);

		var inputBg    = Brushes.LightGray;
		var inputHover = Brushes.DarkGray;
		app.Resources["PanelBackgroundBrush"]      = Brushes.White;
		app.Resources["PanelBorderBrush"]          = Brushes.Gray;
		app.Resources["TextPrimaryBrush"]          = Brushes.Black;
		app.Resources["InputBackgroundBrush"]      = inputBg;
		app.Resources["InputBackgroundHoverBrush"] = inputHover;

		_sut.ApplyTheme(model, "Light");

		Assert.Same(inputBg, model.CaptureRegionButton.Background);

		app.Resources.Remove("PanelBackgroundBrush");
		app.Resources.Remove("PanelBorderBrush");
		app.Resources.Remove("TextPrimaryBrush");
		app.Resources.Remove("InputBackgroundBrush");
		app.Resources.Remove("InputBackgroundHoverBrush");
	}

	private static TrayMenuWindowModel BuildModel()
	{
		EnsureHeadless();
		var window    = new Window();
		var capture   = new Button();
		var full      = new Button();
		var settings  = new Button();
		var about     = new Button();
		var updates   = new Button();
		var exit      = new Button();
		var root      = new Border();
		var separator = new Border();
		var model     = new TrayMenuWindowModel();
		model.Init(window, capture, full, settings, about, updates, exit, root, separator);
		return model;
	}

	private static void RaiseClick(Button button)
	{
		button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
	}

	private static void RunJobs()
	{
		Dispatcher.UIThread.RunJobs();
	}

	private static void EnsureDispatcher()
	{
		EnsureHeadless();
		_ = Dispatcher.UIThread;
	}

	private static void SetApplication(Application? app)
	{
		var prop = typeof(Application).GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
		if (prop?.CanWrite == true)
		{
			prop.SetValue(null, app);
			return;
		}

		var field = typeof(Application).GetField("s_current", BindingFlags.Static | BindingFlags.NonPublic);
		field?.SetValue(null, app);
	}

	private static void SetField(object target, string name, object? value)
	{
		SetField(target.GetType(), target, name, value);
	}

	private static void SetField(Type searchType, object target, string name, object? value)
	{
		var field = searchType.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		if (field != null)
		{
			field.SetValue(target, value);
		}
	}

	private static void EnsureHeadless()
	{
		if (_headlessInitialized)
		{
			return;
		}

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
		_headlessInitialized = true;
	}

	private static void AssertHoverHandlersReplaced(Button button)
	{
		Assert.IsType<HoverHandlers>(button.Tag);
		var handlers = (HoverHandlers)button.Tag!;
		Assert.NotNull(handlers.Enter);
		Assert.NotNull(handlers.Exit);
	}

	private static Screens CreateScreens(PixelRect workingArea)
	{
		var screen  = new Screen(1, workingArea, workingArea, true);
		var screens = (Screens)FormatterServices.GetUninitializedObject(typeof(Screens));
		var fields  = typeof(Screens).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		foreach (var field in fields)
		{
			if (typeof(IEnumerable<Screen>).IsAssignableFrom(field.FieldType))
			{
				field.SetValue(screens, new[]
				{
					screen
				});
			}
			else if (field.FieldType == typeof(Screen))
			{
				field.SetValue(screens, screen);
			}
		}

		return screens;
	}
}
