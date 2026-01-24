using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Metadata;

namespace Shotora.App.Controls;

[ExcludeFromCodeCoverage]
public class CustomTabItem : AvaloniaObject
{
	public static readonly StyledProperty<object?> HeaderProperty =
		AvaloniaProperty.Register<CustomTabItem, object?>(nameof(Header));

	public static readonly StyledProperty<object?> ContentProperty =
		AvaloniaProperty.Register<CustomTabItem, object?>(nameof(Content));

	public object? Header
	{
		get => GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	[Content]
	public object? Content
	{
		get => GetValue(ContentProperty);
		set => SetValue(ContentProperty, value);
	}
}
