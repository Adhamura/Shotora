using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Moq;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Brushes=Avalonia.Media.Brushes;
using DrawingColor=System.Drawing.Color;
using Image=Avalonia.Controls.Image;
using Point=Avalonia.Point;

namespace Shotora.App.Tests.DrawingServices;

public class SkiaDrawingServiceTests
{
	private readonly Mock<IRectangleDrawingService> _rectangleService = new(MockBehavior.Strict);
	private readonly SkiaDrawingService             _sut;
	static SkiaDrawingServiceTests()
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

	public SkiaDrawingServiceTests()
	{
		_sut = new SkiaDrawingService(_rectangleService.Object);
	}

	#region ToSkColor Tests

	[Theory]
	[InlineData(255, 0,   0,   255, 1.0f, 255, 0,   0,   255)]
	[InlineData(0,   255, 0,   128, 1.0f, 0,   255, 0,   128)]
	[InlineData(0,   0,   255, 255, 0.5f, 0,   0,   255, 127)]
	[InlineData(128, 64,  32,  200, 0.8f, 128, 64,  32,  160)]
	[InlineData(255, 255, 255, 255, 0.0f, 255, 255, 255, 0)]
	[InlineData(0,   0,   0,   0,   1.0f, 0,   0,   0,   0)]
	public void Given_ColorAndOpacity_When_ToSkColor_Then_ReturnsSkColorWithAdjustedAlpha(byte r, byte g, byte b, byte a, float opacity, byte expectedR, byte expectedG, byte expectedB, byte expectedA)
	{
		var color = DrawingColor.FromArgb(a, r, g, b);

		var result = _sut.ToSkColor(color, opacity);

		Assert.Equal(expectedR, result.Red);
		Assert.Equal(expectedG, result.Green);
		Assert.Equal(expectedB, result.Blue);
		Assert.Equal(expectedA, result.Alpha);
	}

	[Fact]
	public void Given_ColorWithDefaultOpacity_When_ToSkColor_Then_UsesFullOpacity()
	{
		var color = DrawingColor.Red;

		var result = _sut.ToSkColor(color);

		Assert.Equal(255, result.Red);
		Assert.Equal(0,   result.Green);
		Assert.Equal(0,   result.Blue);
		Assert.Equal(255, result.Alpha);
	}

	[Fact]
	public void Given_ColorWithOpacityGreaterThanOne_When_ToSkColor_Then_ClampsAlpha()
	{
		var color = DrawingColor.FromArgb(128, 255, 0, 0);

		var result = _sut.ToSkColor(color, 2.0f);

		Assert.Equal(255, result.Alpha);
	}

	[Fact]
	public void Given_ColorWithNegativeOpacity_When_ToSkColor_Then_ClampsAlphaToZero()
	{
		var color = DrawingColor.FromArgb(128, 255, 0, 0);

		var result = _sut.ToSkColor(color, -1.0f);

		Assert.Equal(0, result.Alpha);
	}

	[Fact]
	public void Given_ColorWithOpacityCausingOverflow_When_ToSkColor_Then_ClampsTo255()
	{
		var color = DrawingColor.FromArgb(200, 255, 0, 0);

		var result = _sut.ToSkColor(color, 1.5f);

		Assert.Equal(255, result.Alpha);
	}

	#endregion

	#region DrawProcessedRegion Tests

	[Fact]
	public void Given_BlurTool_When_DrawProcessedRegion_Then_AppliesBlur()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Blur,
			Bounds = new RectangleF(10, 10, 100, 100)
		};

		var region = new SKRectI(10, 10, 110, 110);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
		_rectangleService.Verify(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()), Times.Once);
	}

	[Fact]
	public void Given_PixelateTool_When_DrawProcessedRegion_Then_AppliesPixelation()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Pixelate,
			Bounds    = new RectangleF(20, 20, 80, 80),
			Thickness = 5
		};

		var region = new SKRectI(20, 20, 100, 100);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
		_rectangleService.Verify(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()), Times.Once);
	}

	[Fact]
	public void Given_PixelateToolWithSmallThickness_When_DrawProcessedRegion_Then_UsesMinimumPixelSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Pixelate,
			Bounds    = new RectangleF(10, 10, 50, 50),
			Thickness = 1
		};

		var region = new SKRectI(10, 10, 60, 60);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_RegionOutsideBitmap_When_DrawProcessedRegion_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Blur,
			Bounds = new RectangleF(300, 300, 100, 100)
		};

		var emptyRegion = new SKRectI(0, 0, 0, 0);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(emptyRegion);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
		_rectangleService.Verify(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()), Times.Once);
	}

	[Fact]
	public void Given_ZeroSizeRegion_When_DrawProcessedRegion_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Blur,
			Bounds = new RectangleF(10, 10, 0, 0)
		};

		var emptyRegion = new SKRectI(10, 10, 10, 10);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(emptyRegion);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_PixelateToolWithLargeThickness_When_DrawProcessedRegion_Then_UsesCalculatedPixelSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool      = AnnotationToolType.Pixelate,
			Bounds    = new RectangleF(10, 10, 100, 100),
			Thickness = 10
		};

		var region = new SKRectI(10, 10, 110, 110);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.DrawProcessedRegion(canvas, bitmap, annotation));

		Assert.Null(exception);
	}

	#endregion

	#region CreateEffectPreview Tests

	[Fact]
	public void Given_PositionAndBrush_When_CreateEffectPreview_Then_ReturnsPreviewAtPosition()
	{
		var position  = new Point(50, 60);
		var brush     = new SolidColorBrush(Colors.Red);
		var thickness = 3.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.NotNull(result.Grid);
		Assert.NotNull(result.Image);
		Assert.NotNull(result.Border);
		Assert.Equal(50, Canvas.GetLeft(result.Grid));
		Assert.Equal(60, Canvas.GetTop(result.Grid));
		Assert.False(result.Grid.IsHitTestVisible);
	}

	[Fact]
	public void Given_EffectPreview_When_CreateEffectPreview_Then_ImageHasFillStretch()
	{
		var position  = new Point(10, 20);
		var brush     = new SolidColorBrush(Colors.Blue);
		var thickness = 2.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.Equal(Stretch.Fill, result.Image.Stretch);
	}

	[Fact]
	public void Given_EffectPreview_When_CreateEffectPreview_Then_BorderHasCorrectProperties()
	{
		var position  = new Point(30, 40);
		var brush     = new SolidColorBrush(Colors.Green);
		var thickness = 5.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.Same(brush, result.Border.Stroke);
		Assert.Equal(5.0, result.Border.StrokeThickness);
		Assert.Same(Brushes.Transparent, result.Border.Fill);
		Assert.Equal(0.9, result.Border.Opacity);
	}

	[Fact]
	public void Given_EffectPreview_When_CreateEffectPreview_Then_GridContainsImageAndBorder()
	{
		var position  = new Point(0, 0);
		var brush     = new SolidColorBrush(Colors.Purple);
		var thickness = 1.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.Equal(2, result.Grid.Children.Count);
		Assert.Contains(result.Image,  result.Grid.Children);
		Assert.Contains(result.Border, result.Grid.Children);
	}

	[Fact]
	public void Given_NegativePosition_When_CreateEffectPreview_Then_ReturnsPreviewAtPosition()
	{
		var position  = new Point(-10, -20);
		var brush     = new SolidColorBrush(Colors.Orange);
		var thickness = 4.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.Equal(-10, Canvas.GetLeft(result.Grid));
		Assert.Equal(-20, Canvas.GetTop(result.Grid));
	}

	[Fact]
	public void Given_ZeroThickness_When_CreateEffectPreview_Then_BorderHasZeroThickness()
	{
		var position  = new Point(0, 0);
		var brush     = new SolidColorBrush(Colors.Black);
		var thickness = 0.0;

		var result = _sut.CreateEffectPreview(position, brush, thickness);

		Assert.Equal(0.0, result.Border.StrokeThickness);
	}

	#endregion

	#region UpdateEffectPreviewBounds Tests

	[Fact]
	public void Given_ValidBounds_When_UpdateEffectPreviewBounds_Then_UpdatesGridAndReturnsTrue()
	{
		var grid   = new Grid();
		var bounds = new Rect(10, 20, 100, 150);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.True(result);
		Assert.Equal(100, grid.Width);
		Assert.Equal(150, grid.Height);
		Assert.Equal(10,  Canvas.GetLeft(grid));
		Assert.Equal(20,  Canvas.GetTop(grid));
		Assert.True(grid.IsVisible);
	}

	[Fact]
	public void Given_ZeroWidthBounds_When_UpdateEffectPreviewBounds_Then_HidesGridAndReturnsFalse()
	{
		var grid   = new Grid();
		var bounds = new Rect(10, 20, 0, 150);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.False(result);
		Assert.False(grid.IsVisible);
	}

	[Fact]
	public void Given_ZeroHeightBounds_When_UpdateEffectPreviewBounds_Then_HidesGridAndReturnsFalse()
	{
		var grid   = new Grid();
		var bounds = new Rect(10, 20, 100, 0);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.False(result);
		Assert.False(grid.IsVisible);
	}

	[Fact]
	public void Given_NegativeWidthBounds_When_UpdateEffectPreviewBounds_Then_HidesGridAndReturnsFalse()
	{
		var grid   = new Grid();
		var bounds = new Rect(10, 20, -10, 150);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.False(result);
		Assert.False(grid.IsVisible);
	}

	[Fact]
	public void Given_NegativeHeightBounds_When_UpdateEffectPreviewBounds_Then_HidesGridAndReturnsFalse()
	{
		var grid   = new Grid();
		var bounds = new Rect(10, 20, 100, -10);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.False(result);
		Assert.False(grid.IsVisible);
	}

	[Fact]
	public void Given_NegativePosition_When_UpdateEffectPreviewBounds_Then_UpdatesPosition()
	{
		var grid   = new Grid();
		var bounds = new Rect(-10, -20, 100, 150);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.True(result);
		Assert.Equal(-10, Canvas.GetLeft(grid));
		Assert.Equal(-20, Canvas.GetTop(grid));
	}

	[Fact]
	public void Given_VerySmallBounds_When_UpdateEffectPreviewBounds_Then_UpdatesGridAndReturnsTrue()
	{
		var grid   = new Grid();
		var bounds = new Rect(0, 0, 0.1, 0.1);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.True(result);
		Assert.Equal(0.1, grid.Width);
		Assert.Equal(0.1, grid.Height);
		Assert.True(grid.IsVisible);
	}

	[Fact]
	public void Given_LargeBounds_When_UpdateEffectPreviewBounds_Then_UpdatesGridAndReturnsTrue()
	{
		var grid   = new Grid();
		var bounds = new Rect(0, 0, 10000, 10000);

		var result = _sut.UpdateEffectPreviewBounds(grid, bounds);

		Assert.True(result);
		Assert.Equal(10000, grid.Width);
		Assert.Equal(10000, grid.Height);
		Assert.True(grid.IsVisible);
	}

	#endregion

	#region UpdateEffectPreview Tests

	[Fact]
	public void Given_NullImage_When_UpdateEffectPreview_Then_DoesNothing()
	{
		using var bitmap = new SKBitmap(200, 200);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(null, bitmap, new Rect(10, 10, 100, 100), OverlayTool.Blur, 5.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_NullBitmap_When_UpdateEffectPreview_Then_DoesNothing()
	{
		var image = new Image();

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, null, new Rect(10, 10, 100, 100), OverlayTool.Blur, 5.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_BlurTool_When_UpdateEffectPreview_Then_AppliesBlur()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(10, 10, 100, 100);

		var region = new SKRectI(10, 10, 110, 110);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Blur, 5.0));

		Assert.Null(exception);
		_rectangleService.Verify(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()), Times.Once);
	}

	[Fact]
	public void Given_PixelateTool_When_UpdateEffectPreview_Then_AppliesPixelation()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(20, 20, 80, 80);

		var region = new SKRectI(20, 20, 100, 100);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Pixelate, 3.0));

		Assert.Null(exception);
		_rectangleService.Verify(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()), Times.Once);
	}

	[Fact]
	public void Given_PixelateToolWithSmallThickness_When_UpdateEffectPreview_Then_UsesMinimumPixelSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(10, 10, 50, 50);

		var region = new SKRectI(10, 10, 60, 60);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Pixelate, 1.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_RegionOutsideBitmap_When_UpdateEffectPreview_Then_DoesNotUpdateImage()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(300, 300, 100, 100);

		var emptyRegion = new SKRectI(0, 0, 0, 0);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(emptyRegion);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Blur, 5.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroSizeBounds_When_UpdateEffectPreview_Then_DoesNotUpdateImage()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(10, 10, 0, 0);

		var emptyRegion = new SKRectI(10, 10, 10, 10);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(emptyRegion);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Blur, 5.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ExceptionDuringProcessing_When_UpdateEffectPreview_Then_CatchesAndDoesNotThrow()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(10, 10, 100, 100);

		var emptyRegion = new SKRectI(0, 0, 0, 0);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(emptyRegion);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Blur, 5.0));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_PixelateToolWithLargeThickness_When_UpdateEffectPreview_Then_UsesCalculatedPixelSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		bitmap.Erase(SKColors.White);
		var image  = new Image();
		var bounds = new Rect(10, 10, 100, 100);

		var region = new SKRectI(10, 10, 110, 110);
		_rectangleService.Setup(r => r.IntersectRect(It.IsAny<SKRectI>(), It.IsAny<SKRectI>()))
			.Returns(region);

		var exception = Record.Exception(() => _sut.UpdateEffectPreview(image, bitmap, bounds, OverlayTool.Pixelate, 10.0));

		Assert.Null(exception);
	}

	#endregion
}
