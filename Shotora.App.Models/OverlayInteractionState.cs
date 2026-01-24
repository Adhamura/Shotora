using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ItemModels;
using AvaloniaPoint=Avalonia.Point;

namespace Shotora.App.Models;

[ExcludeFromCodeCoverage]
public class OverlayInteractionState
{
	public Dictionary<Control, List<Control>>    ArrowGroups          { get; }      = new();
	public Dictionary<EditorIcon, Control>       IconControls         { get; }      = new();
	public Dictionary<OverlayTool, ToggleButton> ToolButtons          { get; }      = new();
	public Stack<List<AnnotationItem>>           UndoStack            { get; }      = new();
	public Stack<List<AnnotationItem>>           RedoStack            { get; }      = new();
	public Dictionary<Control, AnnotationItem>   VisualToAnnotation   { get; }      = new();
	public CaptureMode                           CaptureMode          { get; set; } = CaptureMode.Region;
	public string?                               ActiveHandle         { get; set; }
	public CaptureResultItemModel?               Capture              { get; set; }
	public MouseButton?                          CopyCloseMouseButton { get; set; }
	public Grid?                                 EffectPreviewGrid    { get; set; }
	public Image?                                EffectPreviewImage   { get; set; }
	public List<AnnotationItem>                  Annotations          { get; } = [];

	public bool IsDrawing       { get; set; }
	public bool IsMoving        { get; set; }
	public bool IsPanelDragging { get; set; }
	public bool IsResizing      { get; set; }
	public bool IsSelecting     { get; set; }
	public bool IsShapeDragging { get; set; }
	public bool IsUpdatingTool  { get; set; }

	public AnnotationItem? SelectedAnnotation  { get; set; }
	public Control?        SelectedVisual      { get; set; }
	public Rect            SelectionRect       { get; set; }
	public Color           CurrentColor        { get; set; } = Colors.Lime;
	public Polyline?       CurrentPolyline     { get; set; }
	public Shape?          CurrentPreviewShape { get; set; }
	public OverlayTool     CurrentTool         { get; set; } = OverlayTool.Selection;
	public AvaloniaPoint   DrawStart           { get; set; }
	public AvaloniaPoint   PanelDragStart      { get; set; }
	public Rect            ResizeStartRect     { get; set; }
	public AvaloniaPoint   SelectionStart      { get; set; }
	public AvaloniaPoint   ShapeDragStart      { get; set; }
	public Vector          ShapeInitialOffset  { get; set; }
	public AvaloniaPoint   MoveOffset          { get; set; }
	public MouseButton?    MoveSelectionButton { get; set; }
	public double          Thickness           { get; set; } = 6;
	public AppSettings     Settings            { get; set; } = new();
	public bool HasSelection => SelectionRect is
	{
		Width: > 0, Height: > 0
	};
}
