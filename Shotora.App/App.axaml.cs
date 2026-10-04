using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using NativeSupport.Services;
using Shared.Models.Constants;
using Shotora.App.Interfaces.Abstractions;
using Shotora.App.Interfaces.Ocr.EastOcr;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Interfaces.Updates;
using Shotora.App.Interfaces.ViewModels;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services.ViewModels;
using Shotora.App.Views;
using StylesNS=Shotora.App.Styles;

namespace Shotora.App;

[ExcludeFromCodeCoverage]
public class App(
	ISettingsSystemService       settingsSystemService,
	IHotkeyService               hotkeyService,
	ITrayService                 trayService,
	IClipboardService            clipboardService,
	MainWindowViewModel          mainWindowViewModel,
	Func<MainWindow>             mainWindowFactory,
	Func<OverlayWindow>          overlayFactory,
	Func<SettingsWindow>         settingsWindowFactory,
	ISettingsViewModelController settingsViewModelFactory,
	Func<AboutWindow>            aboutWindowFactory,
	Func<AboutViewModel>         aboutViewModelFactory,
	IMaintenanceEasyOcrService   maintenanceEasyOcrRuntimeService,
	ILocalizationProvider        localizationProvider,
	IStartupService              startupService,
	IUpdateCoordinator           updateCoordinator,
	Func<UpdateWindow>           updateWindowFactory,
	IActiveWindowService         activeWindowService) : Application
{
	private OverlayWindow? _activeOverlay;
	private string?        _currentLanguage;
	private string?        _currentTheme;
	private bool           _prewarmed;
	private AppSettings    _settings = new();
	private bool           _settingsHandlersAttached;
	private UpdateWindow?  _updateWindow;
	private bool           _updatePromptPending;

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override async void OnFrameworkInitializationCompleted()
	{
		Resources["AppFontFamily"] = BuildPreferredFontFamily();

		_settings = await LoadSettingsWithTimeout();

		ApplyTheme(_settings.Theme);
		ApplyLanguage(_settings.DisplayLanguage);
		RegisterCaptureHotkeys(_settings);
		ApplyStartupSettings(_settings.RunOnStartup);
		PrewarmUiComponents();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			DisableAvaloniaDataAnnotationValidation();
			desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var mainWindow = mainWindowFactory();
			mainWindow.DataContext = mainWindowViewModel;
			desktop.MainWindow     = mainWindow;

			clipboardService.AttachTopLevel(desktop.MainWindow);

			trayService.Initialize(
				() => ShowCaptureAsync(CaptureMode.Region),
				() => ShowCaptureAsync(CaptureMode.Fullscreen),
				ShowSettings,
				ShowAbout,
				() => ShowUpdateWindow(true),
				() => desktop.Shutdown());
			RefreshTrayLocalization();
			RefreshTrayTheme();

			desktop.MainWindow.Closing += (_, e) =>
			{
				e.Cancel = true;
				desktop.MainWindow.Hide();
			};

			updateCoordinator.UpdateAvailable += (_, _) => Dispatcher.UIThread.Post(OnBackgroundUpdateFound);
			desktop.Exit                      += (_, _) => updateCoordinator.Dispose();
			updateCoordinator.Start();
		}

		base.OnFrameworkInitializationCompleted();

		_ = Task.Run(async () =>
		{
			try
			{
				BinariesLoaderService.EnsureLoaded();
				await maintenanceEasyOcrRuntimeService.EnsureRuntimeAsync();
			}
			catch (Exception)
			{
			}
		});
	}

	private async Task<AppSettings> LoadSettingsWithTimeout()
	{
		try
		{
			_settings = await settingsSystemService.LoadAsync();
			return _settings;
		}
		catch (Exception)
		{
		}

		return new AppSettings();
	}

	private static FontFamily BuildPreferredFontFamily()
	{
		const string kazukiAsset = "avares://Shotora.App/Content/KazukiReiwa-Medium.ttf#KazukiReiwa";
		return new FontFamily(kazukiAsset);
	}

	private void ApplyTheme(string? themeName)
	{
		var selected = string.IsNullOrWhiteSpace(themeName) ? "Dark" : themeName;

		if (string.Equals(_currentTheme, selected, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		_currentTheme = selected;

		var preservedNonThemeStyles = Styles.Where(s => s is not StylesNS.Theme and not StylesNS.LightTheme and not StylesNS.SunsetTheme and not StylesNS.Controls).ToList();

		Styles.Clear();
		Styles.Add(new FluentTheme());
		Styles.Add(new StylesNS.Controls());
		Styles.AddRange(preservedNonThemeStyles);
		Styles.Add(selected.Equals("Light", StringComparison.OrdinalIgnoreCase)
			? new StylesNS.LightTheme()
			: selected.Equals("Sunset", StringComparison.OrdinalIgnoreCase)
				? new StylesNS.SunsetTheme()
				: new StylesNS.Theme());

		RequestedThemeVariant = selected.Equals("Light", StringComparison.OrdinalIgnoreCase) ||
			selected.Equals("Sunset",                    StringComparison.OrdinalIgnoreCase)
				? ThemeVariant.Light
				: ThemeVariant.Dark;

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			foreach (var window in desktop.Windows)
			{
				window.RequestedThemeVariant = RequestedThemeVariant;
			}
		}

		RefreshTrayTheme();
	}

	private void ApplyLanguage(string? languageCode)
	{
		var selected = string.IsNullOrWhiteSpace(languageCode) ? LanguageCollection.DefaultOcrLanguageCode : languageCode;

		if (string.Equals(_currentLanguage, selected, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		_currentLanguage = selected;

		var baseUri = new Uri("avares://Shotora.App/");

		ApplyLanguageResources(Resources, baseUri, selected);
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			foreach (var window in desktop.Windows)
			{
				ApplyLanguageResources(window.Resources, baseUri, selected);
			}
		}

		localizationProvider.SetLanguage(selected);
		RefreshTrayLocalization();
	}

	private static void ApplyLanguageResources(IResourceDictionary target, Uri baseUri, string selected)
	{
		var existingLangDictionaries = target.MergedDictionaries
			.OfType<ResourceInclude>()
			.Where(ri =>
				ri.Source != null &&
				ri.Source.OriginalString.Contains("/Resources/Languages/Strings.", StringComparison.OrdinalIgnoreCase))
			.ToList();
		foreach (var dict in existingLangDictionaries)
		{
			target.MergedDictionaries.Remove(dict);
		}

		try
		{
			target.MergedDictionaries.Add(new ResourceInclude(baseUri)
			{
				Source = new Uri($"avares://Shotora.App/Resources/Languages/Strings.{selected}.axaml")
			});
		}
		catch
		{
			if (!selected.Equals(LanguageCollection.DefaultOcrLanguageCode, StringComparison.OrdinalIgnoreCase))
			{
				target.MergedDictionaries.Add(new ResourceInclude(baseUri)
				{
					Source = new Uri("avares://Shotora.App/Resources/Languages/Strings.en.axaml")
				});
			}
		}
	}

	private static void DisableAvaloniaDataAnnotationValidation()
	{
		var dataValidationPluginsToRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();
		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}

	private void OnBackgroundUpdateFound()
	{
		// Never interrupt a capture in progress: prompt once the overlay closes.
		if (_activeOverlay != null)
		{
			_updatePromptPending = true;
			return;
		}

		ShowUpdateWindow(false);
	}

	/// <summary>Shows the single update dialog; <paramref name="checkNow" /> starts a fresh check (manual "Check for updates").</summary>
	private void ShowUpdateWindow(bool checkNow)
	{
		if (_updateWindow == null)
		{
			_updateWindow        =  updateWindowFactory();
			_updateWindow.Closed += (_, _) => _updateWindow = null;
			_updateWindow.Show();
		}

		_updateWindow.Activate();

		if (checkNow && _updateWindow.ViewModel.CheckCommand.CanExecute(null))
		{
			_updateWindow.ViewModel.CheckCommand.Execute(null);
		}
	}

	private void ShowAbout()
	{
		if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
		{
			return;
		}

		var window = aboutWindowFactory();
		var vm     = aboutViewModelFactory();
		window.DataContext = vm;

		var owner = desktop.MainWindow;
		window.WindowStartupLocation = owner is
		{
			IsVisible: true
		}
			? WindowStartupLocation.CenterOwner
			: WindowStartupLocation.CenterScreen;
		if (owner is
			{
				IsVisible: true
			})
		{
			window.Show(owner);
		}
		else
		{
			window.Show();
		}

		window.Activate();
	}

	private async void ShowSettings()
	{
		if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
		{
			return;
		}

		var window = settingsWindowFactory();
		await settingsViewModelFactory.LoadAsync(_settings);
		window.InitializeAsync();
		settingsViewModelFactory.SettingsHandlersAttach(vm =>
		{
			if (!_settingsHandlersAttached)
			{
				vm.ThemeChanged    += (_, theme) => ApplyTheme(theme);
				vm.LanguageChanged += (_, lang) => ApplyLanguage(lang);
				vm.SettingsSaved += (_, settings) =>
				{
					_settings = settings;
					_activeOverlay?.ApplySettings(settings);
					ApplyTheme(settings.Theme);
					ApplyLanguage(settings.DisplayLanguage);
					RegisterCaptureHotkeys(settings);
					ApplyStartupSettings(settings.RunOnStartup);
				};
				vm.LiveSettingsChanged += (_, settings) =>
				{
					_settings = settings;
					_activeOverlay?.ApplySettings(settings);
					ApplyTheme(settings.Theme);
					ApplyLanguage(settings.DisplayLanguage);
					RegisterCaptureHotkeys(settings);
					ApplyStartupSettings(settings.RunOnStartup);
				};
				_settingsHandlersAttached = true;
			}

			var owner = desktop.MainWindow;
			if (owner is
				{
					IsVisible: true
				})
			{
				window.Show(owner);
			}
			else
			{
				window.Show();
			}
			window.Activate();
		});
	}

	private async void ShowCaptureAsync(CaptureMode mode, PixelRect? activeWindowBounds = null)
	{
		if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
		{
			return;
		}

		var overlay = overlayFactory();
		overlay.WindowStartupLocation = WindowStartupLocation.Manual;

		try
		{
			await overlay.InitializeAsync(mode, activeWindowBounds);
		}
		catch (InvalidOperationException ex)
		{
			var messageBox = new Window
			{
				Title                 = "Screen Capture Error",
				Width                 = 500,
				Height                = 200,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				Content = new StackPanel
				{
					Margin  = new Thickness(20),
					Spacing = 10,
					Children =
					{
						new TextBlock
						{
							Text       = "Failed to capture screen:",
							FontWeight = FontWeight.Bold
						},
						new TextBlock
						{
							Text         = ex.Message,
							TextWrapping = TextWrapping.Wrap
						}
					}
				}
			};
			messageBox.Show();
			return;
		}

		_activeOverlay = overlay;
		overlay.Closed += (_, _) =>
		{
			if (_activeOverlay == overlay)
			{
				_activeOverlay = null;
			}

			if (_updatePromptPending)
			{
				_updatePromptPending = false;
				Dispatcher.UIThread.Post(() => ShowUpdateWindow(false), DispatcherPriority.Background);
			}
		};
		var owner = desktop.MainWindow;
		if (owner is
			{
				IsVisible: true
			})
		{
			overlay.Show(owner);
		}
		else
		{
			overlay.Show();
		}
		overlay.Topmost = true;
	}

	private void PrewarmUiComponents()
	{
		if (_prewarmed)
		{
			return;
		}

		_prewarmed = true;

		Dispatcher.UIThread.Post(async () =>
		{
			try
			{
				_ = overlayFactory();
			}
			catch (Exception)
			{
			}

			try
			{
				await settingsViewModelFactory.LoadAsync(_settings).ConfigureAwait(true);
			}
			catch (Exception)
			{
			}

			try
			{
				_ = aboutWindowFactory();
			}
			catch (Exception)
			{
			}
		}, DispatcherPriority.Background);
	}

	private void RegisterCaptureHotkeys(AppSettings settings)
	{
		if (hotkeyService == null)
		{
			return;
		}

		hotkeyService.Reset();
		hotkeyService.Register(settings.RegionHotkey,     () => Dispatcher.UIThread.Post(() => ShowCaptureAsync(CaptureMode.Region)));
		hotkeyService.Register(settings.FullscreenHotkey, () => Dispatcher.UIThread.Post(() => ShowCaptureAsync(CaptureMode.Fullscreen)));
		hotkeyService.Register(settings.ActiveWindowHotkey, () =>
		{
			// Read the foreground window now, before the overlay opens and takes focus.
			var windowBounds = activeWindowService.GetForegroundWindowBounds();
			Dispatcher.UIThread.Post(() => ShowCaptureAsync(CaptureMode.ActiveWindow, windowBounds));
		});
	}

	private void RefreshTrayLocalization()
	{
		trayService.RefreshLocalization();
	}

	private void RefreshTrayTheme()
	{
		trayService.RefreshTheme(_currentTheme ?? "Dark");
	}

	private void ApplyStartupSettings(bool runOnStartup)
	{
		try
		{
			startupService.SetRunOnStartup(runOnStartup);
		}
		catch
		{
		}
	}
}
