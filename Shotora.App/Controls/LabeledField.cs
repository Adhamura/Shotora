using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;

namespace Shotora.App.Controls;

/// <summary>
///     A form row: wrapping label on the left, field on the right.
///     Labels share their column width with every other <see cref="LabeledField" /> inside the nearest ancestor that has
///     <c>Grid.IsSharedSizeScope="True"</c>, so long translations wrap instead of being clipped and fields stay aligned.
///     Leave <see cref="Label" /> empty for check boxes and hints: the label cell collapses, and the content still lines
///     up with the field column whenever other rows in the scope have labels.
///     The label is also exposed as the field's automation name (unless the field sets its own).
///     The template lives in Styles/Controls.axaml (selector <c>controls|LabeledField</c>).
/// </summary>
[ExcludeFromCodeCoverage]
public class LabeledField : ContentControl
{
	public static readonly StyledProperty<string?> LabelProperty =
		AvaloniaProperty.Register<LabeledField, string?>(nameof(Label));

	private string? _appliedName;

	public string? Label
	{
		get => GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);

		if (change.Property == LabelProperty || change.Property == ContentProperty)
		{
			ApplyAutomationName();
		}
	}

	private void ApplyAutomationName()
	{
		if (Content is not Control field || string.IsNullOrEmpty(Label))
		{
			return;
		}

		var current = AutomationProperties.GetName(field);
		if (string.IsNullOrEmpty(current) || current == _appliedName)
		{
			_appliedName = Label;
			AutomationProperties.SetName(field, Label);
		}
	}
}
