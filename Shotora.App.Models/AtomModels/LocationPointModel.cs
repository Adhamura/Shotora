using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
public struct LocationPointModel
{
	public double X;
	public double Y;
}
