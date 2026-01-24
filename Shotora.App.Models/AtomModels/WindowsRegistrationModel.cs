using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
public struct WindowsRegistrationModel
{
	public uint          style;
	public WndProcModel? lpfnWndProc;
	public int           cbClsExtra;
	public int           cbWndExtra;
	public nint          hInstance;
	public nint          hIcon;
	public nint          hCursor;
	public nint          hbrBackground;
	public string?       lpszMenuName;
	public string?       lpszClassName;
}
