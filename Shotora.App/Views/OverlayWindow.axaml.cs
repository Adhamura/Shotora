using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shotora.App.Controls;
using Shotora.App.Interfaces.Adapters;
using Shotora.App.Interfaces.Builders;
using Shotora.App.Interfaces.DrawingServices;
using Shotora.App.Interfaces.PeripheralServices;
using Shotora.App.Interfaces.Providers;
using Shotora.App.Interfaces.System;
using Shotora.App.Models;
using Shotora.App.Models.Constants;
using Shotora.App.Models.Enums;
using Shotora.App.Models.ViewModels;
using Shotora.App.Services.System;
using SkiaSharp;
using AvaloniaColor=Avalonia.Media.Color;
using AvaloniaColors=Avalonia.Media.Colors;
using AvaloniaPoint=Avalonia.Point;
using DrawingColor=System.Drawing.Color;
using DrawingPointF=System.Drawing.PointF;

namespace Shotora.App.Views;

[ExcludeFromCodeCoverage]
public partial class OverlayWindow(
	IScreenshotService          screenshotService,
	ISettingsSystemService      settingsSystemService,
	IMouseInteractionService    mouseInteractionService,
	IRectangleDrawingService    rectangles,
	IPathBuilder                pathBuilder,
	IAnnotationItemBuilder      annotationBuilder,
	IAnnotationDrawingService   annotationDrawer,
	IArrowDrawingService        arrowDrawingService,
	ITranslationDrawingService  translationsDrawing,
	ITextDrawingService         textDrawingService,
	ISaveDrawingService         saveDrawingService,
	IKeyboardInteractionService keyboardInteractionService,
	IAvaloniaEnumAdapter        avaloniaEnumAdapter,
	IIconVisibilityProvider     iconVisibilityProvider) : Window
{
	private static readonly Cursor               ArrowCursor    = new(StandardCursorType.Arrow);
	private static readonly Cursor               DragCursor     = new(StandardCursorType.SizeAll);
	private readonly        OverlayControlsModel _controlsModel = new();

	private readonly OverlayInteractionState _state = new();

	public async Task InitializeAsync(CaptureMode mode)
	{
		EnsureLoaded();
		_state.Settings    = await settingsSystemService.LoadAsync();
		_state.CaptureMode = mode;
		ApplyIconVisibility();
		ApplyEditorSettings();
		_state.CopyCloseMouseButton = mouseInteractionService.ParseMouseButton(_state.Settings.EditorCopyCloseMouseButton,     MouseButton.Right);
		_state.MoveSelectionButton  = mouseInteractionService.ParseMouseButton(_state.Settings.EditorMoveSelectionMouseButton, MouseButton.Right);
		SetTool(OverlayTool.Selection);

		_controlsModel.AnnotationCanvas.Children.Clear();
		_controlsModel.PreviewCanvas.Children.Clear();
		_state.Annotations.Clear();
		_state.VisualToAnnotation.Clear();
		_state.ArrowGroups.Clear();
		_state.SelectedVisual     = null;
		_state.SelectedAnnotation = null;
		_state.UndoStack.Clear();
		_state.RedoStack.Clear();
		_state.UndoStack.Push(CloneAnnotations(_state.Annotations));
		UpdateUndoRedoButtons();

		var bounds = rectangles.GetVirtualBounds(Screens.Primary, Screens.All, Width, Height);
		_state.Capture = await screenshotService.CaptureAsync(bounds);
		if (_state.Capture == null)
		{
			throw new InvalidOperationException("Unable to capture the screen.");
		}

		BackgroundImage.Source                 = _state.Capture.Display;
		OverlayCanvas.Width                    = _state.Capture.Display.Size.Width;
		OverlayCanvas.Height                   = _state.Capture.Display.Size.Height;
		_controlsModel.AnnotationCanvas.Width  = OverlayCanvas.Width;
		_controlsModel.AnnotationCanvas.Height = OverlayCanvas.Height;
		_controlsModel.PreviewCanvas.Width     = OverlayCanvas.Width;
		_controlsModel.PreviewCanvas.Height    = OverlayCanvas.Height;
		Width                                  = _state.Capture.Display.Size.Width;
		Height                                 = _state.Capture.Display.Size.Height;

		Position = bounds.Position;

		if (mode == CaptureMode.Fullscreen)
		{
			_state.SelectionRect = new Rect(0, 0, _state.Capture.Display.Size.Width, _state.Capture.Display.Size.Height);
			UpdateSelectionVisuals();
		}
		else
		{
			if (!TryRestoreSavedSelection())
			{
				ClearSelection();
			}
		}

		Focus();
	}

	private void OnSettingsChanged(object? sender, AppSettings e)
	{
		_state.Settings = e;
		Dispatcher.UIThread.Post(() =>
		{
			ApplyIconVisibility();
			ApplyEditorSettings();
			_state.CopyCloseMouseButton = mouseInteractionService.ParseMouseButton(_state.Settings.EditorCopyCloseMouseButton,     MouseButton.Right);
			_state.MoveSelectionButton  = mouseInteractionService.ParseMouseButton(_state.Settings.EditorMoveSelectionMouseButton, MouseButton.Right);
		});
	}

	private void EnsureLoaded()
	{
		if (Content != null)
		{
			return;
		}

		InitializeComponent();
		EnsureControlsModel();
		rectangles.InitializeAnnotationCanvas(_controlsModel.AnnotationCanvas);
		avaloniaEnumAdapter.EnsureControls(this, _state.ToolButtons,  tool => $"{tool}Button");
		avaloniaEnumAdapter.EnsureControls(this, _state.IconControls, icon => $"{icon}Button");

		settingsSystemService.SettingsChanged += OnSettingsChanged;
	}

	public void ApplySettings(AppSettings settings)
	{
		_state.Settings = settings;
		ApplyIconVisibility();
		ApplyEditorSettings();
		_state.CopyCloseMouseButton = mouseInteractionService.ParseMouseButton(_state.Settings.EditorCopyCloseMouseButton,     MouseButton.Right);
		_state.MoveSelectionButton  = mouseInteractionService.ParseMouseButton(_state.Settings.EditorMoveSelectionMouseButton, MouseButton.Right);
	}

	private void ApplyEditorSettings()
	{
		if (!string.IsNullOrWhiteSpace(_state.Settings.EditorDefaultColor))
		{
			try
			{
				_state.CurrentColor = AvaloniaColor.Parse(_state.Settings.EditorDefaultColor);
				ColorPicker.Color   = _state.CurrentColor;
			}
			catch
			{
				_state.CurrentColor = AvaloniaColors.Lime;
			}
		}
		_state.Thickness                         = _state.Settings.EditorDefaultThickness > 0 ? _state.Settings.EditorDefaultThickness : 6;
		_controlsModel.ThicknessSlider.Value     = _state.Thickness;
		_controlsModel.ThicknessPanel?.IsVisible = _state.Settings.EditorShowThicknessSlider;
		_controlsModel.ColorsPanel?.IsVisible    = _state.Settings.EditorShowColorPalette;

		InitializeColorPalette();
	}

	private void InitializeColorPalette()
	{
		ColorPalette.Children.Clear();
		var colors = new[]
		{
			AvaloniaColors.Lime, AvaloniaColors.Red, AvaloniaColors.Orange, AvaloniaColors.Yellow, AvaloniaColors.Cyan, AvaloniaColors.Blue, AvaloniaColors.Magenta, AvaloniaColors.White, AvaloniaColors.Black
		};

		foreach (var color in colors)
		{
			var swatch = new Button
			{
				Width           = 22,
				Height          = 22,
				Margin          = new Thickness(2),
				Background      = new SolidColorBrush(color),
				BorderBrush     = Brushes.Transparent,
				BorderThickness = new Thickness(1)
			};
			swatch.Click += (_, _) => SetCurrentColor(color);
			ColorPalette.Children.Add(swatch);
		}
	}

	private void SetCurrentColor(AvaloniaColor color)
	{
		_state.CurrentColor = color;
		ColorPicker.Color   = color;

		if (_state.SelectedAnnotation == null)
		{
			return;
		}
		_state.SelectedAnnotation.Color = DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
		annotationDrawer.RefreshSelectedAnnotationVisual(_state);
		RecordState();
	}

	private void OnToolChecked(object? sender, RoutedEventArgs e)
	{
		if (_state.IsUpdatingTool)
		{
			return;
		}

		if (sender is not ToggleButton
			{
				Tag: string tag, IsChecked: true
			})
		{
			return;
		}

		if (Enum.TryParse<OverlayTool>(tag, out var parsed))
		{
			SetTool(parsed);
		}
	}

	private void SetTool(OverlayTool tool)
	{
		avaloniaEnumAdapter.EnsureControls(this, _state.ToolButtons, t => $"{t}Button");
		_state.IsUpdatingTool = true;
		_state.CurrentTool    = tool;
		try
		{
			foreach (var (mappedTool, button) in _state.ToolButtons)
			{
				button.IsChecked = mappedTool == tool;
			}
		}
		finally
		{
			_state.IsUpdatingTool = false;
		}

		if (tool != OverlayTool.Pointer)
		{
			_state.SelectedVisual     = null;
			_state.SelectedAnnotation = null;
		}

		OverlayCanvas.Cursor = tool switch
		{
			OverlayTool.Move    => new Cursor(StandardCursorType.SizeAll),
			OverlayTool.Pointer => new Cursor(StandardCursorType.Arrow),
			OverlayTool.Text    => new Cursor(StandardCursorType.Ibeam),
			_                   => new Cursor(StandardCursorType.Cross)
		};
	}

	private void ApplyIconVisibility()
	{
		avaloniaEnumAdapter.EnsureControls(this, _state.IconControls, icon => $"{icon}Button");
		avaloniaEnumAdapter.EnsureControls(this, _state.ToolButtons,  tool => $"{tool}Button");

		var visibility  = iconVisibilityProvider.BuildVisibilityMap(_state.Settings.EditorHiddenIcons);
		var currentTool = _state.CurrentTool;

		foreach (var icon in Enum.GetValues<EditorIcon>())
		{
			var isVisible = visibility.GetValueOrDefault(icon, true);
			if (_state.IconControls.TryGetValue(icon, out var control))
			{
				control.IsVisible = isVisible;
			}

			if (Enum.TryParse<OverlayTool>(icon.ToString(), out var tool) &&
				_state.ToolButtons.TryGetValue(tool, out var button))
			{
				button.IsVisible = isVisible;
				button.IsEnabled = isVisible && (tool is OverlayTool.Selection or OverlayTool.Pointer || _state.HasSelection);
				if (!isVisible && button.IsChecked == true)
				{
					button.IsChecked = false;
				}

				if (!isVisible && currentTool == tool)
				{
					currentTool = OverlayTool.Selection;
				}
			}
		}

		if (currentTool != _state.CurrentTool)
		{
			SetTool(currentTool);
		}

		ApplyToolEnablement(_state.HasSelection);
	}

	private void EnsureControlsModel()
	{
		_controlsModel.Init(
			UndoButton,
			RedoButton,
			CopyButton,
			SaveButton,
			SelectionBorder,
			HandleCanvas,
			AnnotationCanvas,
			PreviewCanvas,
			ThicknessSlider,
			ThicknessPanel,
			ColorsPanel);

		WireControlEvents();
	}

	private void WireControlEvents()
	{
		_controlsModel.UndoButton?.Click += (_, _) => Undo();
		_controlsModel.RedoButton?.Click += (_, _) => Redo();
	}

	private void ApplyToolEnablement(bool hasSelection)
	{
		_controlsModel.UpdateUndoRedo(hasSelection, _state.UndoStack.Count > 0, _state.RedoStack.Count > 0);
		OverlayControlsModel.ApplyToolEnablement(_state, hasSelection);
	}

	private async void OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (_state.Capture == null)
		{
			return;
		}

		if (ColorPopup.IsOpen)
		{
			ColorPopup.IsOpen = false;
		}

		var pos    = e.GetPosition(OverlayCanvas);
		var button = mouseInteractionService.GetReleasedButton(OverlayCanvas, e);
		var inside = _state.SelectionRect.Contains(pos);
		CoordinateLabel.Text = $"({(int)pos.X}, {(int)pos.Y}) {(int)_state.SelectionRect.Width}x{(int)_state.SelectionRect.Height}";

		if (_state is
			{
				IsDrawing: false, IsResizing: false, IsShapeDragging: false, IsSelecting: false
			}      &&
			inside &&
			_state.MoveSelectionButton is
			{
			} moveBtn &&
			moveBtn == button)
		{
			_state.IsMoving   = true;
			_state.MoveOffset = new AvaloniaPoint(pos.X - _state.SelectionRect.X, pos.Y - _state.SelectionRect.Y);
			e.Pointer.Capture(OverlayCanvas);
			return;
		}

		if (_state.CurrentTool == OverlayTool.Pointer)
		{
			var hit = annotationDrawer.GetTopMostAnnotationAt(_controlsModel.AnnotationCanvas, pos, _state.ArrowGroups);
			if (hit != null)
			{
				hit                       = ResolveArrowContainer(hit);
				_state.SelectedVisual     = hit;
				_state.SelectedAnnotation = _state.VisualToAnnotation.GetValueOrDefault(hit);

				if (e.ClickCount >= 2 && _state.SelectedAnnotation?.Tool == AnnotationToolType.Text)
				{
					await EditTextAnnotationAsync(hit, _state.SelectedAnnotation);
					return;
				}

				_state.IsShapeDragging    = true;
				_state.ShapeDragStart     = pos;
				_state.ShapeInitialOffset = translationsDrawing.GetTranslation(hit);
				rectangles.UpdateAnnotationHighlight(_state);
				e.Pointer.Capture(OverlayCanvas);
				return;
			}

			_state.SelectedVisual     = null;
			_state.SelectedAnnotation = null;
			rectangles.UpdateAnnotationHighlight(_state);
		}

		switch (_state.CurrentTool)
		{
			case OverlayTool.Move or OverlayTool.Pointer when inside:
				_state.IsMoving   = true;
				_state.MoveOffset = new AvaloniaPoint(pos.X - _state.SelectionRect.X, pos.Y - _state.SelectionRect.Y);
				e.Pointer.Capture(OverlayCanvas);
				return;
			case OverlayTool.Selection:
				_state.SelectionStart = pos;
				_state.IsSelecting    = true;
				e.Pointer.Capture(OverlayCanvas);
				_state.SelectionRect = new Rect();
				UpdateSelectionVisuals();
				break;
			case OverlayTool.Text:
				await HandleTextPlacementAsync(pos);
				break;
			default:
			{
				if (DrawingTool.IsDrawingTool(_state.CurrentTool))
				{
					_state.IsDrawing = true;
					_state.DrawStart = pos;
					e.Pointer.Capture(OverlayCanvas);
					BeginDrawing(pos);
				}
				break;
			}
		}
	}

	private void OnPointerMoved(object? sender, PointerEventArgs e)
	{
		if (_state.Capture == null)
		{
			return;
		}

		var pos = e.GetPosition(OverlayCanvas);
		CoordinateLabel.Text = $"({(int)pos.X}, {(int)pos.Y}) {(int)_state.SelectionRect.Width}x{(int)_state.SelectionRect.Height}";
		var captureSize = _state.Capture.Display.Size;

		if (_state is
			{
				IsShapeDragging: true, SelectedVisual: Control selected
			})
		{
			var delta = pos                         - _state.ShapeDragStart;
			var tx    = _state.ShapeInitialOffset.X + delta.X;
			var ty    = _state.ShapeInitialOffset.Y + delta.Y;

			selected.RenderTransform = _state.ArrowGroups.TryGetValue(selected, out _) ? new TranslateTransform(tx, ty) : new TranslateTransform(tx, ty);
			rectangles.UpdateAnnotationHighlight(_state);
			return;
		}

		if (_state.IsSelecting)
		{
			_state.SelectionRect = rectangles.ClampToBounds(rectangles.NormalizeRect(_state.SelectionStart, pos), captureSize);
			UpdateSelectionVisuals();
			return;
		}

		if (_state is
			{
				IsMoving: true, HasSelection: true
			})
		{
			var newLeft = pos.X - _state.MoveOffset.X;
			var newTop  = pos.Y - _state.MoveOffset.Y;
			_state.SelectionRect = rectangles.ClampToBounds(new Rect(newLeft, newTop, _state.SelectionRect.Width, _state.SelectionRect.Height), captureSize);
			UpdateSelectionVisuals();
		}

		if (_state.IsDrawing)
		{
			annotationDrawer.UpdateDrawingPreview(_state, pos);
		}
	}

	private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		var selectionCompleted = false;
		if (_state.IsShapeDragging)
		{
			_state.IsShapeDragging = false;
			if (_state.SelectedVisual is Control selected)
			{
				ApplyShapeTranslation(selected);
			}
			rectangles.UpdateAnnotationHighlight(_state);
			e.Pointer.Capture(null);
		}

		if (_state.IsSelecting)
		{
			_state.IsSelecting = false;
			e.Pointer.Capture(null);
			selectionCompleted = true;
		}

		if (_state.IsMoving)
		{
			_state.IsMoving = false;
			e.Pointer.Capture(null);
		}

		if (_state.IsDrawing)
		{
			var pos = e.GetPosition(OverlayCanvas);
			FinishDrawing(pos);
			_state.IsDrawing = false;
			e.Pointer.Capture(null);
		}

		var button = mouseInteractionService.GetReleasedButton(OverlayCanvas, e);
		if (_state is
			{
				IsDrawing: false,
				IsSelecting: false,
				IsMoving: false,
				IsShapeDragging: false,
				IsResizing: false,
				CopyCloseMouseButton:
				{
				} copyBtn
			}                   &&
			copyBtn == button   &&
			_state.HasSelection &&
			_state.SelectionRect.Contains(e.GetPosition(OverlayCanvas)))
		{
			e.Pointer.Capture(null);
			_ = CopySelectionAsync(true);
		}

		if (selectionCompleted)
		{
			var capture = _state.Capture;
			if (capture?.Raw != null)
			{
				var anchor = new Rect(Position.X + _state.SelectionRect.X, Position.Y + _state.SelectionRect.Y, _state.SelectionRect.Width, _state.SelectionRect.Height);
				_ = textDrawingService.ShowOcrForSelectionAsync(capture.Raw, _state.SelectionRect, anchor, _state.Settings.OcrLanguages, _state.Settings.OcrEngine);
			}
		}
	}

	private async Task HandleTextPlacementAsync(AvaloniaPoint position)
	{
		var created = await textDrawingService.CreateTextAnnotationAsync(this, _state.Thickness, _state.CurrentColor, position);
		if (created == null)
		{
			return;
		}

		var (visual, annotationItem) = created.Value;
		_controlsModel.AnnotationCanvas.Children.Add(visual);

		_state.VisualToAnnotation[visual] = annotationItem;
		_state.Annotations.Add(annotationItem);
		RecordState();
	}

	private async Task EditTextAnnotationAsync(Control visual, AnnotationItem annotation)
	{
		var updated = await textDrawingService.ApplyTextEditAsync(this, visual, annotation, _state.Thickness);
		if (!updated)
		{
			return;
		}

		RecordState();
	}

	private void BeginDrawing(AvaloniaPoint start)
	{
		_state.Thickness = Math.Max(1, _state.Thickness);
		switch (_state.CurrentTool)
		{
			case OverlayTool.Pen:
			case OverlayTool.Highlight:
				_controlsModel.PreviewCanvas.Children.Clear();
				break;
		}

		var previewResult = annotationDrawer.BeginDrawing(_state.CurrentTool, start, _state.CurrentColor, _state.Thickness);
		_state.CurrentPolyline     = previewResult.Polyline;
		_state.CurrentPreviewShape = previewResult.PreviewShape;
		_state.EffectPreviewGrid   = previewResult.EffectGrid;
		_state.EffectPreviewImage  = previewResult.EffectImage;

		if (_state.CurrentPolyline != null)
		{
			_controlsModel.AnnotationCanvas.Children.Add(_state.CurrentPolyline);
		}
		else if (_state.CurrentPreviewShape != null)
		{
			_controlsModel.PreviewCanvas.Children.Add(_state.CurrentPreviewShape);
		}
		else if (_state.EffectPreviewGrid != null)
		{
			_controlsModel.PreviewCanvas.Children.Add(_state.EffectPreviewGrid);
		}
	}

	private void FinishDrawing(AvaloniaPoint end)
	{
		AnnotationItem? annotationItem = null;
		Control?        visualControl  = null;

		if (_state.CurrentPolyline != null)
		{
			if (_state.CurrentPolyline.Points.Count == 1)
			{
				_state.CurrentPolyline.Points.Add(end);
			}

			var strokeThickness = _state.CurrentTool == OverlayTool.Highlight ? _state.Thickness * 3 : _state.Thickness;
			var tool            = _state.CurrentTool == OverlayTool.Highlight ? AnnotationToolType.Highlight : AnnotationToolType.Pen;
			var opacity         = _state.CurrentTool == OverlayTool.Highlight ? 0.5f : 1f;
			annotationItem = annotationBuilder.BuildFreehand(tool, _state.CurrentColor, (float)strokeThickness, opacity, _state.CurrentPolyline!.Points);

			_state.CurrentPolyline.IsHitTestVisible = true;
			visualControl                           = _state.CurrentPolyline;
			_state.CurrentPolyline                  = null;
		}
		else if (_state.EffectPreviewGrid != null)
		{
			_controlsModel.PreviewCanvas.Children.Remove(_state.EffectPreviewGrid);
			if (_state.EffectPreviewGrid.Width > 0 && _state.EffectPreviewGrid.Height > 0)
			{
				_controlsModel.AnnotationCanvas.Children.Add(_state.EffectPreviewGrid);

				var bounds = rectangles.NormalizeRect(_state.DrawStart, end);
				annotationItem = annotationBuilder.BuildBounds(
					_state,
					_state.CurrentTool == OverlayTool.Blur ? AnnotationToolType.Blur : AnnotationToolType.Pixelate,
					bounds,
					true);
				visualControl = _state.EffectPreviewGrid;
			}
			_state.EffectPreviewGrid  = null;
			_state.EffectPreviewImage = null;
		}
		else if (_state.CurrentPreviewShape != null)
		{
			_controlsModel.PreviewCanvas.Children.Remove(_state.CurrentPreviewShape);

			if (_state is
				{
					CurrentTool: OverlayTool.Arrow, CurrentPreviewShape: Line arrowLine
				})
			{
				var arrowBounds = rectangles.GetBoundsFromPoints(arrowLine.StartPoint, arrowLine.EndPoint);
				annotationItem = annotationBuilder.BuildBounds(
					_state,
					AnnotationToolType.Arrow,
					arrowBounds,
					false,
					(float)(_state.Thickness * 4));
				annotationItem.Points = new List<DrawingPointF>
				{
					new((float)arrowLine.StartPoint.X, (float)arrowLine.StartPoint.Y),
					new((float)arrowLine.EndPoint.X, (float)arrowLine.EndPoint.Y)
				};

				var arrowContainer = annotationDrawer.CreateVisualForAnnotation(annotationItem, (container, parts) => _state.ArrowGroups[container] = parts);
				if (arrowContainer != null)
				{
					_controlsModel.AnnotationCanvas.Children.Add(arrowContainer);
					visualControl = arrowContainer;
				}
			}
			else
			{
				_controlsModel.AnnotationCanvas.Children.Add(_state.CurrentPreviewShape);

				var bounds = rectangles.NormalizeRect(_state.DrawStart, end);
				annotationItem = _state.CurrentTool switch
				{
					OverlayTool.Line      => annotationBuilder.BuildBounds(_state, AnnotationToolType.Line,      bounds),
					OverlayTool.Rectangle => annotationBuilder.BuildBounds(_state, AnnotationToolType.Rectangle, bounds),
					OverlayTool.Ellipse   => annotationBuilder.BuildBounds(_state, AnnotationToolType.Ellipse,   bounds),
					_                     => null
				};
				visualControl = _state.CurrentPreviewShape;
			}
			_state.CurrentPreviewShape = null;
		}

		if (visualControl != null && annotationItem != null)
		{
			_state.VisualToAnnotation[visualControl] = annotationItem;
			_state.Annotations.Add(annotationItem);
			RecordState();
			rectangles.UpdateAnnotationHighlight(_state);
		}
	}

	private void ResizeSelection(string handle, Vector delta)
	{
		var left   = _state.ResizeStartRect.Left;
		var top    = _state.ResizeStartRect.Top;
		var right  = _state.ResizeStartRect.Right;
		var bottom = _state.ResizeStartRect.Bottom;

		if (handle.Contains('W'))
		{
			left += delta.X;
		}
		if (handle.Contains('E'))
		{
			right += delta.X;
		}
		if (handle.Contains('N'))
		{
			top += delta.Y;
		}
		if (handle.Contains('S'))
		{
			bottom += delta.Y;
		}

		var clampedLeft   = Math.Clamp(left,   0, OverlayCanvas.Width);
		var clampedTop    = Math.Clamp(top,    0, OverlayCanvas.Height);
		var clampedRight  = Math.Clamp(right,  0, OverlayCanvas.Width);
		var clampedBottom = Math.Clamp(bottom, 0, OverlayCanvas.Height);

		var x      = Math.Min(clampedLeft, clampedRight);
		var y      = Math.Min(clampedTop,  clampedBottom);
		var width  = Math.Max(0, Math.Abs(clampedRight  - clampedLeft));
		var height = Math.Max(0, Math.Abs(clampedBottom - clampedTop));

		_state.SelectionRect = new Rect(x, y, width, height);
		UpdateSelectionVisuals();
	}

	private void UpdateSelectionVisuals()
	{
		var hasSelection = _state.HasSelection;
		_controlsModel.ApplySelectionState(hasSelection);
		ApplyToolEnablement(hasSelection);

		if (!hasSelection && _state.CurrentTool != OverlayTool.Selection && _state.CurrentTool != OverlayTool.Pointer)
		{
			SetTool(OverlayTool.Selection);
		}

		var outer = new Rect(0, 0, OverlayCanvas.Width, OverlayCanvas.Height);

		if (!hasSelection)
		{
			DimmerPath.Data = pathBuilder.CreateOuterDimmer(outer);
			return;
		}
		if (_controlsModel.SelectionBorder is
			{
			} selectionBorder)
		{
			Canvas.SetLeft(selectionBorder, _state.SelectionRect.X);
			Canvas.SetTop(selectionBorder, _state.SelectionRect.Y);
			selectionBorder.Width  = _state.SelectionRect.Width;
			selectionBorder.Height = _state.SelectionRect.Height;
		}

		UpdateHandlePositions();

		var inner = _state.SelectionRect;
		DimmerPath.Data = pathBuilder.CreateOuterWithHole(outer, inner);
	}

	private void UpdateHandlePositions()
	{
		const double size    = 12;
		var          left    = _state.SelectionRect.X                                   - size / 2;
		var          top     = _state.SelectionRect.Y                                   - size / 2;
		var          right   = _state.SelectionRect.Right                               - size / 2;
		var          bottom  = _state.SelectionRect.Bottom                              - size / 2;
		var          centerX = _state.SelectionRect.X + _state.SelectionRect.Width  / 2 - size / 2;
		var          centerY = _state.SelectionRect.Y + _state.SelectionRect.Height / 2 - size / 2;

		Canvas.SetLeft(HandleNw, left);
		Canvas.SetTop(HandleNw, top);

		Canvas.SetLeft(HandleN, centerX);
		Canvas.SetTop(HandleN, top);

		Canvas.SetLeft(HandleNe, right);
		Canvas.SetTop(HandleNe, top);

		Canvas.SetLeft(HandleE, right);
		Canvas.SetTop(HandleE, centerY);

		Canvas.SetLeft(HandleSe, right);
		Canvas.SetTop(HandleSe, bottom);

		Canvas.SetLeft(HandleS, centerX);
		Canvas.SetTop(HandleS, bottom);

		Canvas.SetLeft(HandleSw, left);
		Canvas.SetTop(HandleSw, bottom);

		Canvas.SetLeft(HandleW, left);
		Canvas.SetTop(HandleW, centerY);
	}

	private async void OnCopyClicked(object? sender, RoutedEventArgs e)
	{
		if (_state.Capture == null)
		{
			return;
		}

		await CopySelectionAsync(true);
	}

	private async void OnSaveClicked(object? sender, RoutedEventArgs e)
	{
		if (_state.Capture == null)
		{
			return;
		}

		var saved = await SaveSelectionAsync();
		if (saved)
		{
			Close();
		}
	}

	private void OnCancelClicked(object? sender, RoutedEventArgs e)
	{
		Close();
	}

	private List<AnnotationItem> CloneAnnotations(IEnumerable<AnnotationItem> annotations)
	{
		return annotations.Select(annotationBuilder.CloneAnnotation).ToList();
	}

	private void RecordState()
	{
		_state.UndoStack.Push(CloneAnnotations(_state.Annotations));
		_state.RedoStack.Clear();
		UpdateUndoRedoButtons();
	}

	private void UpdateUndoRedoButtons()
	{
		_controlsModel.UpdateUndoRedo(_state.HasSelection, _state.UndoStack.Count > 1, _state.RedoStack.Count > 0);
	}

	private Control ResolveArrowContainer(Control hit)
	{
		foreach (var (container, parts) in _state.ArrowGroups)
		{
			if (parts.Contains(hit))
			{
				return container;
			}
		}

		return hit;
	}

	private void ApplyAnnotationState(List<AnnotationItem> state)
	{
		_state.Annotations.Clear();
		_state.VisualToAnnotation.Clear();
		_state.ArrowGroups.Clear();
		_controlsModel.AnnotationCanvas.Children.Clear();
		_state.SelectedAnnotation = null;
		_state.SelectedVisual     = null;

		foreach (var item in state)
		{
			_state.Annotations.Add(item);
			var visual = annotationDrawer.CreateVisualForAnnotation(item, (container, parts) => _state.ArrowGroups[container] = parts);
			if (visual != null)
			{
				_state.VisualToAnnotation[visual] = item;
				_controlsModel.AnnotationCanvas.Children.Add(visual);
			}
		}

		rectangles.UpdateAnnotationHighlight(_state);
		UpdateUndoRedoButtons();
	}

	private void Undo()
	{
		if (_state.UndoStack.Count <= 1)
		{
			return;
		}

		var current = CloneAnnotations(_state.Annotations);
		_state.RedoStack.Push(current);
		_state.UndoStack.Pop();
		var target = CloneAnnotations(_state.UndoStack.Peek());
		ApplyAnnotationState(target);
		UpdateUndoRedoButtons();
	}

	private void Redo()
	{
		if (_state.RedoStack.Count == 0)
		{
			return;
		}

		var next = CloneAnnotations(_state.RedoStack.Pop());
		_state.UndoStack.Push(CloneAnnotations(next));
		ApplyAnnotationState(next);
		UpdateUndoRedoButtons();
	}

	private async Task CopySelectionAsync(bool closeAfter = false)
	{
		if (_state.Capture == null)
		{
			return;
		}

		var copied = await saveDrawingService.CopySelectionAsync(CropAnnotated);
		if (copied && closeAfter)
		{
			Close();
		}
	}

	private Task<bool> SaveSelectionAsync(bool prompt = true)
	{
		return _state.Capture == null ? Task.FromResult(false) : saveDrawingService.SaveSelectionAsync(this, _state.Settings, CropAnnotated, prompt);
	}

	private async void OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (keyboardInteractionService.MatchesHotkey(_state.Settings.EditorUndoHotkey, e))
		{
			Undo();
			e.Handled = true;
			return;
		}
		if (keyboardInteractionService.MatchesHotkey(_state.Settings.EditorRedoHotkey, e))
		{
			Redo();
			e.Handled = true;
			return;
		}
		if (keyboardInteractionService.MatchesHotkey(_state.Settings.EditorCopyHotkey, e))
		{
			e.Handled = true;
			await CopySelectionAsync(true);
			return;
		}
		if (keyboardInteractionService.MatchesHotkey(_state.Settings.EditorSaveHotkey, e))
		{
			e.Handled = true;
			await SaveSelectionAsync();
			return;
		}

		if (e.Key == Key.Escape)
		{
			e.Handled = true;

			if (_state.CurrentTool != OverlayTool.Selection && _state.CurrentTool != OverlayTool.Pointer)
			{
				SetTool(OverlayTool.Selection);
				return;
			}

			Close();
		}
	}

	protected override void OnClosed(EventArgs e)
	{
		settingsSystemService.SettingsChanged -= OnSettingsChanged;

		PersistSelectionIfNeeded();

		_state.Capture?.Raw.Dispose();
		_state.Capture = null;
		textDrawingService.CloseOcrWindow();

		base.OnClosed(e);
	}

	private SKBitmap? CropAnnotated()
	{
		if (_state.Capture == null)
		{
			return null;
		}

		var annotated = annotationDrawer.BuildAnnotatedBitmap(_state.Capture.Raw, _state.Annotations);
		if (!_state.HasSelection)
		{
			return annotated;
		}

		var rect = new SKRectI(
			(int)Math.Round(_state.SelectionRect.X),
			(int)Math.Round(_state.SelectionRect.Y),
			(int)Math.Round(_state.SelectionRect.Right),
			(int)Math.Round(_state.SelectionRect.Bottom));
		var full = new SKRectI(0, 0, annotated.Width, annotated.Height);
		rect = rectangles.IntersectRect(rect, full);
		if (rect.Width <= 0 || rect.Height <= 0)
		{
			return annotated;
		}

		var cropped = new SKBitmap(rect.Width, rect.Height, annotated.ColorType, annotated.AlphaType);
		using (var canvas = new SKCanvas(cropped))
		{
			canvas.DrawBitmap(annotated, rect, new SKRect(0, 0, rect.Width, rect.Height));
		}
		annotated.Dispose();
		return cropped;
	}

	private bool TryRestoreSavedSelection()
	{
		if (_state.Capture == null)
		{
			return false;
		}

		if (_state.Settings == null || _state.Settings.LastSelectionWidth <= 0 || _state.Settings.LastSelectionHeight <= 0)
		{
			return false;
		}

		var maxWidth  = _state.Capture.Display.Size.Width;
		var maxHeight = _state.Capture.Display.Size.Height;

		var x = Math.Clamp(_state.Settings.LastSelectionX, 0, maxWidth);
		var y = Math.Clamp(_state.Settings.LastSelectionY, 0, maxHeight);

		var width  = Math.Clamp(_state.Settings.LastSelectionWidth,  0, Math.Max(0, maxWidth  - x));
		var height = Math.Clamp(_state.Settings.LastSelectionHeight, 0, Math.Max(0, maxHeight - y));

		if (width <= 0 || height <= 0)
		{
			return false;
		}

		_state.SelectionRect = new Rect(x, y, width, height);
		UpdateSelectionVisuals();
		return true;
	}

	private void ClearSelection()
	{
		_state.SelectionRect = new Rect();
		if (_controlsModel.SelectionBorder is
			{
			} selectionBorder)
		{
			selectionBorder.IsVisible = false;
		}
		if (_controlsModel.HandleCanvas is
			{
			} handleCanvas)
		{
			handleCanvas.IsVisible = false;
		}
		UpdateSelectionVisuals();
	}

	private void ColorPickerToggle_Click(object? sender, RoutedEventArgs e)
	{
		ColorPopup.IsOpen = !ColorPopup.IsOpen;
	}

	private void ColorPicker_OnColorChanged(object? sender, AvaloniaColor color)
	{
		_state.CurrentColor = color;

		if (_state.SelectedAnnotation != null)
		{
			_state.SelectedAnnotation.Color = DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
			annotationDrawer.RefreshSelectedAnnotationVisual(_state);
		}
	}

	private void PersistSelectionIfNeeded()
	{
		if (_state.Settings == null || _state.CaptureMode != CaptureMode.Region)
		{
			return;
		}

		if (_state.HasSelection)
		{
			_state.Settings.LastSelectionX      = _state.SelectionRect.X;
			_state.Settings.LastSelectionY      = _state.SelectionRect.Y;
			_state.Settings.LastSelectionWidth  = _state.SelectionRect.Width;
			_state.Settings.LastSelectionHeight = _state.SelectionRect.Height;
		}
		else
		{
			_state.Settings.LastSelectionX      = 0;
			_state.Settings.LastSelectionY      = 0;
			_state.Settings.LastSelectionWidth  = 0;
			_state.Settings.LastSelectionHeight = 0;
		}

		try
		{
			_ = Task.Run(() => settingsSystemService.SaveAsync(_state.Settings));
		}
		catch
		{
		}
	}

	private void ApplyShapeTranslation(Control visual)
	{
		if (!_state.VisualToAnnotation.TryGetValue(visual, out var annotation))
		{
			return;
		}

		if (translationsDrawing.ApplyShapeTranslation(visual, annotation, _state.ArrowGroups))
		{
			if (annotation.Tool == AnnotationToolType.Arrow)
			{
				if (_controlsModel.AnnotationCanvas != null)
				{
					arrowDrawingService.ReplaceArrowVisual(
						_controlsModel.AnnotationCanvas,
						visual,
						annotation,
						_state.VisualToAnnotation,
						_state.ArrowGroups,
						_state.SelectedVisual,
						out var updatedSelectedVisual);
					_state.SelectedVisual = updatedSelectedVisual;
				}
			}
			RecordState();
		}
	}

	private void ThicknessSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
	{
		_state.Thickness = e.NewValue;
	}

	private void ColorsPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (ColorPopup.IsOpen || _controlsModel.ColorsPanel == null || !e.GetCurrentPoint(_controlsModel.ColorsPanel).Properties.IsLeftButtonPressed)
		{
			return;
		}

		if (IsInsideInteractiveElement(e.Source as Control))
		{
			_controlsModel.ColorsPanel?.Cursor = ArrowCursor;
			return;
		}

		_state.IsPanelDragging = true;
		_state.PanelDragStart  = e.GetPosition(this);
		if (_controlsModel.ColorsPanel != null)
		{
			_controlsModel.ColorsPanel.Cursor = DragCursor;
			e.Pointer.Capture(_controlsModel.ColorsPanel);
		}
		e.Handled = true;
	}

	private void ColorsPanel_OnPointerMoved(object? sender, PointerEventArgs e)
	{
		if (ColorPopup.IsOpen)
		{
			StopPanelDrag(e.Pointer);
			return;
		}

		var overInteractive = IsInsideInteractiveElement(e.Source as Control);
		if (!_state.IsPanelDragging)
		{
			_controlsModel.ColorsPanel?.Cursor = overInteractive ? ArrowCursor : DragCursor;
			return;
		}

		if (_controlsModel.ColorsPanel == null || !e.GetCurrentPoint(_controlsModel.ColorsPanel).Properties.IsLeftButtonPressed)
		{
			StopPanelDrag(e.Pointer);
			return;
		}

		var current = e.GetPosition(this);
		var delta   = current - _state.PanelDragStart;

		if (_controlsModel.ColorsPanel != null)
		{
			var margin = _controlsModel.ColorsPanel.Margin;
			_controlsModel.ColorsPanel.Margin = new Thickness(
				margin.Left   + delta.X,
				margin.Top    + delta.Y,
				margin.Right  - delta.X,
				margin.Bottom - delta.Y);
		}

		_state.PanelDragStart = current;
		e.Handled             = true;
	}
	private static bool IsInsideInteractiveElement(Control? control)
	{
		while (control != null)
		{
			if (control is ColorPalettePicker or Slider or Button or ToggleButton or TextBox or Thumb)
			{
				return true;
			}
			control = control.GetVisualParent() as Control;
		}

		return false;
	}
	private void ColorsPanel_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		if (!_state.IsPanelDragging)
		{
			return;
		}

		StopPanelDrag(e.Pointer);
		e.Handled = true;
	}
	private void StopPanelDrag(IPointer pointer)
	{
		_state.IsPanelDragging             = false;
		_controlsModel.ColorsPanel?.Cursor = ArrowCursor;
		pointer.Capture(null);
	}
	private void OnHandleDragStarted(object? sender, VectorEventArgs e)
	{
		_state.IsResizing      = true;
		_state.ActiveHandle    = (sender as Control)?.Tag as string;
		_state.ResizeStartRect = _state.SelectionRect;
	}

	private void OnHandleDragDelta(object? sender, VectorEventArgs e)
	{
		if (!_state.IsResizing || string.IsNullOrWhiteSpace(_state.ActiveHandle))
		{
			return;
		}

		ResizeSelection(_state.ActiveHandle!, e.Vector);
	}

	private void OnHandleDragCompleted(object? sender, VectorEventArgs e)
	{
		_state.IsResizing   = false;
		_state.ActiveHandle = null;
	}
}
