using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Shotora.App.Interfaces.Abstractions;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Interfaces.Facades;
using Shotora.App.Interfaces.Ocr;
using Shotora.App.Models;
using Shotora.App.Models.Enums;
using SkiaSharp;
using DrawingColor=System.Drawing.Color;
using DrawingRectangleF=System.Drawing.RectangleF;

namespace Shotora.App.Services.DrawingServices;

public class TextDrawingService(
	ITextInputDialogService dialogService,
	IBaseOcrService         baseOcrService,
	IOcrResultPresenter     ocrResultPresenter,
	IDispatcherFacade       dispatcherFacade) : ITextDrawingService
{
	public void DrawText(SKCanvas canvas, AnnotationItem annotation, SKColor skColor)
	{
		if (string.IsNullOrWhiteSpace(annotation.Text))
		{
			return;
		}

		var typeface = SKTypeface.FromFamilyName(
			annotation.FontFamily,
			annotation.Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
			SKFontStyleWidth.Normal,
			annotation.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);

		using var paint = new SKPaint
		{
			IsAntialias = true,
			Color       = skColor,
			TextSize    = annotation.FontSize > 0 ? annotation.FontSize : 16,
			Typeface    = typeface
		};

		var metrics = paint.FontMetrics;
		var textX   = annotation.Bounds.X;
		var textY   = annotation.Bounds.Y - metrics.Top;
		canvas.DrawText(annotation.Text, textX, textY, paint);
	}

	public TextBlock CreateTextVisual(AnnotationItem annotation, Color color)
	{
		var fontSize = annotation.FontSize > 0 ? annotation.FontSize : 16;
		var text     = CreateTextVisual(annotation.Text ?? string.Empty, annotation.FontFamily, fontSize, annotation.Bold, annotation.Italic, color);
		var size     = MeasureTextVisual(text);
		text.Width  = size.Width;
		text.Height = size.Height;
		Canvas.SetLeft(text, annotation.Bounds.X);
		Canvas.SetTop(text, annotation.Bounds.Y);
		return text;
	}

	public async Task<(Control Visual, AnnotationItem Annotation)?> CreateTextAnnotationAsync(Window owner, double thickness, Color color, Point position)
	{
		var dialogResult = await dialogService.ShowAsync(owner, thickness * 4);
		if (dialogResult == null || string.IsNullOrWhiteSpace(dialogResult.Text))
		{
			return null;
		}

		return CreateTextAnnotation(
			dialogResult.Text,
			dialogResult.FontFamily,
			dialogResult.FontSize,
			dialogResult.IsBold,
			dialogResult.IsItalic,
			color,
			thickness,
			position);
	}

	public async Task<bool> ApplyTextEditAsync(Window owner, Control visual, AnnotationItem annotation, double thickness)
	{
		var dialogResult = await dialogService.ShowAsync(owner, thickness * 4, annotation);
		if (dialogResult == null || string.IsNullOrWhiteSpace(dialogResult.Text))
		{
			return false;
		}

		return ApplyTextEdit(visual, annotation, dialogResult.Text, dialogResult.FontFamily, dialogResult.FontSize, dialogResult.IsBold, dialogResult.IsItalic);
	}

	public void ApplyTranslation(Control visual, AnnotationItem annotation, double dx, double dy)
	{
		if (visual is not TextBlock text)
		{
			return;
		}

		var textLeft = double.IsNaN(Canvas.GetLeft(text)) ? 0 : Canvas.GetLeft(text);
		var textTop  = double.IsNaN(Canvas.GetTop(text)) ? 0 : Canvas.GetTop(text);
		Canvas.SetLeft(text, textLeft + dx);
		Canvas.SetTop(text, textTop   + dy);
		annotation.Bounds = annotation.Bounds with
		{
			X = annotation.Bounds.X + (float)dx,
			Y = annotation.Bounds.Y + (float)dy
		};
	}

	public async Task ShowOcrForSelectionAsync(SKBitmap capture, Rect selection, Rect anchor, string languages, OcrEngineType engine)
	{
		if (capture == null || selection.Width < 4 || selection.Height < 4)
		{
			return;
		}

		var result = await baseOcrService.RecognizeAsync(capture, selection, languages, engine);
		if (!result.Success)
		{
			if (!string.IsNullOrWhiteSpace(result.Error))
			{
				var errorText = $"OCR error: {result.Error}, {result.Text}";
				await dispatcherFacade.InvokeAsync(() => ocrResultPresenter.Present(errorText, anchor));
			}
			return;
		}

		var text = result.Text ?? string.Empty;
		await dispatcherFacade.InvokeAsync(() => ocrResultPresenter.Present(text, anchor));
	}

	public void CloseOcrWindow()
	{
		ocrResultPresenter.Close();
	}

	private static TextBlock CreateTextVisual(string text, string fontFamily, double fontSize, bool bold, bool italic, Color color)
	{
		return new TextBlock
		{
			Text             = text,
			FontFamily       = new FontFamily(fontFamily),
			FontSize         = fontSize,
			FontWeight       = bold ? FontWeight.Bold : FontWeight.Normal,
			FontStyle        = italic ? FontStyle.Italic : FontStyle.Normal,
			Foreground       = new SolidColorBrush(color),
			TextWrapping     = TextWrapping.NoWrap,
			IsHitTestVisible = true
		};
	}

	private static Size MeasureTextVisual(TextBlock textBlock)
	{
		textBlock.Measure(Size.Infinity);
		return textBlock.DesiredSize;
	}

	private static (Control Visual, AnnotationItem Annotation) CreateTextAnnotation(string text, string fontFamily, double fontSize, bool bold, bool italic, Color color, double thickness, Point position)
	{
		var visual = CreateTextVisual(text, fontFamily, fontSize, bold, italic, color);
		var size   = MeasureTextVisual(visual);
		Canvas.SetLeft(visual, position.X);
		Canvas.SetTop(visual, position.Y);
		visual.Width  = size.Width;
		visual.Height = size.Height;

		var drawingColor = DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
		var annotationItem = new AnnotationItem
		{
			Tool      = AnnotationToolType.Text,
			Text      = text,
			Color     = drawingColor,
			Thickness = (float)thickness,
			Opacity   = 1f,
			Bounds = new DrawingRectangleF((float)position.X, (float)position.Y,
				(float)Math.Max(4, size.Width), (float)Math.Max(4, size.Height)),
			FontFamily = fontFamily,
			FontSize   = (float)fontSize,
			Bold       = bold,
			Italic     = italic
		};

		return (visual, annotationItem);
	}

	private static bool ApplyTextEdit(Control visual, AnnotationItem annotation, string text, string fontFamily, double fontSize, bool bold, bool italic)
	{
		annotation.Text       = text;
		annotation.FontFamily = fontFamily;
		annotation.FontSize   = (float)fontSize;
		annotation.Bold       = bold;
		annotation.Italic     = italic;

		if (visual is TextBlock textBlock)
		{
			textBlock.Text       = text;
			textBlock.FontFamily = new FontFamily(fontFamily);
			textBlock.FontSize   = fontSize;
			textBlock.FontWeight = bold ? FontWeight.Bold : FontWeight.Normal;
			textBlock.FontStyle  = italic ? FontStyle.Italic : FontStyle.Normal;

			var size = MeasureTextVisual(textBlock);
			textBlock.Width  = size.Width;
			textBlock.Height = size.Height;

			var left = double.IsNaN(Canvas.GetLeft(textBlock)) ? 0 : Canvas.GetLeft(textBlock);
			var top  = double.IsNaN(Canvas.GetTop(textBlock)) ? 0 : Canvas.GetTop(textBlock);
			annotation.Bounds = new DrawingRectangleF((float)left, (float)top,
				(float)Math.Max(4, size.Width), (float)Math.Max(4, size.Height));
		}

		return true;
	}
}
