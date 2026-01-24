using System.Diagnostics.CodeAnalysis;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models.Constants;

[ExcludeFromCodeCoverage]
public static class DrawingTool
{
	public static bool IsDrawingTool(OverlayTool tool)
	{
		return tool is OverlayTool.Pen or OverlayTool.Line or OverlayTool.Arrow
			or OverlayTool.Rectangle or OverlayTool.Ellipse or OverlayTool.Highlight
			or OverlayTool.Blur or OverlayTool.Pixelate;
	}
}
