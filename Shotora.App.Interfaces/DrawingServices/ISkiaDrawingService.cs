using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Shotora.App.Models;
using Shotora.App.Models.Drawings;
using Shotora.App.Models.Enums;
using SkiaSharp;
using DrawingColor=System.Drawing.Color;
using AvaloniaImage=Avalonia.Controls.Image;
using AvaloniaPoint=Avalonia.Point;

namespace Shotora.App.Interfaces.DrawingServices;

public interface ISkiaDrawingService
{
	void          DrawProcessedRegion(SKCanvas       canvas,   SKBitmap  baseBitmap, AnnotationItem annotation);
	SKColor       ToSkColor(DrawingColor             color,    float     opacity = 1f);
	EffectPreview CreateEffectPreview(AvaloniaPoint  position, IBrush    strokeBrush, double thickness);
	bool          UpdateEffectPreviewBounds(Grid     grid,     Rect      bounds);
	void          UpdateEffectPreview(AvaloniaImage? image,    SKBitmap? captureRaw, Rect bounds, OverlayTool tool, double thickness);
}
