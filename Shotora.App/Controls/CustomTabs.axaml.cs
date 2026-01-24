using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Shotora.App.Controls;

[ExcludeFromCodeCoverage]
public partial class CustomTabs : UserControl
{
	public static readonly StyledProperty<IList<CustomTabItem>> ItemsProperty =
		AvaloniaProperty.Register<CustomTabs, IList<CustomTabItem>>(nameof(Items));

	public static readonly StyledProperty<int> SelectedIndexProperty =
		AvaloniaProperty.Register<CustomTabs, int>(nameof(SelectedIndex));

	public static readonly StyledProperty<object?> SelectedContentProperty =
		AvaloniaProperty.Register<CustomTabs, object?>(nameof(SelectedContent));
	private readonly ListBox? _tabsList;

	static CustomTabs()
	{
		ItemsProperty.Changed.AddClassHandler<CustomTabs>((tabs, args) =>
			tabs.OnItemsChanged(args.OldValue as IList<CustomTabItem>, args.NewValue as IList<CustomTabItem>));
		SelectedIndexProperty.Changed.AddClassHandler<CustomTabs>((tabs, _) => tabs.UpdateSelectedContent());
	}

	public CustomTabs()
	{
		Items = new AvaloniaList<CustomTabItem>();
		InitializeComponent();
		_tabsList = this.FindControl<ListBox>("TabsList");
		if (_tabsList != null)
		{
			_tabsList.SelectionChanged += TabsListOnSelectionChanged;
		}
		OnItemsChanged(null, Items);
	}

	public IList<CustomTabItem> Items
	{
		get => GetValue(ItemsProperty);
		set => SetValue(ItemsProperty, value);
	}

	public int SelectedIndex
	{
		get => GetValue(SelectedIndexProperty);
		set => SetValue(SelectedIndexProperty, value);
	}

	public object? SelectedContent
	{
		get => GetValue(SelectedContentProperty);
		set => SetValue(SelectedContentProperty, value);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		EnsureSelection();
		Dispatcher.UIThread.Post(EnsureSelection, DispatcherPriority.Background);
	}

	protected override void OnDataContextChanged(EventArgs e)
	{
		base.OnDataContextChanged(e);
		ApplyContentDataContext();
	}

	private void EnsureSelection()
	{
		if (Items.Count == 0)
		{
			return;
		}

		var desiredIndex = SelectedIndex;
		if (desiredIndex < 0 || desiredIndex >= Items.Count)
		{
			desiredIndex = 0;
		}

		if (SelectedIndex != desiredIndex)
		{
			SelectedIndex = desiredIndex;
		}

		if (_tabsList != null && _tabsList.SelectedIndex != desiredIndex)
		{
			_tabsList.SelectedIndex = desiredIndex;
		}

		UpdateSelectedContent();
	}

	private void OnItemsChanged(IList<CustomTabItem>? oldValue, IList<CustomTabItem>? newValue)
	{
		if (oldValue is INotifyCollectionChanged oldNotify)
		{
			oldNotify.CollectionChanged -= ItemsCollectionChanged;
		}

		if (newValue is INotifyCollectionChanged newNotify)
		{
			newNotify.CollectionChanged += ItemsCollectionChanged;
		}

		EnsureSelection();
	}

	private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		EnsureSelection();
	}

	private void TabsListOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (_tabsList == null || Items.Count == 0)
		{
			return;
		}

		if (_tabsList.SelectedIndex < 0)
		{
			SelectedIndex           = 0;
			_tabsList.SelectedIndex = 0;
		}

		UpdateSelectedContent();
	}

	private void UpdateSelectedContent()
	{
		if (Items == null || Items.Count == 0 || SelectedIndex < 0 || SelectedIndex >= Items.Count)
		{
			SelectedContent = null;
			return;
		}

		var content = Items[SelectedIndex].Content;
		if (!Equals(SelectedContent, content))
		{
			SelectedContent = content;
		}

		ApplyContentDataContext();
	}

	private void ApplyContentDataContext()
	{
		if (SelectedContent is Control control)
		{
			control.DataContext = DataContext;
		}
	}
}
