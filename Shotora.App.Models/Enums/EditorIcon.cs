using System.ComponentModel;

namespace Shotora.App.Models.Enums;

public enum EditorIcon
{
	[Description("Undo")]
	Undo,
	[Description("Redo")]
	Redo,
	[Description("Move")]
	Move,
	[Description("Pen")]
	Pen,
	[Description("Line")]
	Line,
	[Description("Arrow")]
	Arrow,
	[Description("Rectangle")]
	Rectangle,
	[Description("Ellipse")]
	Ellipse,
	[Description("Text")]
	Text,
	[Description("Highlight")]
	Highlight,
	[Description("Blur")]
	Blur,
	[Description("Pixelate")]
	Pixelate,
	[Description("Save")]
	Save,
	[Description("Copy")]
	Copy,
	[Description("Cancel")]
	Cancel
}
