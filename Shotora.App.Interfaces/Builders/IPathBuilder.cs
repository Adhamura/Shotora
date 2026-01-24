using Avalonia;
using Avalonia.Media;

namespace Shotora.App.Interfaces.Builders;

public interface IPathBuilder
{
	PathGeometry CreateOuterDimmer(Rect   outerBounds);
	PathGeometry CreateOuterWithHole(Rect outerBounds, Rect innerBounds);
}
