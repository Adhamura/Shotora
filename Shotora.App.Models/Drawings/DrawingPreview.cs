using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;

namespace Shotora.App.Models.Drawings;

[ExcludeFromCodeCoverage]
public readonly record struct DrawingPreview(
	Polyline?  Polyline,
	Shape?     PreviewShape,
	Grid?      EffectGrid,
	Image?     EffectImage,
	Rectangle? EffectBorder);
