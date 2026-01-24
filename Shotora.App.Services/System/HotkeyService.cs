using System.Runtime.InteropServices;
using Shared.Models.Enums;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using Shotora.App.Models.AtomModels;

namespace Shotora.App.Services.System;

public sealed class HotkeyService : IHotkeyService
{
	private const int WmHotkey = 0x0312;

	private readonly Dictionary<int, Action> _callbacks = new();
	private readonly bool                    _isWindows = OperatingSystem.IsWindows();
	private          int                     _currentId;
	private          bool                    _disposed;
	private          nint                    _windowHandle;
	private          WndProcModel?           _wndProc;

	public HotkeyService()
	{
		if (_isWindows)
		{
			InitializeMessageWindow();
		}
	}

	public int Register(HotkeySetting setting, Action callback)
	{
		if (!_isWindows || _disposed || _windowHandle == nint.Zero)
		{
			return 0;
		}

		ArgumentNullException.ThrowIfNull(callback);

		var id        = ++_currentId;
		var modifiers = (uint)ConvertModifiers(setting.Modifiers);
		if (!RegisterHotKey(_windowHandle, id, modifiers, (uint)setting.Key))
		{
			return -1;
		}

		_callbacks[id] = callback;
		return id;
	}

	public void Reset()
	{
		if (!_isWindows || _windowHandle == nint.Zero)
		{
			return;
		}

		foreach (var id in _callbacks.Keys.ToList())
		{
			UnregisterHotKey(_windowHandle, id);
		}
		_callbacks.Clear();
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		Reset();

		if (_windowHandle != nint.Zero)
		{
			DestroyWindow(_windowHandle);
			_windowHandle = nint.Zero;
		}

		_disposed = true;
	}

	private void InitializeMessageWindow()
	{
		_wndProc = WindowProc;
		var className = $"ShotoraHotkeyWnd_{Guid.NewGuid():N}";
		var wndClass = new WindowsRegistrationModel
		{
			lpszClassName = className,
			lpfnWndProc   = _wndProc,
			hInstance     = GetModuleHandle(null)
		};

		if (RegisterClass(ref wndClass) == 0)
		{
			return;
		}

		_windowHandle = CreateWindowEx(
			0,
			className,
			string.Empty,
			0,
			0,
			0,
			0,
			0,
			HwndMessage,
			nint.Zero,
			wndClass.hInstance,
			nint.Zero);
	}

	private nint WindowProc(nint hWnd, uint msg, nint wParam, nint lParam)
	{
		if (msg == WmHotkey)
		{
			var id = wParam.ToInt32();
			if (_callbacks.TryGetValue(id, out var callback))
			{
				_ = Task.Run(callback);
			}
		}

		return DefWindowProc(hWnd, msg, wParam, lParam);
	}

	private static ModifierKeys ConvertModifiers(KeyModifiers modifiers)
	{
		var result = ModifierKeys.None;
		if (modifiers.HasFlag(KeyModifiers.Alt))
		{
			result |= ModifierKeys.Alt;
		}
		if (modifiers.HasFlag(KeyModifiers.Control))
		{
			result |= ModifierKeys.Control;
		}
		if (modifiers.HasFlag(KeyModifiers.Shift))
		{
			result |= ModifierKeys.Shift;
		}
		if (modifiers.HasFlag(KeyModifiers.Win))
		{
			result |= ModifierKeys.Win;
		}
		return result;
	}

	#region Win32

	private static readonly nint HwndMessage = new(-3);

	[DllImport("user32.dll", SetLastError = true)] private static extern ushort RegisterClass([In] ref WindowsRegistrationModel lpWndClass);

	[DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern nint CreateWindowEx(
		uint   dwExStyle,
		string lpClassName,
		string lpWindowName,
		uint   dwStyle,
		int    x,
		int    y,
		int    nWidth,
		int    nHeight,
		nint   hWndParent,
		nint   hMenu,
		nint   hInstance,
		nint   lpParam);

	[DllImport("user32.dll", EntryPoint = "DefWindowProcW", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);

	[DllImport("user32.dll", SetLastError = true)] private static extern bool DestroyWindow(nint hWnd);

	[DllImport("user32.dll", SetLastError = true)] private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)] private static extern bool UnregisterHotKey(nint hWnd, int id);

	[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)]
	private static extern nint GetModuleHandle(string? lpModuleName);

	#endregion
}
