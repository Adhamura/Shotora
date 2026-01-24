using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
public struct ImageSizeModel
{
	public double width;
	public double height;
}
