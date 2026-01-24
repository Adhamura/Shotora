using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;

namespace Shotora.App.Controls;

[ExcludeFromCodeCoverage]
public partial class LabeledField : UserControl
{
	public static readonly StyledProperty<string?> LabelProperty =
		AvaloniaProperty.Register<LabeledField, string?>(nameof(Label));

	public LabeledField()
	{
		InitializeComponent();
	}

	public string? Label
	{
		get => GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}
}
