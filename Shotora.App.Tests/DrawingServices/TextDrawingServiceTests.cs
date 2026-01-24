using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Moq;
using Shotora.App.Interfaces.Abstractions;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ItemModels;
using Shotora.App.Services.DrawingServices;
using SkiaSharp;
using Color=Avalonia.Media.Color;
using FontStyle=Avalonia.Media.FontStyle;
using Point=Avalonia.Point;

namespace Shotora.App.Tests.DrawingServices;

public class TextDrawingServiceTests
{
	private readonly Mock<IBaseOcrService> _baseOcrService = new(MockBehavior.Strict);

	private readonly Mock<ITextInputDialogService> _dialogService      = new(MockBehavior.Strict);
	private readonly Mock<IDispatcherFacade>       _dispatcherFacade   = new(MockBehavior.Loose);
	private readonly Mock<IOcrResultPresenter>     _ocrResultPresenter = new(MockBehavior.Strict);
	private readonly TextDrawingService            _sut;
	static TextDrawingServiceTests()
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

	public TextDrawingServiceTests()
	{
		_sut = new TextDrawingService(
			_dialogService.Object,
			_baseOcrService.Object,
			_ocrResultPresenter.Object,
			_dispatcherFacade.Object);
	}

	#region CloseOcrWindow Tests

	[Fact]
	public void Given_OcrWindowOpen_When_CloseOcrWindow_Then_ClosesPresenter()
	{
		_ocrResultPresenter.Setup(p => p.Close());

		_sut.CloseOcrWindow();

		_ocrResultPresenter.Verify(p => p.Close(), Times.Once);
	}

	#endregion

	#region DrawText Tests

	[Fact]
	public void Given_NullText_When_DrawText_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Text,
			Text   = null,
			Bounds = new RectangleF(10, 10, 100, 30)
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_EmptyText_When_DrawText_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Text,
			Text   = "",
			Bounds = new RectangleF(10, 10, 100, 30)
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_WhitespaceText_When_DrawText_Then_DoesNotDraw()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool   = AnnotationToolType.Text,
			Text   = "   ",
			Bounds = new RectangleF(10, 10, 100, 30)
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Red));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ValidText_When_DrawText_Then_DrawsTextOnCanvas()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Hello World",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = 16,
			Bold       = false,
			Italic     = false
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Blue));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_BoldText_When_DrawText_Then_DrawsBoldText()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Bold Text",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = 16,
			Bold       = true,
			Italic     = false
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Green));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ItalicText_When_DrawText_Then_DrawsItalicText()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Italic Text",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = 16,
			Bold       = false,
			Italic     = true
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Purple));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_BoldItalicText_When_DrawText_Then_DrawsBoldItalicText()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Bold Italic",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = 16,
			Bold       = true,
			Italic     = true
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Orange));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_ZeroFontSize_When_DrawText_Then_UsesDefaultFontSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Default Size",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = 0,
			Bold       = false,
			Italic     = false
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Black));

		Assert.Null(exception);
	}

	[Fact]
	public void Given_NegativeFontSize_When_DrawText_Then_UsesDefaultFontSize()
	{
		using var bitmap = new SKBitmap(200, 200);
		using var canvas = new SKCanvas(bitmap);
		canvas.Clear(SKColors.White);

		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Negative Size",
			Bounds     = new RectangleF(10, 30, 100, 30),
			FontFamily = "Arial",
			FontSize   = -10,
			Bold       = false,
			Italic     = false
		};

		var exception = Record.Exception(() => _sut.DrawText(canvas, annotation, SKColors.Gray));

		Assert.Null(exception);
	}

	#endregion

	#region CreateTextVisual Tests

	[Fact]
	public void Given_Annotation_When_CreateTextVisual_Then_ReturnsPositionedTextBlock()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Test Text",
			Bounds     = new RectangleF(50, 60, 100, 30),
			FontFamily = "Segoe UI",
			FontSize   = 14,
			Bold       = false,
			Italic     = false
		};
		var color = Colors.Red;

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		Assert.Equal("Test Text", result.Text);
		Assert.Equal(50,          Canvas.GetLeft(result));
		Assert.Equal(60,          Canvas.GetTop(result));
	}

	[Fact]
	public void Given_AnnotationWithBold_When_CreateTextVisual_Then_TextBlockIsBold()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Bold",
			Bounds     = new RectangleF(10, 20, 50, 20),
			FontFamily = "Arial",
			FontSize   = 12,
			Bold       = true,
			Italic     = false
		};
		var color = Colors.Blue;

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		Assert.Equal(FontWeight.Bold, result.FontWeight);
	}

	[Fact]
	public void Given_AnnotationWithItalic_When_CreateTextVisual_Then_TextBlockIsItalic()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Italic",
			Bounds     = new RectangleF(10, 20, 50, 20),
			FontFamily = "Arial",
			FontSize   = 12,
			Bold       = false,
			Italic     = true
		};
		var color = Colors.Green;

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		Assert.Equal(FontStyle.Italic, result.FontStyle);
	}

	[Fact]
	public void Given_AnnotationWithNullText_When_CreateTextVisual_Then_UsesEmptyString()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = null,
			Bounds     = new RectangleF(10, 20, 50, 20),
			FontFamily = "Arial",
			FontSize   = 12
		};
		var color = Colors.Black;

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		Assert.Equal(string.Empty, result.Text);
	}

	[Fact]
	public void Given_AnnotationWithZeroFontSize_When_CreateTextVisual_Then_UsesDefaultFontSize()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Test",
			Bounds     = new RectangleF(10, 20, 50, 20),
			FontFamily = "Arial",
			FontSize   = 0
		};
		var color = Colors.Purple;

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		Assert.Equal(16, result.FontSize);
	}

	[Fact]
	public void Given_Annotation_When_CreateTextVisual_Then_TextBlockHasCorrectForeground()
	{
		var annotation = new AnnotationItem
		{
			Tool       = AnnotationToolType.Text,
			Text       = "Colored",
			Bounds     = new RectangleF(10, 20, 50, 20),
			FontFamily = "Arial",
			FontSize   = 14
		};
		var color = Color.FromRgb(255, 128, 64);

		var result = _sut.CreateTextVisual(annotation, color);

		Assert.NotNull(result);
		var foreground = result.Foreground as SolidColorBrush;
		Assert.NotNull(foreground);
		Assert.Equal(color, foreground.Color);
	}

	#endregion

	#region CreateTextAnnotationAsync Tests

	[Fact]
	public async Task Given_DialogReturnsNull_When_CreateTextAnnotationAsync_Then_ReturnsNull()
	{
		var owner    = new Window();
		var color    = Colors.Red;
		var position = new Point(100, 100);

		_dialogService.Setup(d => d.ShowAsync(owner, 20, null))
			.ReturnsAsync((TextResultItemModel?)null);

		var result = await _sut.CreateTextAnnotationAsync(owner, 5, color, position);

		Assert.Null(result);
	}

	[Fact]
	public async Task Given_DialogReturnsEmptyText_When_CreateTextAnnotationAsync_Then_ReturnsNull()
	{
		var owner    = new Window();
		var color    = Colors.Red;
		var position = new Point(100, 100);

		_dialogService.Setup(d => d.ShowAsync(owner, 20, null))
			.ReturnsAsync(new TextResultItemModel("", "Arial", 14, false, false));

		var result = await _sut.CreateTextAnnotationAsync(owner, 5, color, position);

		Assert.Null(result);
	}

	[Fact]
	public async Task Given_DialogReturnsWhitespaceText_When_CreateTextAnnotationAsync_Then_ReturnsNull()
	{
		var owner    = new Window();
		var color    = Colors.Red;
		var position = new Point(100, 100);

		_dialogService.Setup(d => d.ShowAsync(owner, 20, null))
			.ReturnsAsync(new TextResultItemModel("   ", "Arial", 14, false, false));

		var result = await _sut.CreateTextAnnotationAsync(owner, 5, color, position);

		Assert.Null(result);
	}

	[Fact]
	public async Task Given_DialogReturnsValidText_When_CreateTextAnnotationAsync_Then_ReturnsVisualAndAnnotation()
	{
		var owner    = new Window();
		var color    = Colors.Blue;
		var position = new Point(50, 60);

		_dialogService.Setup(d => d.ShowAsync(owner, 24, null))
			.ReturnsAsync(new TextResultItemModel("Hello", "Segoe UI", 18, true, false));

		var result = await _sut.CreateTextAnnotationAsync(owner, 6, color, position);

		Assert.NotNull(result);
		Assert.NotNull(result.Value.Visual);
		Assert.NotNull(result.Value.Annotation);
		Assert.Equal("Hello",    result.Value.Annotation.Text);
		Assert.Equal("Segoe UI", result.Value.Annotation.FontFamily);
		Assert.Equal(18,         result.Value.Annotation.FontSize);
		Assert.True(result.Value.Annotation.Bold);
		Assert.False(result.Value.Annotation.Italic);
		Assert.Equal(AnnotationToolType.Text, result.Value.Annotation.Tool);
	}

	[Fact]
	public async Task Given_ValidDialog_When_CreateTextAnnotationAsync_Then_VisualIsPositioned()
	{
		var owner    = new Window();
		var color    = Colors.Green;
		var position = new Point(75, 85);

		_dialogService.Setup(d => d.ShowAsync(owner, 16, null))
			.ReturnsAsync(new TextResultItemModel("Positioned", "Arial", 12, false, true));

		var result = await _sut.CreateTextAnnotationAsync(owner, 4, color, position);

		Assert.NotNull(result);
		Assert.Equal(75, Canvas.GetLeft(result.Value.Visual));
		Assert.Equal(85, Canvas.GetTop(result.Value.Visual));
	}

	#endregion

	#region ApplyTextEditAsync Tests

	[Fact]
	public async Task Given_DialogReturnsNull_When_ApplyTextEditAsync_Then_ReturnsFalse()
	{
		var owner  = new Window();
		var visual = new TextBlock();
		var annotation = new AnnotationItem
		{
			Text = "Original"
		};

		_dialogService.Setup(d => d.ShowAsync(owner, 20, annotation))
			.ReturnsAsync((TextResultItemModel?)null);

		var result = await _sut.ApplyTextEditAsync(owner, visual, annotation, 5);

		Assert.False(result);
	}

	[Fact]
	public async Task Given_DialogReturnsEmptyText_When_ApplyTextEditAsync_Then_ReturnsFalse()
	{
		var owner  = new Window();
		var visual = new TextBlock();
		var annotation = new AnnotationItem
		{
			Text = "Original"
		};

		_dialogService.Setup(d => d.ShowAsync(owner, 20, annotation))
			.ReturnsAsync(new TextResultItemModel("", "Arial", 14, false, false));

		var result = await _sut.ApplyTextEditAsync(owner, visual, annotation, 5);

		Assert.False(result);
	}

	[Fact]
	public async Task Given_DialogReturnsValidText_When_ApplyTextEditAsync_Then_UpdatesAnnotationAndReturnsTrue()
	{
		var owner = new Window();
		var visual = new TextBlock
		{
			Text = "Original"
		};
		Canvas.SetLeft(visual, 10);
		Canvas.SetTop(visual, 20);
		var annotation = new AnnotationItem
		{
			Text       = "Original",
			FontFamily = "Arial",
			FontSize   = 12,
			Bold       = false,
			Italic     = false
		};

		_dialogService.Setup(d => d.ShowAsync(owner, 24, annotation))
			.ReturnsAsync(new TextResultItemModel("Updated", "Segoe UI", 16, true, true));

		var result = await _sut.ApplyTextEditAsync(owner, visual, annotation, 6);

		Assert.True(result);
		Assert.Equal("Updated",  annotation.Text);
		Assert.Equal("Segoe UI", annotation.FontFamily);
		Assert.Equal(16,         annotation.FontSize);
		Assert.True(annotation.Bold);
		Assert.True(annotation.Italic);
	}

	[Fact]
	public async Task Given_DialogReturnsValidText_When_ApplyTextEditAsync_Then_UpdatesTextBlockVisual()
	{
		var owner = new Window();
		var visual = new TextBlock
		{
			Text     = "Original",
			FontSize = 12
		};
		Canvas.SetLeft(visual, 10);
		Canvas.SetTop(visual, 20);
		var annotation = new AnnotationItem
		{
			Text = "Original"
		};

		_dialogService.Setup(d => d.ShowAsync(owner, 20, annotation))
			.ReturnsAsync(new TextResultItemModel("New Text", "Consolas", 20, true, false));

		var result = await _sut.ApplyTextEditAsync(owner, visual, annotation, 5);

		Assert.True(result);
		Assert.Equal("New Text",      visual.Text);
		Assert.Equal(20,              visual.FontSize);
		Assert.Equal(FontWeight.Bold, visual.FontWeight);
	}

	#endregion

	#region ApplyTranslation Tests

	[Fact]
	public void Given_NonTextBlockVisual_When_ApplyTranslation_Then_DoesNothing()
	{
		var visual = new Button();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(10, 20, 50, 30)
		};

		_sut.ApplyTranslation(visual, annotation, 15, 25);

		Assert.Equal(10, annotation.Bounds.X);
		Assert.Equal(20, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_TextBlockVisual_When_ApplyTranslation_Then_UpdatesPositionAndBounds()
	{
		var visual = new TextBlock();
		Canvas.SetLeft(visual, 50);
		Canvas.SetTop(visual, 60);
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(50, 60, 100, 30)
		};

		_sut.ApplyTranslation(visual, annotation, 25, 15);

		Assert.Equal(75, Canvas.GetLeft(visual));
		Assert.Equal(75, Canvas.GetTop(visual));
		Assert.Equal(75, annotation.Bounds.X);
		Assert.Equal(75, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_TextBlockWithNaNPosition_When_ApplyTranslation_Then_TreatsAsZero()
	{
		var visual = new TextBlock();
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(0, 0, 100, 30)
		};

		_sut.ApplyTranslation(visual, annotation, 30, 20);

		Assert.Equal(30, Canvas.GetLeft(visual));
		Assert.Equal(20, Canvas.GetTop(visual));
		Assert.Equal(30, annotation.Bounds.X);
		Assert.Equal(20, annotation.Bounds.Y);
	}

	[Fact]
	public void Given_NegativeOffset_When_ApplyTranslation_Then_MovesBackward()
	{
		var visual = new TextBlock();
		Canvas.SetLeft(visual, 100);
		Canvas.SetTop(visual, 100);
		var annotation = new AnnotationItem
		{
			Bounds = new RectangleF(100, 100, 50, 25)
		};

		_sut.ApplyTranslation(visual, annotation, -30, -20);

		Assert.Equal(70, Canvas.GetLeft(visual));
		Assert.Equal(80, Canvas.GetTop(visual));
		Assert.Equal(70, annotation.Bounds.X);
		Assert.Equal(80, annotation.Bounds.Y);
	}

	#endregion

	#region ShowOcrForSelectionAsync Tests

	[Fact]
	public async Task Given_NullCapture_When_ShowOcrForSelectionAsync_Then_DoesNothing()
	{
		var selection = new Rect(0, 0, 100, 100);
		var anchor    = new Rect(0, 0, 100, 100);

		await _sut.ShowOcrForSelectionAsync(null!, selection, anchor, "eng", OcrEngineType.EasyOcr);

		_baseOcrService.VerifyNoOtherCalls();
		_ocrResultPresenter.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_TooSmallSelectionWidth_When_ShowOcrForSelectionAsync_Then_DoesNothing()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(0, 0, 3,   100);
		var       anchor    = new Rect(0, 0, 100, 100);

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "eng", OcrEngineType.EasyOcr);

		_baseOcrService.VerifyNoOtherCalls();
		_ocrResultPresenter.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_TooSmallSelectionHeight_When_ShowOcrForSelectionAsync_Then_DoesNothing()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(0, 0, 100, 3);
		var       anchor    = new Rect(0, 0, 100, 100);

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "eng", OcrEngineType.EasyOcr);

		_baseOcrService.VerifyNoOtherCalls();
		_ocrResultPresenter.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task Given_OcrSucceeds_When_ShowOcrForSelectionAsync_Then_PresentsResult()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(10, 10, 100, 50);
		var       anchor    = new Rect(10, 10, 100, 50);

		_baseOcrService.Setup(o => o.RecognizeAsync(bitmap, selection, "eng", OcrEngineType.EasyOcr))
			.ReturnsAsync(new OcrResultDataItemModel("Recognized text", null));
		_dispatcherFacade.Setup(d => d.InvokeAsync(It.IsAny<Action>()))
			.Callback<Action>(a => a())
			.Returns(Task.CompletedTask);
		_ocrResultPresenter.Setup(p => p.Present("Recognized text", anchor));

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "eng", OcrEngineType.EasyOcr);

		_ocrResultPresenter.Verify(p => p.Present("Recognized text", anchor), Times.Once);
	}

	[Fact]
	public async Task Given_OcrFailsWithError_When_ShowOcrForSelectionAsync_Then_PresentsError()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(10, 10, 100, 50);
		var       anchor    = new Rect(10, 10, 100, 50);

		_baseOcrService.Setup(o => o.RecognizeAsync(bitmap, selection, "eng", OcrEngineType.Tesseract))
			.ReturnsAsync(new OcrResultDataItemModel(null, "Recognition failed"));
		_dispatcherFacade.Setup(d => d.InvokeAsync(It.IsAny<Action>()))
			.Callback<Action>(a => a())
			.Returns(Task.CompletedTask);
		_ocrResultPresenter.Setup(p => p.Present(It.Is<string>(s => s.Contains("OCR error") && s.Contains("Recognition failed")), anchor));

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "eng", OcrEngineType.Tesseract);

		_ocrResultPresenter.Verify(p => p.Present(It.Is<string>(s => s.Contains("OCR error")), anchor), Times.Once);
	}

	[Fact]
	public async Task Given_OcrFailsWithNoError_When_ShowOcrForSelectionAsync_Then_DoesNotPresentAnything()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(10, 10, 100, 50);
		var       anchor    = new Rect(10, 10, 100, 50);

		_baseOcrService.Setup(o => o.RecognizeAsync(bitmap, selection, "fra", OcrEngineType.EasyOcr))
			.ReturnsAsync(new OcrResultDataItemModel(null, null));

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "fra", OcrEngineType.EasyOcr);

		_ocrResultPresenter.Verify(p => p.Present(It.IsAny<string>(), It.IsAny<Rect>()), Times.Never);
	}

	[Fact]
	public async Task Given_OcrFailsWithEmptyError_When_ShowOcrForSelectionAsync_Then_DoesNotPresentAnything()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(10, 10, 100, 50);
		var       anchor    = new Rect(10, 10, 100, 50);

		_baseOcrService.Setup(o => o.RecognizeAsync(bitmap, selection, "deu", OcrEngineType.Tesseract))
			.ReturnsAsync(new OcrResultDataItemModel(null, ""));

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "deu", OcrEngineType.Tesseract);

		_ocrResultPresenter.Verify(p => p.Present(It.IsAny<string>(), It.IsAny<Rect>()), Times.Never);
	}

	[Fact]
	public async Task Given_OcrReturnsText_When_ShowOcrForSelectionAsync_Then_PresentsText()
	{
		using var bitmap    = new SKBitmap(200, 200);
		var       selection = new Rect(10, 10, 100, 50);
		var       anchor    = new Rect(10, 10, 100, 50);

		_baseOcrService.Setup(o => o.RecognizeAsync(bitmap, selection, "jpn", OcrEngineType.EasyOcr))
			.ReturnsAsync(new OcrResultDataItemModel("Hello", null));
		_dispatcherFacade.Setup(d => d.InvokeAsync(It.IsAny<Action>()))
			.Callback<Action>(a => a())
			.Returns(Task.CompletedTask);
		_ocrResultPresenter.Setup(p => p.Present("Hello", anchor));

		await _sut.ShowOcrForSelectionAsync(bitmap, selection, anchor, "jpn", OcrEngineType.EasyOcr);

		_ocrResultPresenter.Verify(p => p.Present("Hello", anchor), Times.Once);
	}

	#endregion
}
