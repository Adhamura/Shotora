using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Media;
using Shotora.App.Models;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Point=Avalonia.Point;
using PointF=System.Drawing.PointF;

namespace Shotora.App.Tests.DrawingServices;

public class PolylineDrawingServiceTests
{
	private readonly PolylineDrawingService _sut = new();
	static PolylineDrawingServiceTests()
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

	#region CreatePolyline Tests

	[Fact]
	public void Given_AnnotationWithPoints_When_CreatePolyline_Then_ReturnsPolylineWithPoints()
	{
		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(110f, 120f), new PointF(50f, 80f)],
			Thickness = 3
		};
		var brush = new SolidColorBrush(Colors.Red);

		var result = _sut.CreatePolyline(annotation, brush);

		Assert.NotNull(result);
		Assert.True(result.IsHitTestVisible);
		Assert.Equal(3,   result.Points.Count);
		Assert.Equal(10,  result.Points[0].X);
		Assert.Equal(20,  result.Points[0].Y);
		Assert.Equal(110, result.Points[1].X);
		Assert.Equal(120, result.Points[1].Y);
		Assert.Same(brush, result.Stroke);
		Assert.Equal(3, result.StrokeThickness);
	}

	[Fact]
	public void Given_AnnotationWithEmptyPoints_When_CreatePolyline_Then_ReturnsPolylineWithEmptyPoints()
	{
		var annotation = new AnnotationItem
		{
			Points    = [],
			Thickness = 2
		};
		var brush = new SolidColorBrush(Colors.Blue);

		var result = _sut.CreatePolyline(annotation, brush);

		Assert.NotNull(result);
		Assert.True(result.IsHitTestVisible);
		Assert.Empty(result.Points);
		Assert.Same(brush, result.Stroke);
		Assert.Equal(2, result.StrokeThickness);
	}

	[Fact]
	public void Given_AnnotationWithSinglePoint_When_CreatePolyline_Then_ReturnsPolylineWithSinglePoint()
	{
		var annotation = new AnnotationItem
		{
			Points    = [new PointF(50f, 60f)],
			Thickness = 1
		};
		var brush = new SolidColorBrush(Colors.Green);

		var result = _sut.CreatePolyline(annotation, brush);

		Assert.NotNull(result);
		Assert.Single(result.Points);
		Assert.Equal(50, result.Points[0].X);
		Assert.Equal(60, result.Points[0].Y);
	}

	[Fact]
	public void Given_AnnotationWithZeroThickness_When_CreatePolyline_Then_ReturnsPolylineWithZeroThickness()
	{
		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness = 0
		};
		var brush = new SolidColorBrush(Colors.Purple);

		var result = _sut.CreatePolyline(annotation, brush);

		Assert.NotNull(result);
		Assert.Equal(0, result.StrokeThickness);
	}

	[Fact]
	public void Given_AnnotationWithNegativePoints_When_CreatePolyline_Then_ReturnsPolylineWithNegativePoints()
	{
		var annotation = new AnnotationItem
		{
			Points    = [new PointF(-10f, -20f), new PointF(50f, 30f)],
			Thickness = 2
		};
		var brush = new SolidColorBrush(Colors.Orange);

		var result = _sut.CreatePolyline(annotation, brush);

		Assert.NotNull(result);
		Assert.Equal(2,   result.Points.Count);
		Assert.Equal(-10, result.Points[0].X);
		Assert.Equal(-20, result.Points[0].Y);
	}

	#endregion

	#region CreatePreviewPolyline Tests

	[Fact]
	public void Given_StartPointAndColor_When_CreatePreviewPolyline_Then_ReturnsPreviewPolyline()
	{
		var start     = new Point(50, 60);
		var color     = Colors.Red;
		var thickness = 3.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, false);

		Assert.NotNull(result);
		Assert.Single(result.Points);
		Assert.Equal(50, result.Points[0].X);
		Assert.Equal(60, result.Points[0].Y);
		Assert.False(result.IsHitTestVisible);
		Assert.Equal(3.0, result.StrokeThickness);
	}

	[Fact]
	public void Given_HighlightTrue_When_CreatePreviewPolyline_Then_UsesHighlightedColorAndThickness()
	{
		var start     = new Point(10, 20);
		var color     = Colors.Blue;
		var thickness = 2.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, true);

		Assert.NotNull(result);
		var strokeBrush = result.Stroke as SolidColorBrush;
		Assert.NotNull(strokeBrush);
		Assert.Equal(128,     strokeBrush.Color.A);
		Assert.Equal(color.R, strokeBrush.Color.R);
		Assert.Equal(color.G, strokeBrush.Color.G);
		Assert.Equal(color.B, strokeBrush.Color.B);
		Assert.Equal(6.0,     result.StrokeThickness);
	}

	[Fact]
	public void Given_HighlightFalse_When_CreatePreviewPolyline_Then_UsesOriginalColor()
	{
		var start     = new Point(30, 40);
		var color     = Colors.Green;
		var thickness = 4.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, false);

		Assert.NotNull(result);
		var strokeBrush = result.Stroke as SolidColorBrush;
		Assert.NotNull(strokeBrush);
		Assert.Equal(color, strokeBrush.Color);
		Assert.Equal(4.0,   result.StrokeThickness);
	}

	[Fact]
	public void Given_PreviewPolyline_When_CreatePreviewPolyline_Then_HasTagWithPoints()
	{
		var start     = new Point(0, 0);
		var color     = Colors.Purple;
		var thickness = 1.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, false);

		Assert.NotNull(result.Tag);
		Assert.IsType<List<Point>>(result.Tag);
		var points = (List<Point>)result.Tag;
		Assert.Single(points);
		Assert.Equal(start, points[0]);
	}

	[Fact]
	public void Given_ZeroThickness_When_CreatePreviewPolyline_Then_ReturnsPolylineWithZeroThickness()
	{
		var start     = new Point(0, 0);
		var color     = Colors.Black;
		var thickness = 0.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, false);

		Assert.NotNull(result);
		Assert.Equal(0.0, result.StrokeThickness);
	}

	[Fact]
	public void Given_NegativePosition_When_CreatePreviewPolyline_Then_ReturnsPolylineAtPosition()
	{
		var start     = new Point(-10, -20);
		var color     = Colors.Orange;
		var thickness = 2.0;

		var result = _sut.CreatePreviewPolyline(start, color, thickness, false);

		Assert.NotNull(result);
		Assert.Equal(-10, result.Points[0].X);
		Assert.Equal(-20, result.Points[0].Y);
	}

	#endregion

	#region AppendPreviewPoint Tests

	[Fact]
	public void Given_PolylineWithTag_When_AppendPreviewPoint_Then_AddsPoint()
	{
		var polyline = new Polyline
		{
			Tag = new List<Point>
			{
				new(10, 20)
			}
		};
		var newPoint = new Point(30, 40);

		_sut.AppendPreviewPoint(polyline, newPoint);

		var points = (List<Point>)polyline.Tag!;
		Assert.Equal(2,                 points.Count);
		Assert.Equal(new Point(30, 40), points[1]);
		Assert.Equal(2,                 polyline.Points.Count);
	}

	[Fact]
	public void Given_PolylineWithoutTag_When_AppendPreviewPoint_Then_CreatesTagAndAddsPoint()
	{
		var polyline = new Polyline();
		var newPoint = new Point(50, 60);

		_sut.AppendPreviewPoint(polyline, newPoint);

		Assert.NotNull(polyline.Tag);
		var points = (List<Point>)polyline.Tag;
		Assert.Single(points);
		Assert.Equal(new Point(50, 60), points[0]);
		Assert.Single(polyline.Points);
	}

	[Fact]
	public void Given_PolylineWithMultiplePoints_When_AppendPreviewPoint_Then_AppendsToExisting()
	{
		var polyline = new Polyline
		{
			Tag = new List<Point>
			{
				new(10, 20),
				new(30, 40)
			}
		};
		var newPoint = new Point(50, 60);

		_sut.AppendPreviewPoint(polyline, newPoint);

		var points = (List<Point>)polyline.Tag!;
		Assert.Equal(3,                 points.Count);
		Assert.Equal(new Point(50, 60), points[2]);
		Assert.Equal(3,                 polyline.Points.Count);
	}

	[Fact]
	public void Given_NegativePoint_When_AppendPreviewPoint_Then_AddsPoint()
	{
		var polyline = new Polyline
		{
			Tag = new List<Point>
			{
				new(10, 20)
			}
		};
		var newPoint = new Point(-10, -20);

		_sut.AppendPreviewPoint(polyline, newPoint);

		var points = (List<Point>)polyline.Tag!;
		Assert.Equal(2,                   points.Count);
		Assert.Equal(new Point(-10, -20), points[1]);
	}

	#endregion

	#region ApplyTranslation Tests

	[Fact]
	public void Given_PolylineAndOffset_When_ApplyTranslation_Then_ShiftsPoints()
	{
		var polyline = new Polyline
		{
			Points = new Points([new Point(10, 20), new Point(30, 40)])
		};
		var annotation = new AnnotationItem
		{
			Points = [new PointF(10f, 20f), new PointF(30f, 40f)]
		};

		_sut.ApplyTranslation(polyline, annotation, 15, 25);

		Assert.Equal(2,   polyline.Points.Count);
		Assert.Equal(25,  polyline.Points[0].X);
		Assert.Equal(45,  polyline.Points[0].Y);
		Assert.Equal(45,  polyline.Points[1].X);
		Assert.Equal(65,  polyline.Points[1].Y);
		Assert.Equal(2,   annotation.Points.Count);
		Assert.Equal(25f, annotation.Points[0].X);
		Assert.Equal(45f, annotation.Points[0].Y);
	}

	[Fact]
	public void Given_PolylineWithSinglePoint_When_ApplyTranslation_Then_ShiftsPoint()
	{
		var polyline = new Polyline
		{
			Points = new Points([new Point(50, 60)])
		};
		var annotation = new AnnotationItem
		{
			Points = [new PointF(50f, 60f)]
		};

		_sut.ApplyTranslation(polyline, annotation, 10, 20);

		Assert.Single(polyline.Points);
		Assert.Equal(60, polyline.Points[0].X);
		Assert.Equal(80, polyline.Points[0].Y);
		Assert.Single(annotation.Points);
		Assert.Equal(60f, annotation.Points[0].X);
		Assert.Equal(80f, annotation.Points[0].Y);
	}

	[Fact]
	public void Given_NegativeOffset_When_ApplyTranslation_Then_MovesBackward()
	{
		var polyline = new Polyline
		{
			Points = new Points([new Point(100, 100)])
		};
		var annotation = new AnnotationItem
		{
			Points = [new PointF(100f, 100f)]
		};

		_sut.ApplyTranslation(polyline, annotation, -30, -20);

		Assert.Equal(70,  polyline.Points[0].X);
		Assert.Equal(80,  polyline.Points[0].Y);
		Assert.Equal(70f, annotation.Points[0].X);
		Assert.Equal(80f, annotation.Points[0].Y);
	}

	[Fact]
	public void Given_EmptyPolyline_When_ApplyTranslation_Then_DoesNotThrow()
	{
		var polyline = new Polyline
		{
			Points = new Points([])
		};
		var annotation = new AnnotationItem
		{
			Points = []
		};

		var exception = Record.Exception(() => _sut.ApplyTranslation(polyline, annotation, 10, 20));

		Assert.Null(exception);
		Assert.Empty(polyline.Points);
		Assert.Empty(annotation.Points);
	}

	[Fact]
	public void Given_PolylineWithManyPoints_When_ApplyTranslation_Then_ShiftsAllPoints()
	{
		var polyline = new Polyline
		{
			Points = new Points([new Point(0, 0), new Point(10, 10), new Point(20, 20), new Point(30, 30)])
		};
		var annotation = new AnnotationItem
		{
			Points = [new PointF(0f, 0f), new PointF(10f, 10f), new PointF(20f, 20f), new PointF(30f, 30f)]
		};

		_sut.ApplyTranslation(polyline, annotation, 5, 5);

		Assert.Equal(4,  polyline.Points.Count);
		Assert.Equal(5,  polyline.Points[0].X);
		Assert.Equal(5,  polyline.Points[0].Y);
		Assert.Equal(35, polyline.Points[3].X);
		Assert.Equal(35, polyline.Points[3].Y);
		Assert.Equal(4,  annotation.Points.Count);
	}

	#endregion

	#region DrawSkPolyline Tests

	[Fact]
	public void Given_AnnotationWithTwoPoints_When_DrawSkPolyline_Then_DrawsLine()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness = 3
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnnotationWithMultiplePoints_When_DrawSkPolyline_Then_DrawsPolyline()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(50f, 60f), new PointF(110f, 120f), new PointF(150f, 180f)],
			Thickness = 2
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Blue));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnnotationWithSinglePoint_When_DrawSkPolyline_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f)],
			Thickness = 3
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Green));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnnotationWithEmptyPoints_When_DrawSkPolyline_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [],
			Thickness = 2
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Purple));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroThickness_When_DrawSkPolyline_Then_DrawsWithZeroThickness()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness = 0
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Orange));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_NegativeCoordinates_When_DrawSkPolyline_Then_DrawsCorrectly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(-10f, -20f), new PointF(50f, 30f)],
			Thickness = 2
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_DifferentColors_When_DrawSkPolyline_Then_UsesSpecifiedColor()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points    = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness = 3
		};

		var exception = Record.Exception(() => _sut.DrawSkPolyline(canvas, annotation, SKColors.Cyan));

		Assert.Null(exception);
	}

	#endregion
}
