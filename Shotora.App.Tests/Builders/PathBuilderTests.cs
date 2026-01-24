using Avalonia;
using Avalonia.Headless;
using Avalonia.Media;
using Shotora.App.Services.Builders;
using Point=Avalonia.Point;

namespace Shotora.App.Tests.Builders;

public class PathBuilderTests
{
	private readonly PathBuilder _sut = new();
	static PathBuilderTests()
	{
		try
		{
			AppBuilder.Configure<Application>()
				.UseHeadless(new AvaloniaHeadlessPlatformOptions
				{
					UseHeadlessDrawing = true
				})
				.SetupWithoutStarting();
		}
		catch (InvalidOperationException)
		{
		}
	}

	#region CreateOuterDimmer Tests

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_ReturnsPathGeometryWithSingleFigure()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		Assert.NotNull(result);
		Assert.NotNull(result.Figures);
		Assert.Single(result.Figures);
	}

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_FigureHasCorrectStartPoint()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		var figure = result.Figures![0];
		Assert.Equal(new Point(10, 20), figure.StartPoint);
	}

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_FigureIsClosed()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		var figure = result.Figures![0];
		Assert.True(figure.IsClosed);
	}

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_FigureIsFilled()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		var figure = result.Figures![0];
		Assert.True(figure.IsFilled);
	}

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_FigureHasFourLineSegments()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		var figure = result.Figures![0];
		Assert.NotNull(figure.Segments);
		Assert.Equal(3, figure.Segments.Count);
	}

	[Fact]
	public void Given_StandardRect_When_CreateOuterDimmer_Then_LineSegmentsFormRectangle()
	{
		var outerBounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterDimmer(outerBounds);

		var figure   = result.Figures![0];
		var segments = figure.Segments!.Cast<LineSegment>().ToList();
		Assert.Equal(3,                   segments.Count);
		Assert.Equal(new Point(110, 20),  segments[0].Point);
		Assert.Equal(new Point(110, 170), segments[1].Point);
		Assert.Equal(new Point(10,  170), segments[2].Point);
	}

	[Theory]
	[InlineData(0,    0,    100, 100)]
	[InlineData(50,   50,   200, 300)]
	[InlineData(-10,  -20,  150, 200)]
	[InlineData(1000, 2000, 50,  75)]
	public void Given_VariousRects_When_CreateOuterDimmer_Then_ReturnsCorrectGeometry(double x, double y, double width, double height)
	{
		var outerBounds = new Rect(x, y, width, height);

		var result = _sut.CreateOuterDimmer(outerBounds);

		Assert.NotNull(result);
		Assert.Single(result.Figures!);
		var figure = result.Figures![0];
		Assert.Equal(new Point(x, y), figure.StartPoint);
		Assert.True(figure.IsClosed);
		Assert.True(figure.IsFilled);
		Assert.Equal(3, figure.Segments!.Count);
	}

	[Fact]
	public void Given_ZeroSizeRect_When_CreateOuterDimmer_Then_ReturnsGeometryWithZeroSizeFigure()
	{
		var outerBounds = new Rect(10, 20, 0, 0);

		var result = _sut.CreateOuterDimmer(outerBounds);

		Assert.NotNull(result);
		Assert.Single(result.Figures!);
		var figure = result.Figures![0];
		Assert.Equal(new Point(10, 20), figure.StartPoint);
		var segments = figure.Segments!.Cast<LineSegment>().ToList();
		Assert.Equal(new Point(10, 20), segments[0].Point);
		Assert.Equal(new Point(10, 20), segments[1].Point);
		Assert.Equal(new Point(10, 20), segments[2].Point);
	}

	[Fact]
	public void Given_NegativeSizeRect_When_CreateOuterDimmer_Then_ReturnsGeometry()
	{
		var outerBounds = new Rect(10, 20, -50, -30);

		var result = _sut.CreateOuterDimmer(outerBounds);

		Assert.NotNull(result);
		Assert.Single(result.Figures!);
		var figure = result.Figures![0];
		Assert.Equal(new Point(10, 20), figure.StartPoint);
		Assert.Equal(3,                 figure.Segments!.Count);
	}

	[Fact]
	public void Given_OriginRect_When_CreateOuterDimmer_Then_ReturnsCorrectGeometry()
	{
		var outerBounds = new Rect(0, 0, 100, 100);

		var result = _sut.CreateOuterDimmer(outerBounds);

		Assert.NotNull(result);
		var figure = result.Figures![0];
		Assert.Equal(new Point(0, 0), figure.StartPoint);
		var segments = figure.Segments!.Cast<LineSegment>().ToList();
		Assert.Equal(new Point(100, 0),   segments[0].Point);
		Assert.Equal(new Point(100, 100), segments[1].Point);
		Assert.Equal(new Point(0,   100), segments[2].Point);
	}

	#endregion

	#region CreateOuterWithHole Tests

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_ReturnsPathGeometryWithTwoFigures()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.NotNull(result.Figures);
		Assert.Equal(2, result.Figures.Count);
	}

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_HasEvenOddFillRule()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.Equal(FillRule.EvenOdd, result.FillRule);
	}

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_FirstFigureIsOuterBounds()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		var outerFigure = result.Figures![0];
		Assert.Equal(new Point(0, 0), outerFigure.StartPoint);
		Assert.True(outerFigure.IsClosed);
		Assert.True(outerFigure.IsFilled);
		Assert.Equal(3, outerFigure.Segments!.Count);
	}

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_SecondFigureIsInnerBounds()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		var innerFigure = result.Figures![1];
		Assert.Equal(new Point(50, 50), innerFigure.StartPoint);
		Assert.True(innerFigure.IsClosed);
		Assert.True(innerFigure.IsFilled);
		Assert.Equal(3, innerFigure.Segments!.Count);
	}

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_OuterFigureHasCorrectSegments()
	{
		var outerBounds = new Rect(10, 20, 200, 300);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		var outerFigure = result.Figures![0];
		var segments    = outerFigure.Segments!.Cast<LineSegment>().ToList();
		Assert.Equal(new Point(210, 20),  segments[0].Point);
		Assert.Equal(new Point(210, 320), segments[1].Point);
		Assert.Equal(new Point(10,  320), segments[2].Point);
	}

	[Fact]
	public void Given_StandardRects_When_CreateOuterWithHole_Then_InnerFigureHasCorrectSegments()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		var innerFigure = result.Figures![1];
		var segments    = innerFigure.Segments!.Cast<LineSegment>().ToList();
		Assert.Equal(new Point(150, 50),  segments[0].Point);
		Assert.Equal(new Point(150, 150), segments[1].Point);
		Assert.Equal(new Point(50,  150), segments[2].Point);
	}

	[Theory]
	[InlineData(0,   0,   200, 200, 50,  50,  100, 100)]
	[InlineData(10,  20,  300, 400, 100, 150, 50,  75)]
	[InlineData(-50, -30, 200, 150, 0,   0,   100, 100)]
	[InlineData(100, 200, 500, 600, 200, 300, 100, 150)]
	public void Given_VariousRects_When_CreateOuterWithHole_Then_ReturnsCorrectGeometry(
		double outerX,
		double outerY,
		double outerW,
		double outerH,
		double innerX,
		double innerY,
		double innerW,
		double innerH)
	{
		var outerBounds = new Rect(outerX, outerY, outerW, outerH);
		var innerBounds = new Rect(innerX, innerY, innerW, innerH);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(FillRule.EvenOdd,          result.FillRule);
		Assert.Equal(2,                         result.Figures!.Count);
		Assert.Equal(new Point(outerX, outerY), result.Figures[0].StartPoint);
		Assert.Equal(new Point(innerX, innerY), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_ZeroSizeOuter_When_CreateOuterWithHole_Then_ReturnsGeometryWithZeroSizeOuter()
	{
		var outerBounds = new Rect(0,  0,  0,  0);
		var innerBounds = new Rect(10, 10, 50, 50);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                 result.Figures!.Count);
		Assert.Equal(new Point(0,  0),  result.Figures[0].StartPoint);
		Assert.Equal(new Point(10, 10), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_ZeroSizeInner_When_CreateOuterWithHole_Then_ReturnsGeometryWithZeroSizeInner()
	{
		var outerBounds = new Rect(0,  0,  200, 200);
		var innerBounds = new Rect(50, 50, 0,   0);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                 result.Figures!.Count);
		Assert.Equal(new Point(0,  0),  result.Figures[0].StartPoint);
		Assert.Equal(new Point(50, 50), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_BothZeroSize_When_CreateOuterWithHole_Then_ReturnsGeometryWithBothZeroSize()
	{
		var outerBounds = new Rect(10, 20, 0, 0);
		var innerBounds = new Rect(50, 50, 0, 0);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                 result.Figures!.Count);
		Assert.Equal(new Point(10, 20), result.Figures[0].StartPoint);
		Assert.Equal(new Point(50, 50), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_NegativeSizeRects_When_CreateOuterWithHole_Then_ReturnsGeometry()
	{
		var outerBounds = new Rect(0,  0,  -100, -50);
		var innerBounds = new Rect(10, 10, -20,  -30);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(FillRule.EvenOdd, result.FillRule);
		Assert.Equal(2,                result.Figures!.Count);
	}

	[Fact]
	public void Given_InnerLargerThanOuter_When_CreateOuterWithHole_Then_ReturnsGeometry()
	{
		var outerBounds = new Rect(0,   0,   100, 100);
		var innerBounds = new Rect(-50, -50, 200, 200);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                   result.Figures!.Count);
		Assert.Equal(new Point(0,   0),   result.Figures[0].StartPoint);
		Assert.Equal(new Point(-50, -50), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_OverlappingRects_When_CreateOuterWithHole_Then_ReturnsGeometry()
	{
		var outerBounds = new Rect(0,   0,   200, 200);
		var innerBounds = new Rect(150, 150, 100, 100);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                   result.Figures!.Count);
		Assert.Equal(new Point(0,   0),   result.Figures[0].StartPoint);
		Assert.Equal(new Point(150, 150), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_NonOverlappingRects_When_CreateOuterWithHole_Then_ReturnsGeometry()
	{
		var outerBounds = new Rect(0,   0,   100, 100);
		var innerBounds = new Rect(200, 200, 50,  50);

		var result = _sut.CreateOuterWithHole(outerBounds, innerBounds);

		Assert.NotNull(result);
		Assert.Equal(2,                   result.Figures!.Count);
		Assert.Equal(new Point(0,   0),   result.Figures[0].StartPoint);
		Assert.Equal(new Point(200, 200), result.Figures[1].StartPoint);
	}

	[Fact]
	public void Given_IdenticalRects_When_CreateOuterWithHole_Then_ReturnsGeometryWithSameBounds()
	{
		var bounds = new Rect(10, 20, 100, 150);

		var result = _sut.CreateOuterWithHole(bounds, bounds);

		Assert.NotNull(result);
		Assert.Equal(2,                 result.Figures!.Count);
		Assert.Equal(new Point(10, 20), result.Figures[0].StartPoint);
		Assert.Equal(new Point(10, 20), result.Figures[1].StartPoint);
	}

	#endregion
}
