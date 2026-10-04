using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia;
using Shotora.App.Interfaces.System;

namespace Shotora.App.Services.System;

[ExcludeFromCodeCoverage]
public sealed class ActiveWindowService : IActiveWindowService
{
	private const int DwmwaExtendedFrameBounds = 9;

	public PixelRect? GetForegroundWindowBounds()
	{
		return OperatingSystem.IsWindows() ? GetForegroundWindowBoundsWindows() : null;
	}

	[SupportedOSPlatform("windows")]
	private static PixelRect? GetForegroundWindowBoundsWindows()
	{
		var handle = GetForegroundWindow();
		if (handle == nint.Zero || handle == GetShellWindow() || handle == GetDesktopWindow() || IsIconic(handle))
		{
			return null;
		}

		// The extended frame excludes the invisible resize borders that GetWindowRect includes on Windows 10 and later.
		if (DwmGetWindowAttribute(handle, DwmwaExtendedFrameBounds, out var rect, Marshal.SizeOf<NativeRect>()) != 0 &&
			!GetWindowRect(handle, out rect))
		{
			return null;
		}

		var width  = rect.Right  - rect.Left;
		var height = rect.Bottom - rect.Top;
		return width > 0 && height > 0 ? new PixelRect(rect.Left, rect.Top, width, height) : null;
	}

	[DllImport("user32.dll")] private static extern nint GetForegroundWindow();

	[DllImport("user32.dll")] private static extern nint GetShellWindow();

	[DllImport("user32.dll")] private static extern nint GetDesktopWindow();

	[DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool IsIconic(nint hWnd);

	[DllImport("user32.dll", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetWindowRect(nint hWnd, out NativeRect lpRect);

	[DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(nint hWnd, int dwAttribute, out NativeRect pvAttribute, int cbAttribute);

	[StructLayout(LayoutKind.Sequential)]
	private struct NativeRect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}
}
