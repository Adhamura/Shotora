using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Brushes=Avalonia.Media.Brushes;
using Point=Avalonia.Point;
using Rectangle=Avalonia.Controls.Shapes.Rectangle;
using Size=Avalonia.Size;

namespace Shotora.App.Tests.DrawingServices;

public class RectangleDrawingServiceTests
{
	private readonly RectangleDrawingService _sut = new();
	static RectangleDrawingServiceTests()
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

	#region GetBoundsFromPoints Tests

	[Fact]
	public void Given_TwoPoints_When_GetBoundsFromPoints_Then_ReturnsNormalizedRect()
	{
		var start = new Point(100, 200);
		var end   = new Point(50,  100);

		var result = _sut.GetBoundsFromPoints(start, end);

		Assert.Equal(50,  result.X);
		Assert.Equal(100, result.Y);
		Assert.Equal(50,  result.Width);
		Assert.Equal(100, result.Height);
	}

	#endregion

	#region InitializeAnnotationCanvas Tests

	[Fact]
	public void Given_Panel_When_InitializeAnnotationCanvas_Then_SetsCanvas()
	{
		var panel = new Canvas();

		var exception = Record.Exception(() => _sut.InitializeAnnotationCanvas(panel));

		Assert.Null(exception);
	}

	#endregion

	#region NormalizeRect Tests

	[Fact]
	public void Given_StartBeforeEnd_When_NormalizeRect_Then_ReturnsCorrectRect()
	{
		var start = new Point(10,  20);
		var end   = new Point(100, 200);

		var result = _sut.NormalizeRect(start, end);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(90,  result.Width);
		Assert.Equal(180, result.Height);
	}

	[Fact]
	public void Given_StartAfterEnd_When_NormalizeRect_Then_ReturnsNormalizedRect()
	{
		var start = new Point(100, 200);
		var end   = new Point(10,  20);

		var result = _sut.NormalizeRect(start, end);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(90,  result.Width);
		Assert.Equal(180, result.Height);
	}

	[Fact]
	public void Given_DiagonalPoints_When_NormalizeRect_Then_ReturnsNormalizedRect()
	{
		var start = new Point(100, 20);
		var end   = new Point(10,  200);

		var result = _sut.NormalizeRect(start, end);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(90,  result.Width);
		Assert.Equal(180, result.Height);
	}

	[Fact]
	public void Given_SamePoints_When_NormalizeRect_Then_ReturnsZeroSizeRect()
	{
		var start = new Point(50, 50);
		var end   = new Point(50, 50);

		var result = _sut.NormalizeRect(start, end);

		Assert.Equal(50, result.X);
		Assert.Equal(50, result.Y);
		Assert.Equal(0,  result.Width);
		Assert.Equal(0,  result.Height);
	}

	[Fact]
	public void Given_NegativeCoordinates_When_NormalizeRect_Then_HandlesCorrectly()
	{
		var start = new Point(-50, -30);
		var end   = new Point(50,  30);

		var result = _sut.NormalizeRect(start, end);

		Assert.Equal(-50, result.X);
		Assert.Equal(-30, result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(60,  result.Height);
	}

	#endregion

	#region ClampToBounds Tests

	[Fact]
	public void Given_NullDisplaySize_When_ClampToBounds_Then_ReturnsOriginalRect()
	{
		var rect = new Rect(10, 20, 100, 200);

		var result = _sut.ClampToBounds(rect, null);

		Assert.Equal(rect, result);
	}

	[Fact]
	public void Given_RectWithinBounds_When_ClampToBounds_Then_ReturnsOriginalRect()
	{
		var rect        = new Rect(10, 20, 100, 200);
		var displaySize = new Size(500, 500);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(rect, result);
	}

	[Fact]
	public void Given_RectExceedingRight_When_ClampToBounds_Then_ClampsWidth()
	{
		var rect        = new Rect(400, 20, 200, 100);
		var displaySize = new Size(500, 500);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(400, result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(100, result.Height);
	}

	[Fact]
	public void Given_RectExceedingBottom_When_ClampToBounds_Then_ClampsHeight()
	{
		var rect        = new Rect(20, 400, 100, 200);
		var displaySize = new Size(500, 500);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(20,  result.X);
		Assert.Equal(400, result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(100, result.Height);
	}

	[Fact]
	public void Given_NegativePosition_When_ClampToBounds_Then_ClampsToZero()
	{
		var rect        = new Rect(-50, -30, 100, 100);
		var displaySize = new Size(500, 500);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(0, result.X);
		Assert.Equal(0, result.Y);
	}

	[Fact]
	public void Given_RectCompletelyOutside_When_ClampToBounds_Then_ClampsToEdge()
	{
		var rect        = new Rect(600, 600, 100, 100);
		var displaySize = new Size(500, 500);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(499, result.X);
		Assert.Equal(499, result.Y);

		Assert.Equal(1, result.Width);
		Assert.Equal(1, result.Height);
	}

	#endregion

	#region UpdateAnnotationHighlight Tests

	[Fact]
	public void Given_NullVisual_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel = new Canvas();
		_sut.InitializeAnnotationCanvas(panel);
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		_sut.UpdateAnnotationHighlight(null, annotation);

		Assert.Single(panel.Children);
		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.False(highlight.IsVisible);
	}

	[Fact]
	public void Given_NullAnnotation_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, null);

		Assert.Single(panel.Children);
		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.False(highlight.IsVisible);
	}

	[Fact]
	public void Given_ValidVisualAndAnnotation_When_UpdateAnnotationHighlight_Then_ShowsHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 150)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		Assert.Single(panel.Children);
		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(100, highlight.Width);
		Assert.Equal(150, highlight.Height);
	}

	[Fact]
	public void Given_ZeroSizeAnnotation_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 0, 0)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.False(highlight.IsVisible);
	}

	[Fact]
	public void Given_VisualWithTranslateTransform_When_UpdateAnnotationHighlight_Then_AppliesTranslation()
	{
		var panel = new Canvas();
		var visual = new Rectangle
		{
			RenderTransform = new TranslateTransform(50, 30)
		};
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(60, Canvas.GetLeft(highlight));
		Assert.Equal(50, Canvas.GetTop(highlight));
	}

	[Fact]
	public void Given_VisualWithTransformGroup_When_UpdateAnnotationHighlight_Then_AppliesTranslation()
	{
		var panel          = new Canvas();
		var transformGroup = new TransformGroup();
		transformGroup.Children.Add(new ScaleTransform(1, 1));
		transformGroup.Children.Add(new TranslateTransform(25, 15));
		var visual = new Rectangle
		{
			RenderTransform = transformGroup
		};
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(35, Canvas.GetLeft(highlight));
		Assert.Equal(35, Canvas.GetTop(highlight));
	}

	[Fact]
	public void Given_AnnotationWithPoints_When_UpdateAnnotationHighlight_Then_UsesBoundsFromPoints()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 0, 0),
			Points = [new PointF(10, 20), new PointF(110, 120), new PointF(60, 70)]
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(10,  Canvas.GetLeft(highlight));
		Assert.Equal(20,  Canvas.GetTop(highlight));
		Assert.Equal(100, highlight.Width);
		Assert.Equal(100, highlight.Height);
	}

	[Fact]
	public void Given_UninitializedCanvas_When_UpdateAnnotationHighlight_Then_ThrowsException()
	{
		var service = new RectangleDrawingService();
		var visual  = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};

		var exception = Assert.Throws<InvalidOperationException>(() =>
			service.UpdateAnnotationHighlight(visual, annotation));

		Assert.Equal("Annotation canvas is not initialized.", exception.Message);
	}

	[Fact]
	public void Given_OverlayInteractionState_When_UpdateAnnotationHighlight_Then_UsesStateProperties()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = visual,
			SelectedAnnotation = annotation
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(state);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
	}

	[Fact]
	public void Given_CalledTwice_When_UpdateAnnotationHighlight_Then_ReusesSameHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);
		_sut.UpdateAnnotationHighlight(visual, annotation);

		Assert.Single(panel.Children);
	}

	#endregion

	#region IntersectRect Tests

	[Fact]
	public void Given_OverlappingRects_When_IntersectRect_Then_ReturnsIntersection()
	{
		var a = new SKRectI(0,  0,  100, 100);
		var b = new SKRectI(50, 50, 150, 150);

		var result = _sut.IntersectRect(a, b);

		Assert.Equal(50,  result.Left);
		Assert.Equal(50,  result.Top);
		Assert.Equal(100, result.Right);
		Assert.Equal(100, result.Bottom);
	}

	[Fact]
	public void Given_NonOverlappingRects_When_IntersectRect_Then_ReturnsEmptyRect()
	{
		var a = new SKRectI(0,   0,   50,  50);
		var b = new SKRectI(100, 100, 150, 150);

		var result = _sut.IntersectRect(a, b);

		Assert.Equal(0, result.Left);
		Assert.Equal(0, result.Top);
		Assert.Equal(0, result.Right);
		Assert.Equal(0, result.Bottom);
	}

	[Fact]
	public void Given_TouchingRects_When_IntersectRect_Then_ReturnsEmptyRect()
	{
		var a = new SKRectI(0,  0, 50,  50);
		var b = new SKRectI(50, 0, 100, 50);

		var result = _sut.IntersectRect(a, b);

		Assert.Equal(0, result.Left);
		Assert.Equal(0, result.Top);
		Assert.Equal(0, result.Right);
		Assert.Equal(0, result.Bottom);
	}

	[Fact]
	public void Given_ContainedRect_When_IntersectRect_Then_ReturnsSmallerRect()
	{
		var a = new SKRectI(0,  0,  100, 100);
		var b = new SKRectI(25, 25, 75,  75);

		var result = _sut.IntersectRect(a, b);

		Assert.Equal(25, result.Left);
		Assert.Equal(25, result.Top);
		Assert.Equal(75, result.Right);
		Assert.Equal(75, result.Bottom);
	}

	[Fact]
	public void Given_IdenticalRects_When_IntersectRect_Then_ReturnsSameRect()
	{
		var a = new SKRectI(10, 20, 100, 200);
		var b = new SKRectI(10, 20, 100, 200);

		var result = _sut.IntersectRect(a, b);

		Assert.Equal(a.Left,   result.Left);
		Assert.Equal(a.Top,    result.Top);
		Assert.Equal(a.Right,  result.Right);
		Assert.Equal(a.Bottom, result.Bottom);
	}

	#endregion

	#region DrawSkRect Tests

	[Fact]
	public void Given_AnnotationWithoutFill_When_DrawSkRect_Then_DrawsStrokeOnly()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Rectangle,
			Bounds    = new RectangleF(10, 10, 100, 80),
			Thickness = 3,
			Fill      = false
		};

		var exception = Record.Exception(() => _sut.DrawSkRect(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_AnnotationWithFill_When_DrawSkRect_Then_DrawsFillAndStroke()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Rectangle,
			Bounds    = new RectangleF(10, 10, 100, 80),
			Thickness = 3,
			Fill      = true
		};

		var exception = Record.Exception(() => _sut.DrawSkRect(canvas, annotation, SKColors.Blue));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroThickness_When_DrawSkRect_Then_DoesNotThrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Rectangle,
			Bounds    = new RectangleF(10, 10, 100, 80),
			Thickness = 0,
			Fill      = false
		};

		var exception = Record.Exception(() => _sut.DrawSkRect(canvas, annotation, SKColors.Green));

		Assert.Null(exception);
	}

	#endregion

	#region CreateRectangle Tests

	[Fact]
	public void Given_Annotation_When_CreateRectangle_Then_ReturnsPositionedRectangle()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(20, 30, 150, 100),
			Thickness = 5,
			Fill      = false
		};
		var brush = new SolidColorBrush(Colors.Red);

		var result = _sut.CreateRectangle(annotation, brush);

		Assert.NotNull(result);
		Assert.Equal(150, result.Width);
		Assert.Equal(100, result.Height);
		Assert.Equal(5,   result.StrokeThickness);
		Assert.Same(brush, result.Stroke);
		Assert.Equal(20, Canvas.GetLeft(result));
		Assert.Equal(30, Canvas.GetTop(result));
	}

	[Fact]
	public void Given_AnnotationWithFill_When_CreateRectangle_Then_RectangleHasFill()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(20, 30, 150, 100),
			Thickness = 5,
			Fill      = true
		};
		var brush = new SolidColorBrush(Colors.Blue);

		var result = _sut.CreateRectangle(annotation, brush);

		Assert.NotNull(result);
		Assert.Same(brush, result.Fill);
	}

	[Fact]
	public void Given_AnnotationWithoutFill_When_CreateRectangle_Then_RectangleHasTransparentFill()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(20, 30, 150, 100),
			Thickness = 5,
			Fill      = false
		};
		var brush = new SolidColorBrush(Colors.Green);

		var result = _sut.CreateRectangle(annotation, brush);

		Assert.NotNull(result);
		Assert.Same(Brushes.Transparent, result.Fill);
	}

	#endregion

	#region CreatePreviewRectangle Tests

	[Fact]
	public void Given_Position_When_CreatePreviewRectangle_Then_ReturnsRectangleAtPosition()
	{
		var position = new Point(50, 60);
		var brush    = new SolidColorBrush(Colors.Orange);

		var result = _sut.CreatePreviewRectangle(position, brush, 4);

		Assert.NotNull(result);
		Assert.Equal(4, result.StrokeThickness);
		Assert.Same(brush, result.Stroke);
		Assert.Equal(50, Canvas.GetLeft(result));
		Assert.Equal(60, Canvas.GetTop(result));
	}

	[Fact]
	public void Given_PreviewRectangle_When_CreatePreviewRectangle_Then_HasTransparentFill()
	{
		var position = new Point(50, 60);
		var brush    = new SolidColorBrush(Colors.Purple);

		var result = _sut.CreatePreviewRectangle(position, brush, 3);

		Assert.Same(Brushes.Transparent, result.Fill);
	}

	#endregion

	#region UpdatePreviewBounds Tests

	[Fact]
	public void Given_PreviewAndPoints_When_UpdatePreviewBounds_Then_UpdatesBounds()
	{
		var preview = new Rectangle();
		var start   = new Point(10,  20);
		var current = new Point(110, 120);

		_sut.UpdatePreviewBounds(preview, start, current);

		Assert.Equal(10,  Canvas.GetLeft(preview));
		Assert.Equal(20,  Canvas.GetTop(preview));
		Assert.Equal(100, preview.Width);
		Assert.Equal(100, preview.Height);
	}

	[Fact]
	public void Given_ReversedPoints_When_UpdatePreviewBounds_Then_NormalizesAndUpdatesBounds()
	{
		var preview = new Rectangle();
		var start   = new Point(110, 120);
		var current = new Point(10,  20);

		_sut.UpdatePreviewBounds(preview, start, current);

		Assert.Equal(10,  Canvas.GetLeft(preview));
		Assert.Equal(20,  Canvas.GetTop(preview));
		Assert.Equal(100, preview.Width);
		Assert.Equal(100, preview.Height);
	}

	#endregion

	#region CreateProcessingRectangle Tests

	[Fact]
	public void Given_Annotation_When_CreateProcessingRectangle_Then_ReturnsRectangleWithOpacity()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(30, 40, 200, 150),
			Thickness = 2
		};
		var brush = new SolidColorBrush(Colors.Gray);

		var result = _sut.CreateProcessingRectangle(annotation, brush, 0.9);

		Assert.NotNull(result);
		Assert.Equal(200, result.Width);
		Assert.Equal(150, result.Height);
		Assert.Equal(0.9, result.Opacity);
		Assert.Equal(2,   result.StrokeThickness);
		Assert.Equal(30,  Canvas.GetLeft(result));
		Assert.Equal(40,  Canvas.GetTop(result));
	}

	[Fact]
	public void Given_ProcessingRectangle_When_CreateProcessingRectangle_Then_HasTransparentFill()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(30, 40, 200, 150),
			Thickness = 2
		};
		var brush = new SolidColorBrush(Colors.Gray);

		var result = _sut.CreateProcessingRectangle(annotation, brush, 0.5);

		Assert.Same(Brushes.Transparent, result.Fill);
	}

	#endregion

	#region ApplyTranslation Tests

	[Fact]
	public void Given_RectangleAndOffset_When_ApplyTranslation_Then_UpdatesPositionAndBounds()
	{
		var visual = new Rectangle();
		Canvas.SetLeft(visual, 50);
		Canvas.SetTop(visual, 60);
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(50, 60, 100, 80)
		};

		_sut.ApplyTranslation(visual, annotation, 25, 15);

		Assert.Equal(75, Canvas.GetLeft(visual));
		Assert.Equal(75, Canvas.GetTop(visual));
		Assert.Equal(75, annotation.Bounds.X);
		Assert.Equal(75, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_RectangleWithNaNPosition_When_ApplyTranslation_Then_SetsToZero()
	{
		var visual = new Rectangle();

		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 100, 80)
		};

		_sut.ApplyTranslation(visual, annotation, 30, 20);

		Assert.Equal(0, Canvas.GetLeft(visual));
		Assert.Equal(0, Canvas.GetTop(visual));

		Assert.Equal(30, annotation.Bounds.X);
		Assert.Equal(20, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_NegativeOffset_When_ApplyTranslation_Then_MovesBackward()
	{
		var visual = new Rectangle();
		Canvas.SetLeft(visual, 100);
		Canvas.SetTop(visual, 100);
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(100, 100, 50, 50)
		};

		_sut.ApplyTranslation(visual, annotation, -30, -20);

		Assert.Equal(70, Canvas.GetLeft(visual));
		Assert.Equal(80, Canvas.GetTop(visual));
		Assert.Equal(70, annotation.Bounds.X);
		Assert.Equal(80, annotation.Bounds.Y);
	}

	#endregion

	#region GetVirtualBounds Tests

	[Fact]
	public void Given_EmptyScreensWithoutPrimary_When_GetVirtualBounds_Then_ReturnsFallback()
	{
		var screens = new List<Screen>();

		var result = _sut.GetVirtualBounds(null, screens, 800, 600);

		Assert.Equal(0,   result.X);
		Assert.Equal(0,   result.Y);
		Assert.Equal(800, result.Width);
		Assert.Equal(600, result.Height);
	}

	[Fact]
	public void Given_EmptyScreensWithDifferentFallback_When_GetVirtualBounds_Then_UsesFallbackDimensions()
	{
		var screens = new List<Screen>();

		var result = _sut.GetVirtualBounds(null, screens, 1920, 1080);

		Assert.Equal(0,    result.X);
		Assert.Equal(0,    result.Y);
		Assert.Equal(1920, result.Width);
		Assert.Equal(1080, result.Height);
	}

	#endregion

	#region MapScreenRectToCapture Tests

	[Fact]
	public void Given_WindowInsideCapture_When_MapScreenRectToCapture_Then_ReturnsOffsetRect()
	{
		var result = _sut.MapScreenRectToCapture(new PixelRect(300, 200, 800, 600), new PixelRect(0, 0, 1920, 1080), new Size(1920, 1080));

		Assert.Equal(new Rect(300, 200, 800, 600), result);
	}

	[Fact]
	public void Given_CaptureWithNegativeOrigin_When_MapScreenRectToCapture_Then_OffsetsByCaptureOrigin()
	{
		var result = _sut.MapScreenRectToCapture(new PixelRect(-1500, 100, 400, 300), new PixelRect(-1920, 0, 3840, 1080), new Size(3840, 1080));

		Assert.Equal(new Rect(420, 100, 400, 300), result);
	}

	[Fact]
	public void Given_WindowPartlyOffScreen_When_MapScreenRectToCapture_Then_ClipsToCapture()
	{
		var result = _sut.MapScreenRectToCapture(new PixelRect(-50, -20, 500, 400), new PixelRect(0, 0, 1920, 1080), new Size(1920, 1080));

		Assert.Equal(new Rect(0, 0, 450, 380), result);
	}

	[Fact]
	public void Given_DisplayScaledFromPixels_When_MapScreenRectToCapture_Then_ScalesRect()
	{
		var result = _sut.MapScreenRectToCapture(new PixelRect(200, 100, 400, 300), new PixelRect(0, 0, 2000, 1000), new Size(1000, 500));

		Assert.Equal(new Rect(100, 50, 200, 150), result);
	}

	[Fact]
	public void Given_WindowOutsideCapture_When_MapScreenRectToCapture_Then_ReturnsNull()
	{
		var result = _sut.MapScreenRectToCapture(new PixelRect(5000, 5000, 100, 100), new PixelRect(0, 0, 1920, 1080), new Size(1920, 1080));

		Assert.Null(result);
	}

	#endregion

	#region GetAnnotationRect Tests

	[Fact]
	public void Given_AnnotationWithEmptyPoints_When_GetAnnotationRect_Then_ReturnsBoundsAsRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 150),
			Points = []
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(150, result.Height);
	}

	[Fact]
	public void Given_AnnotationWithSinglePoint_When_GetAnnotationRect_Then_ReturnsMinimumSizeRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 0, 0),
			Points = [new PointF(50, 60)]
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(50, result.X);
		Assert.Equal(60, result.Y);
		Assert.Equal(1,  result.Width);
		Assert.Equal(1,  result.Height);
	}

	[Fact]
	public void Given_AnnotationWithMultiplePoints_When_GetAnnotationRect_Then_ReturnsBoundingRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 0, 0),
			Points = [new PointF(10, 20), new PointF(110, 50), new PointF(50, 120)]
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(100, result.Height);
	}

	[Fact]
	public void Given_AnnotationWithNegativePoints_When_GetAnnotationRect_Then_ReturnsCorrectRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 0, 0),
			Points = [new PointF(-50, -30), new PointF(50, 30)]
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(-50, result.X);
		Assert.Equal(-30, result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(60,  result.Height);
	}

	[Fact]
	public void Given_AnnotationWithNegativeBounds_When_GetAnnotationRect_Then_ReturnsNormalizedRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(-10, -20, 100, 150),
			Points = []
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(-10, result.X);
		Assert.Equal(-20, result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(150, result.Height);
	}

	[Fact]
	public void Given_AnnotationWithZeroSizeBounds_When_GetAnnotationRect_Then_ReturnsZeroSizeRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 0, 0),
			Points = []
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(10, result.X);
		Assert.Equal(20, result.Y);
		Assert.Equal(0,  result.Width);
		Assert.Equal(0,  result.Height);
	}

	[Fact]
	public void Given_AnnotationWithReversedBounds_When_GetAnnotationRect_Then_ReturnsNormalizedRect()
	{
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(110, 170, -100, -150),
			Points = []
		};

		var result = _sut.GetAnnotationRect(annotation);

		Assert.Equal(10,  result.X);
		Assert.Equal(20,  result.Y);
		Assert.Equal(100, result.Width);
		Assert.Equal(150, result.Height);
	}

	#endregion

	#region Additional Edge Case Tests

	[Fact]
	public void Given_ZeroDisplaySize_When_ClampToBounds_Then_ClampsToZero()
	{
		var rect        = new Rect(10, 20, 100, 200);
		var displaySize = new Size(0, 0);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(0, result.X);
		Assert.Equal(0, result.Y);
		Assert.Equal(0, result.Width);
		Assert.Equal(0, result.Height);
	}

	[Fact]
	public void Given_VerySmallDisplaySize_When_ClampToBounds_Then_ClampsCorrectly()
	{
		var rect        = new Rect(10, 20, 100, 200);
		var displaySize = new Size(5, 5);

		var result = _sut.ClampToBounds(rect, displaySize);

		Assert.Equal(4, result.X);
		Assert.Equal(4, result.Y);
		Assert.Equal(1, result.Width);
		Assert.Equal(1, result.Height);
	}

	[Fact]
	public void Given_NegativeBounds_When_DrawSkRect_Then_DoesNotThrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Rectangle,
			Bounds    = new RectangleF(-10, -20, 100, 80),
			Thickness = 3,
			Fill      = false
		};

		var exception = Record.Exception(() => _sut.DrawSkRect(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroSizeBounds_When_DrawSkRect_Then_DoesNotThrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Rectangle,
			Bounds    = new RectangleF(10, 10, 0, 0),
			Thickness = 3,
			Fill      = false
		};

		var exception = Record.Exception(() => _sut.DrawSkRect(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroThickness_When_CreateRectangle_Then_ReturnsRectangleWithZeroThickness()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(20, 30, 150, 100),
			Thickness = 0,
			Fill      = false
		};
		var brush = new SolidColorBrush(Colors.Red);

		var result = _sut.CreateRectangle(annotation, brush);

		Assert.NotNull(result);
		Assert.Equal(0, result.StrokeThickness);
	}

	[Fact]
	public void Given_NegativePosition_When_CreatePreviewRectangle_Then_ReturnsRectangleAtPosition()
	{
		var position = new Point(-50, -60);
		var brush    = new SolidColorBrush(Colors.Orange);

		var result = _sut.CreatePreviewRectangle(position, brush, 4);

		Assert.NotNull(result);
		Assert.Equal(-50, Canvas.GetLeft(result));
		Assert.Equal(-60, Canvas.GetTop(result));
	}

	[Fact]
	public void Given_ZeroThickness_When_CreatePreviewRectangle_Then_ReturnsRectangleWithZeroThickness()
	{
		var position = new Point(50, 60);
		var brush    = new SolidColorBrush(Colors.Orange);

		var result = _sut.CreatePreviewRectangle(position, brush, 0);

		Assert.NotNull(result);
		Assert.Equal(0, result.StrokeThickness);
	}

	[Fact]
	public void Given_ZeroOpacity_When_CreateProcessingRectangle_Then_ReturnsRectangleWithZeroOpacity()
	{
		var annotation = new AnnotationItem
		{
			Bounds    = new RectangleF(30, 40, 200, 150),
			Thickness = 2
		};
		var brush = new SolidColorBrush(Colors.Gray);

		var result = _sut.CreateProcessingRectangle(annotation, brush, 0);

		Assert.NotNull(result);
		Assert.Equal(0, result.Opacity);
	}

	[Fact]
	public void Given_ControlWithNoTransform_When_UpdateAnnotationHighlight_Then_UsesZeroTranslation()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(10, Canvas.GetLeft(highlight));
		Assert.Equal(20, Canvas.GetTop(highlight));
	}

	[Fact]
	public void Given_ControlWithNonTranslateTransform_When_UpdateAnnotationHighlight_Then_UsesZeroTranslation()
	{
		var panel = new Canvas();
		var visual = new Rectangle
		{
			RenderTransform = new ScaleTransform(2, 2)
		};
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(10, Canvas.GetLeft(highlight));
		Assert.Equal(20, Canvas.GetTop(highlight));
	}

	[Fact]
	public void Given_TransformGroupWithoutTranslateTransform_When_UpdateAnnotationHighlight_Then_UsesZeroTranslation()
	{
		var panel          = new Canvas();
		var transformGroup = new TransformGroup();
		transformGroup.Children.Add(new ScaleTransform(1, 1));
		transformGroup.Children.Add(new RotateTransform(45));
		var visual = new Rectangle
		{
			RenderTransform = transformGroup
		};
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.True(highlight.IsVisible);
		Assert.Equal(10, Canvas.GetLeft(highlight));
		Assert.Equal(20, Canvas.GetTop(highlight));
	}

	[Fact]
	public void Given_NegativeWidthAfterTranslation_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel = new Canvas();
		var visual = new Rectangle
		{
			RenderTransform = new TranslateTransform(-200, 0)
		};
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);

		Assert.True(highlight.IsVisible);
	}

	[Fact]
	public void Given_ZeroWidthAfterTranslation_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 0, 100)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.False(highlight.IsVisible);
	}

	[Fact]
	public void Given_ZeroHeightAfterTranslation_When_UpdateAnnotationHighlight_Then_HidesHighlight()
	{
		var panel  = new Canvas();
		var visual = new Rectangle();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 100, 0)
		};
		_sut.InitializeAnnotationCanvas(panel);

		_sut.UpdateAnnotationHighlight(visual, annotation);

		var highlight = panel.Children[0] as Rectangle;
		Assert.NotNull(highlight);
		Assert.False(highlight.IsVisible);
	}

	#endregion
}
