using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Shotora.App.Models.Enums;

namespace Shotora.App.Models.ViewModels;

[ExcludeFromCodeCoverage]
public class OverlayControlsModel
{
	public Button  UndoButton       { get; private set; } = new();
	public Button  RedoButton       { get; private set; } = new();
	public Button  CopyButton       { get; set; }         = new();
	public Button  SaveButton       { get; set; }         = new();
	public Border  SelectionBorder  { get; private set; } = new();
	public Canvas  HandleCanvas     { get; private set; } = new();
	public Canvas  AnnotationCanvas { get; private set; } = new();
	public Canvas  PreviewCanvas    { get; private set; } = new();
	public Slider  ThicknessSlider  { get; private set; } = new();
	public Control ThicknessPanel   { get; private set; } = new();
	public Control ColorsPanel      { get; private set; } = new();

	public void Init(Button  undoButton,
					 Button  redoButton,
					 Button  copyButton,
					 Button  saveButton,
					 Border  selectionBorder,
					 Canvas  handleCanvas,
					 Canvas  annotationCanvas,
					 Canvas  previewCanvas,
					 Slider  thicknessSlider,
					 Control thicknessPanel,
					 Control colorsPanel)
	{
		UndoButton       = undoButton;
		RedoButton       = redoButton;
		CopyButton       = copyButton;
		SaveButton       = saveButton;
		SelectionBorder  = selectionBorder;
		HandleCanvas     = handleCanvas;
		AnnotationCanvas = annotationCanvas;
		PreviewCanvas    = previewCanvas;
		ThicknessSlider  = thicknessSlider;
		ThicknessPanel   = thicknessPanel;
		ColorsPanel      = colorsPanel;
	}
	public void ApplySelectionState(bool hasSelection)
	{
		SelectionBorder?.IsVisible = hasSelection;
		HandleCanvas?.IsVisible    = hasSelection;

		CopyButton?.IsEnabled = hasSelection;
		SaveButton?.IsEnabled = hasSelection;
	}

	public static void ApplyToolEnablement(OverlayInteractionState state, bool hasSelection)
	{
		foreach (var (tool, button) in state.ToolButtons)
		{
			button.IsEnabled = button.IsVisible && (tool is OverlayTool.Selection or OverlayTool.Pointer || hasSelection);
			if (button is
				{
					IsChecked: true, IsEnabled: false
				})
			{
				button.IsChecked = false;
			}
		}
	}

	public void UpdateUndoRedo(bool hasSelection, bool canUndo, bool canRedo)
	{
		UndoButton?.IsEnabled = hasSelection && canUndo;
		RedoButton?.IsEnabled = hasSelection && canRedo;
	}
}
