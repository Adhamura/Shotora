using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Shotora.App.Models;
using Shotora.App.Services.DrawingServices;
using Point=Avalonia.Point;
using PointF=System.Drawing.PointF;
using RectangleF=System.Drawing.RectangleF;

namespace Shotora.App.Tests.DrawingServices;

public class GridDrawingServiceTests
{
	private readonly GridDrawingService _sut = new();
	static GridDrawingServiceTests()
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

	#region ApplyTranslation Tests

	[Fact]
	public void Given_GridWithLineAndPolygon_When_ApplyTranslation_Then_TranslatesBoth()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 15, 25, null);

		Assert.Equal(25,  line.StartPoint.X);
		Assert.Equal(45,  line.StartPoint.Y);
		Assert.Equal(125, line.EndPoint.X);
		Assert.Equal(145, line.EndPoint.Y);
		Assert.Equal(25f, annotation.Bounds.X);
		Assert.Equal(45f, annotation.Bounds.Y);
		Assert.Equal(25f, annotation.Points[0].X);
		Assert.Equal(45f, annotation.Points[0].Y);
	}

	[Fact]
	public void Given_GridWithLineAndPolygon_When_ApplyTranslation_Then_UpdatesArrowHead()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 5, 5, null);

		Assert.Equal(3,   polygon.Points.Count);
		Assert.Equal(115, polygon.Points[0].X);
		Assert.Equal(125, polygon.Points[0].Y);
	}

	[Fact]
	public void Given_GridWithArrowParts_When_ApplyTranslation_Then_UsesProvidedParts()
	{
		var grid = new Grid();
		var line1 = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		var line2 = new Line
		{
			StartPoint = new Point(200, 200),
			EndPoint   = new Point(300, 300)
		};
		var polygon = new Polygon();
		grid.Children.Add(line1);
		grid.Children.Add(line2);
		grid.Children.Add(polygon);
		var arrowParts = new List<Control>
		{
			line1,
			polygon
		};
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 10, 20, arrowParts);

		Assert.Equal(20,  line1.StartPoint.X);
		Assert.Equal(40,  line1.StartPoint.Y);
		Assert.Equal(200, line2.StartPoint.X);
		Assert.Equal(200, line2.StartPoint.Y);
	}

	[Fact]
	public void Given_GridWithoutParts_When_ApplyTranslation_Then_TranslatesGridPosition()
	{
		var grid = new Grid();
		Canvas.SetLeft(grid, 50);
		Canvas.SetTop(grid, 60);
		var annotation = new AnnotationItem
		{
			Points = [],
			Bounds = new RectangleF(50, 60, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 15, 25, null);

		Assert.Equal(65,  Canvas.GetLeft(grid));
		Assert.Equal(85,  Canvas.GetTop(grid));
		Assert.Equal(65f, annotation.Bounds.X);
		Assert.Equal(85f, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_GridWithNaNPosition_When_ApplyTranslation_Then_TreatsAsZero()
	{
		var grid = new Grid();

		var annotation = new AnnotationItem
		{
			Points = [],
			Bounds = new RectangleF(0, 0, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 30, 40, null);

		Assert.Equal(30,  Canvas.GetLeft(grid));
		Assert.Equal(40,  Canvas.GetTop(grid));
		Assert.Equal(30f, annotation.Bounds.X);
		Assert.Equal(40f, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_AnnotationWithTwoPoints_When_ApplyTranslation_Then_UpdatesPoints()
	{
		var grid = new Grid();
		var annotation = new AnnotationItem
		{
			Points = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 5, 10, null);

		Assert.Equal(15f,  annotation.Points[0].X);
		Assert.Equal(30f,  annotation.Points[0].Y);
		Assert.Equal(115f, annotation.Points[1].X);
		Assert.Equal(130f, annotation.Points[1].Y);
	}

	[Fact]
	public void Given_AnnotationWithSinglePoint_When_ApplyTranslation_Then_UpdatesFromLine()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		grid.Children.Add(line);
		var annotation = new AnnotationItem
		{
			Points = [new PointF(5f, 5f)],
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 15, 25, null);

		Assert.Equal(2,    annotation.Points.Count);
		Assert.Equal(25f,  annotation.Points[0].X);
		Assert.Equal(45f,  annotation.Points[0].Y);
		Assert.Equal(125f, annotation.Points[1].X);
		Assert.Equal(145f, annotation.Points[1].Y);
	}

	[Fact]
	public void Given_AnnotationWithEmptyPoints_When_ApplyTranslation_Then_UpdatesFromLine()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(50,  60),
			EndPoint   = new Point(150, 160)
		};
		grid.Children.Add(line);
		var annotation = new AnnotationItem
		{
			Points = [],
			Bounds = new RectangleF(50, 60, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 20, 30, null);

		Assert.Equal(2,    annotation.Points.Count);
		Assert.Equal(70f,  annotation.Points[0].X);
		Assert.Equal(90f,  annotation.Points[0].Y);
		Assert.Equal(170f, annotation.Points[1].X);
		Assert.Equal(190f, annotation.Points[1].Y);
	}

	[Fact]
	public void Given_VeryShortLine_When_ApplyTranslation_Then_DoesNotUpdateArrowHead()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,    20),
			EndPoint   = new Point(10.05, 20.05)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(10.05f, 20.05f)],
			Bounds        = new RectangleF(10, 20, 0.05f, 0.05f),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 5, 5, null);

		Assert.Empty(polygon.Points);
	}

	[Fact]
	public void Given_ZeroArrowHeadSize_When_ApplyTranslation_Then_UsesDefaultSize()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			ArrowHeadSize = 0
		};

		_sut.ApplyTranslation(grid, annotation, 0, 0, null);

		Assert.Equal(3, polygon.Points.Count);
	}

	[Fact]
	public void Given_LargeArrowHeadSize_When_ApplyTranslation_Then_LimitsToSeventyFivePercent()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(0,   0),
			EndPoint   = new Point(100, 0)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(0f, 0f), new PointF(100f, 0f)],
			Bounds        = new RectangleF(0, 0, 100, 0),
			ArrowHeadSize = 200
		};

		_sut.ApplyTranslation(grid, annotation, 0, 0, null);

		Assert.Equal(3, polygon.Points.Count);
	}

	[Fact]
	public void Given_NegativeOffset_When_ApplyTranslation_Then_MovesBackward()
	{
		var grid = new Grid();
		Canvas.SetLeft(grid, 100);
		Canvas.SetTop(grid, 100);
		var annotation = new AnnotationItem
		{
			Points = [],
			Bounds = new RectangleF(100, 100, 50, 50)
		};

		_sut.ApplyTranslation(grid, annotation, -30, -20, null);

		Assert.Equal(70,  Canvas.GetLeft(grid));
		Assert.Equal(80,  Canvas.GetTop(grid));
		Assert.Equal(70f, annotation.Bounds.X);
		Assert.Equal(80f, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_GridWithOnlyLine_When_ApplyTranslation_Then_TranslatesLine()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		grid.Children.Add(line);
		var annotation = new AnnotationItem
		{
			Points = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 25, 15, null);

		Assert.Equal(35,  line.StartPoint.X);
		Assert.Equal(35,  line.StartPoint.Y);
		Assert.Equal(135, line.EndPoint.X);
		Assert.Equal(135, line.EndPoint.Y);
		Assert.Equal(35f, annotation.Bounds.X);
		Assert.Equal(35f, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_GridWithOnlyPolygon_When_ApplyTranslation_Then_DoesNotUpdateArrowHead()
	{
		var grid    = new Grid();
		var polygon = new Polygon();
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 10, 20, null);

		Assert.Empty(polygon.Points);

		Assert.NotNull(annotation);
	}

	[Fact]
	public void Given_HorizontalLine_When_ApplyTranslation_Then_UpdatesArrowHeadCorrectly()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(0,   50),
			EndPoint   = new Point(100, 50)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(0f, 50f), new PointF(100f, 50f)],
			Bounds        = new RectangleF(0, 50, 100, 0),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 0, 0, null);

		Assert.Equal(3,   polygon.Points.Count);
		Assert.Equal(100, polygon.Points[0].X);
		Assert.Equal(50,  polygon.Points[0].Y);
	}

	[Fact]
	public void Given_VerticalLine_When_ApplyTranslation_Then_UpdatesArrowHeadCorrectly()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(50, 0),
			EndPoint   = new Point(50, 100)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(50f, 0f), new PointF(50f, 100f)],
			Bounds        = new RectangleF(50, 0, 0, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 0, 0, null);

		Assert.Equal(3,   polygon.Points.Count);
		Assert.Equal(50,  polygon.Points[0].X);
		Assert.Equal(100, polygon.Points[0].Y);
	}

	[Fact]
	public void Given_DiagonalLine_When_ApplyTranslation_Then_UpdatesArrowHeadCorrectly()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(0,   0),
			EndPoint   = new Point(100, 100)
		};
		var polygon = new Polygon();
		grid.Children.Add(line);
		grid.Children.Add(polygon);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(0f, 0f), new PointF(100f, 100f)],
			Bounds        = new RectangleF(0, 0, 100, 100),
			ArrowHeadSize = 20
		};

		_sut.ApplyTranslation(grid, annotation, 0, 0, null);

		Assert.Equal(3,   polygon.Points.Count);
		Assert.Equal(100, polygon.Points[0].X);
		Assert.Equal(100, polygon.Points[0].Y);
	}

	[Fact]
	public void Given_AnnotationWithThreePoints_When_ApplyTranslation_Then_OnlyUpdatesFirstTwo()
	{
		var grid = new Grid();
		var annotation = new AnnotationItem
		{
			Points = [new PointF(10f, 20f), new PointF(110f, 120f), new PointF(200f, 200f)],
			Bounds = new RectangleF(10, 20, 190, 180)
		};

		_sut.ApplyTranslation(grid, annotation, 5, 10, null);

		Assert.Equal(3,    annotation.Points.Count);
		Assert.Equal(15f,  annotation.Points[0].X);
		Assert.Equal(30f,  annotation.Points[0].Y);
		Assert.Equal(115f, annotation.Points[1].X);
		Assert.Equal(130f, annotation.Points[1].Y);
		Assert.Equal(200f, annotation.Points[2].X);
		Assert.Equal(200f, annotation.Points[2].Y);
	}

	[Fact]
	public void Given_EmptyArrowParts_When_ApplyTranslation_Then_TranslatesGridPosition()
	{
		var grid = new Grid();
		Canvas.SetLeft(grid, 50);
		Canvas.SetTop(grid, 60);
		var annotation = new AnnotationItem
		{
			Points = [],
			Bounds = new RectangleF(50, 60, 100, 100)
		};
		var emptyParts = new List<Control>();

		_sut.ApplyTranslation(grid, annotation, 20, 30, emptyParts);

		Assert.Equal(70,  Canvas.GetLeft(grid));
		Assert.Equal(90,  Canvas.GetTop(grid));
		Assert.Equal(70f, annotation.Bounds.X);
		Assert.Equal(90f, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_GridWithNonLineNonPolygonChildren_When_ApplyTranslation_Then_IgnoresThem()
	{
		var grid = new Grid();
		var line = new Line
		{
			StartPoint = new Point(10,  20),
			EndPoint   = new Point(110, 120)
		};
		var button = new Button();
		grid.Children.Add(line);
		grid.Children.Add(button);
		var annotation = new AnnotationItem
		{
			Points = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		_sut.ApplyTranslation(grid, annotation, 15, 25, null);

		Assert.Equal(25,  line.StartPoint.X);
		Assert.Equal(45,  line.StartPoint.Y);
		Assert.Equal(25f, annotation.Bounds.X);
		Assert.Equal(45f, annotation.Bounds.Y);
	}

	#endregion
}
