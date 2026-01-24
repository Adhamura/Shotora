using Avalonia;
using Avalonia.Media;
using Shotora.App.Interfaces.Builders;

namespace Shotora.App.Services.Builders;

public class PathBuilder : IPathBuilder
{
	public PathGeometry CreateOuterDimmer(Rect outerBounds)
	{
		var geometry = new PathGeometry();
		geometry.Figures!.Add(CreateRectangleFigure(outerBounds));
		return geometry;
	}

	public PathGeometry CreateOuterWithHole(Rect outerBounds, Rect innerBounds)
	{
		var geometry = new PathGeometry
		{
			FillRule = FillRule.EvenOdd
		};
		geometry.Figures!.Add(CreateRectangleFigure(outerBounds));
		geometry.Figures!.Add(CreateRectangleFigure(innerBounds));
		return geometry;
	}

	private static PathFigure CreateRectangleFigure(Rect bounds, bool isFilled = true)
	{
		var figure = new PathFigure
		{
			StartPoint = bounds.TopLeft,
			IsClosed   = true,
			IsFilled   = isFilled
		};

		figure.Segments!.Add(new LineSegment
		{
			Point = bounds.TopRight
		});
		figure.Segments!.Add(new LineSegment
		{
			Point = bounds.BottomRight
		});
		figure.Segments!.Add(new LineSegment
		{
			Point = bounds.BottomLeft
		});

		return figure;
	}
}
