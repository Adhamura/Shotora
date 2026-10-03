using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Shotora.App.Controls;

[ExcludeFromCodeCoverage]
public partial class SearchableComboBox : UserControl
{
	private const double DropRegionWidth = 34;

	public static readonly StyledProperty<IEnumerable?>   ItemsSourceProperty = AvaloniaProperty.Register<SearchableComboBox, IEnumerable?>(nameof(ItemsSource));
	public static readonly StyledProperty<bool>           SortAlphabeticallyProperty = AvaloniaProperty.Register<SearchableComboBox, bool>(nameof(SortAlphabetically), true);
	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<SearchableComboBox, IDataTemplate?>(nameof(ItemTemplate));
	public static readonly StyledProperty<string>         DisplayMemberPathProperty = AvaloniaProperty.Register<SearchableComboBox, string>(nameof(DisplayMemberPath), string.Empty);
	public static readonly StyledProperty<object?>        SelectedItemProperty = AvaloniaProperty.Register<SearchableComboBox, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);
	public static readonly StyledProperty<string>         SelectedValuePathProperty = AvaloniaProperty.Register<SearchableComboBox, string>(nameof(SelectedValuePath), string.Empty);
	public static readonly StyledProperty<object?>        SelectedValueProperty = AvaloniaProperty.Register<SearchableComboBox, object?>(nameof(SelectedValue), defaultBindingMode: BindingMode.TwoWay);
	public static readonly StyledProperty<string>         TextProperty = AvaloniaProperty.Register<SearchableComboBox, string>(nameof(Text), string.Empty, defaultBindingMode: BindingMode.TwoWay);
	public static readonly StyledProperty<bool>           IsDropDownOpenProperty = AvaloniaProperty.Register<SearchableComboBox, bool>(nameof(IsDropDownOpen));
	public static readonly StyledProperty<bool>           CommitSelectionOnCloseProperty = AvaloniaProperty.Register<SearchableComboBox, bool>(nameof(CommitSelectionOnClose));

	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<SearchableComboBox, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	private readonly ObservableCollection<object> _displayItems = [];
	private          List<object>                 _allItems     = [];
	private          bool                         _isUpdatingSelection;

	private INotifyCollectionChanged? _itemsSourceNotifier;
	private bool                      _suppressTextChanged;
	private string                    _typedText = string.Empty;
	private bool                      _userEditedText;

	public SearchableComboBox()
	{
		InitializeComponent();
	}

	public IEnumerable? ItemsSource
	{
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

	public bool SortAlphabetically
	{
		get => GetValue(SortAlphabeticallyProperty);
		set => SetValue(SortAlphabeticallyProperty, value);
	}

	public IDataTemplate? ItemTemplate
	{
		get => GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	public string DisplayMemberPath
	{
		get => GetValue(DisplayMemberPathProperty);
		set => SetValue(DisplayMemberPathProperty, value);
	}

	public object? SelectedItem
	{
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public string SelectedValuePath
	{
		get => GetValue(SelectedValuePathProperty);
		set => SetValue(SelectedValuePathProperty, value);
	}

	public object? SelectedValue
	{
		get => GetValue(SelectedValueProperty);
		set => SetValue(SelectedValueProperty, value);
	}

	public string Text
	{
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public bool IsDropDownOpen
	{
		get => GetValue(IsDropDownOpenProperty);
		set => SetValue(IsDropDownOpenProperty, value);
	}

	public bool CommitSelectionOnClose
	{
		get => GetValue(CommitSelectionOnCloseProperty);
		set => SetValue(CommitSelectionOnCloseProperty, value);
	}

	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
	{
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		if (PART_ListBox != null && PART_ListBox.ItemsSource != _displayItems)
		{
			PART_ListBox.ItemsSource = _displayItems;
		}

		RebuildAllItems();
		SyncDisplayItems();
		TrySelectFromValue();
		UpdateTextFromSelection();
	}

	protected override void OnUnloaded(RoutedEventArgs e)
	{
		base.OnUnloaded(e);
		DetachItemsSourceNotifier();
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		// The UserControl itself is not focusable; screen readers land on the inner text box,
		// so it carries the accessible name (e.g. the label supplied by LabeledField).
		if (change.Property == AutomationProperties.NameProperty && PART_TextBox != null)
		{
			AutomationProperties.SetName(PART_TextBox, change.GetNewValue<string?>());
		}

		if (change.Property == ItemsSourceProperty)
		{
			DetachItemsSourceNotifier();
			AttachItemsSourceNotifier(change.NewValue as INotifyCollectionChanged);
			RebuildAllItems();
			SyncDisplayItems();
			TrySelectFromValue();
		}
		else if (change.Property == SortAlphabeticallyProperty)
		{
			RebuildAllItems();
			SyncDisplayItems();
		}
		else if (change.Property == ItemTemplateProperty || change.Property == DisplayMemberPathProperty)
		{
			ApplyPresentationMode();
		}
		else if (change.Property == SelectedItemProperty)
		{
			if (!_isUpdatingSelection)
			{
				UpdateTextFromSelection();
				SyncListBoxSelection();
			}
		}
		else if (change.Property == SelectedValueProperty)
		{
			if (!_isUpdatingSelection)
			{
				TrySelectFromValue();
			}
		}
		else if (change.Property == IsDropDownOpenProperty)
		{
			OnIsDropDownOpenChanged((bool)change.NewValue!);
		}
	}

	private void Shell_OnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (Shell == null)
		{
			return;
		}

		var pos = e.GetPosition(Shell);
		if (Shell.Bounds.Width - pos.X <= DropRegionWidth + 2)
		{
			IsDropDownOpen = !IsDropDownOpen;
			e.Handled      = true;
		}
	}

	private void OnIsDropDownOpenChanged(bool isOpen)
	{
		if (isOpen)
		{
			AdjustPopupPlacement();
			_userEditedText = false;
			_typedText      = string.Empty;

			SyncDisplayItems();
			SyncListBoxSelection();
			PART_TextBox?.Focus();
		}
		else if (CommitSelectionOnClose && !_userEditedText)
		{
			var highlighted = PART_ListBox?.SelectedItem;
			if (highlighted != null && !Equals(highlighted, SelectedItem))
			{
				CommitSelection(highlighted);
			}
		}
	}

	private void RebuildAllItems()
	{
		_allItems = ItemsSource is IEnumerable enumerable
			? SortAlphabetically
				? enumerable.Cast<object>()
					.OrderBy(item => GetDisplayText(item) ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
					.ToList()
				: enumerable.Cast<object>().ToList()
			: [];

		ApplyPresentationMode();
	}

	private void SyncDisplayItems()
	{
		var filtered = string.IsNullOrWhiteSpace(_typedText) || !_userEditedText
			? _allItems
			: _allItems.Where(item =>
				GetDisplayText(item)?.IndexOf(_typedText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

		_displayItems.Clear();
		foreach (var item in filtered)
		{
			_displayItems.Add(item);
		}
	}

	private void SyncListBoxSelection()
	{
		if (PART_ListBox == null)
		{
			return;
		}

		var target = SelectedItem;
		_isUpdatingSelection = true;
		if (target != null && _displayItems.Contains(target))
		{
			if (!Equals(PART_ListBox.SelectedItem, target))
			{
				PART_ListBox.SelectedItem = target;
			}
			PART_ListBox.ScrollIntoView(target);
		}
		else
		{
			PART_ListBox.SelectedItem = null;
		}
		_isUpdatingSelection = false;
	}

	private void ApplyPresentationMode()
	{
		if (PART_ListBox == null)
		{
			return;
		}

		if (ItemTemplate != null)
		{
			PART_ListBox.ItemTemplate = ItemTemplate;
		}
		else if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
		{
			PART_ListBox.ItemTemplate = new FuncDataTemplate<object>((item, _) =>
				new TextBlock
				{
					Text = GetDisplayText(item)
				});
		}
		else
		{
			PART_ListBox.ItemTemplate = null;
		}
	}

	private void AttachItemsSourceNotifier(INotifyCollectionChanged? notifier)
	{
		_itemsSourceNotifier = notifier;
		if (_itemsSourceNotifier != null)
		{
			_itemsSourceNotifier.CollectionChanged += ItemsSourceOnCollectionChanged;
		}
	}

	private void DetachItemsSourceNotifier()
	{
		if (_itemsSourceNotifier != null)
		{
			_itemsSourceNotifier.CollectionChanged -= ItemsSourceOnCollectionChanged;
			_itemsSourceNotifier                   =  null;
		}
	}

	private void ItemsSourceOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		Dispatcher.UIThread.Post(() =>
		{
			RebuildAllItems();
			SyncDisplayItems();
			TrySelectFromValue();
			UpdateTextFromSelection();
		});
	}

	private void CommitSelection(object? item)
	{
		if (item == null)
		{
			return;
		}

		var previous = SelectedItem;
		if (Equals(previous, item))
		{
			IsDropDownOpen = false;
			return;
		}

		_isUpdatingSelection = true;
		try
		{
			SelectedItem  = item;
			SelectedValue = ResolvePathValue(item, SelectedValuePath);
			UpdateTextFromSelection();
		}
		finally
		{
			_isUpdatingSelection = false;
		}

		_userEditedText = false;
		Dispatcher.UIThread.Post(
			() => IsDropDownOpen = false,
			DispatcherPriority.Background);

		if (!Equals(previous, item))
		{
			RaiseEvent(new SelectionChangedEventArgs(
				SelectionChangedEvent,
				new List<object?>
				{
					previous
				},
				new List<object?>
				{
					item
				}));
		}
	}

	private void UpdateTextFromSelection()
	{
		var item = SelectedItem;
		if (ReferenceEquals(item, DataContext))
		{
			item = null;
		}

		if (item == null && SelectedValue != null)
		{
			item = FindItemByValue();
			if (item != null)
			{
				_isUpdatingSelection = true;
				SelectedItem         = item;
				_isUpdatingSelection = false;
			}
		}

		var display = item != null ? GetDisplayText(item) : SelectedValue?.ToString();

		_suppressTextChanged = true;
		Text                 = display ?? Text;
		_suppressTextChanged = false;
	}

	private void TrySelectFromValue()
	{
		if (SelectedValue == null)
		{
			return;
		}

		var match = FindItemByValue();
		if (match == null)
		{
			return;
		}

		_isUpdatingSelection = true;
		SelectedItem         = match;
		_isUpdatingSelection = false;
		UpdateTextFromSelection();
	}

	private object? FindItemByValue()
	{
		if (SelectedValue == null || ItemsSource == null)
		{
			return null;
		}

		return ItemsSource.OfType<object>().FirstOrDefault(item => Equals(ResolvePathValue(item, SelectedValuePath), SelectedValue));
	}

	private string? GetDisplayText(object? item)
	{
		if (item == null)
		{
			return null;
		}

		var paths = new[]
		{
			DisplayMemberPath, "Name", "Code"
		};

		foreach (var path in paths)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				continue;
			}

			var text = ResolvePathValue(item, path)?.ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
		}

		return item.ToString();
	}

	private static object? ResolvePathValue(object item, string? path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return null;
		}

		try
		{
			var current = item;
			foreach (var part in path.Split('.'))
			{
				if (current == null)
				{
					return null;
				}
				var prop = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
				current = prop?.GetValue(current);
			}
			return current;
		}
		catch
		{
			return null;
		}
	}

	private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
	{
		if (_suppressTextChanged)
		{
			return;
		}

		var isUserEdit = PART_TextBox?.IsFocused == true;
		if (!isUserEdit)
		{
			return;
		}

		_typedText      = Text;
		_userEditedText = true;

		if (!IsDropDownOpen && PART_ListBox != null && IsLoaded)
		{
			IsDropDownOpen = true;
		}

		SyncDisplayItems();
	}

	private void TextBox_OnGotFocus(object? sender, GotFocusEventArgs e)
	{
	}

	private void TextBox_OnKeyDown(object? sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.Down:
				e.Handled = true;
				if (!IsDropDownOpen)
				{
					IsDropDownOpen = true;
					return;
				}
				MoveHighlight(1);
				break;
			case Key.Up:
				e.Handled = true;
				if (!IsDropDownOpen)
				{
					IsDropDownOpen = true;
					return;
				}
				MoveHighlight(-1);
				break;
			case Key.Enter:
				if (IsDropDownOpen && PART_ListBox?.SelectedItem != null)
				{
					CommitSelection(PART_ListBox.SelectedItem);
					e.Handled = true;
				}
				break;
			case Key.Escape:
				if (IsDropDownOpen)
				{
					// Close only the drop-down; a second Esc reaches the window (dialog cancel button).
					IsDropDownOpen = false;
					e.Handled      = true;
				}
				break;
		}
	}

	private void MoveHighlight(int direction)
	{
		if (_displayItems.Count == 0 || PART_ListBox == null)
		{
			return;
		}

		var idx = PART_ListBox.SelectedItem != null
			? _displayItems.IndexOf(PART_ListBox.SelectedItem)
			: direction > 0
				? -1
				: _displayItems.Count;

		var next = idx + direction;
		if (next >= 0 && next < _displayItems.Count)
		{
			PART_ListBox.SelectedItem = _displayItems[next];
			PART_ListBox.ScrollIntoView(_displayItems[next]);
		}
	}

	private void ListBox_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
	{
		e.Handled = true;

		if (PART_ListBox?.SelectedItem != null)
		{
			CommitSelection(PART_ListBox.SelectedItem);
		}
	}

	private void ListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
	}

	private void Root_OnKeyDown(object? sender, KeyEventArgs e)
	{
		if (e.Handled)
		{
			return;
		}

		if (IsDropDownOpen         &&
			sender != PART_TextBox &&
			e.Key is Key.Up or Key.Down or Key.Enter or Key.Escape)
		{
			TextBox_OnKeyDown(PART_TextBox, e);
		}
	}

	private void AdjustPopupPlacement()
	{
		if (PART_Popup == null || Shell == null)
		{
			return;
		}

		var root = TopLevel.GetTopLevel(this);
		if (root == null)
		{
			PART_Popup.Placement = PlacementMode.Bottom;
			return;
		}

		var shellBottom = Shell.TranslatePoint(new Point(0, Shell.Bounds.Height), root)?.Y ?? 0;
		var spaceBelow  = root.Bounds.Height - shellBottom;
		var spaceAbove  = Shell.TranslatePoint(new Point(0, 0), root)?.Y ?? 0;
		var desired     = Math.Min(PART_ScrollViewer?.DesiredSize.Height ?? 260, 260);

		PART_Popup.Placement = spaceBelow < desired && spaceAbove > spaceBelow
			? PlacementMode.Top
			: PlacementMode.Bottom;
	}
}
