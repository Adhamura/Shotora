using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using NativeSupport.Enums;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using Shotora.App.Models.ViewModels;

namespace Shotora.App.Services.Providers;

public class TrayMenuWindowService(ILocalizationProvider localizationProvider, IDispatcherFacade dispatcherFacade, IProcessSystemService processSystemService) : ITrayMenuWindowService
{
	public TrayMenuWindowModel CreateWindow(
		Action onCaptureRegion,
		Action onCaptureFull,
		Action onSettings,
		Action onAbout,
		Action onCheckForUpdates,
		Action onExit,
		string themeName)
	{
		var window = new Window
		{
			SystemDecorations                  = SystemDecorations.None,
			ShowInTaskbar                      = false,
			CanResize                          = false,
			Topmost                            = true,
			SizeToContent                      = SizeToContent.WidthAndHeight,
			WindowStartupLocation              = WindowStartupLocation.Manual,
			TransparencyLevelHint              = [WindowTransparencyLevel.Transparent],
			Background                         = Brushes.Transparent,
			ExtendClientAreaToDecorationsHint  = true,
			ExtendClientAreaChromeHints        = ExtendClientAreaChromeHints.NoChrome,
			ExtendClientAreaTitleBarHeightHint = -1
		};

		var captureRegionButton = CreateButton(onCaptureRegion);
		var captureFullButton   = CreateButton(onCaptureFull);
		var settingsButton      = CreateButton(onSettings);
		var aboutButton         = CreateButton(onAbout);
		var updatesButton       = CreateButton(onCheckForUpdates);
		var exitButton          = CreateButton(onExit);

		// Size and spacing come from the shared "menu-separator" style (Shotora.App/Styles/Controls.axaml).
		var separator = new Border();
		separator.Classes.Add("menu-separator");

		var stack = new StackPanel
		{
			Orientation = Orientation.Vertical,
			Spacing     = 2,
			Margin      = new Thickness(6)
		};
		stack.Children.Add(captureRegionButton);
		stack.Children.Add(captureFullButton);
		stack.Children.Add(settingsButton);
		stack.Children.Add(updatesButton);
		stack.Children.Add(aboutButton);
		stack.Children.Add(separator);
		stack.Children.Add(exitButton);

		var root = new Border
		{
			CornerRadius    = new CornerRadius(12),
			BorderThickness = new Thickness(1),
			MinWidth        = 200,
			ClipToBounds    = true,
			Child           = stack,
			BoxShadow = new BoxShadows(new BoxShadow
			{
				Blur    = 16,
				OffsetX = 0,
				OffsetY = 4,
				Color   = Color.FromArgb(80, 0, 0, 0)
			})
		};

		window.Content = root;

		var model = new TrayMenuWindowModel();
		model.Init(window, captureRegionButton, captureFullButton, settingsButton, aboutButton, updatesButton, exitButton, root, separator);
		model.CurrentOs = processSystemService.GetCurrentOs();

		window.Deactivated += (_, _) =>
		{
			if (model.CurrentOs != RuntimeOs.Linux && window.IsVisible)
			{
				window.Hide();
			}
		};
		window.LostFocus += (_, _) =>
		{
			if (model.CurrentOs == RuntimeOs.Mac && window.IsVisible)
			{
				window.Hide();
			}
		};
		window.Opened += (_, _) => UpdatePosition(model, model.LastAnchor);

		UpdateLabels(model);
		ApplyTheme(model, themeName);
		return model;
	}

	public void UpdateLabels(TrayMenuWindowModel model)
	{
		model.CaptureRegionButton.Content = localizationProvider.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion);
		model.CaptureFullButton.Content   = localizationProvider.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull);
		model.SettingsButton.Content      = localizationProvider.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings);
		model.AboutButton.Content         = localizationProvider.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About);
		model.UpdatesButton.Content       = localizationProvider.GetString(LocalizationKeys.TrayCheckForUpdates, LocalizationFallbacks.Tray.CheckForUpdates);
		model.ExitButton.Content          = localizationProvider.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit);
	}

	public void ApplyTheme(TrayMenuWindowModel model, string themeName)
	{
		var app = Application.Current;

		var isLight  = themeName.Equals("Light",  StringComparison.OrdinalIgnoreCase);
		var isSunset = themeName.Equals("Sunset", StringComparison.OrdinalIgnoreCase);

		IBrush backgroundBrush;
		IBrush borderBrush;
		IBrush textBrush;
		IBrush separatorBrush;

		if (app != null                                                    &&
			app.TryFindResource("PanelBackgroundBrush", out var bgRes)     &&
			app.TryFindResource("PanelBorderBrush",     out var borderRes) &&
			app.TryFindResource("TextPrimaryBrush",     out var textRes))
		{
			backgroundBrush = (IBrush)bgRes!;
			borderBrush     = (IBrush)borderRes!;
			textBrush       = (IBrush)textRes!;
			separatorBrush  = GetResource<IBrush>(app, "DividerBrush") ?? borderBrush;
		}
		else
		{
			backgroundBrush = isLight
				? new SolidColorBrush(Color.FromArgb(255, 250, 250, 252))
				: isSunset
					? new SolidColorBrush(Color.FromArgb(255, 248, 220, 194))
					: new SolidColorBrush(Color.FromArgb(255, 25,  26,  32));
			borderBrush = isLight
				? new SolidColorBrush(Color.FromArgb(255, 210, 214, 223))
				: isSunset
					? new SolidColorBrush(Color.FromArgb(255, 203, 142, 116))
					: new SolidColorBrush(Color.FromArgb(255, 60,  63,  75));
			textBrush = isLight
				? new SolidColorBrush(Color.FromArgb(255, 18, 18, 24))
				: isSunset
					? new SolidColorBrush(Color.FromArgb(255, 74,  45,  35))
					: new SolidColorBrush(Color.FromArgb(255, 237, 239, 245));
			separatorBrush = isSunset
				? new SolidColorBrush(Color.FromArgb(255, 217, 140, 104))
				: borderBrush;
		}

		// Menu entries take their idle/hover/pressed/focus visuals from the "menu-item" style; only the
		// window chrome is themed here (the menu is a separate top-level window created in code).
		model.Root.Background      = backgroundBrush;
		model.Root.BorderBrush     = borderBrush;
		model.Separator.Background = separatorBrush;
		model.Window.Foreground    = textBrush;
	}

	public void ShowAt(TrayMenuWindowModel model, PixelPoint anchor)
	{
		var window = model.Window;
		model.LastAnchor = anchor;

		UpdatePosition(model, anchor);

		if (!window.IsVisible)
		{
			window.Show();
			window.Activate();
			window.Topmost = true;
			dispatcherFacade.Post(() =>
			{
				UpdatePosition(model, anchor);
				window.Activate();
				window.Topmost = true;
			}, DispatcherPriority.Loaded);
		}
		else
		{
			window.Topmost = true;
			window.Activate();
		}
	}

	public void Hide(TrayMenuWindowModel model)
	{
		model.Window.Hide();
	}

	public void Close(TrayMenuWindowModel model)
	{
		model.Window.Close();
	}

	public bool IsVisible(TrayMenuWindowModel model)
	{
		return model.Window.IsVisible;
	}

	public Screens? GetScreens(TrayMenuWindowModel model)
	{
		return model.Window.Screens;
	}

	public void ExecuteOnUiThread(Action? action)
	{
		if (action == null)
		{
			return;
		}
		if (dispatcherFacade.CheckAccess())
		{
			action();
		}
		else
		{
			dispatcherFacade.InvokeAsync(action, DispatcherPriority.Send);
		}
	}

	private Button CreateButton(Action onClick)
	{
		// Size, padding, typography and all states come from the shared "menu-item" style
		// (Shotora.App/Styles/Controls.axaml) so the tray menu matches the rest of the UI.
		var button = new Button();
		button.Classes.Add("menu-item");
		button.Click += (_, _) => ExecuteOnUiThread(onClick);
		return button;
	}

	private static void UpdatePosition(TrayMenuWindowModel model, PixelPoint anchor)
	{
		var window = model.Window;
		var screen = window.Screens.ScreenFromPoint(anchor) ?? window.Screens.Primary ?? window.Screens.All[0];

		var widthDips  = window.Bounds.Width  > 0 ? window.Bounds.Width : window.ClientSize.Width;
		var heightDips = window.Bounds.Height > 0 ? window.Bounds.Height : window.ClientSize.Height;

		window.Position = CalculateMenuPosition(anchor, screen.WorkingArea, new Size(widthDips, heightDips), screen.Scaling);
	}

	/// <summary>
	///     Places the menu centred above the tray anchor, clamped to the working area with a 12 DIP margin.
	///     The anchor, working area and result are physical pixels; <paramref name="menuSizeDips" /> is in DIPs and is
	///     converted with <paramref name="scaling" /> (the screen's scaling factor).
	/// </summary>
	public static PixelPoint CalculateMenuPosition(PixelPoint anchor, PixelRect workingArea, Size menuSizeDips, double scaling)
	{
		if (scaling <= 0 || double.IsNaN(scaling))
		{
			scaling = 1;
		}

		var menuWidth  = (int)Math.Ceiling(menuSizeDips.Width  * scaling);
		var menuHeight = (int)Math.Ceiling(menuSizeDips.Height * scaling);

		if (menuWidth <= 0)
		{
			menuWidth = (int)Math.Ceiling(200 * scaling);
		}
		if (menuHeight <= 0)
		{
			menuHeight = (int)Math.Ceiling(220 * scaling);
		}

		var margin = (int)Math.Round(12 * scaling);

		var x = anchor.X - menuWidth / 2;
		var y = anchor.Y - menuHeight - margin;

		if (x < workingArea.X + margin)
		{
			x = workingArea.X + margin;
		}
		else if (x + menuWidth > workingArea.Right - margin)
		{
			x = workingArea.Right - menuWidth - margin;
		}

		if (y < workingArea.Y + margin)
		{
			y = anchor.Y + margin;
		}
		if (y + menuHeight > workingArea.Bottom - margin)
		{
			y = workingArea.Bottom - menuHeight - margin;
		}

		return new PixelPoint(x, y);
	}

	private static T? GetResource<T>(Application app, string key) where T : class
	{
		return app.TryFindResource(key, out var resource) && resource is T typed ? typed : null;
	}
}
