using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Media;
using Moq;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Models;
using Shotora.App.Models.Drawings;
using Shotora.App.Models.Enums;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Color=Avalonia.Media.Color;
using Image=Avalonia.Controls.Image;
using Point=Avalonia.Point;
using Rectangle=Avalonia.Controls.Shapes.Rectangle;

namespace Shotora.App.Tests.DrawingServices;

public class AnnotationDrawingServiceTests
{
	private readonly Mock<IArrowDrawingService>     _arrows     = new(MockBehavior.Strict);
	private readonly Mock<IEllipseDrawingService>   _ellipses   = new(MockBehavior.Strict);
	private readonly Mock<ILineDrawingService>      _lines      = new(MockBehavior.Strict);
	private readonly Mock<IPolylineDrawingService>  _polylines  = new(MockBehavior.Strict);
	private readonly Mock<IRectangleDrawingService> _rectangles = new(MockBehavior.Strict);

	private readonly Mock<ISkiaDrawingService> _skiaDrawing = new(MockBehavior.Strict);
	private readonly AnnotationDrawingService  _sut;
	private readonly Mock<ITextDrawingService> _textDrawing = new(MockBehavior.Strict);
	static AnnotationDrawingServiceTests()
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

	public AnnotationDrawingServiceTests()
	{
		_sut = new AnnotationDrawingService(
			_skiaDrawing.Object,
			_textDrawing.Object,
			_polylines.Object,
			_lines.Object,
			_arrows.Object,
			_rectangles.Object,
			_ellipses.Object);
	}

	#region Multiple Annotations Test

	[Fact]
	public void Given_MultipleAnnotations_When_BuildAnnotatedBitmap_Then_DrawsAllAnnotations()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var textAnnotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Text,
			Color   = global::System.Drawing.Color.Red,
			Opacity = 1f
		};
		var lineAnnotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Line,
			Color   = global::System.Drawing.Color.Blue,
			Opacity = 1f
		};
		var rectAnnotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Rectangle,
			Color   = global::System.Drawing.Color.Green,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(textAnnotation.Color, textAnnotation.Opacity)).Returns(SKColors.Red);
		_skiaDrawing.Setup(s => s.ToSkColor(lineAnnotation.Color, lineAnnotation.Opacity)).Returns(SKColors.Blue);
		_skiaDrawing.Setup(s => s.ToSkColor(rectAnnotation.Color, rectAnnotation.Opacity)).Returns(SKColors.Green);

		_textDrawing.Setup(t => t.DrawText(It.IsAny<SKCanvas>(), textAnnotation, SKColors.Red));
		_lines.Setup(l => l.DrawSkLine(It.IsAny<SKCanvas>(), lineAnnotation, SKColors.Blue));
		_rectangles.Setup(r => r.DrawSkRect(It.IsAny<SKCanvas>(), rectAnnotation, SKColors.Green));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [textAnnotation, lineAnnotation, rectAnnotation]);

		Assert.NotNull(result);
		_textDrawing.Verify(t => t.DrawText(It.IsAny<SKCanvas>(), textAnnotation, SKColors.Red), Times.Once);
		_lines.Verify(l => l.DrawSkLine(It.IsAny<SKCanvas>(), lineAnnotation, SKColors.Blue), Times.Once);
		_rectangles.Verify(r => r.DrawSkRect(It.IsAny<SKCanvas>(), rectAnnotation, SKColors.Green), Times.Once);
		result.Dispose();
	}

	#endregion

	#region BuildAnnotatedBitmap Tests

	[Fact]
	public void Given_NullCaptureRaw_When_BuildAnnotatedBitmap_Then_ThrowsInvalidOperationException()
	{
		var annotations = new List<AnnotationItem>();

		var exception = Assert.Throws<InvalidOperationException>(() => _sut.BuildAnnotatedBitmap(null!, annotations));

		Assert.Equal("Capture bitmap is required.", exception.Message);
	}

	[Fact]
	public void Given_ValidCaptureAndEmptyAnnotations_When_BuildAnnotatedBitmap_Then_ReturnsAnnotatedBitmap()
	{
		using var captureRaw = new SKBitmap(100, 100);

		var result = _sut.BuildAnnotatedBitmap(captureRaw, []);

		Assert.NotNull(result);
		Assert.Equal(100, result.Width);
		Assert.Equal(100, result.Height);
		result.Dispose();
	}

	[Fact]
	public void Given_TextAnnotation_When_BuildAnnotatedBitmap_Then_DrawsTextOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Text,
			Color   = global::System.Drawing.Color.Red,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Red);
		_textDrawing.Setup(t => t.DrawText(It.IsAny<SKCanvas>(), annotation, SKColors.Red));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_textDrawing.Verify(t => t.DrawText(It.IsAny<SKCanvas>(), annotation, SKColors.Red), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_PenAnnotation_When_BuildAnnotatedBitmap_Then_DrawsPolylineOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Pen,
			Color   = global::System.Drawing.Color.Blue,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Blue);
		_polylines.Setup(p => p.DrawSkPolyline(It.IsAny<SKCanvas>(), annotation, SKColors.Blue));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_polylines.Verify(p => p.DrawSkPolyline(It.IsAny<SKCanvas>(), annotation, SKColors.Blue), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_HighlightAnnotation_When_BuildAnnotatedBitmap_Then_DrawsPolylineOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Highlight,
			Color   = global::System.Drawing.Color.Yellow,
			Opacity = 0.5f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(new SKColor(255, 255, 0, 128));
		_polylines.Setup(p => p.DrawSkPolyline(It.IsAny<SKCanvas>(), annotation, It.IsAny<SKColor>()));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_polylines.Verify(p => p.DrawSkPolyline(It.IsAny<SKCanvas>(), annotation, It.IsAny<SKColor>()), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_LineAnnotation_When_BuildAnnotatedBitmap_Then_DrawsLineOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Line,
			Color   = global::System.Drawing.Color.Green,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Green);
		_lines.Setup(l => l.DrawSkLine(It.IsAny<SKCanvas>(), annotation, SKColors.Green));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_lines.Verify(l => l.DrawSkLine(It.IsAny<SKCanvas>(), annotation, SKColors.Green), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_ArrowAnnotation_When_BuildAnnotatedBitmap_Then_DrawsArrowOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Arrow,
			Color   = global::System.Drawing.Color.Red,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Red);
		_arrows.Setup(a => a.DrawArrow(It.IsAny<SKCanvas>(), annotation, SKColors.Red));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_arrows.Verify(a => a.DrawArrow(It.IsAny<SKCanvas>(), annotation, SKColors.Red), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_RectangleAnnotation_When_BuildAnnotatedBitmap_Then_DrawsRectOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Rectangle,
			Color   = global::System.Drawing.Color.Purple,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Purple);
		_rectangles.Setup(r => r.DrawSkRect(It.IsAny<SKCanvas>(), annotation, SKColors.Purple));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_rectangles.Verify(r => r.DrawSkRect(It.IsAny<SKCanvas>(), annotation, SKColors.Purple), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_EllipseAnnotation_When_BuildAnnotatedBitmap_Then_DrawsEllipseOnCanvas()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Ellipse,
			Color   = global::System.Drawing.Color.Orange,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Orange);
		_ellipses.Setup(e => e.DrawSkEllipse(It.IsAny<SKCanvas>(), annotation, SKColors.Orange));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_ellipses.Verify(e => e.DrawSkEllipse(It.IsAny<SKCanvas>(), annotation, SKColors.Orange), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_BlurAnnotation_When_BuildAnnotatedBitmap_Then_DrawsProcessedRegion()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Blur,
			Color   = global::System.Drawing.Color.White,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.White);
		_skiaDrawing.Setup(s => s.DrawProcessedRegion(It.IsAny<SKCanvas>(), captureRaw, annotation));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_skiaDrawing.Verify(s => s.DrawProcessedRegion(It.IsAny<SKCanvas>(), captureRaw, annotation), Times.Once);
		result.Dispose();
	}

	[Fact]
	public void Given_PixelateAnnotation_When_BuildAnnotatedBitmap_Then_DrawsProcessedRegion()
	{
		using var captureRaw = new SKBitmap(100, 100);
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Pixelate,
			Color   = global::System.Drawing.Color.Gray,
			Opacity = 1f
		};

		_skiaDrawing.Setup(s => s.ToSkColor(annotation.Color, annotation.Opacity)).Returns(SKColors.Gray);
		_skiaDrawing.Setup(s => s.DrawProcessedRegion(It.IsAny<SKCanvas>(), captureRaw, annotation));

		var result = _sut.BuildAnnotatedBitmap(captureRaw, [annotation]);

		Assert.NotNull(result);
		_skiaDrawing.Verify(s => s.DrawProcessedRegion(It.IsAny<SKCanvas>(), captureRaw, annotation), Times.Once);
		result.Dispose();
	}

	#endregion

	#region CreateVisualForAnnotation Tests

	[Fact]
	public void Given_PenAnnotation_When_CreateVisualForAnnotation_Then_ReturnsPolyline()
	{
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Pen,
			Color   = global::System.Drawing.Color.Red,
			Opacity = 1f
		};
		var polyline = new Polyline();

		_polylines.Setup(p => p.CreatePolyline(annotation, It.IsAny<IBrush>())).Returns(polyline);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(polyline, result);
	}

	[Fact]
	public void Given_HighlightAnnotation_When_CreateVisualForAnnotation_Then_ReturnsPolyline()
	{
		var annotation = new AnnotationItem
		{
			Tool    = AnnotationToolType.Highlight,
			Color   = global::System.Drawing.Color.Yellow,
			Opacity = 0.5f
		};
		var polyline = new Polyline();

		_polylines.Setup(p => p.CreatePolyline(annotation, It.IsAny<IBrush>())).Returns(polyline);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(polyline, result);
	}

	[Fact]
	public void Given_LineAnnotation_When_CreateVisualForAnnotation_Then_ReturnsLine()
	{
		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Line,
			Color  = global::System.Drawing.Color.Blue,
			Bounds = new RectangleF(10, 20, 30, 40)
		};
		var line = new Line();

		_lines.Setup(l => l.CreateLine(new Point(10, 20), new Point(40, 60), It.IsAny<IBrush>(), annotation.Thickness, true)).Returns(line);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(line, result);
	}

	[Fact]
	public void Given_ArrowAnnotation_When_CreateVisualForAnnotation_Then_ReturnsArrowContainer()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Arrow,
			Color = global::System.Drawing.Color.Green
		};
		var                            arrowContainer = new Grid();
		Action<Control, List<Control>> capturedAction = null!;

		_arrows.Setup(a => a.CreateArrowContainer(annotation, It.IsAny<IBrush>(), It.IsAny<Action<Control, List<Control>>>()))
			.Callback<AnnotationItem, IBrush, Action<Control, List<Control>>>((_, _, action) => capturedAction = action)
			.Returns(arrowContainer);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(arrowContainer, result);
		Assert.NotNull(capturedAction);
	}

	[Fact]
	public void Given_ArrowAnnotationWithRegisterCallback_When_CreateVisualForAnnotation_Then_UsesProvidedCallback()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Arrow,
			Color = global::System.Drawing.Color.Green
		};
		var arrowContainer = new Grid();
		Action<Control, List<Control>> registerCallback = (_, _) =>
		{
		};

		_arrows.Setup(a => a.CreateArrowContainer(annotation, It.IsAny<IBrush>(), registerCallback)).Returns(arrowContainer);

		var result = _sut.CreateVisualForAnnotation(annotation, registerCallback);

		Assert.Same(arrowContainer, result);
	}

	[Fact]
	public void Given_RectangleAnnotation_When_CreateVisualForAnnotation_Then_ReturnsRectangle()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Rectangle,
			Color = global::System.Drawing.Color.Purple
		};
		var rectangle = new Rectangle();

		_rectangles.Setup(r => r.CreateRectangle(annotation, It.IsAny<IBrush>())).Returns(rectangle);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(rectangle, result);
	}

	[Fact]
	public void Given_EllipseAnnotation_When_CreateVisualForAnnotation_Then_ReturnsEllipse()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Ellipse,
			Color = global::System.Drawing.Color.Orange
		};
		var ellipse = new Ellipse();

		_ellipses.Setup(e => e.CreateEllipse(annotation, It.IsAny<IBrush>())).Returns(ellipse);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(ellipse, result);
	}

	[Fact]
	public void Given_TextAnnotation_When_CreateVisualForAnnotation_Then_ReturnsTextBlock()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Text,
			Color = global::System.Drawing.Color.White
		};
		var textBlock = new TextBlock();

		_textDrawing.Setup(t => t.CreateTextVisual(annotation, It.IsAny<Color>())).Returns(textBlock);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(textBlock, result);
	}

	[Fact]
	public void Given_BlurAnnotation_When_CreateVisualForAnnotation_Then_ReturnsProcessingRectangle()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Blur,
			Color = global::System.Drawing.Color.Gray
		};
		var rectangle = new Rectangle();

		_rectangles.Setup(r => r.CreateProcessingRectangle(annotation, It.IsAny<IBrush>(), 0.9)).Returns(rectangle);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(rectangle, result);
	}

	[Fact]
	public void Given_PixelateAnnotation_When_CreateVisualForAnnotation_Then_ReturnsProcessingRectangle()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Pixelate,
			Color = global::System.Drawing.Color.Gray
		};
		var rectangle = new Rectangle();

		_rectangles.Setup(r => r.CreateProcessingRectangle(annotation, It.IsAny<IBrush>(), 0.9)).Returns(rectangle);

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Same(rectangle, result);
	}

	[Fact]
	public void Given_UnknownAnnotationTool_When_CreateVisualForAnnotation_Then_ReturnsNull()
	{
		var annotation = new AnnotationItem
		{
			Tool  = AnnotationToolType.Selection,
			Color = global::System.Drawing.Color.Red
		};

		var result = _sut.CreateVisualForAnnotation(annotation);

		Assert.Null(result);
	}

	#endregion

	#region BeginDrawing Tests

	[Fact]
	public void Given_PenTool_When_BeginDrawing_Then_ReturnsPolylinePreview()
	{
		var start    = new Point(10, 20);
		var color    = Colors.Red;
		var polyline = new Polyline();

		_polylines.Setup(p => p.CreatePreviewPolyline(start, color, 5, false)).Returns(polyline);

		var result = _sut.BeginDrawing(OverlayTool.Pen, start, color, 5);

		Assert.Same(polyline, result.Polyline);
		Assert.Null(result.PreviewShape);
	}

	[Fact]
	public void Given_HighlightTool_When_BeginDrawing_Then_ReturnsPolylinePreviewWithHighlightFlag()
	{
		var start    = new Point(10, 20);
		var color    = Colors.Yellow;
		var polyline = new Polyline();

		_polylines.Setup(p => p.CreatePreviewPolyline(start, color, 5, true)).Returns(polyline);

		var result = _sut.BeginDrawing(OverlayTool.Highlight, start, color, 5);

		Assert.Same(polyline, result.Polyline);
		Assert.Null(result.PreviewShape);
	}

	[Fact]
	public void Given_LineTool_When_BeginDrawing_Then_ReturnsLinePreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Blue;
		var line  = new Line();

		_lines.Setup(l => l.CreateLine(start, start, It.IsAny<SolidColorBrush>(), 3, false)).Returns(line);

		var result = _sut.BeginDrawing(OverlayTool.Line, start, color, 3);

		Assert.Same(line, result.PreviewShape);
		Assert.Null(result.Polyline);
	}

	[Fact]
	public void Given_ArrowTool_When_BeginDrawing_Then_ReturnsLinePreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Green;
		var line  = new Line();

		_lines.Setup(l => l.CreateLine(start, start, It.IsAny<SolidColorBrush>(), 4, false)).Returns(line);

		var result = _sut.BeginDrawing(OverlayTool.Arrow, start, color, 4);

		Assert.Same(line, result.PreviewShape);
		Assert.Null(result.Polyline);
	}

	[Fact]
	public void Given_RectangleTool_When_BeginDrawing_Then_ReturnsRectanglePreview()
	{
		var start     = new Point(10, 20);
		var color     = Colors.Purple;
		var rectangle = new Rectangle();

		_rectangles.Setup(r => r.CreatePreviewRectangle(start, It.IsAny<SolidColorBrush>(), 2)).Returns(rectangle);

		var result = _sut.BeginDrawing(OverlayTool.Rectangle, start, color, 2);

		Assert.Same(rectangle, result.PreviewShape);
		Assert.Null(result.Polyline);
	}

	[Fact]
	public void Given_EllipseTool_When_BeginDrawing_Then_ReturnsEllipsePreview()
	{
		var start   = new Point(10, 20);
		var color   = Colors.Orange;
		var ellipse = new Ellipse();

		_ellipses.Setup(e => e.CreatePreviewEllipse(start, It.IsAny<SolidColorBrush>(), 6)).Returns(ellipse);

		var result = _sut.BeginDrawing(OverlayTool.Ellipse, start, color, 6);

		Assert.Same(ellipse, result.PreviewShape);
		Assert.Null(result.Polyline);
	}

	[Fact]
	public void Given_BlurTool_When_BeginDrawing_Then_ReturnsEffectPreview()
	{
		var start     = new Point(10, 20);
		var color     = Colors.Gray;
		var grid      = new Grid();
		var image     = new Image();
		var rectangle = new Rectangle();

		_skiaDrawing.Setup(s => s.CreateEffectPreview(start, It.IsAny<SolidColorBrush>(), 5))
			.Returns(new EffectPreview(grid, image, rectangle));

		var result = _sut.BeginDrawing(OverlayTool.Blur, start, color, 5);

		Assert.Same(grid,      result.EffectGrid);
		Assert.Same(image,     result.EffectImage);
		Assert.Same(rectangle, result.EffectBorder);
		Assert.Null(result.Polyline);
	}

	[Fact]
	public void Given_PixelateTool_When_BeginDrawing_Then_ReturnsEffectPreview()
	{
		var start     = new Point(10, 20);
		var color     = Colors.White;
		var grid      = new Grid();
		var image     = new Image();
		var rectangle = new Rectangle();

		_skiaDrawing.Setup(s => s.CreateEffectPreview(start, It.IsAny<SolidColorBrush>(), 7))
			.Returns(new EffectPreview(grid, image, rectangle));

		var result = _sut.BeginDrawing(OverlayTool.Pixelate, start, color, 7);

		Assert.Same(grid,      result.EffectGrid);
		Assert.Same(image,     result.EffectImage);
		Assert.Same(rectangle, result.EffectBorder);
	}

	[Fact]
	public void Given_SelectionTool_When_BeginDrawing_Then_ReturnsDefaultPreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Black;

		var result = _sut.BeginDrawing(OverlayTool.Selection, start, color, 5);

		Assert.Null(result.Polyline);
		Assert.Null(result.PreviewShape);
		Assert.Null(result.EffectGrid);
	}

	[Fact]
	public void Given_PointerTool_When_BeginDrawing_Then_ReturnsDefaultPreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Black;

		var result = _sut.BeginDrawing(OverlayTool.Pointer, start, color, 5);

		Assert.Null(result.Polyline);
		Assert.Null(result.PreviewShape);
	}

	[Fact]
	public void Given_MoveTool_When_BeginDrawing_Then_ReturnsDefaultPreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Black;

		var result = _sut.BeginDrawing(OverlayTool.Move, start, color, 5);

		Assert.Null(result.Polyline);
		Assert.Null(result.PreviewShape);
	}

	[Fact]
	public void Given_TextTool_When_BeginDrawing_Then_ReturnsDefaultPreview()
	{
		var start = new Point(10, 20);
		var color = Colors.Black;

		var result = _sut.BeginDrawing(OverlayTool.Text, start, color, 5);

		Assert.Null(result.Polyline);
		Assert.Null(result.PreviewShape);
	}

	#endregion

	#region UpdateDrawingPreview Tests

	[Fact]
	public void Given_PenToolWithPolyline_When_UpdateDrawingPreview_Then_AppendsPoint()
	{
		var polyline = new Polyline();
		var state = new OverlayInteractionState
		{
			CurrentTool     = OverlayTool.Pen,
			CurrentPolyline = polyline
		};
		var current = new Point(50, 60);

		_polylines.Setup(p => p.AppendPreviewPoint(polyline, current));

		_sut.UpdateDrawingPreview(state, current);

		_polylines.Verify(p => p.AppendPreviewPoint(polyline, current), Times.Once);
	}

	[Fact]
	public void Given_PenToolWithoutPolyline_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var state = new OverlayInteractionState
		{
			CurrentTool     = OverlayTool.Pen,
			CurrentPolyline = null
		};
		var current = new Point(50, 60);

		_sut.UpdateDrawingPreview(state, current);

		_polylines.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_HighlightToolWithPolyline_When_UpdateDrawingPreview_Then_AppendsPoint()
	{
		var polyline = new Polyline();
		var state = new OverlayInteractionState
		{
			CurrentTool     = OverlayTool.Highlight,
			CurrentPolyline = polyline
		};
		var current = new Point(50, 60);

		_polylines.Setup(p => p.AppendPreviewPoint(polyline, current));

		_sut.UpdateDrawingPreview(state, current);

		_polylines.Verify(p => p.AppendPreviewPoint(polyline, current), Times.Once);
	}

	[Fact]
	public void Given_LineToolWithLineShape_When_UpdateDrawingPreview_Then_UpdatesEndPoint()
	{
		var line = new Line
		{
			StartPoint = new Point(0,  0),
			EndPoint   = new Point(10, 10)
		};
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Line,
			CurrentPreviewShape = line
		};
		var current = new Point(100, 200);

		_sut.UpdateDrawingPreview(state, current);

		Assert.Equal(new Point(100, 200), line.EndPoint);
	}

	[Fact]
	public void Given_ArrowToolWithLineShape_When_UpdateDrawingPreview_Then_UpdatesEndPoint()
	{
		var line = new Line
		{
			StartPoint = new Point(0,  0),
			EndPoint   = new Point(10, 10)
		};
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Arrow,
			CurrentPreviewShape = line
		};
		var current = new Point(150, 250);

		_sut.UpdateDrawingPreview(state, current);

		Assert.Equal(new Point(150, 250), line.EndPoint);
	}

	[Fact]
	public void Given_LineToolWithNonLineShape_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var rectangle = new Rectangle();
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Line,
			CurrentPreviewShape = rectangle
		};
		var current = new Point(100, 200);

		_sut.UpdateDrawingPreview(state, current);
	}

	[Fact]
	public void Given_RectangleToolWithRectangle_When_UpdateDrawingPreview_Then_UpdatesBounds()
	{
		var rectangle = new Rectangle();
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Rectangle,
			CurrentPreviewShape = rectangle,
			DrawStart           = new Point(10, 20)
		};
		var current = new Point(100, 200);

		_rectangles.Setup(r => r.UpdatePreviewBounds(rectangle, new Point(10, 20), current));

		_sut.UpdateDrawingPreview(state, current);

		_rectangles.Verify(r => r.UpdatePreviewBounds(rectangle, new Point(10, 20), current), Times.Once);
	}

	[Fact]
	public void Given_RectangleToolWithNonRectangle_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var ellipse = new Ellipse();
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Rectangle,
			CurrentPreviewShape = ellipse,
			DrawStart           = new Point(10, 20)
		};
		var current = new Point(100, 200);

		_sut.UpdateDrawingPreview(state, current);

		_rectangles.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_EllipseToolWithEllipse_When_UpdateDrawingPreview_Then_UpdatesBounds()
	{
		var ellipse = new Ellipse();
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Ellipse,
			CurrentPreviewShape = ellipse,
			DrawStart           = new Point(30, 40)
		};
		var current = new Point(150, 250);

		_ellipses.Setup(e => e.UpdatePreviewBounds(ellipse, new Point(30, 40), current));

		_sut.UpdateDrawingPreview(state, current);

		_ellipses.Verify(e => e.UpdatePreviewBounds(ellipse, new Point(30, 40), current), Times.Once);
	}

	[Fact]
	public void Given_EllipseToolWithNonEllipse_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var rectangle = new Rectangle();
		var state = new OverlayInteractionState
		{
			CurrentTool         = OverlayTool.Ellipse,
			CurrentPreviewShape = rectangle,
			DrawStart           = new Point(30, 40)
		};
		var current = new Point(150, 250);

		_sut.UpdateDrawingPreview(state, current);

		_ellipses.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_BlurToolWithGrid_When_UpdateDrawingPreview_Then_UpdatesEffectPreview()
	{
		using var bitmap = new SKBitmap(100, 100);
		var       grid   = new Grid();
		var       image  = new Image();
		var state = new OverlayInteractionState
		{
			CurrentTool        = OverlayTool.Blur,
			EffectPreviewGrid  = grid,
			EffectPreviewImage = image,
			DrawStart          = new Point(10, 20),
			Thickness          = 5
		};
		var current = new Point(100, 200);
		var bounds  = new Rect(10, 20, 90, 180);

		_rectangles.Setup(r => r.NormalizeRect(new Point(10, 20), current)).Returns(bounds);
		_skiaDrawing.Setup(s => s.UpdateEffectPreviewBounds(grid, bounds)).Returns(true);
		_skiaDrawing.Setup(s => s.UpdateEffectPreview(image, null, bounds, OverlayTool.Blur, 5));

		_sut.UpdateDrawingPreview(state, current);

		_skiaDrawing.Verify(s => s.UpdateEffectPreview(image, null, bounds, OverlayTool.Blur, 5), Times.Once);
	}

	[Fact]
	public void Given_BlurToolWithGridAndBoundsNotChanged_When_UpdateDrawingPreview_Then_SkipsEffectUpdate()
	{
		var grid  = new Grid();
		var image = new Image();
		var state = new OverlayInteractionState
		{
			CurrentTool        = OverlayTool.Blur,
			EffectPreviewGrid  = grid,
			EffectPreviewImage = image,
			DrawStart          = new Point(10, 20),
			Thickness          = 5
		};
		var current = new Point(100, 200);
		var bounds  = new Rect(10, 20, 90, 180);

		_rectangles.Setup(r => r.NormalizeRect(new Point(10, 20), current)).Returns(bounds);
		_skiaDrawing.Setup(s => s.UpdateEffectPreviewBounds(grid, bounds)).Returns(false);

		_sut.UpdateDrawingPreview(state, current);

		_skiaDrawing.Verify(s => s.UpdateEffectPreview(It.IsAny<Image>(), It.IsAny<SKBitmap?>(), It.IsAny<Rect>(), It.IsAny<OverlayTool>(), It.IsAny<double>()), Times.Never);
	}

	[Fact]
	public void Given_BlurToolWithoutGrid_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var state = new OverlayInteractionState
		{
			CurrentTool       = OverlayTool.Blur,
			EffectPreviewGrid = null,
			DrawStart         = new Point(10, 20)
		};
		var current = new Point(100, 200);

		_sut.UpdateDrawingPreview(state, current);

		_rectangles.VerifyNoOtherCalls();
		_skiaDrawing.VerifyNoOtherCalls();
	}

	[Fact]
	public void Given_PixelateToolWithGrid_When_UpdateDrawingPreview_Then_UpdatesEffectPreview()
	{
		var grid  = new Grid();
		var image = new Image();
		var state = new OverlayInteractionState
		{
			CurrentTool        = OverlayTool.Pixelate,
			EffectPreviewGrid  = grid,
			EffectPreviewImage = image,
			DrawStart          = new Point(5, 10),
			Thickness          = 8
		};
		var current = new Point(50, 80);
		var bounds  = new Rect(5, 10, 45, 70);

		_rectangles.Setup(r => r.NormalizeRect(new Point(5, 10), current)).Returns(bounds);
		_skiaDrawing.Setup(s => s.UpdateEffectPreviewBounds(grid, bounds)).Returns(true);
		_skiaDrawing.Setup(s => s.UpdateEffectPreview(image, null, bounds, OverlayTool.Pixelate, 8));

		_sut.UpdateDrawingPreview(state, current);

		_skiaDrawing.Verify(s => s.UpdateEffectPreview(image, null, bounds, OverlayTool.Pixelate, 8), Times.Once);
	}

	[Fact]
	public void Given_SelectionTool_When_UpdateDrawingPreview_Then_DoesNothing()
	{
		var state = new OverlayInteractionState
		{
			CurrentTool = OverlayTool.Selection
		};
		var current = new Point(100, 200);

		_sut.UpdateDrawingPreview(state, current);
	}

	#endregion

	#region RefreshSelectedAnnotationVisual Tests

	[Fact]
	public void Given_NullSelectedVisual_When_RefreshSelectedAnnotationVisual_Then_ClearsHighlight()
	{
		var state = new OverlayInteractionState
		{
			SelectedVisual     = null,
			SelectedAnnotation = new AnnotationItem()
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(null, null));

		_sut.RefreshSelectedAnnotationVisual(state);

		_rectangles.Verify(r => r.UpdateAnnotationHighlight(null, null), Times.Once);
	}

	[Fact]
	public void Given_NullSelectedAnnotation_When_RefreshSelectedAnnotationVisual_Then_ClearsHighlight()
	{
		var state = new OverlayInteractionState
		{
			SelectedVisual     = new Rectangle(),
			SelectedAnnotation = null
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(null, null));

		_sut.RefreshSelectedAnnotationVisual(state);

		_rectangles.Verify(r => r.UpdateAnnotationHighlight(null, null), Times.Once);
	}

	[Fact]
	public void Given_PolylineVisual_When_RefreshSelectedAnnotationVisual_Then_UpdatesStroke()
	{
		var polyline = new Polyline();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Red
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = polyline,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(polyline, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(polyline.Stroke);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(polyline, annotation), Times.Once);
	}

	[Fact]
	public void Given_LineVisual_When_RefreshSelectedAnnotationVisual_Then_UpdatesStroke()
	{
		var line = new Line();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Blue
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = line,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(line, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(line.Stroke);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(line, annotation), Times.Once);
	}

	[Fact]
	public void Given_RectangleVisualWithFill_When_RefreshSelectedAnnotationVisual_Then_UpdatesStrokeAndFill()
	{
		var rectangle = new Rectangle();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Green,
			Fill  = true
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = rectangle,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(rectangle, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(rectangle.Stroke);
		Assert.NotNull(rectangle.Fill);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(rectangle, annotation), Times.Once);
	}

	[Fact]
	public void Given_RectangleVisualWithoutFill_When_RefreshSelectedAnnotationVisual_Then_UpdatesOnlyStroke()
	{
		var rectangle = new Rectangle();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Green,
			Fill  = false
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = rectangle,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(rectangle, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(rectangle.Stroke);
		Assert.Null(rectangle.Fill);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(rectangle, annotation), Times.Once);
	}

	[Fact]
	public void Given_EllipseVisualWithFill_When_RefreshSelectedAnnotationVisual_Then_UpdatesStrokeAndFill()
	{
		var ellipse = new Ellipse();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Orange,
			Fill  = true
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = ellipse,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(ellipse, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(ellipse.Stroke);
		Assert.NotNull(ellipse.Fill);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(ellipse, annotation), Times.Once);
	}

	[Fact]
	public void Given_EllipseVisualWithoutFill_When_RefreshSelectedAnnotationVisual_Then_UpdatesOnlyStroke()
	{
		var ellipse = new Ellipse();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Orange,
			Fill  = false
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = ellipse,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(ellipse, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(ellipse.Stroke);
		Assert.Null(ellipse.Fill);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(ellipse, annotation), Times.Once);
	}

	[Fact]
	public void Given_GridVisualWithArrowChildren_When_RefreshSelectedAnnotationVisual_Then_UpdatesAllChildren()
	{
		var grid      = new Grid();
		var arrowLine = new Line();
		var arrowHead = new Polygon();
		grid.Children.Add(arrowLine);
		grid.Children.Add(arrowHead);

		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Purple
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = grid,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(grid, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(arrowLine.Stroke);
		Assert.NotNull(arrowHead.Fill);
		Assert.NotNull(arrowHead.Stroke);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(grid, annotation), Times.Once);
	}

	[Fact]
	public void Given_TextBlockVisual_When_RefreshSelectedAnnotationVisual_Then_UpdatesForeground()
	{
		var textBlock = new TextBlock();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.White
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = textBlock,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(textBlock, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		Assert.NotNull(textBlock.Foreground);
		_rectangles.Verify(r => r.UpdateAnnotationHighlight(textBlock, annotation), Times.Once);
	}

	[Fact]
	public void Given_UnknownControlVisual_When_RefreshSelectedAnnotationVisual_Then_OnlyUpdatesHighlight()
	{
		var button = new Button();
		var annotation = new AnnotationItem
		{
			Color = global::System.Drawing.Color.Cyan
		};
		var state = new OverlayInteractionState
		{
			SelectedVisual     = button,
			SelectedAnnotation = annotation
		};

		_rectangles.Setup(r => r.UpdateAnnotationHighlight(button, annotation));

		_sut.RefreshSelectedAnnotationVisual(state);

		_rectangles.Verify(r => r.UpdateAnnotationHighlight(button, annotation), Times.Once);
	}

	#endregion

	#region GetTopMostAnnotationAt Tests

	[Fact]
	public void Given_NoHitsAtPosition_When_GetTopMostAnnotationAt_Then_ReturnsNull()
	{
		var canvas      = new Canvas();
		var position    = new Point(50, 50);
		var arrowGroups = new Dictionary<Control, List<Control>>();

		var result = _sut.GetTopMostAnnotationAt(canvas, position, arrowGroups);

		Assert.Null(result);
	}

	[Fact]
	public void Given_EmptyArrowGroups_When_GetTopMostAnnotationAt_Then_DoesNotThrow()
	{
		var canvas = new Canvas
		{
			Width  = 100,
			Height = 100
		};
		var position    = new Point(50, 50);
		var arrowGroups = new Dictionary<Control, List<Control>>();

		var exception = Record.Exception(() => _sut.GetTopMostAnnotationAt(canvas, position, arrowGroups));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ArrowGroupsWithMultipleContainers_When_GetTopMostAnnotationAt_Then_HandlesCorrectly()
	{
		var canvas = new Canvas
		{
			Width  = 100,
			Height = 100
		};

		var container1 = new Grid();
		var container2 = new Grid();
		var line1      = new Line();
		var line2      = new Line();

		var arrowGroups = new Dictionary<Control, List<Control>>
		{
			{
				container1, [line1]
			},
			{
				container2, [line2]
			}
		};

		var position = new Point(50, 50);

		var exception = Record.Exception(() => _sut.GetTopMostAnnotationAt(canvas, position, arrowGroups));

		Assert.Null(exception);
	}

	#endregion
}
