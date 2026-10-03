using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;

namespace Shotora.App.Controls;

/// <summary>
///     A form row: wrapping label on the left, field on the right.
///     Labels share their column width with every other <see cref="LabeledField" /> inside the nearest ancestor that has
///     <c>Grid.IsSharedSizeScope="True"</c>, so long translations wrap instead of being clipped and fields stay aligned.
///     Leave <see cref="Label" /> empty to align content (check boxes, hints) with the field column.
///     The template lives in Styles/Controls.axaml (selector <c>controls|LabeledField</c>).
/// </summary>
[ExcludeFromCodeCoverage]
public class LabeledField : ContentControl
{
	public static readonly StyledProperty<string?> LabelProperty =
		AvaloniaProperty.Register<LabeledField, string?>(nameof(Label));

	public string? Label
	{
		get => GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}
}
