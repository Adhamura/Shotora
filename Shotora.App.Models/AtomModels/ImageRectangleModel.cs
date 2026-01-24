using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
public struct ImageRectangleModel
{
	public LocationPointModel Location;
	public ImageSizeModel     SizeModel;
}
