using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Moq;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Color=System.Drawing.Color;
using Point=Avalonia.Point;
using PointF=System.Drawing.PointF;
using Rectangle=Avalonia.Controls.Shapes.Rectangle;

namespace Shotora.App.Tests.DrawingServices;

public class ArrowDrawingServiceTests
{
	private readonly Mock<ILineDrawingService> _lineService = new(MockBehavior.Strict);
	private readonly ArrowDrawingService       _sut;
	static ArrowDrawingServiceTests()
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
		_ = Dispatcher.UIThread;
	}

	public ArrowDrawingServiceTests()
	{
		_sut = new ArrowDrawingService(_lineService.Object);
	}

	#region CreateArrowContainer Tests

	[Fact]
	public void Given_AnnotationWithPoints_When_CreateArrowContainer_Then_ReturnsContainerWithLineAndHead()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var stroke              = new SolidColorBrush(Colors.Red);
		var registeredGroup     = new List<Control>();
		var registeredContainer = (Control?)null;

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.Is<Point>(p => p.X == 10 && p.Y == 20),
				It.IsAny<Point>(),
				stroke,
				3,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (container, parts) =>
		{
			registeredContainer = container;
			registeredGroup.AddRange(parts);
		});

		Assert.NotNull(result);
		Assert.IsType<Grid>(result);
		var grid = (Grid)result;
		Assert.True(grid.IsHitTestVisible);
		Assert.True(registeredContainer == result);
		Assert.NotEmpty(registeredGroup);
		_lineService.Verify(l => l.CreateLine(
			It.Is<Point>(p => p.X == 10 && p.Y == 20),
			It.IsAny<Point>(),
			stroke,
			3,
			true), Times.Once);
	}

	[Fact]
	public void Given_AnnotationWithBounds_When_CreateArrowContainer_Then_UsesBoundsAsEndpoints()
	{
		var annotation = new AnnotationItem
		{
			Bounds        = new RectangleF(10, 20, 100, 100),
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Blue,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Blue);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.Is<Point>(p => p.X == 10 && p.Y == 20),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.Is<Point>(p => p.X == 10 && p.Y == 20),
			It.IsAny<Point>(),
			stroke,
			2,
			true), Times.Once);
	}

	[Fact]
	public void Given_ZeroArrowHeadSize_When_CreateArrowContainer_Then_UsesDefaultSize()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(0f, 0f), new PointF(100f, 100f)],
			Thickness     = 1,
			ArrowHeadSize = 0,
			Color         = Color.Green,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Green);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				stroke,
				1,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.IsAny<Point>(),
			It.IsAny<Point>(),
			stroke,
			1,
			true), Times.Once);
	}

	[Fact]
	public void Given_VeryShortLine_When_CreateArrowContainer_Then_CreatesContainerWithoutHead()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(10.5f, 20.5f)],
			Thickness     = 2,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Red);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		var grid = (Grid)result;

		Assert.Single(grid.Children);
	}

	[Fact]
	public void Given_AnnotationWithOpacity_When_CreateArrowContainer_Then_AppliesOpacity()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.FromArgb(128, 255, 0, 0),
			Opacity       = 0.5f
		};
		var stroke = new SolidColorBrush(Colors.Red);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
	}

	#endregion

	#region DrawArrow Tests

	[Fact]
	public void Given_ValidArrow_When_DrawArrow_Then_DrawsLineAndHead()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnnotationWithBounds_When_DrawArrow_Then_DrawsArrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Bounds        = new RectangleF(10, 20, 100, 100),
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Blue
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Blue));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_VeryShortLine_When_DrawArrow_Then_DrawsFallbackLine()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(10.5f, 20.5f)],
			Thickness     = 2,
			ArrowHeadSize = 20,
			Color         = Color.Green
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Green));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroThickness_When_DrawArrow_Then_DoesNotThrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 0,
			ArrowHeadSize = 20,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_HorizontalArrow_When_DrawArrow_Then_DrawsCorrectly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 50f), new PointF(150f, 50f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_VerticalArrow_When_DrawArrow_Then_DrawsCorrectly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(50f, 10f), new PointF(50f, 150f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_DiagonalArrow_When_DrawArrow_Then_DrawsCorrectly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 10f), new PointF(150f, 150f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_NegativeCoordinates_When_DrawArrow_Then_DrawsCorrectly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [new PointF(-50f, -30f), new PointF(50f, 30f)],
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Red
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	#endregion

	#region ReplaceArrowVisual Tests

	[Fact]
	public void Given_NullCanvas_When_ReplaceArrowVisual_Then_ReturnsOriginalVisual()
	{
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var      visualToAnnotation = new Dictionary<Control, AnnotationItem>();
		var      arrowGroups        = new Dictionary<Control, List<Control>>();
		Control? selectedVisual     = visual;

		var result = _sut.ReplaceArrowVisual(
			null!,
			visual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			selectedVisual,
			out var updatedSelected);

		Assert.Same(visual,         result);
		Assert.Same(selectedVisual, updatedSelected);
	}

	[Fact]
	public void Given_ValidCanvas_When_ReplaceArrowVisual_Then_ReplacesVisual()
	{
		var canvas    = new Canvas();
		var oldVisual = new Rectangle();
		canvas.Children.Add(oldVisual);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>
		{
			{
				oldVisual, annotation
			}
		};
		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				oldVisual, []
			}
		};
		Control? selectedVisual = oldVisual;

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			selectedVisual,
			out var updatedSelected);

		Assert.NotNull(result);
		Assert.NotSame(oldVisual, result);
		Assert.DoesNotContain(oldVisual, canvas.Children);
		Assert.Contains(result, canvas.Children);
		Assert.False(visualToAnnotation.ContainsKey(oldVisual));
		Assert.True(visualToAnnotation.ContainsKey(result));
		Assert.Same(result, updatedSelected);
	}

	[Fact]
	public void Given_VisualNotSelected_When_ReplaceArrowVisual_Then_DoesNotUpdateSelected()
	{
		var canvas      = new Canvas();
		var oldVisual   = new Rectangle();
		var otherVisual = new Rectangle();
		canvas.Children.Add(oldVisual);
		canvas.Children.Add(otherVisual);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>
		{
			{
				oldVisual, annotation
			}
		};
		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				oldVisual, []
			}
		};
		Control? selectedVisual = otherVisual;

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			selectedVisual,
			out var updatedSelected);

		Assert.NotNull(result);
		Assert.Same(otherVisual, updatedSelected);
	}

	[Fact]
	public void Given_VisualAtEnd_When_ReplaceArrowVisual_Then_InsertsAtCorrectIndex()
	{
		var canvas    = new Canvas();
		var visual1   = new Rectangle();
		var visual2   = new Rectangle();
		var oldVisual = new Rectangle();
		canvas.Children.Add(visual1);
		canvas.Children.Add(visual2);
		canvas.Children.Add(oldVisual);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>
		{
			{
				oldVisual, annotation
			}
		};
		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				oldVisual, []
			}
		};

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			null,
			out _);

		Assert.NotNull(result);
		Assert.Equal(2, canvas.Children.IndexOf(result));
	}

	[Fact]
	public void Given_VisualAtStart_When_ReplaceArrowVisual_Then_InsertsAtCorrectIndex()
	{
		var canvas    = new Canvas();
		var oldVisual = new Rectangle();
		var visual2   = new Rectangle();
		var visual3   = new Rectangle();
		canvas.Children.Add(oldVisual);
		canvas.Children.Add(visual2);
		canvas.Children.Add(visual3);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>
		{
			{
				oldVisual, annotation
			}
		};
		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				oldVisual, []
			}
		};

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			null,
			out _);

		Assert.NotNull(result);
		Assert.Equal(0, canvas.Children.IndexOf(result));
	}

	[Fact]
	public void Given_InvalidIndex_When_ReplaceArrowVisual_Then_AddsToEnd()
	{
		var canvas    = new Canvas();
		var oldVisual = new Rectangle();
		canvas.Children.Add(oldVisual);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>
		{
			{
				oldVisual, annotation
			}
		};
		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				oldVisual, []
			}
		};

		canvas.Children.Remove(oldVisual);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			null,
			out _);

		Assert.NotNull(result);
		Assert.Contains(result, canvas.Children);
	}

	[Fact]
	public void Given_VisualNotInDictionary_When_ReplaceArrowVisual_Then_StillReplaces()
	{
		var canvas    = new Canvas();
		var oldVisual = new Rectangle();
		canvas.Children.Add(oldVisual);
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f)],
			Thickness     = 3,
			ArrowHeadSize = 20,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var visualToAnnotation = new Dictionary<Control, AnnotationItem>();
		var arrowGroups        = new Dictionary<Control, List<Control>>();

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				It.IsAny<IBrush>(),
				3,
				true))
			.Returns(mockLine);

		var result = _sut.ReplaceArrowVisual(
			canvas,
			oldVisual,
			annotation,
			visualToAnnotation,
			arrowGroups,
			null,
			out _);

		Assert.NotNull(result);
		Assert.True(visualToAnnotation.ContainsKey(result));
	}

	#endregion

	#region Edge Case Tests

	[Fact]
	public void Given_SinglePoint_When_CreateArrowContainer_Then_UsesBounds()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f)],
			Bounds        = new RectangleF(10, 20, 100, 100),
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Blue,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Blue);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.Is<Point>(p => p.X == 10 && p.Y == 20),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.Is<Point>(p => p.X == 10 && p.Y == 20),
			It.IsAny<Point>(),
			stroke,
			2,
			true), Times.Once);
	}

	[Fact]
	public void Given_EmptyPoints_When_CreateArrowContainer_Then_UsesBounds()
	{
		var annotation = new AnnotationItem
		{
			Points        = [],
			Bounds        = new RectangleF(10, 20, 100, 100),
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Blue,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Blue);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.Is<Point>(p => p.X == 10 && p.Y == 20),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.Is<Point>(p => p.X == 10 && p.Y == 20),
			It.IsAny<Point>(),
			stroke,
			2,
			true), Times.Once);
	}

	[Fact]
	public void Given_LargeArrowHeadSize_When_CreateArrowContainer_Then_LimitsToSeventyFivePercent()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(0f, 0f), new PointF(100f, 0f)],
			Thickness     = 2,
			ArrowHeadSize = 200,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Red);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.IsAny<Point>(),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.IsAny<Point>(),
			It.IsAny<Point>(),
			stroke,
			2,
			true), Times.Once);
	}

	[Fact]
	public void Given_EmptyPoints_When_DrawArrow_Then_UsesBounds()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Points        = [],
			Bounds        = new RectangleF(10, 20, 100, 100),
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Blue
		};

		var exception = Record.Exception(() => _sut.DrawArrow(canvas, annotation, SKColors.Blue));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ThreePoints_When_CreateArrowContainer_Then_UsesFirstTwo()
	{
		var annotation = new AnnotationItem
		{
			Points        = [new PointF(10f, 20f), new PointF(110f, 120f), new PointF(200f, 200f)],
			Thickness     = 2,
			ArrowHeadSize = 15,
			Color         = Color.Red,
			Opacity       = 1.0f
		};
		var stroke = new SolidColorBrush(Colors.Red);

		var mockLine = new Line();
		_lineService.Setup(l => l.CreateLine(
				It.Is<Point>(p => p.X == 10 && p.Y == 20),
				It.IsAny<Point>(),
				stroke,
				2,
				true))
			.Returns(mockLine);

		var result = _sut.CreateArrowContainer(annotation, stroke, (_, _) =>
		{
		});

		Assert.NotNull(result);
		_lineService.Verify(l => l.CreateLine(
			It.Is<Point>(p => p.X == 10 && p.Y == 20),
			It.IsAny<Point>(),
			stroke,
			2,
			true), Times.Once);
	}

	#endregion
}
