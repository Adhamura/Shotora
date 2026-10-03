using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Shotora.App.Interfaces.PeripheralServices;

namespace Shotora.App.Controls;

/// <summary>
///     Shared chrome for Shotora's custom-decorated windows (SystemDecorations="None"):
///     rounded panel, title bar with title / optional subtitle / close button, scrollable content area and a footer
///     for dialog actions. The title bar and the empty parts of the footer are the drag area; when
///     <see cref="ResizeService" /> is set and the window can resize, the outer edges resize the window.
///     The template lives in Styles/Controls.axaml (selector <c>controls|DialogShell</c>).
/// </summary>
[ExcludeFromCodeCoverage]
public class DialogShell : ContentControl
{
	public static readonly StyledProperty<string?> TitleProperty =
		AvaloniaProperty.Register<DialogShell, string?>(nameof(Title));

	public static readonly StyledProperty<string?> SubtitleProperty =
		AvaloniaProperty.Register<DialogShell, string?>(nameof(Subtitle));

	public static readonly StyledProperty<object?> FooterProperty =
		AvaloniaProperty.Register<DialogShell, object?>(nameof(Footer));

	public static readonly StyledProperty<bool> ShowCloseButtonProperty =
		AvaloniaProperty.Register<DialogShell, bool>(nameof(ShowCloseButton), true);

	private readonly Dictionary<WindowEdge, Cursor> _resizeCursors = new();

	private Button?     _closeButton;
	private Control?    _footerPresenter;
	private WindowEdge? _hoverEdge;
	private Control?    _titleBar;

	static DialogShell()
	{
		FooterProperty.Changed.AddClassHandler<DialogShell>((shell, e) => shell.OnFooterChanged(e.OldValue, e.NewValue));
	}

	public DialogShell()
	{
		AddHandler(PointerPressedEvent, OnShellPointerPressed);
		AddHandler(PointerMovedEvent,   OnShellPointerMoved, RoutingStrategies.Bubble, true);
		AddHandler(PointerExitedEvent,  OnShellPointerExited);
	}

	public string? Title
	{
		get => GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string? Subtitle
	{
		get => GetValue(SubtitleProperty);
		set => SetValue(SubtitleProperty, value);
	}

	/// <summary>Footer content, normally a <c>StackPanel Classes="dialog-actions"</c> with the primary action last.</summary>
	public object? Footer
	{
		get => GetValue(FooterProperty);
		set => SetValue(FooterProperty, value);
	}

	public bool ShowCloseButton
	{
		get => GetValue(ShowCloseButtonProperty);
		set => SetValue(ShowCloseButtonProperty, value);
	}

	/// <summary>Optional edge hit-testing service. When null the window can only be moved, not resized.</summary>
	public IMouseInteractionService? ResizeService { get; set; }

	private Window? HostWindow => TopLevel.GetTopLevel(this) as Window;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);

		if (_closeButton != null)
		{
			_closeButton.Click -= CloseButton_OnClick;
		}

		_titleBar        = e.NameScope.Find<Control>("PART_TitleBar");
		_footerPresenter = e.NameScope.Find<Control>("PART_FooterPresenter");
		_closeButton     = e.NameScope.Find<Button>("PART_CloseButton");
		if (_closeButton != null)
		{
			_closeButton.Click += CloseButton_OnClick;
		}
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_hoverEdge = null;
		foreach (var cursor in _resizeCursors.Values)
		{
			cursor.Dispose();
		}

		_resizeCursors.Clear();
	}

	private void OnFooterChanged(object? oldValue, object? newValue)
	{
		// Keep footer content in the logical tree so DynamicResource (localized strings) and IsDefault/IsCancel resolve.
		if (oldValue is ILogical oldLogical)
		{
			LogicalChildren.Remove(oldLogical);
		}

		if (newValue is ILogical newLogical && newLogical.LogicalParent == null)
		{
			LogicalChildren.Add(newLogical);
		}
	}

	private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
	{
		HostWindow?.Close();
	}

	private WindowEdge? GetResizeEdge(Window window, PointerEventArgs e)
	{
		if (ResizeService == null || !window.CanResize || window.WindowState != WindowState.Normal)
		{
			return null;
		}

		return ResizeService.GetResizeEdge(window, e.GetPosition(window));
	}

	/// <summary>
	///     Pointer events from pop-ups (combo-box lists, colour pickers) bubble through the shell's logical tree;
	///     only react to events raised inside this shell's own window.
	/// </summary>
	private bool IsFromHostWindow(RoutedEventArgs e, Window window)
	{
		return e.Source is Visual source && TopLevel.GetTopLevel(source) == window;
	}

	private bool IsInDragArea(object? source)
	{
		if (source is not Visual visual)
		{
			return false;
		}

		foreach (var ancestor in visual.GetSelfAndVisualAncestors())
		{
			if (ReferenceEquals(ancestor, _titleBar) || ReferenceEquals(ancestor, _footerPresenter))
			{
				return true;
			}

			if (ReferenceEquals(ancestor, this))
			{
				break;
			}
		}

		return false;
	}

	private void OnShellPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		var window = HostWindow;
		if (window == null || !IsFromHostWindow(e, window) || !e.GetCurrentPoint(window).Properties.IsLeftButtonPressed)
		{
			return;
		}

		var edge = GetResizeEdge(window, e);
		if (edge.HasValue)
		{
			window.BeginResizeDrag(edge.Value, e);
			e.Handled = true;
		}
		else if (IsInDragArea(e.Source))
		{
			window.BeginMoveDrag(e);
			e.Handled = true;
		}
	}

	private void OnShellPointerMoved(object? sender, PointerEventArgs e)
	{
		var window = HostWindow;
		if (window == null || ResizeService == null || !IsFromHostWindow(e, window))
		{
			return;
		}

		SetHoverEdge(window, GetResizeEdge(window, e));
	}

	private void OnShellPointerExited(object? sender, PointerEventArgs e)
	{
		if (HostWindow is { } window)
		{
			SetHoverEdge(window, null);
		}
	}

	/// <summary>Updates the window cursor only when the hovered edge changes; cursors are cached per edge.</summary>
	private void SetHoverEdge(Window window, WindowEdge? edge)
	{
		if (edge == _hoverEdge)
		{
			return;
		}

		_hoverEdge = edge;
		if (edge is not { } value || ResizeService == null)
		{
			window.Cursor = null;
			return;
		}

		if (!_resizeCursors.TryGetValue(value, out var cursor))
		{
			cursor                = new Cursor(ResizeService.GetResizeCursor(value));
			_resizeCursors[value] = cursor;
		}

		window.Cursor = cursor;
	}
}
