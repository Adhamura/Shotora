using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class AnnotationItem
{
	public Guid Id { get; } = Guid.NewGuid();

	public AnnotationToolType Tool { get; init; }

	public Color Color { get; set; } = Color.Red;

	public float Thickness { get; init; } = 3f;

	public float Opacity { get; init; } = 1f;

	public RectangleF Bounds { get; set; }

	public bool Fill { get; init; }

	public string? Text { get; set; }

	public string FontFamily { get; set; } = "Segoe UI";

	public float FontSize { get; set; } = 16f;

	public bool Bold { get; set; } = true;

	public bool Italic { get; set; }

	public IList<PointF> Points { get; set; } = new List<PointF>();

	public float ArrowHeadSize { get; init; } = 10f;
}
