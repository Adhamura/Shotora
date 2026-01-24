using System.Diagnostics.CodeAnalysis;

namespace Shotora.App.Models.AtomModels;

[ExcludeFromCodeCoverage]
public struct NativePointModel(int x, int y)
{
	public readonly int X = x;
	public readonly int Y = y;
}
