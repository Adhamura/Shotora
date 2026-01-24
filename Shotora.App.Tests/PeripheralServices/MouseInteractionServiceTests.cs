using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Moq;
using Shotora.App.Services.PeripheralServices;

namespace Shotora.App.Tests.PeripheralServices;

public class MouseInteractionServiceTests
{
	private readonly MouseInteractionService _sut = new();

	[Theory]
	[InlineData(null,  MouseButton.Right,  MouseButton.Right)]
	[InlineData("",    MouseButton.Left,   MouseButton.Left)]
	[InlineData("   ", MouseButton.Middle, MouseButton.Middle)]
	public void Given_NullOrWhitespaceText_When_ParseMouseButton_Then_ReturnsFallback(string? text, MouseButton fallback, MouseButton expected)
	{
		var result = _sut.ParseMouseButton(text, fallback);

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("left",     MouseButton.Left)]
	[InlineData("RIGHT",    MouseButton.Right)]
	[InlineData("middle",   MouseButton.Middle)]
	[InlineData("none",     null)]
	[InlineData("  left  ", MouseButton.Left)]
	public void Given_TextButtonNames_When_ParseMouseButton_Then_ReturnsExpectedButton(string value, MouseButton? expected)
	{
		var result = _sut.ParseMouseButton(value, MouseButton.Left);

		Assert.Equal(expected, result);
	}

	[Fact]
	public void Given_UnknownText_When_ParseMouseButton_Then_ReturnsFallback()
	{
		var result = _sut.ParseMouseButton("side", MouseButton.Right);

		Assert.Equal(MouseButton.Right, result);
	}

	[Theory]
	[InlineData(PointerUpdateKind.LeftButtonReleased,   MouseButton.Left)]
	[InlineData(PointerUpdateKind.RightButtonReleased,  MouseButton.Right)]
	[InlineData(PointerUpdateKind.MiddleButtonReleased, MouseButton.Middle)]
	[InlineData(PointerUpdateKind.Other,                null)]
	public void Given_PointerReleased_When_GetReleasedButton_Then_MapsToMouseButton(PointerUpdateKind kind, MouseButton? expected)
	{
		var overlay = new Canvas();
		var args = new PointerEventArgs(
			InputElement.PointerReleasedEvent,
			overlay,
			Mock.Of<IPointer>(),
			overlay,
			new Point(),
			0,
			new PointerPointProperties(RawInputModifiers.None, kind),
			KeyModifiers.None);

		var result = _sut.GetReleasedButton(overlay, args);

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(3,  3,  WindowEdge.NorthWest)]
	[InlineData(97, 3,  WindowEdge.NorthEast)]
	[InlineData(3,  57, WindowEdge.SouthWest)]
	[InlineData(97, 57, WindowEdge.SouthEast)]
	[InlineData(3,  30, WindowEdge.West)]
	[InlineData(97, 30, WindowEdge.East)]
	[InlineData(50, 3,  WindowEdge.North)]
	[InlineData(50, 57, WindowEdge.South)]
	public void Given_PositionNearBorder_When_GetResizeEdge_Then_ReturnsEdge(double x, double y, WindowEdge expected)
	{
		var window = CreateWindow(100, 60);

		var result = _sut.GetResizeEdge(window, new Point(x, y));

		Assert.Equal(expected, result);
	}

	[Fact]
	public void Given_BorderThicknessOverride_When_GetResizeEdge_Then_RespectsCustomThickness()
	{
		var window = CreateWindow(100, 100);
		var result = _sut.GetResizeEdge(window, new Point(18, 50), 20);

		Assert.Equal(WindowEdge.West, result);
	}

	[Fact]
	public void Given_CenterPosition_When_GetResizeEdge_Then_ReturnsNull()
	{
		var window = CreateWindow(100, 100);

		var result = _sut.GetResizeEdge(window, new Point(50, 50));

		Assert.Null(result);
	}

	[Theory]
	[InlineData(WindowEdge.NorthWest, StandardCursorType.TopLeftCorner)]
	[InlineData(WindowEdge.NorthEast, StandardCursorType.TopRightCorner)]
	[InlineData(WindowEdge.SouthWest, StandardCursorType.BottomLeftCorner)]
	[InlineData(WindowEdge.SouthEast, StandardCursorType.BottomRightCorner)]
	[InlineData(WindowEdge.North,     StandardCursorType.TopSide)]
	[InlineData(WindowEdge.South,     StandardCursorType.BottomSide)]
	[InlineData(WindowEdge.West,      StandardCursorType.LeftSide)]
	[InlineData(WindowEdge.East,      StandardCursorType.RightSide)]
	[InlineData((WindowEdge)999,      StandardCursorType.Arrow)]
	public void Given_WindowEdge_When_GetResizeCursor_Then_ReturnsMappedStandardCursor(WindowEdge edge, StandardCursorType expected)
	{
		var cursor = _sut.GetResizeCursor(edge);

		Assert.Equal(expected, cursor);
	}

	private static Window CreateWindow(double width, double height)
	{
		var window = (Window)RuntimeHelpers.GetUninitializedObject(typeof(Window));
		SetWindowBounds(window, new Rect(0, 0, width, height));
		return window;
	}

	private static void SetWindowBounds(Window window, Rect bounds)
	{
		var field = typeof(Visual).GetField("_bounds", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException("_bounds field not found.");
		field.SetValue(window, bounds);
	}
}
