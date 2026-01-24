using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Shotora.App.Interfaces.PeripheralServices;

namespace Shotora.App.Services.PeripheralServices;

public class MouseInteractionService : IMouseInteractionService
{
	private const double DefaultResizeBorderThickness = 6;
	public MouseButton? ParseMouseButton(string? text, MouseButton? fallback)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return fallback;
		}

		return text.Trim().ToLowerInvariant() switch
		{
			"left"   => MouseButton.Left,
			"right"  => MouseButton.Right,
			"middle" => MouseButton.Middle,
			"none"   => null,
			_        => fallback
		};
	}

	public MouseButton? GetReleasedButton(Canvas OverlayCanvas, PointerEventArgs e)
	{
		return e.GetCurrentPoint(OverlayCanvas).Properties.PointerUpdateKind switch
		{
			PointerUpdateKind.LeftButtonReleased   => MouseButton.Left,
			PointerUpdateKind.RightButtonReleased  => MouseButton.Right,
			PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
			_                                      => null
		};
	}
	public WindowEdge? GetResizeEdge(Window window, Point position, double borderThickness = DefaultResizeBorderThickness)
	{
		var width  = window.Bounds.Width;
		var height = window.Bounds.Height;
		var left   = position.X <= borderThickness;
		var right  = position.X >= width - borderThickness;
		var top    = position.Y <= borderThickness;
		var bottom = position.Y >= height - borderThickness;

		if (left && top)
		{
			return WindowEdge.NorthWest;
		}

		if (right && top)
		{
			return WindowEdge.NorthEast;
		}

		if (left && bottom)
		{
			return WindowEdge.SouthWest;
		}

		if (right && bottom)
		{
			return WindowEdge.SouthEast;
		}

		if (left)
		{
			return WindowEdge.West;
		}

		if (right)
		{
			return WindowEdge.East;
		}

		if (top)
		{
			return WindowEdge.North;
		}

		if (bottom)
		{
			return WindowEdge.South;
		}

		return null;
	}

	public StandardCursorType GetResizeCursor(WindowEdge edge)
	{
		return edge switch
		{
			WindowEdge.NorthWest => StandardCursorType.TopLeftCorner,
			WindowEdge.NorthEast => StandardCursorType.TopRightCorner,
			WindowEdge.SouthWest => StandardCursorType.BottomLeftCorner,
			WindowEdge.SouthEast => StandardCursorType.BottomRightCorner,
			WindowEdge.North     => StandardCursorType.TopSide,
			WindowEdge.South     => StandardCursorType.BottomSide,
			WindowEdge.West      => StandardCursorType.LeftSide,
			WindowEdge.East      => StandardCursorType.RightSide,
			_                    => StandardCursorType.Arrow
		};
	}
}
