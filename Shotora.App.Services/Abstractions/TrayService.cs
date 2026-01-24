using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using Shotora.App.Interfaces.Abstractions;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Models.AtomModels;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Localization;
using Shotora.App.Models.ViewModels;
using AvaloniaApplication=Avalonia.Application;
using AvaloniaTrayIcon=Avalonia.Controls.TrayIcon;
using Bitmap=Avalonia.Media.Imaging.Bitmap;

#if WINDOWS
using System.Drawing;
using H.NotifyIcon.Core;
using DrawingColor=System.Drawing.Color;
using DrawingBitmap=System.Drawing.Bitmap;
using DrawingSize=System.Drawing.Size;
#endif

namespace Shotora.App.Services.Abstractions;

public class TrayService(ILocalizationProvider  localizationProvider,
						 ITrayMenuWindowService trayMenuWindowService) : ITrayService
{
	private Bitmap? _avaloniaIconBitmap;

	private AvaloniaTrayIcon? _avaloniaTrayIcon;
	private Action _captureFull = () =>
	{
	};
	private Action _captureRegion = () =>
	{
	};
	private string _currentTheme = "Dark";
	private Action _exitApp = () =>
	{
	};
	private PixelPoint?          _lastClickPoint;
	private NativeMenuItem?      _linuxAboutItem;
	private NativeMenuItem?      _linuxCaptureFullItem;
	private NativeMenuItem?      _linuxCaptureRegionItem;
	private NativeMenuItem?      _linuxExitItem;
	private NativeMenuItem?      _linuxSettingsItem;
	private NativeMenuItem?      _macAboutItem;
	private NativeMenuItem?      _macCaptureFullItem;
	private NativeMenuItem?      _macCaptureRegionItem;
	private NativeMenuItem?      _macExitItem;
	private NativeMenuItem?      _macSettingsItem;
	private TrayMenuWindowModel? _menuWindow;
	private Action _showAbout = () =>
	{
	};
	private Action _showSettings = () =>
	{
	};
	private static bool IsMac     => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
	private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	public void Initialize(Action captureRegion, Action captureFull, Action showSettings, Action showAbout, Action exitApp)
	{
		Dispose();

		_captureRegion = captureRegion;
		_captureFull   = captureFull;
		_showSettings  = showSettings;
		_showAbout     = showAbout;
		_exitApp       = exitApp;

		_menuWindow = CreateMenuWindow();
		if (_menuWindow != null)
		{
			trayMenuWindowService.Hide(_menuWindow);
		}

		if (IsWindows)
		{
#if WINDOWS
			InitializeWindowsTray();
#endif
		}
		else
		{
			InitializeAvaloniaTray();
		}

		if (AvaloniaApplication.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.Exit += (_, _) => Dispose();
		}

		RefreshLocalization();
	}

	public void RefreshLocalization()
	{
		_macCaptureRegionItem?.Header   = localizationProvider.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion);
		_macCaptureFullItem?.Header     = localizationProvider.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull);
		_macSettingsItem?.Header        = localizationProvider.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings);
		_macAboutItem?.Header           = localizationProvider.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About);
		_macExitItem?.Header            = localizationProvider.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit);
		_linuxCaptureRegionItem?.Header = localizationProvider.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion);
		_linuxCaptureFullItem?.Header   = localizationProvider.GetString(LocalizationKeys.TrayCaptureFull,   LocalizationFallbacks.Tray.CaptureFull);
		_linuxSettingsItem?.Header      = localizationProvider.GetString(LocalizationKeys.TraySettings,      LocalizationFallbacks.Tray.Settings);
		_linuxAboutItem?.Header         = localizationProvider.GetString(LocalizationKeys.TrayAbout,         LocalizationFallbacks.Tray.About);
		_linuxExitItem?.Header          = localizationProvider.GetString(LocalizationKeys.TrayExit,          LocalizationFallbacks.Tray.Exit);
		if (_menuWindow != null)
		{
			trayMenuWindowService.UpdateLabels(_menuWindow);
		}
	}

	public void RefreshTheme(string themeName)
	{
		_currentTheme = string.IsNullOrWhiteSpace(themeName) ? "Dark" : themeName;

#if WINDOWS
		if (IsWindows && _winTrayIcon != null)
		{
			_winTrayIcon.Icon = UpdateIconHandle(_currentTheme);
		}
#endif

		if (_menuWindow != null)
		{
			trayMenuWindowService.ApplyTheme(_menuWindow, _currentTheme);
		}
	}

	public void Dispose()
	{
#if WINDOWS
		if (_winTrayIcon != null)
		{
			var messageWindow = _winTrayIcon.MessageWindow;
			messageWindow.MouseEventReceived -= TrayIconOnMouseEventReceived;
			messageWindow.InitMenuPopup      -= OnContextMenuOpening;
			_winTrayIcon.Dispose();
			_winTrayIcon = null;
		}

		DisposeIconHandle();
		_baseIconBitmap?.Dispose();
		_baseIconBitmap = null;
#endif

		if (_avaloniaTrayIcon != null)
		{
			_avaloniaTrayIcon.Clicked   -= OnAvaloniaTrayClicked;
			_avaloniaTrayIcon.IsVisible =  false;
			_avaloniaTrayIcon.Dispose();
			_avaloniaTrayIcon = null;
		}

		_avaloniaIconBitmap?.Dispose();
		_avaloniaIconBitmap = null;

		if (_menuWindow != null)
		{
			trayMenuWindowService.Close(_menuWindow);
		}
		_menuWindow = null;
	}

	private void InitializeAvaloniaTray()
	{
		_avaloniaIconBitmap = LoadAvaloniaIconBitmap();

		if (IsMac)
		{
			InitializeMacOsTray();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			InitializeLinuxTray();
		}
		else
		{
			_avaloniaTrayIcon = new AvaloniaTrayIcon
			{
				ToolTipText = "Shotora - Click to open menu",
				Icon        = _avaloniaIconBitmap != null ? new WindowIcon(_avaloniaIconBitmap) : null,
				Menu        = null,
				IsVisible   = true
			};
			_avaloniaTrayIcon.Clicked += OnAvaloniaTrayClicked;
		}
	}

	private void InitializeMacOsTray()
	{
		var (nativeMenu, captureRegionItem, captureFullItem, settingsItem, aboutItem, exitItem) = CreateNativeMenu();

		_macCaptureRegionItem = captureRegionItem;
		_macCaptureFullItem   = captureFullItem;
		_macSettingsItem      = settingsItem;
		_macAboutItem         = aboutItem;
		_macExitItem          = exitItem;

		_avaloniaTrayIcon = new AvaloniaTrayIcon
		{
			ToolTipText = "Shotora",
			Icon        = _avaloniaIconBitmap != null ? new WindowIcon(_avaloniaIconBitmap) : null,
			Menu        = nativeMenu,
			IsVisible   = true
		};
	}

	private void InitializeLinuxTray()
	{
		var (nativeMenu, captureRegionItem, captureFullItem, settingsItem, aboutItem, exitItem) = CreateNativeMenu();

		_linuxCaptureRegionItem = captureRegionItem;
		_linuxCaptureFullItem   = captureFullItem;
		_linuxSettingsItem      = settingsItem;
		_linuxAboutItem         = aboutItem;
		_linuxExitItem          = exitItem;

		_avaloniaTrayIcon = new AvaloniaTrayIcon
		{
			ToolTipText = "Shotora",
			Icon        = _avaloniaIconBitmap != null ? new WindowIcon(_avaloniaIconBitmap) : null,
			Menu        = nativeMenu,
			IsVisible   = true
		};
	}

	private (NativeMenu menu, NativeMenuItem captureRegion, NativeMenuItem captureFull, NativeMenuItem settings, NativeMenuItem about, NativeMenuItem exit) CreateNativeMenu()
	{
		var nativeMenu = new NativeMenu();

		var captureRegionItem = new NativeMenuItem
		{
			Header = localizationProvider.GetString(LocalizationKeys.TrayCaptureRegion, LocalizationFallbacks.Tray.CaptureRegion)
		};
		captureRegionItem.Click += (_, _) =>
		{
			trayMenuWindowService.ExecuteOnUiThread(_captureRegion);
		};

		var captureFullItem = new NativeMenuItem
		{
			Header = localizationProvider.GetString(LocalizationKeys.TrayCaptureFull, LocalizationFallbacks.Tray.CaptureFull)
		};
		captureFullItem.Click += (_, _) =>
		{
			trayMenuWindowService.ExecuteOnUiThread(_captureFull);
		};

		var settingsItem = new NativeMenuItem
		{
			Header = localizationProvider.GetString(LocalizationKeys.TraySettings, LocalizationFallbacks.Tray.Settings)
		};
		settingsItem.Click += (_, _) =>
		{
			trayMenuWindowService.ExecuteOnUiThread(_showSettings);
		};

		var aboutItem = new NativeMenuItem
		{
			Header = localizationProvider.GetString(LocalizationKeys.TrayAbout, LocalizationFallbacks.Tray.About)
		};
		aboutItem.Click += (_, _) =>
		{
			trayMenuWindowService.ExecuteOnUiThread(_showAbout);
		};

		var exitItem = new NativeMenuItem
		{
			Header = localizationProvider.GetString(LocalizationKeys.TrayExit, LocalizationFallbacks.Tray.Exit)
		};
		exitItem.Click += (_, _) =>
		{
			trayMenuWindowService.ExecuteOnUiThread(_exitApp);
		};

		nativeMenu.Add(captureRegionItem);
		nativeMenu.Add(captureFullItem);
		nativeMenu.Add(new NativeMenuItemSeparator());
		nativeMenu.Add(settingsItem);
		nativeMenu.Add(aboutItem);
		nativeMenu.Add(new NativeMenuItemSeparator());
		nativeMenu.Add(exitItem);

		return (nativeMenu, captureRegionItem, captureFullItem, settingsItem, aboutItem, exitItem);
	}

	private void OnAvaloniaTrayClicked(object? sender, EventArgs e)
	{
		Dispatcher.UIThread.Post(() =>
		{
			try
			{
				var anchor = GetCursorPositionCrossPlatform();
				ShowCustomMenu(anchor);
			}
			catch (Exception)
			{
			}
		}, DispatcherPriority.Normal);
	}

	private Bitmap? LoadAvaloniaIconBitmap()
	{
		try
		{
			var       uri    = new Uri("avares://Shotora.App/Assets/Shotora.png");
			using var stream = AssetLoader.Open(uri);
			return new Bitmap(stream);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private PixelPoint GetCursorPositionCrossPlatform()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return GetCursorPositionLinux();
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return GetCursorPositionMacOs();
		}

		return GetFallbackPosition();
	}

	private PixelPoint GetFallbackPosition()
	{
		var screens = _menuWindow != null
			? trayMenuWindowService.GetScreens(_menuWindow)
			: (AvaloniaApplication.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.Screens;
		if (screens?.All?.Count > 0)
		{
			var area = screens.All[0].WorkingArea;
			return new PixelPoint(area.Right - 220, area.Bottom - 20);
		}

		return new PixelPoint(100, 100);
	}

	private void ShowCustomMenu(PixelPoint anchor)
	{
		trayMenuWindowService.ExecuteOnUiThread(() =>
		{
			try
			{
				if (_menuWindow == null || IsMac && !trayMenuWindowService.IsVisible(_menuWindow))
				{
					_menuWindow = CreateMenuWindow();
				}

				if (_menuWindow != null)
				{
					var normalized = NormalizeAnchor(anchor);
					trayMenuWindowService.UpdateLabels(_menuWindow);
					trayMenuWindowService.ApplyTheme(_menuWindow, _currentTheme);
					trayMenuWindowService.ShowAt(_menuWindow, normalized);
				}
			}
			catch (Exception)
			{
			}
		});
	}

	private PixelPoint NormalizeAnchor(PixelPoint anchor)
	{
		var screens = _menuWindow != null
			? trayMenuWindowService.GetScreens(_menuWindow)
			: (AvaloniaApplication.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.Screens;

		Screen? screen = null;
		try
		{
			screen = screens?.ScreenFromPoint(anchor) ?? screens?.Primary ?? (screens?.All?.Count > 0 ? screens.All[0] : null);
		}
		catch (Exception)
		{
		}
		if (screen == null)
		{
			return anchor;
		}

		var area = screen.WorkingArea;
		var x    = Math.Clamp(anchor.X, area.X, area.Right);
		var y    = Math.Clamp(anchor.Y, area.Y, area.Bottom);
		return new PixelPoint(x, y);
	}

	private TrayMenuWindowModel CreateMenuWindow()
	{
		if (_menuWindow != null)
		{
			trayMenuWindowService.UpdateLabels(_menuWindow);
			trayMenuWindowService.ApplyTheme(_menuWindow, _currentTheme);
			return _menuWindow;
		}

		_menuWindow = trayMenuWindowService.CreateWindow(
			_captureRegion,
			_captureFull,
			_showSettings,
			_showAbout,
			_exitApp,
			_currentTheme);
		return _menuWindow;
	}

#if WINDOWS
	private TrayIconWithContextMenu? _winTrayIcon;
	private DrawingBitmap?           _baseIconBitmap;
	private IntPtr                   _iconHandle = IntPtr.Zero;
#endif

#if WINDOWS
	private void InitializeWindowsTray()
	{
		var iconHandle = UpdateIconHandle(_currentTheme);

		_winTrayIcon = new TrayIconWithContextMenu
		{
			ToolTip = "Shotora",
			Icon    = iconHandle
		};

		_winTrayIcon.Create();
		_winTrayIcon.MessageWindow.SubscribeToMouseEventReceived(TrayIconOnMouseEventReceived);
		_winTrayIcon.MessageWindow.SubscribeToInitMenuPopup(OnContextMenuOpening);
	}

	private void TrayIconOnMouseEventReceived(object? sender, MessageWindow.MouseEventReceivedEventArgs e)
	{
		if (e.MouseEvent == MouseEvent.IconRightMouseUp)
		{
			_lastClickPoint = GetCursorPixelPointWindows(new PixelPoint(e.Point.X, e.Point.Y));
			ShowCustomMenu(_lastClickPoint ?? GetCursorPixelPointWindows(null));
		}
		else if (e.MouseEvent == MouseEvent.IconLeftMouseUp)
		{
			trayMenuWindowService.ExecuteOnUiThread(_captureRegion);
		}
	}

	private void OnContextMenuOpening(object? sender, EventArgs e)
	{
		ShowCustomMenu(_lastClickPoint ?? GetCursorPixelPointWindows(null));
	}

	private IntPtr UpdateIconHandle(string themeName)
	{
		var newHandle = CreateThemedIconHandle(themeName);
		if (newHandle == IntPtr.Zero)
		{
			return _iconHandle;
		}

		DisposeIconHandle();
		_iconHandle = newHandle;
		return _iconHandle;
	}

	private IntPtr CreateThemedIconHandle(string themeName)
	{
		try
		{
			var baseBitmap = LoadBaseIconBitmap();
			if (baseBitmap == null)
			{
				return IntPtr.Zero;
			}

			var tint = themeName.Equals("Light", StringComparison.OrdinalIgnoreCase)
				? DrawingColor.FromArgb(80, 0, 0, 0)
				: themeName.Equals("Sunset", StringComparison.OrdinalIgnoreCase)
					? DrawingColor.FromArgb(110, 255, 153, 51)
					: DrawingColor.FromArgb(60,  255, 255, 255);

			using var tinted = new DrawingBitmap(baseBitmap.Width, baseBitmap.Height);
			using (var g = Graphics.FromImage(tinted))
			{
				g.Clear(DrawingColor.Transparent);
				g.DrawImage(baseBitmap, new Rectangle(0, 0, tinted.Width, tinted.Height));
				using var overlay = new SolidBrush(tint);
				g.FillRectangle(overlay, new Rectangle(0, 0, tinted.Width, tinted.Height));
			}

			return tinted.GetHicon();
		}
		catch
		{
			return IntPtr.Zero;
		}
	}

	private DrawingBitmap? LoadBaseIconBitmap()
	{
		if (_baseIconBitmap != null)
		{
			return _baseIconBitmap;
		}

		try
		{
			var       uri      = new Uri("avares://Shotora.App/Assets/Shotora.png");
			using var stream   = AssetLoader.Open(uri);
			using var original = new DrawingBitmap(stream);
			_baseIconBitmap = new DrawingBitmap(original, new DrawingSize(32, 32));
			return _baseIconBitmap;
		}
		catch
		{
			return null;
		}
	}

	private void DisposeIconHandle()
	{
		if (_iconHandle == IntPtr.Zero)
		{
			return;
		}

		DestroyIcon(_iconHandle);
		_iconHandle = IntPtr.Zero;
	}

	[DllImport("user32.dll", SetLastError = true)] private static extern bool DestroyIcon(IntPtr hIcon);

	[DllImport("user32.dll")] private static extern bool GetCursorPos(out NativePointModel lpPointModel);

	private PixelPoint GetCursorPixelPointWindows(PixelPoint? fallback)
	{
		try
		{
			if (GetCursorPos(out var p))
			{
				return new PixelPoint(p.X, p.Y);
			}
		}
		catch
		{
		}

		return fallback ?? GetFallbackPosition();
	}
#endif

	#region Linux X11 cursor position

	[DllImport("libX11.so.6")] private static extern IntPtr XOpenDisplay(IntPtr display);

	[DllImport("libX11.so.6")] private static extern int XCloseDisplay(IntPtr display);

	[DllImport("libX11.so.6")] private static extern int XDefaultScreen(IntPtr display);

	[DllImport("libX11.so.6")] private static extern IntPtr XRootWindow(IntPtr display, int screen);

	[DllImport("libX11.so.6")]
	private static extern bool XQueryPointer(
		IntPtr     display,
		IntPtr     window,
		out IntPtr rootReturn,
		out IntPtr childReturn,
		out int    rootXReturn,
		out int    rootYReturn,
		out int    winXReturn,
		out int    winYReturn,
		out uint   maskReturn);

	private PixelPoint GetCursorPositionLinux()
	{
		var display = XOpenDisplay(IntPtr.Zero);
		if (display == IntPtr.Zero)
		{
			return GetFallbackPosition();
		}

		try
		{
			var screen     = XDefaultScreen(display);
			var rootWindow = XRootWindow(display, screen);

			if (XQueryPointer(display, rootWindow,
					out _,             out _,
					out var rootX,     out var rootY,
					out _,             out _, out _))
			{
				return new PixelPoint(rootX, rootY);
			}
		}
		finally
		{
			XCloseDisplay(display);
		}

		return GetFallbackPosition();
	}

	#endregion

	#region macOS cursor position

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern LocationPointModel CGEventGetLocation(IntPtr eventRef);

	[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
	private static extern IntPtr CGEventCreate(IntPtr source);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern void CFRelease(IntPtr cf);

	private PixelPoint GetCursorPositionMacOs()
	{
		var eventRef = CGEventCreate(IntPtr.Zero);
		if (eventRef == IntPtr.Zero)
		{
			return GetFallbackPosition();
		}

		try
		{
			var point = CGEventGetLocation(eventRef);
			var screens = _menuWindow != null
				? trayMenuWindowService.GetScreens(_menuWindow)
				: (AvaloniaApplication.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.Screens;
			var primaryScreen = screens?.Primary ?? (screens?.All?.Count > 0 ? screens.All[0] : null);

			if (primaryScreen != null)
			{
				var x = (int)point.X;
				var y = (int)point.Y;
				return new PixelPoint(x, y);
			}

			return new PixelPoint((int)point.X, (int)point.Y);
		}
		catch (Exception)
		{
			return GetFallbackPosition();
		}
		finally
		{
			CFRelease(eventRef);
		}
	}

	#endregion
}
